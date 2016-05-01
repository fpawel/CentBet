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

    let renderKef side price size  = Doc.EmbedView <| View.Do{
        let cls = 
            match side with
            | Back -> "back"
            | Lay -> "lay"
            |> sprintf "price %s"
            |> attr.``class``

        let! price = View.FromVar price
        let price =
            price 
            |> Option.map( fun price -> 
                tdAttr [cls] [
                    b [ price |> formatDecimal |>  text ] ] )
            |> Option.getWith (td[])
            
        let! size = View.FromVar size
        let size =
            size 
            |> Option.map( fun size -> 
                td[ i [ size |> formatDecimal |> sprintf "%s $" |>  text ] ] )
            |> Option.getWith (td[])
        return Doc.Concat [size; price] }
   


    let renderRunner (r:RunnerBook) = 
        tr[
            td[ text r.runnerName ]
            renderKef Back r.backPrice r.backSize
            renderKef Lay  r.layPrice  r.laySize ] 
       
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
                if not expanded then title else 
                tableAttr [ %% "my-table-striped" ] [ 
                    tr[ tdAttr[ attr.colspan "5" ][ title] ]
                    market.runners |> List.map  (renderRunner >> doc) |> Doc.Concat ] 
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
        match meetup.marketsBook |> window 3 with        
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

    
   
    
    