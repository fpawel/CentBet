[<WebSharper.Pervasives.JavaScript>]
module CentBet.Client.Coupon 

open System

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html
    
open Utils
open Betfair.Football
open CentBet.Client.Football

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

type Meetups = ListModel<GameId,Meetup>
let meetups = ListModel.Create Meetup.id []

let varCurrentPageNumber = Var.Create 0
let varTargetPageNumber = Var.Create 0
let varPagesCount = Var.Create 1
let varColumnGpbVisible = Var.Create false
let varColumnCountryVisible = Var.Create false

let updateColumnGpbVisible() = 
    let hasGpb = meetups.Value |> Seq.exists( fun x -> x.totalMatched.Value.IsSome )
    if varColumnGpbVisible.Value <> hasGpb then
        varColumnGpbVisible.Value <- hasGpb

let updateColumnCountryVisible() = 
    let hasCountry = meetups.Value |> Seq.exists( fun x -> x.country.Value <> "")
    if varColumnCountryVisible.Value <> hasCountry then
        varColumnCountryVisible.Value <- hasCountry

module PageLen =
    let private localStorageKey = "pageLen"
    let private validateValue v = 
        if v < 10 then 30
        elif v > 40 then 40 
        else v

    let private var = 
        LocalStorage.getWithDef localStorageKey 30
        |> validateValue 
        |> Var.Create 

    let set value =
        let value = validateValue value
        if value <> var.Value then
            var.Value <- value 
            LocalStorage.set localStorageKey value

    let get() = validateValue var.Value

    let view = var.View

let updateTotalMatched gameId toltalMatched = 
    match meetups.TryFindByKey gameId with
    | Some game -> game.totalMatched.Value <- Some toltalMatched
    | _ -> ()

let tryGetEvent gameId = eventsCatalogue.Value |> Seq.tryFind( fun {gameId = gameId'} -> gameId = gameId' )

let tryGetCountry gameId  =
    tryGetEvent gameId  |> function
        | Some {country = Some country} -> country
        | _ -> ""

let getLastTotakMatched gameId = 
    eventsCatalogue.TryFindByKey gameId 
    |> Option.bind( fun e -> 
        match e.markets |> List.choose( fun m -> m.totalMatched ) with
        | [] -> None
        | v -> v |> List.sum |> Some )

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
            totalMatched = Var.Create <| getLastTotakMatched game.gameId} )

    |> Seq.append existedMeetups 
    |> Seq.sortBy ( fun x -> x.order.Value )  
    |> Seq.iter meetups.Add

let doc (x :Elt) = x :> Doc

