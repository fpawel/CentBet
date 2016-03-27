module Atom

open System
open System.Diagnostics


module Trace =
    type private S = unit -> Async<string>

    type private M1 = 
        | Add of string * S
        | GetValue of string * AsyncReplyChannel<string> 
        | GetNames of AsyncReplyChannel<string list> 


    let private mbox = MailboxProcessor.Start(fun agent ->  async {        
        
        let rec loop (xs) = async{            
            let! msg = agent.Receive()
            match msg with 
            | GetValue (s,r) ->
                match Map.tryFind s xs with
                | Some x -> 
                    let! x = x()
                    r.Reply x
                | _ -> r.Reply ""
                return! loop xs
            | GetNames r -> 
                Map.toList xs |> List.map fst |> r.Reply
                return! loop xs
            | Add (s,f) ->             
                return! Map.add s f xs  |> loop}        
        return! loop Map.empty  } )

    let getNames() = 
        mbox.PostAndAsyncReply( fun r -> GetNames r)

    let getAtomValue atom = 
        mbox.PostAndAsyncReply( fun r -> GetValue(atom,r) )

    let add wht get = 
        mbox.Post <| Add ( wht, get )


[<AutoOpen>]
module Helpers = 
    type AtomChangedHandler<'a> =  string -> 'a -> 'a -> Async<unit>  

    type M1<'T> = 
        | Get of AsyncReplyChannel<'T> 
        | Upd of ('T -> Async<'T> ) * AsyncReplyChannel<'T>

module Logs = 


    type Options<'a> = {
        Equality : 'a -> 'a -> bool
        Format : 'a -> string }

    let byValue<'a when 'a : equality> : Options<_>  = { Equality = (=); Format = fun (x : 'a) -> sprintf "%A" x}

    let none = { Equality = fun _ _ -> true 
                 Format = fun _ -> "" }

type Atom<'a> ( what, init, logs : Logs.Options<'a>   ) =
   
    
    let processMessage (msg,value) = async{        
        match msg with 
        | Get (r : AsyncReplyChannel<'a>) -> 
            r.Reply value         
            return value
        | Upd (f,r) ->        
            try
                let! newvalue = f value 
                r.Reply newvalue
                return newvalue 
            with e ->
                Logging.error "-atom-update-exception %A : %A" what e
                return value }

    let mbox = MailboxProcessor.Start(fun agent ->  async {        
        Logging.debug "-atom-started %s" what 
        let rec loop (value) = async{            
            let! msg = agent.Receive()
            let! value' = processMessage (msg, value)
            if not <| logs.Equality value value' then
                Logging.debug  "-atom-cahnged  %s : %s -> %s" what (logs.Format value) (logs.Format value')   
            return! loop value' }        
        return! loop (init)
        } )

    let get() = mbox.PostAndAsyncReply Get
    do
        let get() = async{
            let! v = get()
            return sprintf "%A" v }
        Trace.add what get
        


    member __.What = what
    member __.Get ()  = mbox.PostAndAsyncReply Get
    member x.UpdateAsync f = mbox.PostAndAsyncReply <| fun r -> Upd (f,r) 
    member x.Update f = x.UpdateAsync (fun x -> async{ return f x  } )
    member x.Set value = 
        x.Update (fun _ -> value)
        |> Async.map ignore
    
    

let atom what init logs = Atom<'a> ( what, init, logs )


type Status(what,init) = 
    let atom = 
        atom what (DateTime.Now, Logging.Info, init)
            { Equality = fun (_,x,y) (_,x',y') -> (x,y) = (x',y')  
              Format = sprintf "%A"}

    member x.Atom = atom

    member x.Set1 (value : string option )  = 
        match value with
        | Some error -> DateTime.Now, Logging.Error, error
        | _ -> DateTime.Now,  Logging.Info, "Ok"
        |> atom.Set

    member x.Set (level,value) = 
        atom.Set (DateTime.Now,  level, value)


type Temporary<'a> (what, validTime, logs) = 
    let atom = atom what None logs    

    let update f = atom.UpdateAsync <| fun value -> async{
        let! (value : 'a option) = value |> Option.bind( fun (d, x) ->  if validTime d then Some x else None) |> f
        return value |> Option.map( fun x -> DateTime.Now,x ) } 

    member __.Get() = 
        update Async.id |> Async.mapOption snd

    member __.Set x = atom.Set (Some (DateTime.Now,x))

    member __.Update f = 
        update f 
        |> Async.mapOption snd
        |> Async.map ignore

let temporary what logs validTime  = 
    Temporary( what, validTime, logs)


let today what logs = 
    temporary what logs ( fun d -> d.DateEquals DateTime.Now)
    
let withLogsByValue<'a when 'a : equality> what (init : 'a ) = atom what init Logs.byValue
let status what init = Status(what,init)

let withListLogs =  
    let eq x y = 
        match x,y with
        | _::_,[] -> false
        | [], _::_ -> false
        | _ -> true
    let f = function 
        | [] -> "empty"
        | _ -> "items"
    fun wht vl ->
        atom wht vl {Equality = eq; Format = f}

let withNoLogs wht vl =  
    atom wht vl Logs.none