[<WebSharper.Pervasives.JavaScript>]
module CentBet.Client.Coupon 

open System

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

open Betfair.Football

    
open Utils

type Event = 
    {   gameId : GameId
        eventName : string
        country : string 
        openDate : DateTime option }
    static member id x = x.gameId

let events = 
    //ListModel.CreateWithStorage Event.id (Storage.LocalStorage "CentBetApiNgEvents" Serializer.Default)
    ListModel.Create Event.id []




type Meetup =
    {   game : Game
        gameInfo : Var<GameInfo>
        country : Var<string>
        eventName : Var<string>
        mutable hash : int }
    static member id x = x.game.gameId
    static member viewGameInfo x = x.gameInfo.View
    static member inplay x = 
        x.gameInfo.Value.playMinute.IsSome
    static member notinplay x = 
        x.gameInfo.Value.playMinute.IsNone



type Meetups = ListModel<GameId,Meetup>

let meetups = ListModel.Create Meetup.id []
let varInplayOnly = Var.Create true


let tryGetEvent gameId = events.Value |> Seq.tryFind( fun {gameId = gameId'} -> gameId = gameId' )

let tryGetCountry gameId  =
    tryGetEvent gameId  |> function
        | Some e -> e.country,e.eventName
        | _ -> "",""

let addNewGames newGames = 
    let existedMeetups = meetups.Value |> Seq.toList                    
    meetups.Clear()
    newGames 
    |> List.map ( fun (game : Game, gameInfo, hash) -> 
        let country, eventName = tryGetCountry game.gameId  
        {   game = game
            gameInfo = Var.Create gameInfo
            hash = hash 
            country = Var.Create country
            eventName = Var.Create eventName } )

    |> Seq.append existedMeetups 
    |> Seq.sortBy ( fun x -> x.gameInfo.Value.order )  
    |> Seq.iter meetups.Add

let updateCoupon (newGms,updGms,outGms) =             
    
    if List.isEmpty newGms |> not then
        printfn "adding new games %d" newGms.Length
        addNewGames newGms    

    if Seq.isEmpty outGms |> not then
        outGms |> Set.iter( meetups.RemoveByKey )
        printfn "removing out games %d" ( Seq.length outGms)
    
    updGms |> List.iter ( fun (gameId, gameInfo, hash) -> 
        match meetups.TryFindByKey gameId with
        | None -> ()
        | Some x ->
            x.gameInfo.Value <- gameInfo
            x.hash <- hash )

let viewGames =     
    View.Do {
        let! meetups = meetups.View 
        let! onlyInplay = varInplayOnly.View
        let! games = 
            meetups 
            |> Seq.filter( fun x -> not onlyInplay || x.gameInfo.Value.playMinute.IsSome)
            |> Seq.map( fun x -> 
                x.gameInfo.View |> View.Map( fun i -> x.game, i ) )
            |> View.Sequence
        return games } 

let viewCountries onlyInplay =     
    View.Do {
        let! events = events.View
        let events = events |> Seq.map ( fun evt -> evt.gameId, evt) |> Map.ofSeq
        let! games = viewGames 
        return
            games 
            |> Seq.choose( fun (g,_) -> 
                events.TryFind g.gameId
                |> Option.map( fun e -> e.country) ) 
            |> Seq.distinct
            |> Seq.sort  } 


let row  ( x : Meetup, inplayOnly) =
    if inplayOnly && x.gameInfo.Value.playMinute.IsNone then Doc.Empty else
    let tx x = td[ x ]
    let tx0 x = td[ Doc.TextNode x ]
    let gameInfo f = 
        View.Map f x.gameInfo.View
        |> Doc.TextView
        |> tx    
    let (~%%) = formatDecimalOption
    trAttr [] [   
        gameInfo ( fun y -> 
            let page,n = y.order
            sprintf "%d.%d" page n)
        tx0 x.game.home
        tx0 x.game.away
        td[ Doc.TextView x.country.View ]
        gameInfo (fun y -> y.status)
        gameInfo (fun y -> y.summary)
        gameInfo (fun y -> %% y.winBack)
        gameInfo (fun y -> %% y.winLay)
        gameInfo (fun y -> %% y.drawBack)
        gameInfo (fun y -> %% y.drawLay)
        gameInfo (fun y -> %% y.loseBack)
        gameInfo (fun y -> %% y.loseLay) ]
    :> Doc


let table () = 
    meetups.View 
    |> Doc.BindSeqCachedView  ( fun x -> 
        View.Do {
            let! x = x
            let! inplayOnly = varInplayOnly.View
            return x, inplayOnly } 
        |> View.Map row
        |> Doc.EmbedView )


let Menu() = 
    varInplayOnly.View
    |> View.Map( fun isInplayOnly -> 
        let countries =  
            viewCountries isInplayOnly |> View.Map( fun countries -> 
                countries |> Seq.map( fun country ->
                    aAttr [
                        yield attr.href "#"
                        yield Attr.Handler "click" (fun e x -> () ) ]
                        [text country]  
                    :> Doc )
                |> Doc.Concat )
            |> Doc.EmbedView

        [   aAttr [
                yield attr.href "#"
                yield Attr.Handler "click" (fun e x -> varInplayOnly.Value <- true )
                if isInplayOnly then yield attr.``class`` "active"] 
                [text "В игре"] :> Doc
            aAttr [
                yield attr.href "#"
                yield Attr.Handler "click" (fun e x -> varInplayOnly.Value <- false )
                if not isInplayOnly then yield attr.``class`` "active"] 
                [text "Все матчи"] :> Doc
            countries ]        
        |> Doc.Concat )
    |> Doc.EmbedView

let MenuToday() = 
    varInplayOnly.View
    |> View.Map( fun isInplayOnly -> 
        liAttr 
            [ if not isInplayOnly then yield attr.``class`` "active"] 
            [ aAttr [
                attr.href "#"
                Attr.Handler "click" (fun e x -> varInplayOnly.Value <- false )] [text "Все матчи"] ]  )
    |> Doc.EmbedView




let processCoupon() = async{
    let request =
        meetups.Value 
        |> Seq.map(fun m -> m.game.gameId, m.hash)
        |> Seq.toList
    let! newGms,updGms,outGms = CentBet.Remote.getCoupon (request, varInplayOnly.Value)
    updateCoupon (newGms,updGms,outGms) } 

let processEvents() = async{
    let events' = events.Value
    let request =
        meetups.Value 
        |> Seq.choose(fun m -> 
            match events' |> Seq.tryFind( fun e -> e.gameId = m.game.gameId) with
            | None -> Some m.game.gameId
            | _ -> None )
        |> Seq.toList
    if request.IsEmpty then () else
    let! newEvents =  CentBet.Remote.getApiNgEvents request
    for gameId, name, country, openDate in newEvents do
        events.Add  { gameId = gameId; eventName = name; country = country; openDate = openDate } 

    for m in meetups.Value do
        let country, eventName = tryGetCountry m.game.gameId
        m.country.Value <- country
        m.eventName.Value <- eventName } 


let Main () =  
    
    async{
        while true do
            try
                do! processCoupon () 
                do! processEvents() 
            with e ->
                printfn "error updating coupon : %A" e
                do! Async.Sleep 5000 } 
    |> Async.Start

        
    View.Do {
        let! isInplayOnly = varInplayOnly.View
        let! meetups = meetups.View

        let hasToday = meetups |> Seq.isEmpty |> not

        let hasInplay = 
            meetups 
            |> Seq.filter Meetup.inplay 
            |> Seq.isEmpty 
            |> not

        return hasToday, hasInplay, isInplayOnly } 
    |> View.Map( function  
        | false, _, _ -> h1 [ text "Произошла какая-то ошибка :( Приносим свои извинения. "]
        | true, false, true -> h1 [ text "Футбольные матчи сегодня ещё не начались"]            
        | _ -> 
            tableAttr [] [ tbody [ table() ] ] )
    |> Doc.EmbedView    

