[<WebSharper.Pervasives.JavaScript>]
module CentBet.Client.Admin 

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

open CentBet.Remote
open CentBet.Client.Utils

let doc (x : Elt) = x :> Doc 
let (~%%) = doc
let private Br = %% br []

let varMsg = Var.Create None
let setMsg level x = (level, x) |> Some |> Var.Set varMsg
     
let setInfo = setMsg Info
let setError = setMsg Error

let viewMsg = 
    varMsg.View |> View.Map( function
        | None -> Doc.Empty
        | Some (level,s) -> 
            let back,fore = Utils.Level.color level
            pAttr 
                [ Attr.Style "color" fore
                  Attr.Style "background" back  ] 
                [ text  s ] 
            :> Doc  )

let submitButton value onclick = 
    let varOn = Var.Create false
    let do' = async{
        varOn.Value <- true
        do! onclick
        varOn.Value <- false }

    varOn.View |> View.Map( fun isOn ->
        let atrs = [ if isOn then yield attr.disabled "disabled" ]
        Doc.Button value atrs (fun () -> Async.Start do' ) )
    |> Doc.EmbedView

let msgPart =
    varMsg.View 
    |> View.Map( function 
        | Some (level,text) -> 
            let back,fore = CentBet.Client.Utils.Level.color level
            let style = sprintf "background-color : %s; color : %s;" back fore
            %% pAttr [ attr.style style ] [ Doc.TextNode text ] 
        | _ -> Doc.Empty )  
    |> Doc.EmbedView

let loginAdminPart onok = 
    let varAdminKey = Var.Create ""
    let loginAdmin = async {
        printfn "authorize with %A" varAdminKey.Value
        Var.Set varMsg <| Some (Info, "logining admin..")
        let! isOk = CentBet.Remote.authorizeAdmin varAdminKey.Value
        if isOk then             
            setInfo "login admin : success"
            onok()
        else
            setError "login admin : access denied" }
        
    [   %% Doc.PasswordBox [] varAdminKey 
        submitButton "login admin" loginAdmin ]
    

let loginBetfairPart () = 
    let varUser = Var.Create ""
    let varPass = Var.Create ""    
    let loginBetfair =  async{
        setInfo "logining betfair..."
        let! x = CentBet.Remote.loginBetfair(varUser.Value, varPass.Value)
        match x with 
        | Some error ->  Error,error 
        | _ -> Info, "successed!"
        |> Some  |> Var.Set varMsg } 
        

    [   text "User" 
        %% Doc.PasswordBox [] varUser
        text "Pass" 
        %% Doc.PasswordBox [] varPass         
        submitButton "Login betfair" loginBetfair]

let Page isAdmin = 
    let varIsAdmin = Var.Create isAdmin
    varIsAdmin.View
    |> View.Map( function
        | true -> loginBetfairPart () 
        | _ -> loginAdminPart (fun () -> Var.Set varIsAdmin true) ) 
    |> View.Map( fun xs -> xs @ [ msgPart ] |> Doc.Concat )
    |> Doc.EmbedView

