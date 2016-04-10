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

    
    let renderGameInfo meetup = 
        divAttr [ 
            %% "w3-indigo" ] [ 
            divAttr [
                %% "w3-row" ][
                aAttr[
                    attr.style "text-align: left; margin-left : 5px;"
                    %% "w3-col s6"             
                    attr.href "#" 
                    on.click ( fun _ _ -> State.varMode.Value <- PageModeCoupon ) ][ 
                    text "Назад к списку матчей" ]
                
                divAttr[
                    %% "w3-col s4" 
                    attr.style "text-align: right;" ][
                    meetup.country.View 
                        |> View.Map( Option.map text >> Option.getWith Doc.Empty )
                        |> Doc.EmbedView ] 

                divAttr[
                    %% "w3-col s2"  
                    attr.style "color : #b4e500;" ][
                        meetup.totalMatched.View|> View.Map( fun totalMatchs -> 
                            let totalMatchs = totalMatchs |> List.map snd |> Seq.fold (+) 0
                            if totalMatchs=0 then Doc.Empty else 
                            text (sprintf "%d GPB" totalMatchs)   )
                        |> Doc.EmbedView ] ]
            
            divAttr [ 
                %% "w3-row" ] [ 
                h1Attr [ 
                    %% "w3-col s5" 
                    attr.style "text-align: right;"][ 
                    text meetup.game.home ]
                
                
                h1Attr [      
                    %% "w3-col s2"                  
                    attr.style "text-align: center; color : #b4e500;"  ][ 
                    Doc.TextView meetup.summary.View ]
                                
                h1Attr [ 
                    %% "w3-col s5" 
                    attr.style "text-align: left;"][ 
                    text meetup.game.away ] ] 

            divAttr[
                attr.style "width : 100%;"][
                h2Attr [                    
                    attr.style "text-align: center;" ][ 
                    Doc.TextView meetup.status.View ]
                    
                ]
                    
                    ]
       
    let renderPrice back ends v  = 
        tdAttr  [   
            %% sprintf "price %s" (if back then  "back" else "lay") ][
            View.FromVar v |> View.Map( 
                Option.map( formatDecimal >> ( fun x -> sprintf "%s%s" x ends) >> text )
                >> Option.getWith Doc.Empty )
            |> Doc.EmbedView ] 



    let renderRunner (r:RunnerBook) = 
        tr[
           td[ text r.runnerName ]
           renderPrice true "" r.backPrice 
           renderPrice true "$" r.backSize 
           renderPrice false "" r.layPrice 
           renderPrice false "$" r.laySize ]
       
    
    let renderRunnersLink market visible  = 
        aAttr [
            attr.href "#"
            on.click( fun _ _ -> 
                market.visible.Value <- not market.visible.Value ) 
            %% (if visible then "w3-teal" else "w3-dark-grey")
            attr.style "padding-left : 5px; padding-bottom : 5px; padding-top : 5px;" ] [
            text market.marketName ]

    let renderRunnersLinkRow market visible  = 
        let runnersLink = renderRunnersLink market visible 
        let td = if visible then tdAttr[ attr.colspan "5" ][ runnersLink ] else td [ runnersLink ]
        tr[ td ] 


    let renderMarketTable market visible  =         
        [   yield doc <| renderRunnersLinkRow market visible 
            if visible then yield! market.runners |> List.map  (renderRunner >> doc) ]
        |> Doc.Concat
        
        
    let renderMarket market = 
        li[ 
            table [ 
                market.visible.View 
                |> View.Map( renderMarketTable market) 
                |> Doc.EmbedView ] ]

    let window n =
        Seq.mapi(fun i x -> i % n,x)
        >> Seq.groupBy fst
        >> Seq.map( fun (_,xs) -> Seq.map snd xs |> Seq.toList )
        >> Seq.toList
        

    let renderMarkets meetup  =
        let a1 = attr.style "border-right : 1px solid #ddd;"
        //let a2 = attr.style "border-radius: 0 0 10px 10px;"
        let renderMarkets = List.map (renderMarket >> doc) 
        match meetup.marketsBook |> window 3 with        
        | [ markets ] -> doc <| ulAttr [ %% "w3-ul" ] (renderMarkets markets)
        | [ xs1; xs2 ] -> 
            divAttr [%% "w3-row"] [    
                ulAttr [ %% "w3-ul w3-col s6" ] (renderMarkets xs1)
                ulAttr [ %% "w3-ul w3-col s6" ] (renderMarkets xs2) ] 
            |> doc
        | [ xs1; xs2; xs3 ] -> 
            divAttr [%% "w3-row"] [    
                ulAttr [ %% "w3-ul w3-col s4" ] (renderMarkets xs1)
                ulAttr [ %% "w3-ul w3-col s4" ] (renderMarkets xs2) 
                ulAttr [ %% "w3-ul w3-col s4" ] (renderMarkets xs3)  ] 
            |> doc
        | _ -> Doc.Empty
        
    
    

let renderExploreMarkets meetup =     
    [   doc <| renderGameInfo meetup        
        renderMarkets meetup ]
    |> Doc.Concat

    
   
    
    