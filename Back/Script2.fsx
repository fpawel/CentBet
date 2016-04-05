#time "on" 
type AtomChangedHandler<'a> =  string -> 'a -> 'a -> Async<unit>  

type M1<'T> = 
    | Get of AsyncReplyChannel<'T> 
    | Swap of ('T -> Async<'T> ) * AsyncReplyChannel<'T>

type Atom<'a> (init) =
    let processMessage (msg,value) = async{        
        match msg with 
        | Get (r : AsyncReplyChannel<'a>) -> 
            r.Reply value         
            return value
        | Swap (f,r) ->        
            try
                let! newvalue = f value 
                r.Reply newvalue
                return newvalue 
            with e ->                
                return value }

    let mbox = MailboxProcessor.Start(fun agent ->  async {        
        let rec loop (value) = async{            
            let! msg = agent.Receive()
            let! value' = processMessage (msg, value)
            return! loop value' }        
        return! loop (init)
        } )

    let get() = mbox.PostAndAsyncReply Get
    
    member __.Swap f = mbox.PostAndAsyncReply <| fun r -> Swap (f,r) 
    member __.Value  = mbox.PostAndAsyncReply Get

let atom value = 
    new Atom<_>(value)
    
let deref (atom : Atom<_>) = atom.Value
    
let swap (atom : Atom<_>) (f : _ -> _) =
    atom.Swap f


let counter = atom 0


[   for _ in [1..1000000] do
        yield swap counter (fun x -> async{ return x + 1 } ) |> Async.Ignore
        ]
|> Async.Parallel |> Async.RunSynchronously |> ignore

async{ 
    let! value = deref counter // returns 1000000
    printfn "%d" value }
|> Async.Start
