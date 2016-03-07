module Logging

open System
open System.Net
open System.Diagnostics

type Level =
    | Debug 
    | Info
    | Warn
    | Error
    | Fatal    
    static member trace  (l : Level) (s : string)  = 
        (match l with
         | Debug 
         | Info -> Trace.TraceInformation
         | Warn -> Trace.TraceWarning
         | Error 
         | Fatal -> Trace.TraceError) (sprintf "%A %A %s" DateTime.Now l s)

type Record = 
    {   Dates: int64 list
        Level : Level 
        Text : string}    

type Request = int list

type Response =  Record list

[<AutoOpen>]
module private Helpers = 
    let consoleTracer = new ConsoleTraceListener(false)

    do
        Trace.Listeners.Add(consoleTracer) |> ignore 
    

    let low (s:string) = s.ToLower()
    
    type M = Add of Level * string | Get of Request * AsyncReplyChannel<Response>

    let hashes = ResizeArray<int>()
    module X = let get, set, upd = createAtom Map.empty

    let write' level text = 
        Level.trace level text
        X.upd <| fun records ->        
        let h = (level,text).GetHashCode()
        let date = DateTime.nowJavaScriptMillisecons()
        let dates,records' = 
            match Map.tryFind h records with
            | None -> 
                hashes.Add h
                [], records
            | Some {Dates = dates} ->                 
                dates, Map.remove h records
        let records'' = Map.add h { Dates = date :: dates; Level=level; Text=text } records'
        if hashes.Count = 1000 then 
            let removeKey = hashes.[0]
            hashes.RemoveAt 0
            Map.remove removeKey records''
        else
            records''

let getRecords req = async{
    let! recs = X.get()
    let extIds = Set.ofSeq hashes
    let item id = recs.[id]
    let req = Set.ofList req
    return 
        Set.difference extIds req
        |> Seq.map item
        |> Seq.toList }
            
let write level format = Printf.kprintf (write' level) format
let error format = Printf.kprintf (write' Error) format
let info format = Printf.kprintf (write' Info) format
let warn format = Printf.kprintf (write' Warn) format
let debug format = Printf.kprintf (write' Debug) format

