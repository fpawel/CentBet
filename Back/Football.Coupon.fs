module Betfair.Football.Coupon


open System

open Betfair
open Betfair.Football
open Betfair.Football.Services

let ApiNgEvents = Atom.todayValueRef "COUPON-EVENTS" Map.empty  Atom.Logs.none

[<AutoOpen>]
module private Helpers = 

    let readApiNgEvents ids = async{
        let! events = ApiNgEvents.Get()
        let existedIds, missingIds = ids |> List.partition( fun k -> events.ContainsKey k )
        let existed = existedIds |> List.map(fun id -> id, events.[id]) |> Map.ofList
        return existed, missingIds }

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
        let! xs = Coupon.Inplay.Get()
        let! _, missingIds =
            xs 
            |> List.map(fun ({gameId = eventId,_},_) -> eventId )
            |> readApiNgEvents
        let! newEvents' = ApiNG.Services.listEvents auth missingIds                     
        match newEvents' with
        | Left _ -> ()
        | Right newEvents ->
            let! existedEvents = ApiNgEvents.Get()
            do!
                newEvents |> List.map(fun r -> r.event.id,r)
                |> Map.ofList
                |> Map.union existedEvents 
                |> ApiNgEvents.Set 
        return newEvents' }

let Inplay = initStatus "INPLAY-GAMES"
let Today = initStatus "TODAY-GAMES" 
let Events = initStatus "TODAY-APING-EVENTS" 

let start = 
    start' Inplay Coupon.updateInplay
    start' Today Coupon.updateToday
    start' Events processEvents

    

    fun () -> ()

