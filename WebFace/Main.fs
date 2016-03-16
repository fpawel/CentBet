namespace CentBet

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Server

open CentBet.Client

type EndPoint =
    | [<EndPoint "/">] Coupon    
    | [<EndPoint "/console">] Console
    

module Templating =
    open WebSharper.UI.Next.Html

    type MainTemplate = Templating.Template<"Main.html">
    

    // Compute a menubar where the menu item for the given endpoint is active
    let NavBar (ctx: Context<EndPoint>) endpoint =
        let refEndpoint a = 
            [   yield attr.href (ctx.Link a) 
                if endpoint = a then yield attr.``class`` "active" ]
        [   match endpoint with 
            | Coupon -> yield client <@ Coupon.RenderMenu() @>
            | Console -> 
                yield aAttr (refEndpoint Coupon) [text "Bact to coupon"] :> Doc 
                yield client <@ Admin.RenderMenu() @> ]
    let Main ctx action title body =
       Content.Page(            
            MainTemplate.Doc(
                title = title,
                navbar = NavBar ctx action,
                body = body ))

   

module Site =
    open WebSharper.Sitelets.ActionEncoding
    open WebSharper.UI.Next.Html
    open WebSharper.UI.Next.Server


    
    let CouponPage ctx =
        Templating.Main ctx EndPoint.Coupon "Купон" [ client <@ Coupon.Render() @> ]

    let ConsolePage ctx = async{         
        return!
            Templating.Main ctx Console "Console" [ client <@ Admin.Render()@> ]
        }


        

    [<Website>]
    let Main =
        let passwordKey = "E018CB561EE1DB0EF3892AE22FCCDD5C" 

        let culture = System.Globalization.CultureInfo("ru-RU")      
        System.Threading.Thread.CurrentThread.CurrentCulture <- culture
        System.Threading.Thread.CurrentThread.CurrentUICulture <- culture

        Betfair.Football.Coupon.start()
        Application.MultiPage (fun ctx -> function
            | Coupon -> CouponPage ctx 
            | Console -> ConsolePage ctx )
