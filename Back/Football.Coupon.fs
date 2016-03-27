module Betfair.Football.Coupon


open System

open Betfair
open Betfair.Football
open Betfair.Football.Services

[<AutoOpen>]
module private Helpers = 
    let ``initialized`` = "initialized"
    let ``started`` = Logging.Info, "started"
    let ``stoped`` = Logging.Error, "stoped"
    let ``ok`` = Logging.Info, "Ok"
    let (~%%) = Async.Start
    let error x = Logging.Error, x
    
    let initStatus what = Atom.status (what + "-STATUS") ``initialized``
    
    let start' (status : Atom.Status) sleepInterval sleepErrorInterval work =         
        %% async{
            Logging.info "loop started : %A" status.Atom.What
            do! status.Set ``started``
            while true do     
                let! inf = work
                do! status.Set inf
                do! Async.Sleep <| match inf with
                                   | Logging.Error,_ -> sleepErrorInterval
                                   | _ -> sleepInterval
            do! status.Set ``stoped`` 
            Logging.error "loop terminated : %A" status.Atom.What}

    let start'auth' status sleepInterval sleepErrorInterval work = 
        start' status sleepInterval sleepErrorInterval <| async{
            let! auth = Login.auth.Get()
            match auth with
            | None ->  return Logging.Warn, "waiting for auth betfair"
            | Some auth -> return! work auth }

let getAllGames() = async{
    let! inplay = Coupon.Inplay.Get()
    let! foreplay = Coupon.Foreplay.Get()
    return inplay @ foreplay }



module Events = 
    [<AutoOpen>]
    module Helpers1 =
        open System.Globalization
        open System.Threading

        let shortCountries = 
            [   "Боливарийская Республика Венесуэла", "Венесуэла"
                "Чешская республика", "Чехия"
                "Китайская Народная Республика", "Китай"
                "Македония (Бывшая Югославская Республика Македония)", "Македония"
                "Сербия и Черногория (бывшая)", "Сербия" ]
            |> Map.ofList

        let countries1 = 
            [ "GI", "Гибралтар" ]
            |> Map.ofList

        let getCountryFromRegionInfo (countryCode : string) = 
            // Change current culture
            let culture = CultureInfo("ru-RU")      
            Thread.CurrentThread.CurrentCulture <- culture
            Thread.CurrentThread.CurrentUICulture <- culture

            let i = Globalization.RegionInfo(countryCode)
            if i=null then countryCode else 
            shortCountries.TryFind i.DisplayName 
            |> Option.getWith i.DisplayName 

        let countryFromEvent (e: ApiNG.Event) = 
            e.countryCode |> Option.map( fun countryCode ->
                countries1.TryFind countryCode
                |> Option.getBy ( fun () -> 
                    try
                        getCountryFromRegionInfo(countryCode)
                    with _ -> 
                        countryCode ) )

    let state = Atom.today "EVENTS" Atom.Logs.none 
    
    let private getExisted ids = async{
        let! events = state.Get()
        match events with
        | None -> return Map.empty, ids
        | Some events ->
            let existedIds, missingIds = ids |> List.partition( fun k -> Map.containsKey k events )
            let existed = existedIds |> List.map(fun id -> id, events.[id]) |> Map.ofList
            return existed, missingIds }

    let update auth = async{       
        let! allGames = getAllGames()
        let! existedEvents, missingIds =
            allGames 
            |> List.map(fun ({gameId = gameId},_) -> gameId )
            |> getExisted
        if missingIds.IsEmpty then return Logging.Info, "nothing to read" else
        let! newEvents = 
            List.map fst missingIds
            |> ApiNG.Services.listEvents auth 
        match newEvents with
        | Right newEvents when newEvents.Length = missingIds.Length ->            
            let newEvents = 
                newEvents |> List.choose( fun e -> 
                    missingIds |> List.tryFind( fst >> ( (=)  e.event.id ) )
                    |> Option.map( fun gameId -> gameId, e) )
                |> Map.ofList
            do! state.Set <| Map.union existedEvents newEvents
            return ``ok``
        | Right newEvents  ->
            return error <| sprintf "responsed size %d mismatch, waiting %d" newEvents.Length missingIds.Length 
        | Left x -> return error x }

    let get ids = 
        getExisted ids
        |> Async.map ( fun (rdds, msng) -> 
            rdds 
            |> Map.toList
            |> List.map(fun (gameId,{event = e}) ->
                let country = 
                    try
                        countryFromEvent e
                    with exn -> 
                        Logging.error "%A"  exn
                        None
                gameId, e.name, country ) )