module SettingsDialog = 
    
    let id' = "id-settings-dialog"
    let private (~%%) = attr.``class``

    let private buttonElt n pageLen =             
        buttonAttr [ 
            %% "w3-btn w3-teal"
            attr.style "margin: 10px; width: 50px; height: 50px;"
            on.click (fun _ _ -> 
                match n, pageLen with
                | true, 40 -> ()
                | false, 10 -> ()
                | _ ->  PageLen.set <| pageLen + (if n then 1 else -1) ) ]
            [text <| if n then "+" else "-"] 

    let private buttonDoc n =            
        PageLen.view |> View.Map( fun pageLen ->  
            if n && pageLen = 40 || not n && pageLen = 10 then Doc.Empty else
            doc <| buttonElt n pageLen )
        |> Doc.EmbedView
    
    let render =    
        divAttr[ 
            %% "w3-modal" 
            attr.id id'] [
            divAttr [ %% "w3-modal-content w3-animate-zoom w3-card-8" ] [
                headerAttr [ %% "w3-container w3-teal" ] [
                    spanAttr [ 
                        %% "w3-closebtn" 
                        Attr.Create "onclick" (sprintf "document.getElementById('%s').style.display='none'" id')] 
                        [ text "×" ] 
                    h2 [ text "Количество матчей на странице" ] ]
                divAttr [
                    %% "w3-xxlarge"
                    attr.style "margin : 10px; float : left;"] [ 
                    Doc.TextView ( View.Map string  PageLen.view ) ]
                buttonDoc true
                buttonDoc false ] ] 

let renderPagination =  Doc.EmbedView <| View.Do{

    let renderPageLink npage n  =
        let aattrs = [
            yield attr.href "#"
            yield Attr.Handler "click" (fun e x -> 
                varTargetPageNumber.Value <- n  )
            if n=npage then yield attr.``class`` "w3-teal"]        
        li[ aAttr aattrs [ text <| sprintf "%d" (n+1) ] ] 

    let! pagescount = varPagesCount.View
    
    let! npage = varCurrentPageNumber.View
    let aShowDialog = 
        Attr.Create "onclick" (sprintf "document.getElementById('%s').style.display='block'" SettingsDialog.id')
    return 
        [   for n in 0..pagescount-1 do                
                yield renderPageLink npage n 
            yield li[ aAttr [attr.href "#"; aShowDialog ] [ text "..." ] ] ]
        |> List.map doc
        |> Doc.Concat }

let renderMeetup = Meetup.renderMeetup (varColumnGpbVisible.View, varColumnCountryVisible.View)

let renderСoupon = 
    let etable = 
        divAttr [ attr.``class`` "w3-responsive" ][
            tableAttr[ attr.``class`` "w3-table w3-bordered w3-striped w3-hoverable" ] [   
                thead[ trAttr [ Attr.Class "coupon-header-row w3-teal" ] (Meetup.renderGamesHeaderRow varColumnGpbVisible.View)  ]
                tbody [
                    meetups.View |> View.Map( Seq.map (renderMeetup  >> doc)  >> Doc.Concat )
                    |> Doc.EmbedView ] ] ] 

    divAttr [attr.``class`` "w3-container"][
        divAttr [ attr.``class`` "w3-center" ] [
            ulAttr [attr.``class`` "w3-pagination w3-border w3-round"] [ renderPagination ] ]   

        View.Do{
            let! currentPageNumber = varCurrentPageNumber.View
            let! targetPageNumber = varTargetPageNumber.View
            return 
                if currentPageNumber = targetPageNumber then etable else 
                    h1 [ text <| sprintf "Выполняется переход к странице № %d..." (targetPageNumber + 1) ]
                |> doc }
        |> Doc.EmbedView
        SettingsDialog.render ]

let stvr<'a when 'a : equality> (x:Var<'a>) (value : 'a) =
    if x.Value <> value then
        x.Value <- value

let updateCoupon (newGms,updGms,outGms) = 
    
    if List.isEmpty newGms |> not then
        addNewGames newGms    

    if Seq.isEmpty outGms |> not then
        outGms |> Set.iter( meetups.RemoveByKey )
        
    
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

    updateColumnCountryVisible()
    updateColumnGpbVisible()

let processCoupon varDataRecived  = async{
    let request =
        meetups.Value 
        |> Seq.map(fun m -> m.game.gameId, m.hash)
        |> Seq.toList
    let pagelen = PageLen.get()
    let! newGms,updGms,outGms, gamesCount = CentBet.Remote.getCouponPage (request, varTargetPageNumber.Value, pagelen)
    let pagesCount = gamesCount / pagelen + ( if gamesCount % pagelen = 0 then 0 else 1 )
    if varPagesCount.Value <> pagesCount then
        varPagesCount.Value <- pagesCount
    if varTargetPageNumber.Value  > pagesCount then
        varTargetPageNumber.Value <- 0
    if Var.Get varDataRecived |> not then
        Var.Set varDataRecived true
    if varCurrentPageNumber.Value <> varTargetPageNumber.Value then
        varCurrentPageNumber.Value <- varTargetPageNumber.Value
    updateCoupon (newGms,updGms,outGms) } 

module ServerBetfairsSession = 
    let mutable private hasServerBetfairsSession = true
    let check = async{ 
        let! x = CentBet.Remote.hasServerBetfairsSession()
        hasServerBetfairsSession <- x }
    let has() = hasServerBetfairsSession
    let hasNot() = not hasServerBetfairsSession

let getGamesEvents() = 
    let events = eventsCatalogue.Value
    meetups.Value 
    |> Seq.map( fun m -> 
        m.game.gameId, events |> Seq.tryFind( fun e -> e.gameId = m.game.gameId ) ) 
    |> Seq.toList

let processEvents = async{
    updateColumnCountryVisible()
    let gamesWithoutEvent =
        getGamesEvents()
        |> List.choose ( function gameId, None -> Some gameId | _ -> None)
    if gamesWithoutEvent.IsEmpty || ServerBetfairsSession.hasNot() then () else    
    let! newEvents =  CentBet.Remote.getEventsCatalogue gamesWithoutEvent    
    for gameId, name, country in newEvents do
        eventsCatalogue.Add  { gameId = gameId; country = country; markets = []  } 
    for m in meetups.Value do
        m.country.Value <- tryGetCountry m.game.gameId } 

let setMarket gameId values =
    values 
    |> List.choose( fun (_,_,_,x) -> x ) 
    |> List.fold (+) 0
    |> updateTotalMatched gameId


    let markets = values |> List.map( fun (marketId, marketName, runners, totalMatched) -> 
        {   marketName = marketName 
            marketId = marketId            
            totalMatched = totalMatched
            runners = runners |> List.map( fun (runnerNamem, selectionId) -> 
                {   selectionId = selectionId
                    runnerName = runnerNamem } ) } )
    eventsCatalogue.UpdateBy (fun x -> Some {x with markets = markets}) gameId

let processMarkets = async{
    let gamesWithoutMarkets = 
        getGamesEvents()
        |> List.choose ( function gameId, Some {markets = []} -> Some gameId | _ -> None)
    if List.isEmpty gamesWithoutMarkets || ServerBetfairsSession.hasNot() then () else
    for gameId in gamesWithoutMarkets do
        let! m = CentBet.Remote.getMarketsCatalogue gameId
        match m with
        | None -> ()
        | Some values -> setMarket gameId values  } 



let processTotalMatched = async{
    updateColumnGpbVisible()
    let gamesWithMarkets = 
        getGamesEvents()
        |> List.choose ( function gameId, Some {markets = _::_} -> Some gameId | _ -> None)

    if List.isEmpty gamesWithMarkets || ServerBetfairsSession.hasNot() then () else  

    for gameId in gamesWithMarkets do
        let! m = CentBet.Remote.getTotalMatched gameId
        if m.IsEmpty then () else
            m |> Map.fold( fun acc _ value -> acc + value ) 0
            |> updateTotalMatched gameId } 
   
type Work = 
    {   what : string
        sleepInterval : int
        sleepErrorInterval : int 
        work : Async<unit> }
    
    static member ``new`` (what,sleepInterval,sleepErrorInterval) work =
        {   what = what
            sleepInterval  = sleepInterval
            sleepErrorInterval = sleepErrorInterval
            work = work }
        |> Work.run
                    
    static member loop x = async{        
        try
            do! x.work
            do! Async.Sleep x.sleepInterval 
        with e ->
            printfn "task error %A : %A" x.what e
            do! Async.Sleep x.sleepErrorInterval 
        return! Work.loop x }

    static member run x = Async.Start <| async { 
        printfn "task %A : started" x.what
        do! Work.loop x
        printfn "task %A : terminated" x.what }

let Render() =
    let varDataRecived = Var.Create false
    Work.``new`` ("COUPON", 0, 0) (processCoupon varDataRecived)
    Work.``new`` ("CHECK-SERVER-BETFAIRS-SESSION", 0, 0) ServerBetfairsSession.check        
    Work.``new`` ("EVENTS-CATALOGUE", 0, 0) processEvents
    Work.``new`` ("MARKETS-CATALOGUE", 0, 0) processMarkets
    Work.``new`` ("TOTAL-MATCHED", 0, 0) processTotalMatched
    varDataRecived.View |> View.Map ( function
        | false -> h1 [ text "Данные загружаются с сервера. Пожалуйста, подождите."] 
        | _ -> renderСoupon )     
    |> Doc.EmbedView    
   

