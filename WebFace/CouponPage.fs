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
    let x = 
        try
            ListModel.CreateWithStorage EventCatalogue.id (Storage.LocalStorage k Serializer.Default)
        with e  ->
            printfn "error when restoring %A - %A" k e
            ListModel.Create EventCatalogue.id []
    printfn "%A - %d, %A" k x.Length dt
    x

type VarKef = Var<decimal option>

type Meetup =
    {   game : Game
        
        playMinute  : Var<int option>
        status      : Var<string>
        summary     : Var<string>
        order       : Var<int * int>
        winBack     : VarKef
        winLay      : VarKef
        drawBack    : VarKef
        drawLay     : VarKef
        loseBack    : VarKef
        loseLay     : VarKef
        country : Var<string>
        totalMatched : Var<int option>
        mutable hash : int }
    static member id x = x.game.gameId

type Meetups = ListModel<GameId,Meetup>
let meetups = ListModel.Create Meetup.id []
let varDataRecived = Var.Create false
let varCurrentPageNumber = Var.Create 0
let varPagesCount = Var.Create 1

let updateTotalMatched gameId toltalMatched = 
    match meetups.TryFindByKey gameId with
    | Some game -> game.totalMatched.Value <- Some toltalMatched
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
    |> List.map ( fun (game : Game, i : GameInfo, hash) -> 
        let country = tryGetCountry game.gameId  
        {   game = game
            
            playMinute  = Var.Create i.playMinute
            status      = Var.Create i.status
            summary     = Var.Create i.summary
            order       = Var.Create i.order
            winBack     = Var.Create i.winBack
            winLay      = Var.Create i.winLay
            drawBack    = Var.Create i.drawBack
            drawLay     = Var.Create i.drawLay
            loseBack    = Var.Create i.loseBack
            loseLay     = Var.Create i.loseLay
            hash = hash 
            country = Var.Create country 
            totalMatched = Var.Create None} )

    |> Seq.append existedMeetups 
    |> Seq.sortBy ( fun x -> x.order.Value )  
    |> Seq.iter meetups.Add

let doc (x :Elt) = x :> Doc

let renderMeetup (x : Meetup) = 
    let tx x = td[ x ]
    let tx0 x = td[ Doc.TextNode x ]
        
    let span' ``class`` x = 
        spanAttr [ attr.``class`` ``class`` ] [x]
    
    let kef' back v = 
        doc <| tdAttr [attr.``class`` (if back then  "kef kef-back" else "kef kef-lay" ) ] [             
            View.FromVar v |> View.Map formatDecimalOption |> textView   ] 
    let bck' = kef' true 
    let lay' = kef' false 

    [   doc <| td[ x.order.View |> View.Map ( fun (page,n) -> sprintf "%d.%d" page n) |> textView ]
        doc <| tdAttr [ attr.``class`` "home-team" ]  [Doc.TextNode x.game.home ] 
        View.Do{
                let! playMinute = x.playMinute.View
                let! summary = x.summary.View
                
                return 
                    match playMinute with
                    | Some _ | _ when summary <> "" -> 
                        doc <| tdAttr [ attr.``class`` "game-status"] [ text summary ]  
                    | _ -> doc <| td [] } |> Doc.EmbedView
        doc <| tdAttr [ attr.``class`` "away-team"]   [Doc.TextNode x.game.away ] 
        doc <| tdAttr [ attr.``class`` "game-status"] [ textView x.status.View ] 
        bck' x.winBack
        lay' x.winLay
        bck' x.drawBack
        lay' x.drawLay
        bck' x.loseBack
        lay' x.loseLay 
        x.totalMatched.View |> View.Map( function 
            | None -> td [] 
            | Some totalMatched -> 
                    tdAttr [attr.``class`` "game-gpb"] [text <| sprintf "%d" totalMatched ]  ) 
            |> Doc.EmbedView         
        doc <| tdAttr [ attr.``class`` "game-country" ] [ Doc.TextView x.country.View ]  ] 
    |> tr 

let renderGamesHeaderRow = [   
    th [text "№"]
    th [text "1"]
    th [] 
    th [text "2"]
    th []
    thAttr [ attr.colspan "2" ] [text "1"]
    thAttr [ attr.colspan "2" ] [text "×"]
    thAttr [ attr.colspan "2" ] [text "2"] 
    th  [text "GPB"]  
    th  [] ] 
    
    
let renderPagination =  Doc.EmbedView <| View.Do{
    let! pagescount = varPagesCount.View
    if pagescount<2 then return Doc.Empty else
    let! npage = varCurrentPageNumber.View
    return 
        [   for n in 0..pagescount ->
                let aattrs = [
                    yield attr.href "#"
                    yield Attr.Handler "click" (fun e x -> 
                        varCurrentPageNumber.Value <- n  )
                    if n=npage then yield attr.``class`` "w3-green"] 
                let v = text <| sprintf "Страница %d" (n+1)
                doc <| li[ aAttr aattrs [v] ] ]
        |> Doc.Concat }

let renderСoupon = 
    divAttr [attr.``class`` "w3-container"][
        divAttr [ attr.``class`` "w3-center" ] [
            ulAttr [attr.``class`` "w3-pagination"] [ renderPagination ] ]   
        table[   
            thead[ trAttr [ Attr.Class "coupon-header-row" ] ( Seq.map doc renderGamesHeaderRow) ]
            tbody [
                meetups.View |> View.Map( Seq.map (renderMeetup >> doc)  >> Doc.Concat )
                |> Doc.EmbedView   ] ] ]
    
    