module MarketsCatalogue = 
    let state = Atom.today "MARKET-CATALOGUE" Atom.Logs.none 

    let get gameId = async{ 
        let! xs = state.Get()
        return xs  |> Option.bind (Map.tryFind gameId) }

    let private upd' gameId x = 
        function 
            | Some m -> m 
            | _ -> Map.empty
        >> Map.add gameId x 
        >> Some >> Async.id

    let private  read'and'upd' auth ((eventId,_) as gameId) =  async{
        let! r = ApiNG.Services.listMarketCatalogue auth eventId
        match r with
        | Right x -> do! state.Update (upd' gameId x)
        | _ -> ()
        return r }

    

    let update auth = async{
        let! games = getAllGames()
        let mutable errors = []
        for {gameId = gameId},_ in games do
            let! x = read'and'upd' auth gameId
            errors <- match x with Left x -> x::errors | _ -> errors
        return 
            if errors.IsEmpty then ``ok`` else
            error errors.[0] }

    

module TotalMatched = 

    type private ItemsMap = Map< int * int * int, DateTime * decimal >
    
    let state = Atom.atom "TOTAL-MATCHED" Map.empty Atom.Logs.none 
    let status = initStatus "TOTAL-MATCHED" 
    let private cacheLifeTime = TimeSpan.FromMinutes 3.
    let private isLive d = DateTime.Now - d < cacheLifeTime

    let private getExisted (eventId:int, mainMarketId:int) marketsIds = async{
        let! xs = state.Get()
        let existed, missedIds = 
            marketsIds 
            |> List.map( fun marketId -> 
                let key = eventId, mainMarketId, marketId
                marketId, Map.tryFind key xs |> Option.bind( fun (d,v) -> 
                    if isLive d then Some v else None ))
            |> List.partition ( snd >> Option.isSome) 
        let missedIds = List.map fst missedIds
        let existed = 
            existed 
            |> List.choose ( fun (x, y) -> y |> Option.map( fun y -> x,y)) 
            |> Map.ofList 
        return existed, missedIds }

    let ofMarkets ((eventId:int, mainMarketId:int) as gameId) marketsIds = async{
        let! (existed, missedIds) = getExisted gameId marketsIds
        if missedIds.IsEmpty then return existed else
        let! auth = Login.auth.Get()
        match auth with
        | None ->  return existed
        | Some auth ->
            let! r = ApiNG.Services.listMarketCatalogue auth eventId
            do! status.Set1 ( leftSome r )
            match r with
            | Left _ -> return Map.empty
            | Right r ->
                let readed =
                    r |> List.map( fun x -> 
                        let k = eventId, mainMarketId, x.marketId.marketId
                        let v = DateTime.Now, Option.fold (+) 0m x.totalMatched
                        k, v)
                do! state.Update ( fun m -> Map.union m ( Map.ofList readed) )
                    |> Async.Ignore
                return 
                    readed 
                    |> List.map( fun ((_,_,marketId),(_,v)) -> marketId, v)
                    |> Map.ofList
                    |> Map.union existed }

    

module Status = 
    let Inplay = initStatus "INPLAY"
    let Today = initStatus "TODAY" 
    let Events = initStatus "EVENTS" 
    let MarketsCatalogue = initStatus "MARKET-CATALOGUE" 

