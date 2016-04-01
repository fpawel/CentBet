[<WebSharper.Pervasives.JavaScript>]
module CentBet.Client.Games 

open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html
    
open Utils
open Betfair.Football
open CentBet.Client.Football

let meetups = ListModel.Create Meetup.id []
let varCurrentPageNumber = Var.Create 0
let varTargetPageNumber = Var.Create 0
let varPagesCount = Var.Create 1
let varColumnGpbVisible = Var.Create false
let varColumnCountryVisible = Var.Create false

let varMode = Var.Create PageModeCoupon

[<AutoOpen>]
module private Helpers1 = 
    
    let addNewGames (getCountry,getMarkets,getLastTotalMatched) newGames = 
        let existedMeetups = meetups.Value |> Seq.toList                    
        meetups.Clear()
        newGames 
        |> List.map ( fun (game : Game, i : GameInfo, hash) -> 
            let country = getCountry game.gameId  
            let markets = getMarkets game.gameId  
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
                totalMatched = Var.Create <| getLastTotalMatched game.gameId} )
        |> Seq.append existedMeetups 
        |> Seq.sortBy ( fun x -> x.order.Value )  
        |> Seq.iter meetups.Add

    let isCouponMode() = 
        varMode.Value = PageModeCoupon
    let notCouponMode() = not <| isCouponMode() 

let updateColumnGpbVisible() = 
    let hasGpb = meetups.Value |> Seq.exists( fun x -> x.totalMatched.Value.IsSome )
    if varColumnGpbVisible.Value <> hasGpb then
        varColumnGpbVisible.Value <- hasGpb

let updateColumnCountryVisible() = 
    let hasCountry = meetups.Value |> Seq.exists( fun x -> x.country.Value <> "")
    if varColumnCountryVisible.Value <> hasCountry then
        varColumnCountryVisible.Value <- hasCountry

let updateTotalMatched gameId toltalMatched = 
    match meetups.TryFindByKey gameId with
    | Some game -> updateVarValue game.totalMatched (Some toltalMatched)
    | _ -> ()


let updateGameInfo (x, i : GameInfo, hash) =     
    x.hash <- hash
    updateVarValue x.playMinute i.playMinute 
    updateVarValue x.status i.status
    updateVarValue x.summary i.summary
    updateVarValue x.order i.order
    updateVarValue x.winBack i.winBack
    updateVarValue x.winLay i.winLay
    updateVarValue x.drawBack i.drawBack
    updateVarValue x.drawLay i.drawLay
    updateVarValue x.loseBack i.loseBack
    updateVarValue x.loseLay i.loseLay

