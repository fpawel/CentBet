﻿module Betfair.Football.Coupon


open System

open Betfair
open Betfair.Football
open Betfair.Football.Services

open Concurency    

let adminBetafirAuth = atom None

[<AutoOpen>]
module private Helpers = 
    
    let start' name sleepInterval sleepErrorInterval work = 
        Async.Start <| async{
            let status = Status(name)
            status.Set Logging.Info "loop started" 
            while true do     
                let! (level,text) = work
                status.Set level "%s" text
                do! Async.Sleep <| match level with
                                   | Logging.Error -> sleepErrorInterval
                                   | _ -> sleepInterval
            status.Set Logging.Error "loop terminated" }

    let start'auth' status sleepInterval sleepErrorInterval work = 
        start' status sleepInterval sleepErrorInterval <| async{
            match adminBetafirAuth.Value with
            | None ->  return Logging.Warn, "waiting for auth betfair"
            | Some auth -> return! work auth }

let getTodayGames() = Coupon.inplay.Value @ Coupon.foreplay.Value

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

    let state = todayRef()
    
    let private getExisted ids = 
        match state.Unwrap() with
        | None -> Map.empty, ids
        | Some events ->
            let existedIds, missingIds = ids |> List.partition( fun k -> Map.containsKey k events )
            let existed = existedIds |> List.map(fun id -> id, events.[id]) |> Map.ofList
            existed, missingIds 

    let update auth = async{       
        let allGames = getTodayGames()
        let existedEvents, missingIds =
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
            state.Set <| Map.union existedEvents newEvents
            return Logging.Info, sprintf "readed new %d" newEvents.Count
        | Right newEvents  ->
            return Logging.Error, sprintf "responsed size %d mismatch, waiting %d" newEvents.Length missingIds.Length 
        | Left x -> return Logging.Error, x }

    let get ids = 
        let rdds, msng = getExisted ids
        rdds 
        |> Map.toList
        |> List.map(fun (gameId,{event = e}) ->
            let country = 
                try
                    countryFromEvent e
                with exn -> 
                    Logging.error "%A"  exn
                    None
            gameId, e.name, country ) 

module MarketsCatalogue = 
    let state = todayRef()

    let get gameId = 
        state.Value 
        |> Option.bind( Map.tryFind gameId)     
        
    let private readAndUpd auth ((eventId,_) as gameId) =  async{
        let! r = ApiNG.Services.listMarketCatalogue auth eventId
        r |> rightSome |> Option.iter( fun x -> 
            match state.Value with
            | None ->  Map.add gameId x Map.empty
            | Some y ->  Map.add gameId x y
            |> state.Set )        
        return r }

    let update auth = async{
        let games = getTodayGames()
        let errors = ref []
        let gamesWithoutMarkets = 
            games
            |> List.map( fun ({gameId = gameId},_) -> gameId)
            |> List.filter ( get >> Option.isNone )

        for gameId in gamesWithoutMarkets do
            let! x = readAndUpd auth gameId
            errors := match x with Left x -> x::!errors | _ -> !errors            
        return 
            if List.isEmpty !errors then Logging.Info, "ok"  else
            Logging.Error, (!errors).[0] }

module TotalMatched =        
    
    let state = atom Map.empty
    let traceStatus = Status "TOTAL-MATCHED"
    
    let private cacheLifeTime = TimeSpan.FromMinutes 3.
    let private isLive d = DateTime.Now - d < cacheLifeTime

    let private getExisted gameId = 
        let total = state.Value
        let alive = total |> Map.filter(fun _ (d,_) -> isLive d)
        if alive.Count <> total.Count then
            state.Set alive
        Map.tryFind gameId alive |> Option.map snd 

    let private set gameId totalMatchedList = 
        state.Swap ( fun m -> 
            Map.add gameId (DateTime.Now, totalMatchedList) m )
        |> ignore
        

    let get ((eventId,marketId:int) as gameId) = async{
        let existed = getExisted gameId
        match existed with
        | Some found -> return found
        | _ ->
            match adminBetafirAuth.Value with
            | None ->  return []
            | Some auth ->
                set gameId []
                let! r = ApiNG.Services.listMarketCatalogue auth eventId
                match r with
                | Left x -> 
                    traceStatus.Set Logging.Error "%s" x
                    return []
                | Right marketCatalogue ->
                    traceStatus.Set Logging.Info "ok"
                    let totalMatched =
                        marketCatalogue 
                        |> List.map( fun x ->                             
                            let ttm = 
                                x.totalMatched 
                                |> Option.bind decimalToInt32Safety
                                |> Option.fold (+) 0                             
                            x.marketId.marketId, ttm)
                    set gameId totalMatched                    
                    return totalMatched }


module MarketBook = 

    type private ItemsMap = Map< int, DateTime * ApiNG.MarketBook >
    
    let state = atom Map.empty 
    let traceStatus = Status( "MARKET-BOOK" )
    let private cacheLifeTime = TimeSpan.FromSeconds 2.
    let private isLive d = DateTime.Now - d < cacheLifeTime

    let private getExisted marketsIds = 
        let total = state.Value
        let alive = total |> Map.filter(fun _ (d,_) -> isLive d)
        if alive.Count <> total.Count then
            state.Set alive
        
        let existed, missedIds = 
            marketsIds 
            |> List.map( fun marketId ->                 
                marketId, Map.tryFind marketId alive |> Option.map snd )
            |> List.partition ( snd >> Option.isSome) 
        let missedIds = List.map fst missedIds
        let existed = 
            existed 
            |> List.choose ( fun (x, y) -> y |> Option.map( fun y -> x,y)) 
            |> Map.ofList 
        existed, missedIds 

    let get marketsIds = async{
        let existed, missedIds = getExisted marketsIds
        if missedIds.IsEmpty then return existed else
            match adminBetafirAuth.Value with
            | None -> return existed
            | Some auth ->
                let! r = ApiNG.Services.listMarketBook auth missedIds
                match r with
                | Left x -> 
                    traceStatus.Set Logging.Error "%s" x
                    return Map.empty
                | Right r ->
                    traceStatus.Set Logging.Info "ok"
                    let readed =
                        r |> List.map( fun x -> x.marketId.marketId, x )
                        |> Map.ofList
                    state.Swap( fun m -> 
                        readed 
                        |> Map.map( fun _ x -> DateTime.Now,x) 
                        |> Map.union m )
                    |> ignore
                    return Map.union existed readed  }

let start = 
    let isstarted = ref false
    let lock' = obj()
    fun () -> 
        lock lock' <| fun () ->
            if !isstarted then () else
            start' "INPLAY" 0 0 Coupon.updateInplay
            start' "FOREPLAY" 0 0 Coupon.updateForeplay
            start'auth' "EVENTS" 0 0 Events.update
            start'auth' "MARKET-CATALOGUE" 0 0 MarketsCatalogue.update
            isstarted := true
        

type NewGame1 = Game * GameInfo * int
type UpdGame1 = GameId * GameInfo * int
type OutGame1 = GameId

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
    let games = getTodayGames()
    let pagelen = if pagelen < 1 then games.Length else pagelen
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


let getGame gameId =     
    getTodayGames()
    |> List.tryFind( function {gameId = gameId'},_ when gameId' = gameId -> true | _ -> false)
    |> Option.map snd 
    |> Option.map ( fun x -> x, x.GetHash() ) 





    




