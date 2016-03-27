namespace Betfair.Football

open System
open System.Collections.ObjectModel

type PlayStatus = 
    | Inplay 
    | Forpaly
    | GameOver
    | TimeUnknown

type GameId = int * int

type Game =  
    {   gameId      : GameId
        winId       : int
        drawId      : int
        loseId      : int 
        home        : string
        away        : string} 
    static member id x = x.gameId

type Kef = 
    {   back : decimal option
        lay : decimal option }

type GameInfo = 
    {   score       : (int *int) option
        playMinute  : int option
        forplay     : (int * int)  option
        playStatus  : string
        status      : string
        summary     : string 
        order       : int * int 
        winBack     : decimal option
        winLay      : decimal option
        drawBack    : decimal option
        drawLay     : decimal option
        loseBack    : decimal option 
        loseLay     : decimal option }

    static member inplayInfo = function
        | {score = Some (h,a); playMinute = Some m} -> Some (h,a,m)
        | _ -> None

    static member getHash x =
       let y = x.summary, x.status, x.score, x.playMinute, x.playStatus, x.forplay, x.order, x.winBack, x.winLay, x.drawBack, x.drawLay, x.loseBack, x.loseLay
       y.GetHashCode() 
    
    member x.GetHash () = GameInfo.getHash x

type Meetup = Game * GameInfo

    



        






    
