#time "on" 
open System
open System.Diagnostics
open System.Threading

type Atom<'T when 'T : not struct>(value : 'T) =
    let refCell = ref value
    
    let rec swap f = 
        let currentValue = !refCell
        let result = Interlocked.CompareExchange<'T>(refCell, f currentValue, currentValue)
        if obj.ReferenceEquals(result, currentValue) then result
        else Thread.SpinWait 20; swap f
        
    member self.Value with get() = !refCell
    member self.Swap (f : 'T -> 'T) = swap f
    
let atom value = 
    new Atom<_>(value)
    
let deref (atom : Atom<_>) =  
    atom.Value
    
let swap (atom : Atom<_>) (f : _ -> _) =
    atom.Swap f


let counter = atom (fun () -> 0)


[   for _ in [1..1000000] do
        yield async { 
            swap counter (fun f -> (fun result () -> result + 1) <| f()) |> ignore
      } ]
|> Async.Parallel |> Async.RunSynchronously |> ignore

let value = (deref counter)() // returns 1000000
printfn "%d" value


let ref1 = atom [|0|]
ref1.Value
ref1.Swap( fun _ -> [|1|] )

let x = ref1.Value
x.[0] <- 13