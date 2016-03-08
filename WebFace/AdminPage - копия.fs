[<WebSharper.Pervasives.JavaScript>]
module CentBet.Client.Admin 

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

open CentBet.Admin

let PromptEnterKey onSuccess = 
    let varAdminKey = Var.Create ""
    let varOn = Var.Create false
    let varError = Var.Create None
    let varSubmit = Var.Create ()
        

    let authorize () = async {
        printfn "authorize with %A" varAdminKey.Value
        Var.Set varOn true
        Var.Set varError None            
        let! x = CentBet.Admin.authorize varAdminKey.Value            
        Var.Set varOn false

        match x with 
        | None -> 
            printfn "authorized" 
            onSuccess()
        | Some error -> 
            printfn "authorization error %A" error
            Var.Set varError (Some error) } 

    let vAuth = 
        varAdminKey.View 
        |> View.Map( fun x -> 
            if x="" then () else
            authorize() |> Async.Start )
        |> View.SnapshotOn () varSubmit.View


    [   
        p [ Doc.InputArea [] varAdminKey ] :> Doc
        p [ Doc.Button "submit" [] <| fun () -> Var.Set varSubmit () ] :> Doc
        
        vAuth |> View.Map (fun () -> Doc.Empty) |> Doc.EmbedView

        varOn.View |> View.Map( function
            | false -> Doc.Empty
            | _ -> p[ text "Выполняется авторизация..." ] :> Doc )
        |> Doc.EmbedView

        varError.View |> View.Map( function
            | None -> Doc.Empty
            | Some error -> pAttr [Attr.Style "color" "red"] [ text error ] :> Doc )
        |> Doc.EmbedView ]
    |> Doc.Concat

let Config () = 
    let varSessionToken = Var.Create ""
    let varAppKey = Var.Create ""
    let varOn = Var.Create false
    let varError = Var.Create None
    [   p[ text "session token" ]
        p[ Doc.Input [] varSessionToken ]
        p[ text "developers app key" ]
        p[ Doc.Input [] varAppKey ]
        p[ Doc.Button "Send" [] <| fun () -> 
            ()
            ] ]
    |> List.map( fun x -> x :> Doc )
    |> Doc.Concat

let Main isAdmin = 
    let varIsAdmin = Var.Create isAdmin
    varIsAdmin.View |> View.Map( function
        | false -> PromptEnterKey (fun () -> Var.Set varIsAdmin true)
        | _ -> Config () )
    |> Doc.EmbedView
    
