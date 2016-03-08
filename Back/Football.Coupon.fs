module Betfair.Football.Coupon


open System

open Betfair
open Betfair.Football
open Betfair.Football.Services

let Events = Atoms.TodayValueRef ("COUPON-EVENTS",Map.empty)

let getEvents ids = async{
    let! events = Events.Get()
    let existedIds, missingIds = ids |> List.partition( fun k -> events.ContainsKey k )
    let existed = existedIds |> List.map(fun id -> id, events.[id]) |> Map.ofList
    return existed, missingIds }


module Status =
    open Status
    let Inplay = Status("INPLAY-GAMES", "not recived")
    let Today = Status( "TODAY-GAMES" , "not recived")
    let Events = Status( "TODAY-APING-EVENTS" , "not recived")

    


let start =
    let (~%%) = Async.Start
    %% async{
        Status.Inplay.Set Logging.Info "started"
        while true do     
            let! x = Coupon.updateInplay()
            x |> Status.Inplay.Set1( fun (count, next) -> sprintf "%d games, next is %A" count next) 
        Status.Inplay.Set Logging.Error "stoped" }

    %% async{            
        Status.Today.Set Logging.Info "started"
        while true do        
            let! x = Coupon.updateToday()
            x |> Status.Inplay.Set1 ( fun (count, next) -> sprintf "%d games, next is %A" count next)
        Status.Today.Set Logging.Error "stoped" }

    %% async{            
        Status.Events.Set Logging.Info "started"
        while true do 
            let! auth = Login.auth.Get()
            match auth with
            | None -> Status.Events.Set Logging.Warn "waiting for auth to read"
            | Some auth ->
                let! xs = Coupon.Inplay.Get()
                let! _, missingIds =
                    xs 
                    |> List.map(fun ({gameId = eventId,_},_) -> eventId )
                    |> getEvents
                let! newEvents' = ApiNG.Services.listEvents auth missingIds                
                newEvents' 
                |> Status.Events.Set1 ( fun xs -> sprintf "%d readed" xs.Length)
                     
                match newEvents' with
                | Left _ -> ()
                | Right newEvents ->
                    let! existedEvents = Events.Get()
                    newEvents |> List.map(fun r -> r.event.id,r)
                    |> Map.ofList
                    |> Map.union existedEvents 
                    |> Events.Set 
        Status.Events.Set Logging.Error "stoped" }
    fun () -> ()

