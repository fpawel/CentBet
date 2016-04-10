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

let renderTotalMatched viewColumnGpbVisible (x : Meetup) = Doc.EmbedView <| View.Do{
    let! columnGpbVisible = viewColumnGpbVisible
    if not columnGpbVisible then return Doc.Empty else
    let! totalMatchs = x.totalMatched.View
    let totalMatchs = 
        totalMatchs |> List.map snd |> Seq.fold (+) 0
    return
        if totalMatchs=0 then td [] else
            tdAttr [attr.``class`` "game-gpb"] [text <| sprintf "%d" totalMatchs ]
        |> doc }

let renderCountry viewColumnCountryVisible countryView  = 
    Doc.EmbedView <| View.Do{
    let! columnCountryVisible = viewColumnCountryVisible
    if not columnCountryVisible then return Doc.Empty else
    let! country = countryView
    return        
        match country with
        | None -> td []
        | Some country -> tdAttr [ attr.``class`` "game-country" ] [ text country ]
        |> doc }
    

let renderGameStat x = Doc.EmbedView <| View.Do{
    let! playMinute = x.playMinute.View
    let! summary = x.summary.View                
    return 
        match playMinute with
        | Some _ | _ when summary <> "" -> 
            doc <| tdAttr [ attr.``class`` "game-status"] [ text summary ]  
        | _ -> doc <| td [] }

let renderKef back sel v = 
    tdAttr  [   
        sprintf "kef %s %s" sel (if back then  "back" else "lay")
        |> attr.``class`` ][
        View.FromVar v |> View.Map formatDecimalOption |> textView   ] 
    

let renderMeetup (viewColumnGpbVisible, viewColumnCountryVisible) (x : Meetup) = 
    let tx x = td[ x ]
    let tx0 x = td[ Doc.TextNode x ]

    let span' ``class`` x = 
        spanAttr [ attr.``class`` ``class`` ] [x]

    
    let bck' = renderKef true 
    let lay' = renderKef false 

    let showGameDetailOnClick = 
        on.click ( fun _ _ -> 
            State.varMode.Value <- PageModeGameDetail x )

    let renderMarketsLink text' =         
        doc <| aAttr [ attr.href "#"; showGameDetailOnClick ] [text text']

    [   doc <| td[ x.order.View |> View.Map ( fun (page,n) -> 
            sprintf "%d.%d" page n) |> textView ]
        doc <| tdAttr [ attr.``class`` "home-team" ] [ renderMarketsLink x.game.home ]        
        renderGameStat x
        doc <| tdAttr [ attr.``class`` "away-team" ] [ renderMarketsLink x.game.away]
        doc <| tdAttr [ attr.``class`` "game-status"] [ textView x.status.View ] 
        doc <| bck' "win" x.winBack
        doc <| lay' "win" x.winLay
        doc <| bck' "draw" x.drawBack
        doc <| lay' "draw" x.drawLay
        doc <| bck' "lose" x.loseBack
        doc <| lay' "lose" x.loseLay 
        renderTotalMatched viewColumnGpbVisible x
        renderCountry viewColumnCountryVisible x.country.View ] |> tr 