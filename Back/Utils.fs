﻿[<AutoOpen>]
module Utils

open System
open System.Net
open System.ComponentModel

open Microsoft.FSharp.Reflection

let rec exnRoot (exn:System.Exception) = 
    if exn.InnerException=null then exn else exnRoot exn.InnerException

let htmlDecode (s:string) = System.Web.HttpUtility.HtmlDecode(s.Trim())



let (|RootException|) = exnRoot

let (|ListContains|) list elem  = list |> List.exists( (=) elem ) 
let (|ListContainsBy|) list f  = list |> List.exists( f ) 
let (|EqualsTo|) lhs rhs = lhs=rhs

type DateTime with

    static member dateEquals(x:DateTime,y:DateTime) = 
        x.Date.Equals(y.Date)
    
    member x.DateEquals(y:DateTime) = 
        x.Date.Equals(y.Date)

    static member MinTimeTicks =
        (DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks
      
    
    static member toJavaScriptMilliseconds(dt:DateTime) =
        (( dt.Ticks - DateTime.MinTimeTicks) / 10000L) 

    static member fromJavaScriptMilliseconds(n:int64) =
        DateTime( n * 10000L + DateTime.MinTimeTicks ) 

    static member nowJavaScriptMillisecons() = 
        DateTime.toJavaScriptMilliseconds <| DateTime.Now.ToUniversalTime()

module Map = 
    let inline union m1 m2 =
        Map.fold (fun acc key value -> Map.add key value acc) m1 m2

module Seq = 
    let inline ids<'T,'a when 'a:comparison> (x : 'T seq)  (getid : 'T -> 'a)  =
        let m = x |> Seq.map(fun g -> (getid g), g) |> Map.ofSeq
        let s = x |> Seq.map( getid ) |> Set.ofSeq
        s,m

module List = 
    let inline ids<'T,'a when 'a:comparison> (x : 'T list)  (getid : 'T -> 'a)  =
        Seq.ids<'T,'a> x getid

let md5hash (input : string) =
    use md5 = System.Security.Cryptography.MD5.Create()
    input
    |> System.Text.Encoding.ASCII.GetBytes
    |> md5.ComputeHash
    |> Seq.map (fun c -> c.ToString("X2"))
    |> Seq.reduce (+)


let tryGetCaseAttribute<'T,'a> (x:'a) = 
    let case,_ = FSharpValue.GetUnionFields(x, x.GetType() )
    case.GetCustomAttributes() |> 
    Seq.tryFind( fun e -> e.GetType()=typeof< 'T > ) |> 
    Option.map( fun atr -> atr :?> 'T )

let caseDescr<'T> (x:'T) = 
    match tryGetCaseAttribute<DescriptionAttribute,'T> x with 
    | None -> sprintf "%A" x
    | Some d -> d.Description


module Logging =

    open System.Diagnostics

    type Level =
        | Debug 
        | Info
        | Warn
        | Error
        | Fatal    
        static member trace  (l : Level) (s : string)  = 
            (match l with
             | Debug -> Trace.WriteLine
             | Info -> Trace.TraceInformation
             | Warn -> Trace.TraceWarning
             | Error 
             | Fatal -> Trace.TraceError) (sprintf "%A %A %s" DateTime.Now l s)

        static member fromResult<'T, 'E> f = function
            | Ok (x : 'T)-> Info, f x
            | Err (e : 'E) -> Error, e

    [<AutoOpen>]
    module private Helpers = 
        let write' level text = 
            Level.trace level text
           
    let write level format = Printf.kprintf (write' level) format
    let error format = Printf.kprintf (write' Error) format
    let info format = Printf.kprintf (write' Info) format
    let warn format = Printf.kprintf (write' Warn) format
    let debug format = Printf.kprintf (write' Debug) format

let (|HttpExcpetion|_|)  =     
    let (<+>) l r = Some (if l=r then l else l + ", " + r )
    let rec loop (x : System.Exception) = 
        match x with
        | null -> None
        | :? Sockets.SocketException as e ->
            Some ( e.Message)
        | :? WebException as exn -> 
            match exn.Response with
            | :? HttpWebResponse as r -> 
                r.StatusDescription <+> exn.Message
            | _ -> Some exn.Message
        | :?  System.AggregateException as exn -> loop exn.InnerException        
        | _ -> None
    loop

let catchInetErrors<'T> (work : Async< Result<'T,string>>) =  async {    
    try 
        return! work
    with 
    | :? System.Net.ProtocolViolationException as exn ->            
        return Err <| sprintf "protocol violation - %A" exn 
    | RootException( HttpExcpetion (message) ) -> 
        return Err <| sprintf "http error - %s" message                
    | RootException exn ->            
        return Err  <| sprintf  "unhandled exection when reading web - %A" exn    } 

let decimalToInt32Safety d = 
    if d <= 2147483647m  && d >= -2147483648m then
        Decimal.ToInt32 d |> Some 
    else
        None




