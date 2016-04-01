[<WebSharper.Pervasives.JavaScript>]
module CentBet.Client.Football 

open WebSharper.UI.Next
open WebSharper.UI.Next.Client

open Betfair.Football
    
open Utils

///Information about the Runners (selections) in a market
type RunnerCatalog = {   
    selectionId : int 
    runnerName : string }

type MarketCatalogue = {   
    marketId : int
    marketName : string 
    totalMatched : int option
    runners : RunnerCatalog list }

type EventCatalogue = 
    {   gameId : GameId
        country : string option  
        markets : MarketCatalogue list }
    static member id x = x.gameId

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
        country     : Var<string>        
        totalMatched : Var<int option>
        mutable hash : int }
    static member id x = x.game.gameId

type GameDetail = {
    Meetup : Meetup
    Market : Var<MarketCatalogue option> }
    
type PageMode = 
    | PageModeCoupon
    | PageModeGameDetail of GameDetail

