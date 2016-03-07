namespace CentBet

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Server

type EndPoint =
    | [<EndPoint "/">] Coupon
    | [<EndPoint "/admin">] Admin

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
            | Admin -> yield aAttr (refEndpoint Coupon) [text "Игры"] :> Doc ]

    let Main ctx action title body =
       Content.Page(            
            MainTemplate.Doc(
                title = title,
                navbar = NavBar ctx action,
                body = body ))

module Site =
    open WebSharper.UI.Next.Html

    
        
    let CouponPage ctx =
        Templating.Main ctx EndPoint.Coupon "Купон" [
            client <@ CentBet.Client.Coupon.Main() @> ]

    let AdminPage1 ctx isAdmin = 
        let isAdmin = isAdmin
        Templating.Main ctx EndPoint.Coupon "Администрирование" [
            client <@ CentBet.Client.Admin.Main isAdmin @> 
            
            ]

    let AdminPage ctx = async{ 
        let! user = ctx.UserSession.GetLoggedInUser()
        return! AdminPage1 ctx user.IsSome }
        

    [<Website>]
    let Main =
        CentBet.ProcessCoupon.start()
        Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | Coupon -> CouponPage ctx 
            | Admin -> AdminPage ctx  )
