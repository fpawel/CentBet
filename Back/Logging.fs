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

[<AutoOpen>]
module private Helpers = 
    let consoleTracer = new ConsoleTraceListener(false)

    do
        Trace.Listeners.Add(consoleTracer) |> ignore 
    
    let m = Atoms.Atom( "LOGS", (DateTime.Now, Info, "") )

    let write' level text = 
        Level.trace level text
        m.Set ( DateTime.Now, level, text)
           
let write level format = Printf.kprintf (write' level) format
let error format = Printf.kprintf (write' Error) format
let info format = Printf.kprintf (write' Info) format
let warn format = Printf.kprintf (write' Warn) format
let debug format = Printf.kprintf (write' Debug) format

let addListener f = m.AddChanged (fun _ x -> f x)
