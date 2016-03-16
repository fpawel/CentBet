﻿[<WebSharper.Pervasives.JavaScript>]
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



type Level = 
    | RespOk
    | RespError
    | Req
    static member color = function
        | RespOk -> "white", "navy"
        | RespError -> "lightgrey", "red"
        | Req -> "lightgrey", "green"

type Record = { 
    Id : Key
    Text: string
    Level: Level }
let recordKey x = x.Id

type Cmd =  { 
    Id : Key
    Text: string }

let cmdKey x = x.Id

let varCommandsHistory = 
    ListModel.CreateWithStorage cmdKey (Storage.LocalStorage "CentBetConsoleCommandsHistory" Serializer.Default)
let varConsole = 
    ListModel.CreateWithStorage recordKey (Storage.LocalStorage "CentBetConsole" Serializer.Default)

let ``cmd-input`` = "cmd-input"

[<Inline "$el.focus()">]
let focus (el : Dom.Element) = ()

let renderRecord = 
    View.Map( fun r ->
        let back,fore = Level.color r.Level
        %% spanAttr 
            [Attr.Style "color" fore; Attr.Style "background" back] 
            [ text r.Text ]  )
    >> Doc.EmbedView



let addRecord level text = 
    varConsole.Add {Id = Key.Fresh(); Text = text; Level = level }
    

let send (inputText : string) = async{        
    let inputText = inputText.Trim()
    addRecord Req inputText 
    
    if (let xs = varCommandsHistory.Value in
        Seq.isEmpty xs || ( let x = Seq.last xs in x.Text <> inputText )) then
        varCommandsHistory.Add { Id = Key.Fresh(); Text = inputText }
          
    try
        let! (res,msg) = CentBet.Remote.perform inputText
        addRecord RespOk msg 
    with e ->
        addRecord RespError e.Message  }


let mutable varCmd = None
let tryGetCommandFromHistory isnext =
    let xs = varCommandsHistory.Value
    let count = Seq.length xs
    if count = 0 then None else   
    let n = 
        match varCmd with
        | None -> count - 1
        | Some {Cmd.Id = id'} ->
            let v = 
                varCommandsHistory.Value 
                |> Seq.mapi( fun n x -> x,n)    
                |> Seq.tryFind( fun ({Id = id''},_) -> id''=id' )   
            
            match v, isnext with
            | Some (v,n), true when count > 0 && n < count - 1 -> n + 1
            | Some (v,n), false when count > 0 && n > 0 -> n - 1
            | _, true -> count - 1
            | _ -> 0
    Seq.nth n xs |> Some

let renderInput () =         
   

    let varInput = Var.Create ""
    let rvFocusInput = Var.Create ()
    let varDisableInput = Var.Create false

    let doSend = async{
        varDisableInput.Value <- true
        do! send varInput.Value
        varDisableInput.Value <- false
        varInput.Value <- "" }

    let setCommandFromHistory  = 
        tryGetCommandFromHistory 
        >> Option.map( fun cmd -> 
            varCmd <- Some cmd
            varInput.Value <- cmd.Text ) 
        >> ignore

    varDisableInput.View  |> View.Map( fun disable -> 
        [   Doc.Input [  
                yield attr.id ``cmd-input``
                if disable then yield attr.disabled "disabled" 
                yield Attr.Style "width" "80%" 
                yield Attr.CustomVar rvFocusInput (fun el _ -> focus(el)) (fun _ -> None) 
                yield on.keyDown ( fun _ e -> 
                    let key : int = e?keyCode
                    match key with
                    | 13 -> Async.Start doSend 
                    | 38 -> setCommandFromHistory true
                    | 40 -> setCommandFromHistory false 
                    | _ -> () )
                ] varInput  :> Doc ]
        |> Doc.Concat )
                

                
                
//            vHandleKeypressed |> View.Map (fun _ -> Doc.Empty) |> Doc.EmbedView 
            
         
    |> Doc.EmbedView

let RenderCommandPrompt() = 
    
    [   %% spanAttr [ Attr.Style "margin-left" "10px" ] []
        %% labelAttr [ attr.``for`` ``cmd-input`` ] [ text "Input here:" ]
        renderInput ()  ] 
    |>  Doc.Concat 

let RenderMenu() =     
    [   aAttr [
            yield attr.href "#"
            yield Attr.Handler "click" (fun _ _ -> varConsole.Clear()  ) ]
            [text "Clear console"]  
        aAttr [
            yield attr.href "#"
            yield Attr.Handler "click" (fun _ _ -> varCommandsHistory.Clear()  ) ]
            [text "Clear history"] ] 
    |> List.map( fun x -> x :> Doc )
    |> Doc.Concat 


let RenderRecords() = varConsole.View |> Doc.BindSeqCachedView  ( fun r -> 
    [   renderRecord r
        %% br [] ]
    |> Doc.Concat )

let Render() = 
    [   RenderCommandPrompt()
        %% br []
        RenderRecords() ]
    |> Doc.Concat
