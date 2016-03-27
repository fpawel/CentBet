[<AutoOpen>]
module Prelude1

open System

type Either<'a,'b> = Choice<'a,'b>
let (|Left|Right|) = function
    | Choice1Of2 a -> Left a
    | Choice2Of2 b -> Right b

let Left = Choice1Of2
let Right = Choice2Of2



            

[<AutoOpen>]
module Either =
    
    let isLeft = function
        | Left _ -> true
        | _      -> false

    let isRight = function
        | Right _ -> true
        | _      -> false

    let right (Right x) = x
    let left (Left x) = x

    let rightSome = function
        | Right x -> Some x
        | _      -> None

    let leftSome = function
        | Left x -> Some x
        | _      -> None

    let mapRight f = function
        | Left x -> Left x
        | Right x -> Right (f x)

    let mapLeft f = function
        | Left x -> Left (f x)
        | Right x -> Right x

    let map2 fe f = function
        | Left x -> Left (fe x)
        | Right x -> Right (f x)

    let bindRight f = function
        | Left x -> Left x
        | Right x -> f x

    type AsyncBuilder() =
        let bind f v' = async{        
            let! v = v'
            match v with
            | Left x -> return Left x
            | Right x -> return! f x }

        member x.Bind(v,f) = bind f  v
    
        member x.Return v = async { return Right v }
        member x.ReturnFrom o = o

        member b.Combine( v, f) = 
            bind f v

        member b.Delay(f ) =  
            f

        member x.Run(f) = 
            bind f (async{ return Right ()})

let asyncEiter = Either.AsyncBuilder()

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
        let! x = x
        return f x }

    let inline bind f x = async{
        let! x = x
        return! f x }

    let inline mapOption f x = async{
        let! x' = x
        return 
            match x' with
            | None -> None
            | Some x'' -> Some (f x'') }

    let inline bindOption f x = async{
        let! x' = x
        match x' with
        | None -> return None
        | Some x'' -> 
            return! f x'' }

    let bindEither f x = async{
        let! x' = x
        match x' with
        | Left x -> return Left x
        | Right x -> return! f x }

    let bindRight f x = async{
        let! x' = x
        match x' with
        | Left x -> return Left x
        | Right x -> return f x }

    let mapLeft f x = async{
        let! x' = x
        match x' with
        | Left x -> return Left ( f x)
        | x -> return x }

    let id<'a> (x:'a) = async{ return x } 
    
    

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



