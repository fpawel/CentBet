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

    type GameHeaderTemplate = Templating.Template<"Templates/game-header.html">
    
    let renderGameInfo meetup = 
        let country = 
            meetup.country.View 
            |> View.Map( Option.getWith "" >> text )
            |> Doc.EmbedView
        let totalMatched =
            meetup.totalMatched.View|> View.Map( fun totalMatchs -> 
                let totalMatchs = totalMatchs |> List.map snd |> Seq.fold (+) 0
                if totalMatchs=0 then "" else sprintf "%d GPB" totalMatchs
                |> text )
            |> Doc.EmbedView
        let navback =
            aAttr[
                attr.href "#" 
                on.click ( fun _ _ -> State.varMode.Value <- PageModeCoupon ) ][ 
                text "Назад к списку матчей" ]

        GameHeaderTemplate.Doc
            (   navback = [navback], 
                home = meetup.game.home, 
                score = [ Doc.TextView meetup.summary.View ], 
                away = meetup.game.away, 
                country = [country], 
                status = [Doc.TextView meetup.status.View], 
                gpb = [totalMatched] )

    let renderSize = round >> formatDecimal  >> sprintf "%s$" >> text

    let renderPriceSizeButton price size = 
        [   "runner-kef-price", text <| formatDecimal price 
            "runner-kef-size",  renderSize size ]
        |> List.map( fun (cls, elt) ->
            doc <| divAttr [ %% cls] [elt] )

    let kefClass side = sprintf "runner-kef runner-%s" (Side.what side)

    let renderAvailPricesTooltip = 
        Seq.map( fun (price,size) -> 
            [   tdAttr [ %% "runner-tooltip-price" ] [ text <| formatDecimal price  ]
                tdAttr [ %% "runner-tooltip-size" ] [ renderSize size  ] ]
            |> List.map doc |> tr |> doc )            
        >> tableAttr [ %% "tooltip-content runner-avail-prices-container" ]
        >> doc
            
        
    let renderKef side (r:RunnerBook) = Doc.EmbedView <| View.Do{
        let varPrices = if side = Back then r.back else r.lay
        let! prices = varPrices.View
        return
            match prices with
            | (price, size) :: [] -> 
                tdAttr
                    [ %% kefClass side ]
                    (renderPriceSizeButton price size)
            | (price, size) :: availPrices -> 
                let eltAvailPrices = renderAvailPricesTooltip availPrices
                let eltsBtn = renderPriceSizeButton price size
                tdAttr
                    [ %% kefClass side ]
                    [ divAttr 
                        [ %% "tooltip"; Attr.Style "width" "100%" ] 
                        (eltAvailPrices :: eltsBtn) ]
                    
            | _ -> tdAttr [ %% "runner-kef"] [] }

    let renderRunner (r:RunnerBook) = 
        tr[   
            tdAttr[ %% "runner-name"][ text r.runnerName ]
            renderKef Back r
            renderKef Lay r ]

    let renderTitle expanded market = 
        let chv1,cls1 = if expanded then "up", "expanded" else "down",""

        aAttr [
            attr.href "#"
            on.click( fun _ _ ->  market.expanded.Value <- not market.expanded.Value ) 
            %% sprintf "runners-title %s" cls1 ] [
            iAttr [%% sprintf "fa fa-chevron-%s" chv1 ] []
            text market.marketName  ]
    
    let renderMarket market  =
        // expanded mouseOver setIsMouseOver
        market.expanded.View |> View.Map( fun expanded ->
            let title = renderTitle expanded market
            let elt =
                if not expanded then title   else 
                tableAttr [
                    %% "table-horiz-lines" ] [ 
                    tr[ tdAttr [ attr.colspan "3" ] [title] ]                     
                    market.runners |> List.map  (renderRunner >> doc)  |> Doc.Concat  ] 
            li [elt] )
        |> Doc.EmbedView
        
    let window n =
        Seq.mapi(fun i x -> i % n,x)
        >> Seq.groupBy fst
        >> Seq.map( fun (_,xs) -> Seq.map snd xs |> Seq.toList )
        >> Seq.toList
        

    let renderMarkets meetup  =
        let a1 = attr.style "border-right : 1px solid #ddd;"
        //let a2 = attr.style "border-radius: 0 0 10px 10px;"
        let (~&&) = List.map renderMarket  

        let markets = 
            meetup.marketsBook
            |> Seq.filter(fun m -> m.marketName <> "Азиатский гандикап")            
            |> Seq.sortBy(fun m -> m.marketName <> "Ставки", m.marketName)
            |> window 3

        match markets with        
        | [ markets ] -> doc <| ulAttr [ %% "w3-ul" ] (&& markets)
        | [ xs1; xs2 ] -> 
            divAttr [%% "w3-row"] [    
                ulAttr [ %% "w3-ul w3-col s6" ] (&& xs1)
                ulAttr [ %% "w3-ul w3-col s6" ] (&& xs2) ] 
            |> doc
        | [ xs1; xs2; xs3 ] -> 
            divAttr [%% "w3-row"] [    
                ulAttr [ %% "w3-ul w3-col s4" ] (&& xs1)
                ulAttr [ %% "w3-ul w3-col s4" ] (&& xs2) 
                ulAttr [ %% "w3-ul w3-col s4" ] (&& xs3)  ] 
            |> doc
        | _ -> Doc.Empty

let renderExploreMarkets meetup =     
    [   renderGameInfo meetup        
        renderMarkets meetup ]
    |> Doc.Concat

    
   
    
    