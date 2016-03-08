module Status 

open System
open System.Diagnostics

open Logging


let addAtomValueListener (atom : Atoms.Atom<_>) formater = 
    atom.AddChanged <| fun x y -> 
        if x <> y then
            Logging.debug "Status %s cahnged from %s to %s" atom.What (formater x) (formater y)


let createLoggableAtom what init formater = 
    let atom = Atoms.Atom(what, init)
    addAtomValueListener atom formater
    atom


type Status(what,init) = 
    let atom = createLoggableAtom what  (DateTime.Now, Logging.Info, init) <| fun (_,level,text) -> 
        sprintf "(%A,%s)" level text

    member x.Atom = atom

    member x.Set1<'a> (format : 'a -> string) (value : Either<string,'a> )  = 
        match value with
        | Left error -> DateTime.Now, Logging.Error, error
        | Right value -> DateTime.Now,  Logging.Info, ( format value )
        |> atom.Set

    member x.Set level value = atom.Set (DateTime.Now,  level, value)

    