[<AutoOpen>]
module private Helpers2 = 
    let updateCurentCoupon = 
        List.choose ( fun (gameId, x, y) ->  
            meetups.TryFindByKey gameId
            |> Option.map( fun m -> m, x, y) )
        //>> List.filter ( fun (x, i : GameInfo, hash) -> x.hash  <> hash )
        >> List.iter updateGameInfo

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

    let tryGetEvent gameId = eventsCatalogue.Value |> Seq.tryFind( fun {gameId = gameId'} -> gameId = gameId' )

let getCountry gameId  =
    tryGetEvent gameId  |> function
        | Some {country = Some country} -> country
        | _ -> ""

let getMarkets gameId  =
    tryGetEvent gameId  |> function
        | Some {markets = _::_ as markets} -> markets
        | _ -> []

let getLastTotalMatched gameId = 
    eventsCatalogue.TryFindByKey gameId 
    |> Option.bind( fun e -> 
        match e.markets |> List.choose( fun m -> m.totalMatched ) with
        | [] -> None
        | v -> v |> List.sum |> Some )

    

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

module ServerBetfairsSession = 
    let mutable private hasServerBetfairsSession = true
    let check = async{ 
        let! x = CentBet.Remote.hasServerBetfairsSession()
        hasServerBetfairsSession <- x }
    let has() = hasServerBetfairsSession
    let hasNot() = not hasServerBetfairsSession

[<AutoOpen>]
module private Helpers3 = 

    let zipMeetupsWithEvents() = 
        meetups.Value 
        |> Seq.map( fun m -> 
            m.game.gameId, eventsCatalogue.Value |> Seq.tryFind( fun e -> e.gameId = m.game.gameId ) ) 
        |> Seq.toList

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

    let processCoupon varDataRecived  = async{
        if notCouponMode() then () else
        let request =
            meetups.Value 
            |> Seq.map(fun m -> m.game.gameId, m.hash)
            |> Seq.toList
        let pagelen = PageLen.get()
        let! newGms,updGms,outGms, gamesCount = CentBet.Remote.getCouponPage (request, varTargetPageNumber.Value, pagelen)
        let pagesCount = gamesCount / pagelen + ( if gamesCount % pagelen = 0 then 0 else 1 )
        updateVarValue varPagesCount pagesCount
        if varTargetPageNumber.Value  > pagesCount then
            varTargetPageNumber.Value <- 0
    
        if List.isEmpty newGms |> not then
            addNewGames (getCountry, getMarkets, getLastTotalMatched) newGms

        if Seq.isEmpty outGms |> not then
            outGms |> Set.iter( meetups.RemoveByKey )
       
        updateCurentCoupon updGms  
        updateColumnCountryVisible()
        updateColumnGpbVisible() 
        updateVarValue varDataRecived true
        updateVarValue varCurrentPageNumber varTargetPageNumber.Value } 

    let processEvents = async{
        if notCouponMode() then () else
        let gamesWithoutEvent =
            zipMeetupsWithEvents()
            |> List.choose ( function gameId, None -> Some gameId | _ -> None)
        if gamesWithoutEvent.IsEmpty || ServerBetfairsSession.hasNot() then () else    
        let! newEvents =  CentBet.Remote.getEventsCatalogue gamesWithoutEvent    
        for gameId, name, country in newEvents do
            eventsCatalogue.Add  { gameId = gameId; country = country; markets = []  } 
        for m in meetups.Value do
            m.country.Value <- getCountry m.game.gameId 
        updateColumnCountryVisible() } 

    let processMarkets = async{
        if notCouponMode() then () else
        let gamesWithoutMarkets = 
            zipMeetupsWithEvents()
            |> List.choose ( function gameId, Some {markets = []} -> Some gameId | _ -> None)
        if List.isEmpty gamesWithoutMarkets || ServerBetfairsSession.hasNot() then () else
        for gameId in gamesWithoutMarkets do
            if notCouponMode() then () else
            let! m = CentBet.Remote.getMarketsCatalogue gameId
            match m with
            | None -> ()
            | Some values -> setMarket gameId values  } 

    let processTotalMatched = async{
        if notCouponMode() then () else
        let gamesWithMarkets = 
            zipMeetupsWithEvents()
            |> List.choose ( function gameId, Some {markets = _::_} -> Some gameId | _ -> None)

        if List.isEmpty gamesWithMarkets || ServerBetfairsSession.hasNot() then () else  

        for gameId in gamesWithMarkets do
            if notCouponMode() then () else
            let! m = CentBet.Remote.getTotalMatched gameId
            if m.IsEmpty then () else
                m |> Map.fold( fun acc _ value -> acc + value ) 0
                |> updateTotalMatched gameId 
        updateColumnGpbVisible() }   

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

let varGameDetailError = Var.Create false
let processGameDetail = async{
    match varMode.Value with
    | PageModeGameDetail game ->
        let! x = CentBet.Remote.getGame game.Meetup.game.gameId
        Var.Set varGameDetailError (Option.isNone x)
        match x with
        | None -> () // ошибка
        | Some (i,h) -> updateGameInfo(game.Meetup, i, h)
    | _ -> return () }

let start varDataRecived =
    let (==>) conf work = Work.``new`` conf work
    ("COUPON", 0, 0) 
        ==> processCoupon varDataRecived
    ("CHECK-SERVER-BETFAIRS-SESSION", 0, 0) 
        ==> ServerBetfairsSession.check        
    ("EVENTS-CATALOGUE", 0, 0) 
        ==> processEvents
    ("MARKETS-CATALOGUE", 0, 0) 
        ==> processMarkets
    ("TOTAL-MATCHED", 0, 0) 
        ==> processTotalMatched 
    ("GAME-DETAIL", 0, 0) 
        ==> processGameDetail 
    