namespace CentBet

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Server

open CentBet.Client

type EndPoint =
    | [<EndPoint "/">] Coupon    
    | [<EndPoint "/console">] Console
    | [<EndPoint "POST /api">] ApiCall


//module CouponTemple =
//    //open WebSharper.UI.Next.Html
//    let page ctx action body =
//       Content.Page(            
//            Templating.Template<"Templates/coupon.html">.Doc(
//                body = body ))

module Site =
    open WebSharper.Sitelets.ActionEncoding
    open WebSharper.UI.Next.Html
    open WebSharper.UI.Next.Server

    open Json

    let betfairBrandImage =
        let url = "http://corporate.betfair.com/~/media/Images/B/Betfair-Corporate/Images/logo/logo.png?h=30&la=en&w=183"
        imgAttr 
            [ attr.src url; attr.style "margin-bottom : -1px;" ] 
            []
        :> Doc 

    [<Website>]
    let Main =
        Application.MultiPage (fun ctx -> function
            | Coupon -> 
                let body = [ client <@ Coupon.Render() @> ]
                Content.Page(Templating.Template<"Templates/coupon.html">.Doc( [ betfairBrandImage ], body))
            | Console -> 
                Content.Page(Templating.Template<"Templates/console.html">.Doc( [ client <@ Admin.Render() @> ] ))
            | ApiCall -> 
                CentBet.Api.processInput ctx.Request.Body
                |> Async.bind
                    (   Content.Text
                        >> Content.WithContentType "application/json"
                        >> Content.WithHeaders  [
                            Http.Header.Custom "ContentType" "UTF-8"
                            Http.Header.Custom "AcceptEncoding" "gzip,deflate,sdch"] ))

