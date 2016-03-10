namespace CentBet

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Server

type UserPass = {
    [<FormData "user">]  user : string
    [<FormData "pass">]  pass : string }
type EndPoint =
    | [<EndPoint "/auth"; Method "POST"; FormData("user", "pass") >] Auth of user : string * pass : string
    | [<EndPoint "/login">] Login
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
            | _ -> yield aAttr (refEndpoint Coupon) [text "Игры"] :> Doc ]
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
        Templating.Main ctx EndPoint.Coupon "Купон" [
            client <@ CentBet.Client.Coupon.Main() @> ]

    let AdminPage ctx = 
        Templating.Main ctx Admin "Adminig" [
            client <@ CentBet.Client.Admin.Config() @>             
            ]

    [<Website>]
    let Main =
        let passwordKey = "E018CB561EE1DB0EF3892AE22FCCDD5C" 

        Betfair.Football.Coupon.start()
        Application.MultiPage (fun ctx -> function
            | Auth (user,pass) -> async{
                if user <> "admin" then                    
                    return!                         
                        Content.Text ("wrong user name", encoding = System.Text.UTF32Encoding())
                        |> Content.SetStatus Http.Status.Unauthorized
                elif md5hash pass <> passwordKey then
                    return!                         
                        Content.Text ("wrong password", encoding = System.Text.UTF32Encoding())
                        |> Content.SetStatus Http.Status.Unauthorized
                else 
                    do! ctx.UserSession.LoginUser("admin",false)
                    return! Content.RedirectPermanent Admin
                }
            | Login -> Content.File("PromptPassword.html", ContentType="text/HTML")
            | Coupon -> CouponPage ctx 
            | Admin -> async { 
                let! user = ctx.UserSession.GetLoggedInUser()
                return!
                    match user with 
                    | Some "admin" -> AdminPage ctx  
                    | _ -> Content.RedirectTemporary Login  } )
