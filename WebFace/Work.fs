[<WebSharper.Pervasives.JavaScript>]
module CentBet.Client.Work

open WebSharper.UI.Next
open CentBet.Client.Football
open CentBet.Client.State

type GameInfo = Betfair.Football.GameInfo
type Game = Betfair.Football.Game

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

let updateCurentCoupon = 
    List.choose ( fun (gameId, x, y) ->  
        meetups.TryFindByKey gameId
        |> Option.map( fun m -> m, x, y) )
    >> List.iter updateGameInfo
    
let addNewGames newGames = 
    let existedMeetups = meetups.Value |> Seq.toList                    
    meetups.Clear()

    newGames 
    |> List.map ( fun (game : Game, i : GameInfo, hash) -> 
        let marketsBook =
            tryGetEventById game.gameId 
            |> Option.map( fun e -> List.map MarketBook.New e.markets )
            |> Option.getWith []

        {   game        = game            
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
            marketsBook = marketsBook
            totalMatched = Var.Create []
            country = Var.Create (State.tryGetCountry  game.gameId)
            hash = hash } )
    |> Seq.append existedMeetups 
    |> Seq.sortBy ( fun x -> x.order.Value )  
    |> Seq.iter meetups.Add
   
let updateColumnGpbVisible() =     
    meetups.Value |> Seq.exists( fun x -> 
        x.totalMatched.Value
        |> Seq.map snd
        |> Seq.exists ( (<) 0 ) )
    |> updateVarValue varColumnGpbVisible

let updateColumnCountryVisible() = 
    meetups.Value 
    |> Seq.exists( fun x -> 
        x |> mtpevt |> Option.bind( fun evt -> evt.country )
        |> function Some x when x <> "" -> true | _ -> false)
    |> updateVarValue varColumnCountryVisible
    
let updateTotalMatched gameId toltalMatched = 
    meetups.TryFindByKey gameId 
    |> Option.iter( fun mtp -> updateVarValue mtp.totalMatched toltalMatched )

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
        addNewGames newGms
    outGms |> Set.iter( meetups.RemoveByKey )       
    updateCurentCoupon updGms  
    updateColumnCountryVisible()
    updateColumnGpbVisible() 
    updateVarValue varDataRecived true
    updateVarValue varCurrentPageNumber varTargetPageNumber.Value  } 

let processEvents = async{
    if notCouponMode() then () else
    let gamesWithoutEvent = meetups.Value |> Seq.filter( mtpevt >> Option.isNone )
    if Seq.isEmpty gamesWithoutEvent || ServerBetfairsSession.hasNot() then () else    
    let! newEvents = 
        gamesWithoutEvent 
        |> Seq.map(fun m -> m.game.gameId) 
        |> Seq.toList
        |> CentBet.Remote.getEventsCatalogue 
    for gameId, name, country in newEvents do
        let evt = { 
            gameId = gameId
            country = country
            markets = []  }
        events.Add  evt
        meetups.TryFindByKey gameId |> Option.iter( fun m -> 
            m.country.Value <- evt.country)
    updateColumnCountryVisible()
    updateColumnGpbVisible()  } 

let processMarkets = async{
    if notCouponMode() then () else
    let gamesWithoutMarkets = 
        meetups.Value |> Seq.choose( fun m -> 
            m |> mtpevt |> Option.bind( function 
                | {markets = []} -> Some m.game.gameId 
                | _ -> None ) )
    if Seq.isEmpty gamesWithoutMarkets || ServerBetfairsSession.hasNot() then () else
    let gameId = Seq.head gamesWithoutMarkets 
    let! m' = CentBet.Remote.getMarketsCatalogue gameId
    m' |> Option.iter ( fun markets  ->
        gameId |> events.UpdateBy( fun evt -> 
            Some { evt with markets = List.map Market.New markets  } ) 
        gameId |> meetups.UpdateBy( fun meetup -> 
            Some { meetup with marketsBook = List.map (Market.New >> MarketBook.New) markets  } ) ) } 

let processTotalMatched = async{
    if notCouponMode() then () else
    let gamesWithMarkets = 
        meetups.Value |> Seq.choose( fun m ->
            m |> mtpevt
            |> Option.map( fun evt -> evt.markets ) 
            |> Option.bind( function [] -> None | _ -> Some m.game.gameId ) )
    if Seq.isEmpty gamesWithMarkets || ServerBetfairsSession.hasNot() then () else  
    for gameId in gamesWithMarkets do
        if notCouponMode() then () else
        let! m = CentBet.Remote.getTotalMatched gameId
        if m.IsEmpty then () else
            updateTotalMatched gameId m
    updateColumnGpbVisible() }   

let processGameDetail = async{
    match varMode.Value with
    | PageModeCoupon -> ()
    | PageModeGameDetail mtp ->
        let! x = CentBet.Remote.getGame mtp.game.gameId
        Var.Set varGameDetailError (Option.isNone x)
        match x with
        | None -> () // ошибка
        | Some (i,h) -> updateGameInfo(mtp, i, h)
        let markets = mtp.marketsBook |> Seq.filter( fun m -> m.expanded.Value)
        let! readedMarketBook = 
            markets             
            |> Seq.map MarketBook.id 
            |> Seq.toList
            |> CentBet.Remote.getMarketsBook 
        for market in markets do 
            updateVarValue market.pricesReaded true
            Map.tryFind market.marketId readedMarketBook  
            |> Option.iter(fun readedRunnerPrices ->
                for runner in market.runners do
                    Map.tryFind runner.selectionId readedRunnerPrices
                    |> Option.iter(fun (back,lay) ->
                        updateVarValue runner.back back
                        updateVarValue runner.lay lay ) ) } 

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