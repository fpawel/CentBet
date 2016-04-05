module Concurency

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
    member self.Set (x : 'T) = 
        swap ( fun _ -> x )
        |> ignore
    
let atom value = 
    new Atom<_>(value)
    
let deref (atom : Atom<_>) =  
    atom.Value
    
let swap (atom : Atom<_>) (f : _ -> _) =
    atom.Swap f

module private Statuses = 
    let xs = atom Map.empty
    let add name f =
        xs.Swap( fun m -> Map.add name f m )
        |> ignore
    let getNames() = xs.Value |> Map.toList |> List.map fst
    let getValue name = 
        xs.Value.TryFind name
        |> Option.map( fun f -> f() )
        
        

type Status (name) =
    let atom = atom ( DateTime.Now, Logging.Info, "initialized"  )    
    do
        Statuses.add name (fun () -> atom.Value)
        Logging.info "-status-%s : initialized" name  
    member __.Set<'a> level (format : Printf.StringFormat<'a,unit>) = 
        format |> Printf.kprintf ( fun text ->
            let _, level', text' = atom.Value
            if level'<>level || text' <> text then
                Logging.write level "-status-%s : %s --> %s" name text' text
                atom.Swap( fun f -> 
                    DateTime.Now, level, text )
                |> ignore )
    static member getNames = Statuses.getNames
    static member getValue = Statuses.getValue

type Temporary<'a> (validTime) = 
    let atom = 
        let init : (DateTime * 'a) option = None
        atom init

    let unwrap() = 
        deref atom 
        |> Option.bind( fun (d,value) -> if validTime d then Some value else None )

    member __.Unwrap() = unwrap()

    member __.Value =  unwrap()

    member __.Set value = 
        atom.Set ( Some (DateTime.Now, value) )

let temporaryRef validTime  = 
    Temporary<_>( validTime )

let todayRef() = 
    temporaryRef ( fun d -> d.DateEquals DateTime.Now)
    
