#I "../packages"
#r "FParsec.1.0.2/lib/net40-client/FParsecCS.dll"
#r "FParsec.1.0.2/lib/net40-client/FParsec.dll"
#r "FsCheck/lib/net45/FsCheck.dll"

#load "Prelude1.fs"
#load "Json.fs"
#load "JsonSerialization.fs"

open System

open FsCheck


open Json
open Json.Serialization


type D1 = 
    | A1
    | A2 of int
    | B2 of int * string
    

type D = {
    E : Map<string, int option>
    F : Map<D1, D1 option>
    J : D1 list  }
    
type AAA = {   
    B : int
    C : string
    G : D }

let test<'a when 'a : equality> ( x: 'a ) = 
    x 
    |> serialize      
    |> Json.formatWith JsonFormattingOptions.Pretty
    |> Json.parse
    |> Result.Unwrap.ok
    |> deserialize<'a>
    |> Result.Unwrap.ok
    |> (=) x

let validcahrs = 
    [ 'a'..'z' ] @ ['A' .. 'Z'] @ ['0' .. '9']
    
let genJsonChar = gen {     
    let! i = Gen.choose (0, List.length validcahrs - 1)     
    return validcahrs.[i] }

Check.Quick ( test<Map<string,string>> |@ "Map<string,string>" )

type MyGenerators =
  static member String() =
      {new Arbitrary<String>() with
          override x.Generator = gen{
            let! xs = Gen.arrayOfLength 50 genJsonChar
            return new String(xs) }
          override x.Shrinker t = Seq.empty }
Arb.register<MyGenerators>()

Check.Quick ( test<string>)
Check.Quick ( test<Map<string,Set<int>>> )
Check.Quick ( test<AAA> )


let d = DateTime.Now


serialize () 
|> Json.formatWith JsonFormattingOptions.Pretty
|> Json.parse
|> Result.Unwrap.ok
|> deserialize<unit>
|> Result.Unwrap.ok

test "3"

let revRevIsOrigFloat (xs:list<float>) = List.rev(List.rev xs) = xs
Check.Quick revRevIsOrigFloat


