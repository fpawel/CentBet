[<AutoOpen>]
module Prelude1

open System

type Either<'a, 'b> =
    | Left of 'a
    | Right of 'b

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

    let bindRightAsync f x = async{
        let! x' = x
        match x' with
        | Left x -> return Left x
        | Right x -> return f x }

    let bindLeftAsync f x = async{
        let! x' = x
        match x' with
        | Left x -> return Left ( f x)
        | x -> return x }
    
    let bindRightAsyncR f x = async{
        let! x' = x
        match x' with
        | Left x -> return Left x
        | Right x -> return! f x }

    let mapAsync f x = async{
        let! x' = x
        return f x'}


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



