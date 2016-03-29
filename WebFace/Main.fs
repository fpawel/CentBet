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


module CouponTemple =
    //open WebSharper.UI.Next.Html
    let page ctx action body =
       Content.Page(            
            Templating.Template<"coupon.html">.Doc(
                body = body ))

module Site =
    open WebSharper.Sitelets.ActionEncoding
    open WebSharper.UI.Next.Html
    open WebSharper.UI.Next.Server

    open Json

    [<Website>]
    let Main =
        Betfair.Football.Coupon.start()
        Application.MultiPage (fun ctx -> function
            | Coupon -> 
                Content.Page(Templating.Template<"coupon.html">.Doc( [ client <@ Coupon.Render() @> ] ))
            | Console -> 
                Content.Page(Templating.Template<"console.html">.Doc( [ client <@ Admin.Render() @> ] ))
            | ApiCall -> 
                CentBet.Api.processInput ctx.Request.Body
                |> Async.bind
                    (   Content.Text
                        >> Content.WithContentType "application/json"
                        >> Content.WithHeaders  [
                            Http.Header.Custom "ContentType" "UTF-8"
                            Http.Header.Custom "AcceptEncoding" "gzip,deflate,sdch"] ))