let stvr<'a when 'a : equality> (x:Var<'a>) (value : 'a) =
    if x.Value <> value then
        x.Value <- value

let updateCoupon (newGms,updGms,outGms) = 
    
    if List.isEmpty newGms |> not then
        printfn "adding new games %d" newGms.Length
        addNewGames newGms    

    if Seq.isEmpty outGms |> not then
        outGms |> Set.iter( meetups.RemoveByKey )
        printfn "removing out games %d" ( Seq.length outGms)
    
    updGms |> List.iter ( fun (gameId, i : GameInfo, hash) -> 
        match meetups.TryFindByKey gameId with
        | Some x when x.hash  <> hash ->
            x.hash <- hash
            stvr x.playMinute i.playMinute 
            stvr x.status i.status
            stvr x.summary i.summary
            stvr x.order i.order
            stvr x.winBack i.winBack
            stvr x.winLay i.winLay
            stvr x.drawBack i.drawBack
            stvr x.drawLay i.drawLay
            stvr x.loseBack i.loseBack
            stvr x.loseLay i.loseLay
        | _ -> () )

let processCoupon() = async{
    let request =
        meetups.Value 
        |> Seq.map(fun m -> m.game.gameId, m.hash)
        |> Seq.toList
    let pagelen = 30
    let! newGms,updGms,outGms, gamesCount = CentBet.Remote.getCouponPage (request, varCurrentPageNumber.Value, pagelen)
    let pagesCount = gamesCount / pagelen
    if varPagesCount.Value <> pagesCount then
        varPagesCount.Value <- pagesCount
    if varCurrentPageNumber.Value  > pagesCount then
        varCurrentPageNumber.Value <- 0
    if not varDataRecived.Value then
        varDataRecived.Value <- true
    updateCoupon (newGms,updGms,outGms) } 

module ServerBetfairsSession = 
    let mutable private hasServerBetfairsSession = true
    let check() = async{ 
        let! x = CentBet.Remote.hasServerBetfairsSession()
        hasServerBetfairsSession <- x }
    let has() = hasServerBetfairsSession
    let hasNot() = not hasServerBetfairsSession

let processEvents() = async{
    let events' = eventsCatalogue.Value
    let request =
        meetups.Value 
        |> Seq.choose(fun m -> 
            match events' |> Seq.tryFind( fun e -> e.gameId = m.game.gameId) with
            | None -> Some m.game.gameId
            | _ -> None )
        |> Seq.toList
    if request.IsEmpty || ServerBetfairsSession.hasNot() then () else
    
    let! newEvents =  CentBet.Remote.getEventsCatalogue request
    
    for gameId, name, country in newEvents do
        eventsCatalogue.Add  { gameId = gameId; country = country; markets = []  } 
    for m in meetups.Value do
        m.country.Value <- tryGetCountry m.game.gameId } 

let processMarkets() = async{
    let events' = 
        eventsCatalogue.Value |> Seq.filter( fun x -> x.markets.IsEmpty )
        |> Seq.toList

    if List.isEmpty events' || ServerBetfairsSession.hasNot() then () else  

    for ev in events' do
        let! m = CentBet.Remote.getMarketsCatalogue ev.gameId
        match m with
        | None -> ()
        | Some value ->             
            value 
            |> List.choose( fun (_,_,_,x) -> x ) 
            |> List.fold (+) 0
            |> updateTotalMatched ev.gameId
            let markets = value |> List.map( fun (marketId, marketName, runners,_) -> 
                {   marketId = marketId
                    marketName = marketName 
                    runners = runners |> List.map( fun (runnerNamem, selectionId) -> 
                        {   selectionId = selectionId
                            runnerName = runnerNamem } ) } )
            eventsCatalogue.UpdateBy (fun x -> Some {x with markets = markets}) ev.gameId } 
   
type Work = 
    {   what : string
        sleepInterval : int
        sleepErrorInterval : int 
        work : unit -> Async<unit> }
    
    static member ``new`` (what,sleepInterval,sleepErrorInterval) work =
        {   what = what
            sleepInterval  = sleepInterval
            sleepErrorInterval = sleepErrorInterval
            work = work }
        |> Work.run
                    
    static member loop x = async{        
        try
            do! x.work()
            do! Async.Sleep x.sleepInterval 
        with e ->
            printfn "task error %A : %A" x.what e
            do! Async.Sleep x.sleepErrorInterval 
        return! Work.loop x }

    static member run x = Async.Start <| async { 
        printfn "task %A : started" x.what
        do! Work.loop x
        printfn "task %A : terminated" x.what }




[<Require(typeof<Resources.W3Css>)>]
[<Require(typeof<Resources.CouponCss>)>]
[<Require(typeof<Resources.UtilsJs>)>]
let Render() =
    Work.``new`` ("COUPON", 0, 0) ServerBetfairsSession.check    
    Work.``new`` ("CHECK-SERVER-BETFAIRS-SESSION", 0, 0) processCoupon    
    Work.``new`` ("EVENTS-CATALOGUE", 0, 0) processEvents
    Work.``new`` ("MARKETS-CATALOGUE", 0, 0) processMarkets
    varDataRecived.View |> View.Map ( function
        | false -> h1 [ text "Данные загружаются с сервера. Пожалуйста, подождите."] 
        | _ -> renderСoupon )     
    |> Doc.EmbedView    
   

