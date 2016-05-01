[<WebSharper.Pervasives.JavaScript>]
module CentBet.Client.Coupon 

open System

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html
    
open Utils
open CentBet.Client
open CentBet.Client.Football
open CentBet.Client.State


let doc (x :Elt) = x :> Doc

module SettingsDialog =    
    
    [<AutoOpen>]
    module private Helpers =
        let (~%%) = attr.``class``
        let buttonElt n pageLen =
            buttonAttr [ 
                %% "w3-btn w3-teal"
                attr.style "margin: 10px; width: 50px; height: 50px;"
                on.click (fun _ _ -> 
                    match n, pageLen with
                    | true, 40 -> ()
                    | false, 10 -> ()
                    | _ ->  PageLen.set <| pageLen + (if n then 1 else -1) ) ]
                [text <| if n then "+" else "-"] 

        let buttonDoc n =            
            PageLen.view |> View.Map( fun pageLen ->  
                if n && pageLen = 40 || not n && pageLen = 10 then Doc.Empty else
                doc <| buttonElt n pageLen )
            |> Doc.EmbedView    

        let varVisible = Var.Create false
        let close() = varVisible.Value <- false

        let content =    
            [   doc <| titleAndCloseButton "Количество матчей на странице" close
                doc <| divAttr [
                    %% "w3-xxlarge"
                    attr.style "margin : 10px; float : left;"] [ 
                    Doc.TextView ( View.Map string  PageLen.view ) ]
                buttonDoc true
                buttonDoc false ]

        
        let renderDialog = createModal varVisible.View close

    let elt = renderDialog [] content

    let show() = varVisible.Value <- true

let renderPagination =  Doc.EmbedView <| View.Do{

    let renderPageLink npage n  =
        let aattrs = [
            yield attr.href "#"
            yield Attr.Handler "click" (fun e x -> 
                varTargetPageNumber.Value <- n  )
            if n=npage then yield attr.``class`` "w3-teal"]        
        li[ aAttr aattrs [ text <| sprintf "%d" (n+1) ] ] 

    let! pagescount = varPagesCount.View
    
    let! npage = varCurrentPageNumber.View
    let aShowDialog = 
        Attr.Handler "click" ( fun _ _ -> 
            SettingsDialog.show() )
    return 
        [   for n in 0..pagescount-1 do                
                yield renderPageLink npage n 
            yield li[ aAttr [attr.href "#"; aShowDialog ] [ text "..." ] ] ]
        |> List.map doc
        |> Doc.Concat }

let renderMeetup = Meetup.renderMeetup (varColumnGpbVisible.View, varColumnCountryVisible.View)

let renderСoupon = 
    let etable = 
        divAttr [ attr.``class`` "w3-responsive" ][
            tableAttr[ attr.``class`` "football-games-list w3-table w3-bordered w3-striped w3-hoverable" ] [   
                thead[ trAttr [ Attr.Class "coupon-header-row w3-teal" ] 
                              (Meetup.renderGamesHeaderRow varColumnGpbVisible.View)  ]
                tbody [
                    meetups.View |> View.Map( Seq.map renderMeetup  >> Doc.Concat )
                    |> Doc.EmbedView ] ] ] 

    divAttr [attr.``class`` "w3-container"][
        divAttr [ attr.``class`` "w3-center" ] [
            ulAttr [attr.``class`` "w3-pagination w3-border w3-round"] [ renderPagination ] ]
        View.Do{
            let! currentPageNumber = varCurrentPageNumber.View
            let! targetPageNumber = varTargetPageNumber.View
            return 
                if currentPageNumber = targetPageNumber then etable else 
                    h1 [ text <| sprintf "Выполняется переход к странице № %d..." (targetPageNumber + 1) ]
                |> doc }
        |> Doc.EmbedView
        SettingsDialog.elt ]

let Render() =
    
    let varDataRecived = Var.Create false
    Work.start varDataRecived

    [   varMode.View|> View.Map( function
            | PageModeGameDetail x -> GameDetail.renderExploreMarkets x
            | _ -> 
                varDataRecived.View |> View.Map( function
                    | false -> doc <| h1 [ text "Данные загружаются с сервера. Пожалуйста, подождите."] 
                    | _ -> doc <| renderСoupon )
                |> Doc.EmbedView )
        |> Doc.EmbedView ]
    |> Doc.Concat
