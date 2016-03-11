namespace CentBet

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Server

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
            | Coupon -> yield client <@ CentBet.Client.Coupon.Menu() @>
            | Console -> 
                yield aAttr (refEndpoint Coupon) [text "Bact to coupon"] :> Doc 
                yield client <@ CentBet.Client.Admin.RenderMenu() @>
                
                ]
    let Main ctx action title footer body =
       Content.Page(            
            MainTemplate.Doc(
                title = title,
                navbar = NavBar ctx action,
                body = body,
                footer = footer))

   

module Site =
    open WebSharper.Sitelets.ActionEncoding
    open WebSharper.UI.Next.Html
    open WebSharper.UI.Next.Server


    
    let CouponPage ctx =
        Templating.Main ctx EndPoint.Coupon "Купон" [] [
            client <@ CentBet.Client.Coupon.Main() @> ]

    let ConsolePage ctx = async{ 
        //let! isAdminCtx = CentBet.Remote.isAdminCtx ctx
        return!
            Templating.Main ctx Console "Console" 
                [ client <@ CentBet.Client.Admin.RenderCommandPrompt()@>] 
                [ client <@ CentBet.Client.Admin.RenderConsole()@> ]
        }


        

    [<Website>]
    let Main =
        let passwordKey = "E018CB561EE1DB0EF3892AE22FCCDD5C" 

        Betfair.Football.Coupon.start()
        Application.MultiPage (fun ctx -> function
            | Coupon -> CouponPage ctx 
            | Console -> ConsolePage ctx )
