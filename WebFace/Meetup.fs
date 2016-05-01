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
            td [text <| sprintf "%d" totalMatchs ]
        |> doc }

let renderCountry viewColumnCountryVisible countryView  = 
    Doc.EmbedView <| View.Do{
    let! columnCountryVisible = viewColumnCountryVisible
    if not columnCountryVisible then return Doc.Empty else
    let! country = countryView
    return        
        match country with
        | None -> td []
        | Some country -> 
            tdAttr [  
                attr.style "color : #009900;" ] [ 
                text country ]
        |> doc }
    

let renderKef v = 
    View.FromVar v 
    |> View.Map ( function 
        | None -> Doc.Empty
        | Some v -> formatDecimal v |> text  ) 
    |> Doc.EmbedView
    

type MeetupRowTemplate = Templating.Template<"Templates/meetup-row.html">

let renderMeetup (viewColumnGpbVisible, viewColumnCountryVisible) (x : Meetup) = 
    
    let renderMarketsLink text' =         
        aAttr [ 
            attr.href "#"
            attr.style "display: block; text-decoration: none;"
            on.click ( fun _ _ -> 
                State.varMode.Value <- PageModeGameDetail x ) ] [
            text text']

    MeetupRowTemplate.Doc
        (   order = [x.order.View |> View.Map ( fun (page,n) -> sprintf "%d.%d" page n) |> textView],
            home = [ renderMarketsLink x.game.home ],
            away = [ renderMarketsLink x.game.away ],
            score = [ textView x.summary.View ],
            status = [ textView x.status.View ],
            winback = [renderKef x.winBack],
            winlay = [renderKef x.winLay],
            drawback = [renderKef x.drawBack],
            drawlay = [renderKef x.drawLay],
            loseback = [renderKef x.loseBack],
            loselay = [renderKef x.loseLay],
            totalMatched = [renderTotalMatched viewColumnGpbVisible x],
            country = [ renderCountry viewColumnCountryVisible x.country.View] )



    