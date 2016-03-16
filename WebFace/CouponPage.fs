﻿[<WebSharper.Pervasives.JavaScript>]
module CentBet.Client.Coupon 

open System

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

open Betfair.Football

    
open Utils

///Information about the Runners (selections) in a market
type RunnerCatalog = {   
    selectionId : int 
    runnerName : string }

type MarketCatalogue = {   
    marketId : int
    marketName : string 
    runners : RunnerCatalog list }

type EventCatalogue = 
    {   gameId : GameId
        country : string option  
        markets : MarketCatalogue list }
    static member id x = x.gameId

let eventsCatalogue = 
    let k = "CentBetEventsCatalogue"
    let dt = LocalStorage.checkTodayKey "CentBetEventsCatalogueCreated" k
    let x = ListModel.CreateWithStorage EventCatalogue.id (Storage.LocalStorage k Serializer.Default)
    printfn "%A - %d, %A" k x.Length dt
    x


type Meetup =
    {   game : Game
        gameInfo : Var<GameInfo>
        country : Var<string>
        totalMatched : Var<decimal option>
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
let varDataRecived = Var.Create false
let varSelectedCountry = Var.Create None

let updateTotalMatched gameId toltalMatched = 
    match meetups.TryFindByKey gameId with
    | Some game -> game.totalMatched.Value <- toltalMatched
    | _ -> ()


let tryGetEvent gameId = eventsCatalogue.Value |> Seq.tryFind( fun {gameId = gameId'} -> gameId = gameId' )

let tryGetCountry gameId  =
    tryGetEvent gameId  |> function
        | Some {country = Some country} -> country
        | _ -> ""

let addNewGames newGames = 
    let existedMeetups = meetups.Value |> Seq.toList                    
    meetups.Clear()
    newGames 
    |> List.map ( fun (game : Game, gameInfo, hash) -> 
        let country = tryGetCountry game.gameId  
        {   game = game
            gameInfo = Var.Create gameInfo
            hash = hash 
            country = Var.Create country 
            totalMatched = Var.Create None} )

    |> Seq.append existedMeetups 
    |> Seq.sortBy ( fun x -> x.gameInfo.Value.order )  
    |> Seq.iter meetups.Add



let viewGames = 
    View.Do {
        let! meetups = meetups.View 
        if Seq.isEmpty meetups then return Seq.empty  else 
        let! onlyInplay = varInplayOnly.View
        let! games = 
            meetups 
            |> Seq.filter( fun x -> not onlyInplay || x.gameInfo.Value.playMinute.IsSome)
            |> Seq.map( fun x -> 
                x.gameInfo.View |> View.Map( fun i -> x.game, i ) )
            |> View.Sequence 
        return games } 
    

let viewCountries() = View.Do {    
    let! events = eventsCatalogue.View    
    let events = events |> Seq.map ( fun evt -> evt.gameId, evt) |> Map.ofSeq    
    let! games = viewGames     
    return
        games 
        |> Seq.choose( fun (g,_) -> 
            events.TryFind g.gameId
            |> Option.map ( fun e -> e.country ) ) 
        |> Seq.filter ( (<>) None)
        |> Seq.choose id            
        |> Seq.distinct
        |> Seq.sort  } 


let renderMeetup1 (x : Meetup, countryIsSelected) = 
    let tx x = td[ x ]
    let tx0 x = td[ Doc.TextNode x ]
    
    let vinfo f = 
        View.Map f x.gameInfo.View
        |> Doc.TextView
        
    let span' ``class`` x = 
        spanAttr [ attr.``class`` ``class`` ] [x]
    let kef' back f = 
        tdAttr [attr.``class`` (if back then  "kef kef-back" else "kef kef-lay" ) ] [ vinfo f ]

    let (~%%) = formatDecimalOption
    [   yield td[ vinfo ( fun y -> let page,n = y.order in sprintf "%d.%d" page n) ]        
        yield tdAttr [ attr.``class`` "home-team" ]  [Doc.TextNode x.game.home ]
        yield tdAttr [ attr.``class`` "game-status"] [vinfo (fun y -> y.summary) ]
        yield tdAttr [ attr.``class`` "away-team"]   [Doc.TextNode x.game.away ]
        yield tdAttr [ attr.``class`` "game-status"] [vinfo (fun y -> y.status)]
        yield td [             
            x.totalMatched.View |> View.Map( function 
                | None -> Doc.Empty
                | Some m -> text <| sprintf "%2.2M GPB" m) |> Doc.EmbedView ]

        yield kef' true (fun y -> %% y.winBack)
        yield kef' false (fun y -> %% y.winLay)
        yield kef' true (fun y -> %% y.drawBack)
        yield kef' false (fun y -> %% y.drawLay)
        yield kef' true (fun y -> %% y.loseBack)
        yield kef' false (fun y -> %% y.loseLay) 
        if not countryIsSelected then             
            yield  td [ span' "game-status" <| Doc.TextView x.country.View ] ]
    

let renderMeetup  ( x : Meetup, inplayOnly, selectedCountry) =
    match inplayOnly, x.gameInfo.Value.playMinute, selectedCountry with
    | true, None, _ -> Doc.Empty 
    | _, _, Some selectedCountry 
        when selectedCountry <> "" &&  
             selectedCountry <> x.country.Value -> Doc.Empty 
    | _ -> 
        x.gameInfo.View |>  View.Map(  fun i ->
            if [i.drawBack; i.drawLay; i.winBack; i.winLay; i.loseBack; i.loseLay] |> List.exists( Option.isSome ) then
                renderMeetup1(x,selectedCountry.IsSome)  
                |> List.map( fun x -> x :> Doc)
                |> tr :> Doc
            else
                Doc.Empty )
        |> Doc.EmbedView

let renderMeetups () = 
    meetups.View 
    |> Doc.BindSeqCachedView  ( fun x -> 
        View.Do {
            let! x = x
            let! inplayOnly = varInplayOnly.View
            let! selectedCountry = varSelectedCountry.View
            return x, inplayOnly, selectedCountry } 
        |> View.Map renderMeetup
        |> Doc.EmbedView )


let renderMenuItemAllCountries() = 
    varSelectedCountry.View |> View.Map( fun selectedCountry ->         
        aAttr [
            match selectedCountry with
            | Some selectedCountry when selectedCountry <> "" -> ()
            | _ -> yield attr.``class`` "active"
            yield attr.href "#"
            yield Attr.Handler "click" (fun e x -> 
                varSelectedCountry.Value <- None ) ]
            [text "Все страны"]  
        :> Doc )        
    |> Doc.EmbedView

let renderMenuItemCountry selectedCountry country  =
    aAttr [
        match selectedCountry with
        | Some selectedCountry when selectedCountry = country -> 
            yield attr.``class`` "active"
        | _ -> ()
        yield attr.href "#"
        yield Attr.Handler "click" (fun e x -> 
            varSelectedCountry.Value <- Some country ) ]
        [text country]

let renderMenuCountries() = 
    View.Do{
        let! countries = viewCountries ()
        let! selectedCountry = varSelectedCountry.View
        
        return countries, selectedCountry } 
    |> View.Map( fun (countries,selectedCountry) -> 
        
        if Seq.isEmpty countries then Doc.Empty else
        countries |> Seq.map( fun country ->
            renderMenuItemCountry selectedCountry country :> Doc )
        |> Doc.Concat 
        |> Doc.Append (renderMenuItemAllCountries() ) )
    |> Doc.EmbedView

let renderMenusInplay() = 
    varInplayOnly.View
    |> View.Map( fun isInplayOnly -> 
        [   aAttr [
                yield attr.href "#"
                yield Attr.Handler "click" (fun e x -> 
                    varSelectedCountry.Value <- None
                    varInplayOnly.Value <- true 
                    varDataRecived.Value <- false  )
                if isInplayOnly then yield attr.``class`` "active"]  [text "В игре"]
            aAttr [
                yield attr.href "#"
                yield Attr.Handler "click" (fun e x -> 
                    varSelectedCountry.Value <- None
                    varInplayOnly.Value <- false 
                    varDataRecived.Value <- false )
                if not isInplayOnly then yield attr.``class`` "active"]  [text "Все матчи"]  ] 
        |> List.map ( fun x -> x :> Doc )
        |> Doc.Concat )
    |> Doc.EmbedView

let RenderMenu() = 
    [   renderMenusInplay()
        renderMenuCountries() ]
    |> Doc.Concat

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

let processCoupon() = async{
    let request =
        meetups.Value 
        |> Seq.map(fun m -> m.game.gameId, m.hash)
        |> Seq.toList
    let! newGms,updGms,outGms = CentBet.Remote.getCoupon (request, varInplayOnly.Value)
    updateCoupon (newGms,updGms,outGms) } 

let processEvents() = async{
    let events' = eventsCatalogue.Value
    let request =
        meetups.Value 
        |> Seq.choose(fun m -> 
            match events' |> Seq.tryFind( fun e -> e.gameId = m.game.gameId) with
            | None -> Some m.game.gameId
            | _ -> None )
        |> Seq.toList
    if request.IsEmpty then do! Async.Sleep 5000 else
    let! newEvents =  CentBet.Remote.getEventsCatalogue request
    for gameId, name, country in newEvents do
        eventsCatalogue.Add  { gameId = gameId; country = country; markets = []  } 
    for m in meetups.Value do
        m.country.Value <- tryGetCountry m.game.gameId } 

let processMarkets() = async{
    let events' = 
        eventsCatalogue.Value |> Seq.filter( fun x -> x.markets.IsEmpty )
        |> Seq.toList

    if List.isEmpty events' then () else
    let ev = events'.Head
    let! m = CentBet.Remote.getMarketCatalogue ev.gameId
    match m with
    | Choice1Of2 x -> printfn "error of market catalog request : %s" x
    | Choice2Of2 value -> 
        value |> List.choose( fun (_,_, _,totalMatched )-> totalMatched ) 
        |> function [] -> None | xs ->  Some <| List.sum xs 
        |> updateTotalMatched ev.gameId
        let markets = value |> List.map( fun (marketId, marketName, runners,_) -> 
            {   marketId = marketId
                marketName = marketName 
                runners = runners |> List.map( fun (runnerNamem, selectionId) -> 
                    {   selectionId = selectionId
                        runnerName = runnerNamem } ) } )
        eventsCatalogue.UpdateBy (fun x -> Some {x with markets = markets}) ev.gameId } 
    
let processTotalMatched() = async{
    let gamesIds =
        meetups.Value 
        |> Seq.map(fun m -> m.game.gameId)
        |> Seq.toList
    if List.isEmpty gamesIds then () else
    let gameId = gamesIds.Head
    
    let! m = CentBet.Remote.getEventTotalMatched gameId
    match m with
    | Choice1Of2 x -> printfn "error of total matched request : %s" x
    | Choice2Of2 value -> 
        updateTotalMatched gameId value } 


let rec workloop() = async{
    try
        do! processCoupon () 
        do! processEvents() 
        do! processMarkets()
        do! processTotalMatched()
        varDataRecived.Value <- true 
    with e ->
        printfn "error updating coupon : %A" e
        do! Async.Sleep 5000 
    return! workloop() }

let Render() =
    Async.Start  (workloop())
    View.Do {
        let! inplayOnly = varInplayOnly.View
        let! dataRecived = varDataRecived.View
        let! meetups = meetups.View
        let hasgames = meetups |> Seq.isEmpty |> not
        return hasgames, inplayOnly, dataRecived } 
    |> View.Map( function  
        | _,_,false -> h1 [ text "Данные загружаются с сервера. Пожалуйста, подождите."]
        | true, _, _ ->  tableAttr [] [ tbody [ renderMeetups() ] ] 
        | false, true, _ -> h1 [ text "Нет данных о разыгрываемых в настоящий момент футбольных матчах"]
        | false, false, _ -> h1 [ text "Нет данных о футбольных матчах на сегодня"] )
    |> Doc.EmbedView    

