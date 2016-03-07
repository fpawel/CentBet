﻿[<AutoOpen>]
module Prelude

open System
open System.Net
open System.ComponentModel

open Microsoft.FSharp.Reflection

let rec exnRoot (exn:System.Exception) = 
    if exn.InnerException=null then exn else exnRoot exn.InnerException

let htmlDecode (s:string) = System.Web.HttpUtility.HtmlDecode(s.Trim())

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

type ObservableRef<'a>(value:'a) =
    let mutable value = value
    let mutable listeners = []
    
    member __.AddListener1 f =        
        let f = Action<'a * 'a>(f)
        listeners <-  f::listeners
        fun () -> 
            listeners <- List.filter ((<>) f) listeners

    member x.AddListener f =
        x.AddListener1 f |> ignore

    member __.Set v = 
        listeners |> List.iter( fun f -> f.Invoke(value,v) )
        value <- v
    member __.Get () = value 
    member x.Value 
        with get() = x.Get()
        and set v = x.Set v

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

[<AutoOpen>]
module AsyncHelpers = 

    type private M1<'T> = 
        | Get of AsyncReplyChannel<'T> 
        | Set of 'T
        | Upd of ('T -> 'T)

    let createAtom<'T> (init:'T) = 
        let mbox = MailboxProcessor.Start(fun agent ->  
            let rec loop value = async {
                let! msg = agent.Receive()
                return! 
                    match msg with 
                    | Get (r : AsyncReplyChannel<'T>) -> 
                        r.Reply value
                        value
                    | Set (newValue : 'T) -> 
                        newValue 
                    | Upd f -> 
                        f value 
                    |> loop }
            loop init )
        let get() = mbox.PostAndAsyncReply Get
        let set = Set >> mbox.Post  
        let upd = Upd >> mbox.Post  
        get,set,upd

    let createTodayAtom<'T> (request: unit -> Async< 'T option> ) = 
        let get',set,_ = createAtom None

        let update() = async {
            let! x = request()
            match x with
            | Some x -> Some (DateTime.Now, x)
            | _ -> None
            |> set 
            return x }

        fun () -> async{
            let! x = get'()
            match x with
            | None  -> return! update()
            | Some (d,_) when not (d.DateEquals DateTime.Now) -> return! update()
            | Some(_,x) -> return Some x }

    let createTodayAtom1<'T> (init:'T) = 
        let get',set',_ = createAtom None

        let get() = async {
            let! x = get'()
            match x with
            | None  -> return init
            | Some (d,_) when not ( DateTime.dateEquals (d,DateTime.Now) ) -> return init
            | Some(_,x) -> return x }
        let set x = set' (Some (DateTime.Now,init))
        get,set

type OptionBuilder() =
    let bind f v = 
        match v with
        | None -> None
        | Some x -> f x

    member x.Bind(v,f) = bind f  v
    member x.Return v = Some v
    member x.ReturnFrom o = o

    member b.Combine( v, f) = 
        bind f v

    member b.Delay(f ) =  
        f

    member x.Run(f) = 
        bind f (Some ())


type AsyncOptionBuilder() =
    let bind f v' = async{        
        let! v = v'
        match v with
        | None -> return None
        | Some x -> return! f x }

    member x.Bind(v,f) = bind f  v
    
    member x.Return v = async { return Some v }
    member x.ReturnFrom o = o

    member b.Combine( v, f) = 
        bind f v

    member b.Delay(f ) =  
        f

    member x.Run(f) = 
        bind f (async{ return Some ()})

let asyncOpt = AsyncOptionBuilder()

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



