[<WebSharper.Pervasives.JavaScript>]
module CentBet.Client.Football 

open WebSharper.UI.Next
open WebSharper.UI.Next.Client
   
open Utils

type VarDecOpt = Var<decimal option>
type VarIntOpt = Var<int option>
type VarStr = Var<string>
type VarBool = Var<bool>

type Runner = {   
    selectionId : int 
    runnerName : string   }

type Market = 
    {   marketId : int
        marketName : string 
        runners : Runner list  }
    static member id x = x.marketId
    static member New (marketId, marketName, runners, totalMatched) =
        {   marketName = marketName
            marketId = marketId
            runners = runners |> List.map( fun (runnerNamem, selectionId) -> 
                {   selectionId = selectionId
                    runnerName = runnerNamem  } ) }
    

type Event = 
    {   gameId : int * int
        country : string option  
        markets : Market list }
    static member id x = x.gameId


type RunnerBook = 
    {   selectionId : int 
        runnerName : string
        back : Var< (decimal*decimal) list >
        lay : Var< (decimal*decimal) list > }
    static member New (r:Runner) = 
        
        {   selectionId = r.selectionId
            runnerName = r.runnerName
            back = Var.Create []
            lay = Var.Create [] }
        

type MarketBook = 
    {   marketId : int 
        marketName : string
        expanded : VarBool
        runners : RunnerBook list
        pricesReaded : VarBool}
    static member id x = x.marketId
    static member New (m:Market) = 
        {   marketId  = m.marketId 
            marketName  = m.marketName
            expanded  = Var.Create false
            runners = m.runners |> List.map RunnerBook.New 
            pricesReaded = Var.Create false}
        

type Meetup =
    {   game        : Betfair.Football.Game
        playMinute  : VarIntOpt
        status      : VarStr
        summary     : VarStr
        order       : Var<int * int>
        winBack     : VarDecOpt
        winLay      : VarDecOpt
        drawBack    : VarDecOpt
        drawLay     : VarDecOpt
        loseBack    : VarDecOpt
        loseLay     : VarDecOpt
        marketsBook  : MarketBook list
        totalMatched : Var< (int * int) list>
        country : Var<string option>
        mutable hash : int }
    static member id x = x.game.gameId    

   
type PageMode = 
    | PageModeCoupon
    | PageModeGameDetail of Meetup

type Side = 
    | Back 
    | Lay
    static member what = function 
        | Back -> "back"
        | Lay -> "lay"

