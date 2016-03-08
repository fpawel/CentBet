module Atoms

open System

[<AutoOpen>]
module Helpers = 
    type AtomChangedHandler<'a> = Action<'a,'a> 
    type AtomChangedHandlers<'a> = AtomChangedHandler<'a> list

    type M1<'T> = 
        | Get of AsyncReplyChannel<'T> 
        | Set of 'T
        | Upd of ('T -> 'T)
        | AddChanged of Action<'T,'T> 
        | RemoveChanged of Action<'T,'T> 


type Atom<'a> (what:string,init:'a) =
    //let changedHandlers = ResizeArray<Action<'a,'a>>()
    let raiseCahnged (changedHandlers : AtomChangedHandlers<'a>) value newValue =         
        for h in changedHandlers do
            h.Invoke(value,newValue)

    let mbox = MailboxProcessor.Start(fun agent ->  
        let rec loop (value,changedHandlers : AtomChangedHandlers<'a>) = async {
            let! msg = agent.Receive()
            return! 
                match msg with 
                | Get (r : AsyncReplyChannel<'a>) -> 
                    r.Reply value
                    value, changedHandlers
                | Set (newValue : 'a) -> 
                    raiseCahnged changedHandlers value newValue
                    newValue, changedHandlers
                | Upd f -> 
                    let newValue = f value
                    raiseCahnged changedHandlers value newValue
                    newValue, changedHandlers 
                | AddChanged h -> 
                    value, h::changedHandlers
                | RemoveChanged h -> 
                    value, ( List.filter ((<>) h) changedHandlers )
                |> loop }
        loop (init,[]) )
    member __.What = what
    member __.Get() = mbox.PostAndAsyncReply Get
    member __.Set value = Set value |> mbox.Post  
    member __.Update f = Upd f |> mbox.Post  
    member __.AddChangedHandler h = AddChanged h |> mbox.Post  
    member __.RemoveChangedHandler h = RemoveChanged h |> mbox.Post  
    member x.AddChanged f = x.AddChangedHandler( Action<'a,'a> (f) )

    

type TodayValue<'a> (what,request: unit -> Async< 'a option> ) = 

    let atom = Atom( what, None )

    let update() = async {
        let! x = request()
        match x with
        | Some x -> Some (DateTime.Now, x)
        | _ -> None
        |> atom.Set 
        return x }

    member __.Get() = async{
        let! x = atom.Get()
        match x with
        | None  -> return! update()
        | Some (d,_) when not (d.DateEquals DateTime.Now) -> return! update()
        | Some(_,x) -> return Some x }
    member x.AddChanged f = atom.AddChanged f

type TodayValueRef<'a> (what,init:'a) = 
    let atom = Atom(what,None)

    member __.Get() = async{
        let! x = atom.Get()
        match x with
        | None  -> return init
        | Some (d,_) when not ( DateTime.dateEquals (d,DateTime.Now) ) -> return init
        | Some(_,x) -> return x }
    member __.Set x = atom.Set (Some (DateTime.Now,init))
    member x.AddChanged f = atom.AddChanged f

