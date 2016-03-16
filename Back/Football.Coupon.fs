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

let MarketCatalogue = Atom.todayValueRef "MARKET-CATALOGUE" Map.empty  Atom.Logs.none

let getMarketCatalogue ((eventId,_) as gameId : GameId ) = async{
    let! existedMarketCatalogue = MarketCatalogue.Get()
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
                do! MarketCatalogue.Update( fun m ->  Map.add gameId x m )                
            | _ -> ()
            return r }

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
    fun () -> ()

