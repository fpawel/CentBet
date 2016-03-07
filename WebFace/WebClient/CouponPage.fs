[<WebSharper.Pervasives.JavaScript>]
module CentBet.Client.Coupon 

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

open Betfair.Football

    
open Utils

type Meetup =
    {   game : Game
        gameInfo : Var<GameInfo>
        mutable hash : int }
    static member id x = x.game.gameId
    static member viewGameInfo x = x.gameInfo.View
    static member inplay x = 
        x.gameInfo.Value.playMinute.IsSome
    static member notinplay x = 
        x.gameInfo.Value.playMinute.IsNone

type Meetups = ListModel<GameId,Meetup>

let meetups = ListModel.Create Meetup.id []

let downloadCoupon inplayOnly requst  = CentBet.ProcessCoupon.get (requst, inplayOnly)


let addNewGames newGames = 
    let existedMeetups = meetups.Value |> Seq.toList                    
    meetups.Clear()
    newGames 
    |> List.map ( fun (game, gameInfo, hash) -> 
        {   game = game
            gameInfo = Var.Create gameInfo
            hash = hash } )

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
            

let processCoupon inplayOnly = async{
    let! newGms,updGms,outGms = 
        meetups.Value 
        |> Seq.map(fun m -> m.game.gameId, m.hash)
        |> Seq.toList
        |> downloadCoupon inplayOnly
    updateCoupon (newGms,updGms,outGms) } 


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
        gameInfo (fun y -> y.status)
        gameInfo (fun y -> y.summary)
        gameInfo (fun y -> %% y.winBack)
        gameInfo (fun y -> %% y.winLay)
        gameInfo (fun y -> %% y.drawBack)
        gameInfo (fun y -> %% y.drawLay)
        gameInfo (fun y -> %% y.loseBack)
        gameInfo (fun y -> %% y.loseLay) ]
    :> Doc


let varInplayOnly = Var.Create true
//let varLoading = Var.Create true

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
        [   aAttr [
                yield attr.href "#"
                yield Attr.Handler "click" (fun e x -> varInplayOnly.Value <- true )
                if isInplayOnly then yield attr.``class`` "active"] 
                [text "В игре"]
            aAttr [
                yield attr.href "#"
                yield Attr.Handler "click" (fun e x -> varInplayOnly.Value <- false )
                if not isInplayOnly then yield attr.``class`` "active"] 
                [text "Все матчи"] ]
        |> List.map( fun x -> x :> Doc )
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


let Main () =  
    
    async{
        while true do
            do! processCoupon varInplayOnly.Value } 
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
        | false, _, _ -> h1 [ text "Сегодня нет футбольных матчей"]
        | true, false, true -> h1 [ text "Футбольные матчи сегодня ещё не начались"]            
        | _ -> 
            tableAttr [] [ tbody [ table() ] ] )
    |> Doc.EmbedView    

