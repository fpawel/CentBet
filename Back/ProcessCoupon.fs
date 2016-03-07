module CentBet.ProcessCoupon

open System
open WebSharper

open CentBet.Prelude
open Betfair
open Betfair.Football
open Betfair.Football.Services

module Events = 
    let get,set = createTodayAtom1 Map.empty

    let getEvents ids = async{
        let! events = get()
        let existedIds, missingIds = ids |> List.partition( fun k -> events.ContainsKey k )
        let existed = existedIds |> List.map(fun id -> id, events.[id]) |> Map.ofList
        return existed, missingIds }

let start =
    let (~%%) = Async.Start
    %% async{
        while true do        
            let! _ = Coupon.updateInplay()
            () }

    %% async{            
        while true do        
            let! _ = Coupon.updateToday()
            () }

    %% async{            
        while true do 
            let! auth = Admin.BetfairAuth.get()
            match auth with
            | None -> ()
            | Some auth ->
                let! xs = Coupon.Inplay.get()
                let! _, missingIds =
                    xs 
                    |> List.map(fun ({gameId = eventId,_},_) -> eventId )
                    |> Events.getEvents

                let! newEvents' = ApiNG.Services.listEvents auth missingIds
                let! existedEvents = Events.get()
                newEvents' |> List.map(fun r -> r.event.id,r)
                |> Map.ofList
                |> Map.union existedEvents 
                |> Events.set }
    fun () -> ()

[<Rpc>]
let get ( (reqGames,inplayOnly) as request)  = async{ 
        
    let reqIds, reqGames = List.ids reqGames fst
    let! games = async{ 
        let! inplay = Coupon.Inplay.get()
        if inplayOnly then return inplay else
        let! today1 = Coupon.Today1.get()
        let! today2 = Coupon.Today2.get()
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