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
        | Set of 'T * AsyncReplyChannel<unit> 
        | Upd of ('T -> 'T) * AsyncReplyChannel<unit>

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
        | Set (newValue : 'a, r) -> 
            r.Reply ()                
            return newValue
        | Upd (f,r) ->             
            r.Reply ()
            return f value }

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
    member x.Set value = mbox.PostAndAsyncReply <| fun r -> Set (value,r) 
    member x.Update f = mbox.PostAndAsyncReply <| fun r -> Upd (f,r) 

let atom what init logs = Atom<'a> ( what, init, logs )


type Status(what,init) = 
    let atom = 
        atom what (DateTime.Now, Logging.Info, init)
            { Equality = fun (_,x,y) (_,x',y') -> (x,y) = (x',y')  
              Format = sprintf "%A"}

    member x.Atom = atom

    member x.Set1<'a> (format : 'a -> string) (value : Either<string,'a> )  = 
        match value with
        | Left error -> DateTime.Now, Logging.Error, error
        | Right value -> DateTime.Now,  Logging.Info, ( format value )
        |> atom.Set

    member x.Set level value = 
        atom.Set (DateTime.Now,  level, value)

type TodayValue<'a> (what, request: unit -> Async< 'a option>, logs ) = 

    let atom = atom what None logs

    let update() = async {
        let! x = request()
        do! atom.Set <|
            match x with
            | Some x -> Some (DateTime.Now, x)
            | _ -> None        
        return x }

    member __.Get() = 
        async{
            let! x = atom.Get()
            match x with
            | None  -> return! update()
            | Some (d,_) when not (d.DateEquals DateTime.Now) -> return! update()
            | Some(_,x) -> return Some x }



type TodayValueRef<'a > (what ,init : 'a, logs) = 
    let atom = atom what None logs

    member __.Get() = async{
        let! x = atom.Get()
        match x with
        | None  -> return init
        | Some (d,_) when not ( DateTime.dateEquals (d,DateTime.Now) ) -> return init
        | Some(_,x) -> return x }
    member __.Set x = atom.Set (Some (DateTime.Now,x))

let todayValue what init logs = TodayValue(what, init, logs)
let todayValueRef what init logs = TodayValueRef(what, init, logs)
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