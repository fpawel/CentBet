[<WebSharper.Pervasives.JavaScript>]
module CentBet.Client.Meetup 

open CentBet.Client
open CentBet.Client.Football

open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Client

let private doc (x :Elt) = x :> Doc

let renderGamesHeaderRow viewColumnGpbVisible = [   
    yield![   
        th [text "№"]
        th [text "Дома"]
        th [] 
        th [text "В гостях"]
        th []
        thAttr [ attr.colspan "2" ] [text "Победа"]
        thAttr [ attr.colspan "2" ] [text "Ничья"]
        thAttr [ attr.colspan "2" ] [text "Поражение"] ] |> List.map doc
    
    yield   
        viewColumnGpbVisible |> View.Map( function
            | true -> doc <| th  [text "GPB"]   
            | false -> Doc.Empty )
        |> Doc.EmbedView
    yield doc <| th [] ] 

let renderMeetup (viewColumnGpbVisible, viewColumnCountryVisible) (x : Meetup) = 
    let tx x = td[ x ]
    let tx0 x = td[ Doc.TextNode x ]

    let span' ``class`` x = 
        spanAttr [ attr.``class`` ``class`` ] [x]

    let kef' back sel v = 
        tdAttr  [   
            sprintf "kef %s %s" sel (if back then  "back" else "lay")
            |> attr.``class`` ][
            View.FromVar v |> View.Map formatDecimalOption |> textView   ] 
        |> doc
    let bck' = kef' true 
    let lay' = kef' false 

    let showGameDetailOnClick = 
        on.click ( fun _ _ -> 
            Games.varMode.Value <- PageModeGameDetail( {Meetup = x; Market = Var.Create None} ))

    let renderMarketsLink text' =         
        doc <| aAttr [ attr.href "#"; showGameDetailOnClick ] [text text']

    [   doc <| td[ x.order.View |> View.Map ( fun (page,n) -> 
            sprintf "%d.%d" page n) |> textView ]

        doc <| tdAttr [ attr.``class`` "home-team" ] [ renderMarketsLink x.game.home ]
        
        View.Do{
                let! playMinute = x.playMinute.View
                let! summary = x.summary.View                
                return 
                    match playMinute with
                    | Some _ | _ when summary <> "" -> 
                        doc <| tdAttr [ attr.``class`` "game-status"] [ text summary ]  
                    | _ -> doc <| td [] } |> Doc.EmbedView

        doc <| tdAttr [ attr.``class`` "away-team" ] [ renderMarketsLink x.game.away]


        doc <| tdAttr [ attr.``class`` "game-status"] [ textView x.status.View ] 
        bck' "win" x.winBack
        lay' "win" x.winLay
        bck' "draw" x.drawBack
        lay' "draw" x.drawLay
        bck' "lose" x.loseBack
        lay' "lose" x.loseLay 
        View.Do{
            let! x1 = x.totalMatched.View
            let! x2 = viewColumnGpbVisible
            return x1,x2 }  
        |> View.Map( function 
            | _,false -> Doc.Empty
            | None,true -> doc <| td []             
            | Some totalMatched,_ -> 
                tdAttr [attr.``class`` "game-gpb"] [text <| sprintf "%d" totalMatched ]
                |> doc ) 
        |> Doc.EmbedView 
            
        View.Do{
            let! x1 = x.country.View
            let! x2 = viewColumnCountryVisible
            return x1,x2 } 
        |> View.Map( function 
                | _,false -> Doc.Empty
                | country,_ ->
                    tdAttr [ attr.``class`` "game-country" ] [ Doc.TextView x.country.View ]
                    |> doc )
        |> Doc.EmbedView  ] 
    |> tr 