let start = 
    let mutable isstarted = false
    let lock' = obj()
    fun () -> 
        lock lock' <| fun () ->
            if isstarted then () else
            start' Status.Inplay 0 0 Coupon.updateInplay
            start' Status.Today 0 0 Coupon.updateForeplay
            start'auth' Status.Events 0 0 Events.update
            start'auth' Status.MarketsCatalogue 0 0 MarketsCatalogue.update
            isstarted <- true
        

type NewGame1 = Game * GameInfo * int
type UpdGame1 = GameId * GameInfo * int
type OutGame1 = GameId



let getTodayGamesCount () = async{
    let! inplay = Coupon.Inplay.Get()
    let! foreplay = Coupon.Foreplay.Get()
    return inplay.Length + foreplay.Length  }


let calcGamesDiff reqGames games = 
    let reqIds, reqGames = List.ids reqGames fst
    let extIds, extGames = List.ids games ( fst >> Game.id )
    let newIds = Set.difference extIds reqIds
    let outIds = Set.difference reqIds extIds 
    let updIds = Set.intersect reqIds extIds        
    let newGames =
        newIds |> Seq.map ( fun id -> 
            let game, gameInfo = extGames.[id] 
            game,  gameInfo, GameInfo.getHash gameInfo )
        |> Seq.toList
    let update = 
        updIds
        |> Seq.map( fun id -> 
            let _, gameInfo = extGames.[id] 
            id, gameInfo, GameInfo.getHash gameInfo  )
        |> Seq.filter( fun(gameId, gameInfo, hash) -> 
            let _,reqHash = reqGames.[gameId]     
            hash <> reqHash  )
        |> Seq.toList
    newGames, update, outIds 


type CouponResponse = NewGame1 list * UpdGame1 list * Set<OutGame1> * int
let getCouponPage (reqGames, npage, pagelen) : Async<CouponResponse> = async{
    let! games = async{ 
        let! inplay = Coupon.Inplay.Get()
        let! foreplay = Coupon.Foreplay.Get()
        return inplay @ foreplay }
    let pages = 
        games 
        |> Seq.mapi( fun n x -> n,x)
        |> Seq.groupBy( fun (n,_) -> n / pagelen  )        
        |> Map.ofSeq
    let targetPageGames = 
        match pages.TryFind npage with
        | None -> []
        | Some targetPageGames -> targetPageGames |> Seq.map snd |> Seq.toList
    let new', upd', out' = calcGamesDiff reqGames targetPageGames
    return new', upd', out', games.Length }




//let getCoupon ( (reqGames,inplayOnly) as request) = async {
//    let reqIds, reqGames = List.ids reqGames fst
//    let! games = async{ 
//        let! inplay = Coupon.Inplay.Get()
//        if inplayOnly then return inplay else
//        let! today1 = Coupon.Today1.Get()
//        let! today2 = Coupon.Today2.Get()
//        return inplay @ today1 @ today2 } 
//    let extIds, extGames = List.ids games ( fst >> Game.id )
//    let newIds = Set.difference extIds reqIds
//    let outIds = Set.difference reqIds extIds 
//    let updIds = Set.intersect reqIds extIds        
//    let newGames =
//        newIds |> Seq.map ( fun id -> 
//            let game, gameInfo = extGames.[id] 
//            game,  gameInfo, gameInfo.GetHash() )
//        |> Seq.toList
//    let update = 
//        updIds
//        |> Seq.map( fun id -> 
//            let _, gameInfo = extGames.[id] 
//            id, gameInfo, gameInfo.GetHash()  )
//        |> Seq.filter( fun(gameId, gameInfo, hash) -> 
//            let _,reqHash = reqGames.[gameId]     
//            hash <> reqHash  )
//        |> Seq.toList
//    return newGames, update, outIds }



    




