module Betfair.Football.Coupon


open System

open Betfair
open Betfair.Football
open Betfair.Football.Services

let ApiNgEvents = Atom.todayValueRef "COUPON-EVENTS" Map.empty  Atom.Logs.none

let getExistedApiNgEvents ids = async{
    let! events = ApiNgEvents.Get()
    let existedIds, missingIds = ids |> List.partition( fun k -> events.ContainsKey k )
    let existed = existedIds |> List.map(fun id -> id, events.[id]) |> Map.ofList
    return existed, missingIds }

[<AutoOpen>]
module private Helpers = 

    

    let ``initialized`` = "initialized"
    let ``started`` = "started"
    let ``stoped`` = "stoped"
    let ``ok`` = "Ok"
    let (~%%) = Async.Start
    
    let initStatus what = Atom.status what ``initialized``
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



       

let private processEvents() = async{        
    let! auth = Login.auth.Get()
    match auth with
    | None ->         
        do! Async.Sleep 5000
        return Left "waiting for auth to read"
    | Some auth ->
        let! inplay = Coupon.Inplay.Get()
        let! today1 = Coupon.Today1.Get()
        let! today2 = Coupon.Today2.Get()
        let allGames = inplay @ today1 @ today2

        let! _, missingIds =
            allGames 
            |> List.map(fun ({gameId = gameId},_) -> gameId )
            |> getExistedApiNgEvents
        if missingIds.IsEmpty then return Right [] else
        let! newEvents = 
            List.map fst missingIds
            |> ApiNG.Services.listEvents auth 
        match newEvents with
        | Right newEvents as r when newEvents.Length = missingIds.Length ->
            let! existedEvents = ApiNgEvents.Get()
            let newValue = 
                newEvents |> List.choose( fun e -> 
                    missingIds |> List.tryFind( fst >> ( (=)  e.event.id ) )
                    |> Option.map( fun gameId -> gameId, e) )
                |> Map.ofList
                |> Map.union existedEvents 
            do! ApiNgEvents.Set newValue
            return r
        | Right newEvents  ->
            return Left <| sprintf "-aping-events responsed size %d mismatch, waiting %d" newEvents.Length missingIds.Length 
        | x -> return x }

let Inplay = initStatus "INPLAY-GAMES"
let Today = initStatus "TODAY-GAMES" 
let Events = initStatus "TODAY-APING-EVENTS" 

let start = 
    start' Inplay Coupon.updateInplay
    start' Today Coupon.updateToday
    start' Events processEvents
    fun () -> ()

