[<WebSharper.Pervasives.JavaScript>]
module CentBet.Client.GameDetail


open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html
    
open Utils
open CentBet.Client
open CentBet.Client.Football

let varError = Var.Create false

let private doc (x :Elt) = x :> Doc
let private (~%%) = attr.``class``

[<AutoOpen>]
module private Helpers =
    let renderDialog = 
        (fun () -> 
            printfn "back to coupon"
            Games.varMode.Value <- PageModeCoupon)
        |> createModal
            (Games.varMode.View |> View.Map ( function
                | PageModeGameDetail _ -> true
                | _ -> false) )

    let renderGameTeams home away =   
        divAttr [ %% "w3-row" ] [ 
            h3Attr [ 
                %% "w3-col s5" 
                attr.style "text-align: right;"][ 
                text home ]
            h2Attr [ 
                %% "w3-col s2" 
                attr.style "color : #b4e500; text-align: center;" ] [ 
                text "V" ]             
            h3Attr [ 
                %% "w3-col s5" 
                attr.style "text-align: left;"][ 
                text away ] ] 

    let renderGameScore status summary = 
        // header-content-football
        divAttr [ %% "w3-row" ] [             
            h5Attr [
                %% "w3-col s6"
                attr.style "text-align: right; padding-right : 10px;" ][ 
                Doc.TextView status ] 
            h5Attr [
                %% "w3-col s6"
                attr.style "text-align: left; color : #b4e500;" ][ 
                Doc.TextView summary ] ]  

    let renderGameInfo home away status summary = 
        divAttr [ %% "w3-row header-content-football" ] [ 
            divAttr [%% "w3-col s12"] [ 
                renderGameTeams home away
                renderGameScore status summary ]]
        
        

    let renderMarketsList varMarket meetup markets  =        
        ulAttr [ %% "w3-ul w3-hoverable" ] [
            for (m : MarketCatalogue) in markets ->
                let atrs = [ 
                    attr.href "#"
                    on.click ( fun _ _ -> 
                        Var.Set varMarket (Some m) ) ]
                doc <| li[ aAttr atrs [ text m.marketName ] ] ]

    let window n =
        Seq.mapi(fun i x -> i % n,x)
        >> Seq.groupBy fst
        >> Seq.map( fun (_,xs) -> Seq.map snd xs |> Seq.toList )
        >> Seq.toList
        

    let renderMeetupMarkets meetup varMarket =
        let render = renderMarketsList varMarket meetup >> doc

        let a1 = attr.style "border-right : 1px solid #ddd;"
        let a2 = attr.style "border-radius: 0 0 10px 10px;"

        match Games.getMarkets meetup.game.gameId |> window 3 with        
        | [ markets ] -> render markets 
        | [ xs1; xs2 ] -> 
            divAttr [%% "w3-row w3-sand"; a2] [    
                divAttr [ %% "w3-col s6"; a1 ] [ render xs1 ]
                divAttr [ %% "w3-col s6" ] [ render xs2 ] ]
            |> doc
        | [ xs1; xs2; xs3 ] -> 
            divAttr [%% "w3-row w3-sand"; a2] [    
                divAttr [ %% "w3-col s4"; a1 ] [ render xs1 ]
                divAttr [ %% "w3-col s4"; a1 ] [ render xs2 ] 
                divAttr [ %% "w3-col s4" ] [ render xs3 ] ]
            |> doc
        | _ -> Doc.Empty
        
    
    let renderContent game = 
        Doc.EmbedView <| View.Do{ 
            let! vmarket =  game.Market.View
            match vmarket with
            | None -> return renderMeetupMarkets game.Meetup game.Market
            | Some market -> return text market.marketName }
        


let renderExploreMarkets (game : GameDetail) =     
    let x = game.Meetup
    renderDialog       
        [ attr.style "border-radius: 10px;" ]  
        [   renderGameInfo x.game.home x.game.away x.status.View x.summary.View            
            renderContent game ]

    
   
    
    