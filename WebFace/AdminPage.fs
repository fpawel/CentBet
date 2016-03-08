[<WebSharper.Pervasives.JavaScript>]
module CentBet.Client.Admin 

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

open CentBet.Remote
open CentBet.Client.Utils

let private Br = br [] :> Doc

let Config () = 
    let varUser = Var.Create ""
    let varPass = Var.Create ""
    let varOn = Var.Create false
    let varMsg = Var.Create (Info,"")

    let login() = 
        async{
            varOn.Value <- true 
            Var.Set varMsg (Info,"logining betfair...")
            let! x = CentBet.Remote.loginBetfair(varUser.Value, varPass.Value)
            varOn.Value <- true
            Var.Set varMsg <| match x with 
                                | Some error -> Error,error
                                | _ -> Info, "successed!" } 
        |> Async.Start 

    [   text "User" ; Br 
        Doc.PasswordBox [] varUser :> Doc; Br
        text "Pass" ; Br
        Doc.PasswordBox [] varPass :> Doc; Br; Br;        
        varOn.View |> View.Map( fun isOn ->
            let atrs = [ if isOn then yield attr.``class`` "hide" ]
            Doc.Button "Login" atrs login :> Doc )
        |> Doc.EmbedView
        varMsg.View |> View.Map( fun (level,text) -> 
            let back,fore = CentBet.Client.Utils.Level.color level
            let style = sprintf "background-color : %s; color : %s;" back fore
            pAttr [ attr.style style ] [ Doc.TextNode text ] )
        |> Doc.EmbedView ]     
    |> Doc.Concat

