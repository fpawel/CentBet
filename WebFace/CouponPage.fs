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

type Event = 
    {   gameId : GameId
        country : string option
        openDate : DateTime option }
    static member id x = x.gameId

let events = 
    //ListModel.CreateWithStorage Event.id (Storage.LocalStorage "CentBetApiNgEvents" Serializer.Default)
    ListModel.Create Event.id []




type Meetup =
    {   game : Game
        gameInfo : Var<GameInfo>
        country : Var<string>
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
let varSelectedCountry = Var.Create None


let tryGetEvent gameId = events.Value |> Seq.tryFind( fun {gameId = gameId'} -> gameId = gameId' )

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
            country = Var.Create country } )

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
    let! events = events.View    
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


let renderMeetup1 (x : Meetup) = 
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
    trAttr [] [   
        td[ vinfo ( fun y -> let page,n = y.order in sprintf "%d.%d" page n) ]        
        td[ span' "home-team" <| Doc.TextNode x.game.home
            span' "game-status" <| vinfo (fun y -> y.summary) 
            span' "away-team" <| Doc.TextNode x.game.away 
            span' "game-status" <| vinfo (fun y -> y.status)  ]



        kef' true (fun y -> %% y.winBack)
        kef' false (fun y -> %% y.winLay)
        kef' true (fun y -> %% y.drawBack)
        kef' false (fun y -> %% y.drawLay)
        kef' true (fun y -> %% y.loseBack)
        kef' false (fun y -> %% y.loseLay) 
        td[ Doc.TextView x.country.View ]
        ]

let renderMeetup  ( x : Meetup, inplayOnly, selectedCountry) =
    match inplayOnly, x.gameInfo.Value.playMinute, selectedCountry with
    | true, None, _ -> Doc.Empty 
    | _, _, Some selectedCountry 
        when selectedCountry <> "" &&  
             selectedCountry <> x.country.Value -> Doc.Empty 
    | _ -> 
        x.gameInfo.View |>  View.Map(  fun i ->
            if [i.drawBack; i.drawLay; i.winBack; i.winLay; i.loseBack; i.loseLay] |> List.exists( Option.isSome ) then
                renderMeetup1 x :> Doc
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
    printfn "render all countries menu..."
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
    printfn "render countries menu..."
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
                    varInplayOnly.Value <- true )
                if isInplayOnly then yield attr.``class`` "active"]  [text "В игре"]
            aAttr [
                yield attr.href "#"
                yield Attr.Handler "click" (fun e x -> 
                    varSelectedCountry.Value <- None
                    varInplayOnly.Value <- false )
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
        events.Add  { gameId = gameId; country = country; openDate = openDate } 

    for m in meetups.Value do
        m.country.Value <- tryGetCountry m.game.gameId } 


let Render() =  
    
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
        | false, _, _ -> h1 [ text "Нет данных о футболе на сегодня"]
        | true, false, true -> h1 [ text "Футбол сегодня ещё не начался"]            
        | _ -> 
            tableAttr [] [ tbody [ renderMeetups() ] ] )
    |> Doc.EmbedView    

