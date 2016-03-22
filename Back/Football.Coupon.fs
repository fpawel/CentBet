module Betfair.Football.Coupon


open System

open Betfair
open Betfair.Football
open Betfair.Football.Services

let Events = Atom.todayValueRef "EVENTS" Map.empty  Atom.Logs.none


let getExistedEvents ids = async{
    let! events = Events.Get()
    let existedIds, missingIds = ids |> List.partition( fun k -> events.ContainsKey k )
    let existed = existedIds |> List.map(fun id -> id, events.[id]) |> Map.ofList
    return existed, missingIds }



module MarketCatalogue = 
    let state = Atom.todayValueRef "MARKET-CATALOGUE" Map.empty  Atom.Logs.none

    let getNotMemoized ((eventId,_) as gameId : GameId ) = 
        async{
            let! auth = Betfair.Login.auth.Get()
            match auth with
            | None -> return Left "no befair's session"
            | Some auth ->
                let! r = ApiNG.Services.listMarketCatalogue auth eventId
                match r with
                | Right x -> 
                    do! state.Update( fun m ->  Map.add gameId x m )                
                | _ -> ()
                return r  }

    let getMemoized ((eventId,_) as gameId : GameId ) = 
        async{
            let! existedMarketCatalogue = state.Get()
            match existedMarketCatalogue.TryFind gameId with
            | Some markets -> return Right markets
            | _ ->
                let! auth = Betfair.Login.auth.Get()
                match auth with
                | None -> return Left "no befair's session"
                | Some auth ->
                    let! r = ApiNG.Services.listMarketCatalogue auth eventId
                    match r with
                    | Right x -> 
                        do! state.Update( fun m ->  Map.add gameId x m )                
                    | _ -> ()
                    return r }

    let private toltalMatched xs =
        let xs = xs |> List.choose( fun (x : ApiNG.MarketCatalogue )-> x.totalMatched ) 
        if xs.IsEmpty then None else Some <| List.sum xs 
        |> Option.map int

    let getTotalMatchedOfEvent gameId = async{
        let! m' = getNotMemoized gameId    
        return Either.mapRight toltalMatched m'}

    let getWithTotalMatched gameId = async{    
        let! m = getNotMemoized gameId
        return m |> Either.mapRight ( fun m ->       
            m |> List.map( fun x ->             
                let runners = x.runners |> List.map( fun rnr -> rnr.runnerName, rnr.selectionId)
                x.marketId.marketId, x.marketName, runners, Option.map int x.totalMatched ) ) }
    

[<AutoOpen>]
module private Helpers = 
    let ``initialized`` = "initialized"
    let ``started`` = "started"
    let ``stoped`` = "stoped"
    let ``ok`` = "Ok"
    let (~%%) = Async.Start
    
    let initStatus what = Atom.status (what + "-STATUS") ``initialized``
    let start' (status : Atom.Status) work =         
        %% async{
            do! status.Set Logging.Info ``started``
            while true do     
                let! (x : _) = work()
                do! x |> status.Set1( fun _ -> ``ok``)
                match x with
                | Left _ -> do! Async.Sleep 5000
                | _ -> ()
            do! status.Set Logging.Error ``stoped`` }

let getAllGames() = async{
    let! inplay = Coupon.Inplay.Get()
    let! today1 = Coupon.Today1.Get()
    let! today2 = Coupon.Today2.Get()
    return inplay @ today1 @ today2 }

let private processEvents() = async{
    let! auth = Login.auth.Get()
    match auth with
    | None ->         
        do! Async.Sleep 5000
        return Left "waiting for auth to read"
    | Some auth ->
        let! allGames = getAllGames()

        let! _, missingIds =
            allGames 
            |> List.map(fun ({gameId = gameId},_) -> gameId )
            |> getExistedEvents
        if missingIds.IsEmpty then return Right [] else
        let! newEvents = 
            List.map fst missingIds
            |> ApiNG.Services.listEvents auth 
        match newEvents with
        | Right newEvents as r when newEvents.Length = missingIds.Length ->            
            let newEvents = 
                newEvents |> List.choose( fun e -> 
                    missingIds |> List.tryFind( fst >> ( (=)  e.event.id ) )
                    |> Option.map( fun gameId -> gameId, e) )
                |> Map.ofList
            do! Events.Update <| fun existedEvents ->                
                Map.union existedEvents newEvents
            return r
        | Right newEvents  ->
            return Left <| sprintf "-aping-events responsed size %d mismatch, waiting %d" newEvents.Length missingIds.Length 
        | x -> return x }

module Status = 
    let Inplay = initStatus "INPLAY"
    let Today = initStatus "TODAY" 
    let Events = initStatus "EVENTS" 

let start = 
    start' Status.Inplay Coupon.updateInplay
    start' Status.Today Coupon.updateToday
    start' Status.Events processEvents

    #if DEBUG
    #else
    let culture = System.Globalization.CultureInfo("ru-RU")      
    System.Threading.Thread.CurrentThread.CurrentCulture <- culture
    System.Threading.Thread.CurrentThread.CurrentUICulture <- culture
    #endif

    fun () -> ()

type NewGame1 = Game * GameInfo * int
type UpdGame1 = GameId * GameInfo * int
type OutGame1 = GameId

type CouponResponse = NewGame1 list * UpdGame1 list * Set<OutGame1>

let getCoupon ( (reqGames,inplayOnly) as request) : Async<CouponResponse>  = async{
    let reqIds, reqGames = List.ids reqGames fst
    let! games = async{ 
        let! inplay = Coupon.Inplay.Get()
        if inplayOnly then return inplay else
        let! today1 = Coupon.Today1.Get()
        let! today2 = Coupon.Today2.Get()
        return inplay @ today1 @ today2 } 
    let extIds, extGames = List.ids games ( fst >> Game.id )
    let newIds = Set.difference extIds reqIds
    let outIds = Set.difference reqIds extIds 
    let updIds = Set.intersect reqIds extIds        
    let newGames =
        newIds |> Seq.map ( fun id -> 
            let game, gameInfo = extGames.[id] 
            game,  gameInfo, gameInfo.GetHash() )
        |> Seq.toList
    let update = 
        updIds
        |> Seq.map( fun id -> 
            let _, gameInfo = extGames.[id] 
            id, gameInfo, gameInfo.GetHash()  )
        |> Seq.filter( fun(gameId, gameInfo, hash) -> 
            let _,reqHash = reqGames.[gameId]     
            hash <> reqHash  )
        |> Seq.toList
    return newGames, update, outIds }

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

    

let getEventsCatalogue ids =     
    getExistedEvents ids
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



