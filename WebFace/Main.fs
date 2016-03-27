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


module Templating =
    open WebSharper.UI.Next.Html

    type MainTemplate = Templating.Template<"Main.html">
        
        
    let Main ctx action title body =
       Content.Page(            
            MainTemplate.Doc(
                title = title,                
                body = body ))

module Site =
    open WebSharper.Sitelets.ActionEncoding
    open WebSharper.UI.Next.Html
    open WebSharper.UI.Next.Server

    open Json


    
    let CouponPage ctx =
        Templating.Main ctx EndPoint.Coupon "Купон" [ client <@ Coupon.Render() @> ]

    let ConsolePage ctx = async{         
        return!
            Templating.Main ctx Console "Console" [ client <@ Admin.Render()@> ]
        }

    [<Website>]
    let Main =
        #if DEBUG

        #else
        Betfair.Football.Coupon.start()
        #endif        
        Application.MultiPage (fun ctx -> function
            | Coupon -> CouponPage ctx 
            | Console -> ConsolePage ctx 
            | ApiCall -> 
                CentBet.Api.processInput ctx.Request.Body
                |> Async.bind
                    (   Content.Text
                        >> Content.WithContentType "application/json"
                        >> Content.WithHeaders  [
                            Http.Header.Custom "ContentType" "UTF-8"
                            Http.Header.Custom "AcceptEncoding" "gzip,deflate,sdch"] ))
