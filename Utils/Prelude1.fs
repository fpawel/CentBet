[<AutoOpen>]
module Prelude1

open System

type Result<'T, 'E> = 
    | Ok of 'T
    | Err of 'E

module Result =
    let isErr = function
      | Err _ -> true
      | _      -> false

    let isOk = function
      | Ok _ -> true
      | _      -> false

    let map f = function
      | Ok x -> Ok( f x )
      | Err e -> Err e  

    let mapErr f = function
      | Ok x -> Ok x
      | Err e -> Err ( f e  )

    let inline bind f = function
      | Ok x ->  f x
      | Err e -> Err e

    module Err = 

        let bind f = function
          | Ok x ->  Ok x
          | Err e -> f e

        let some = function
          | Ok _ ->  None
          | Err e -> Some e

    module Async = 

        let bind f x = async{
            let! x' = x
            match x' with
            | Err x -> return Err x
            | Ok x -> return! f x }

        let map f x = async{
            let! x' = x
            match x' with
            | Err x -> return Err x
            | Ok x -> return Ok ( f x) }

        module Err = 
            let map f x = async{
                let! x' = x
                match x' with
                | Err x -> return Err ( f x )
                | Ok x -> return Ok x }
            let bind f x = async{
                let! x' = x
                match x' with
                | Err x -> return! f x 
                | Ok x -> return Ok x }

        type Builder() =
            
            member x.Bind(v,f) = bind f v
    
            member x.Return v = async { return Ok v }
            member x.ReturnFrom o = o

            member b.Combine( v, f) = 
                bind f v

            member b.Delay(f ) =  
                f

            member x.Run(f) = 
                bind f (async{ return Ok ()})

        let async = Builder()

    module Unwrap = 
        
        let ok = function
          | Ok x -> x
          | Err e -> failwithf "unwraping Err %A as Ok" e
        
        let err = function
          | Ok x -> failwithf "unwraping Ok %A as Err" x
          | Err e -> e

    



module Option = 
    
    let getWith def x =
        match x with
        | Some x' -> x'
        | None -> def

    let getBy f x =
        match x with
        | Some x' -> x'
        | None -> f()

    module Async = 
       
        let inline map f x = async{
            let! x' = x
            return 
                match x' with
                | None -> None
                | Some x'' -> Some (f x'') }

        let inline bind f x = async{
            let! x' = x
            match x' with
            | None -> return None
            | Some x'' -> 
                return! f x'' }

        let bindNone f x = async{
            let! x' = x
            match x' with
            | Some x -> return Some x 
            | _ -> return f x }
        

module Async =
    let inline map f x = async{
        let! x = x
        return f x }

    let inline bind f x = async{
        let! x = x
        return! f x }

    let return'<'a> (x:'a) = async{ return x } 
    
    

type Decimal with
    static member Pow (value,base') =  
        System.Diagnostics.Contracts.Contract.Assert (base'>=0)
        if base'=1 then value 
        elif base'=0 then 1m else
        let mutable A = 1m
        let e = System.Collections.BitArray(BitConverter.GetBytes(base'))
        let t = e.Count
        for i=t-1 downto 0 do
            A <- A * A
            if e.[i] then A <- A * value
        A
    static member DecimalNumbersCount value =
        BitConverter.GetBytes(Decimal.GetBits(value).[3]).[2]

    

let cuttext1 len (text:string)  = 
    if text.Length < len then text 
    elif len < 4 then text 
    else sprintf "%s...[%d]" (text.Substring(0,len-3)) text.Length 

let listToStr<'T> delimString conv (collection : 'T seq )  = 
    collection |> Seq.fold( fun acc x ->
        acc + (if acc |> String.IsNullOrEmpty then acc else delimString) + (conv x) ) ""

let getUniqueString len = 
    let rec loop x = 
        let x = x + Guid.NewGuid().ToString().GetHashCode().ToString("x")
        if x.Length < len then loop x else x
    loop ""



