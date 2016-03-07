#I "../packages"
#r "../Utils/bin/Debug/Utils.dll"
#r "FsCheck.2.2.4/lib/net45/FsCheck.dll"

#load "Prelude.fs"
#load "ApiNg.fs"

open System
open FsCheck
open Json
open Json.Serialization

let test<'a when 'a : equality> ( x: 'a ) = 
    x 
    |> serialize      
    |> Json.formatWith JsonFormattingOptions.Pretty
    |> Json.parse
    |> right
    |> deserialize<'a>
    |> right
    |> (=) x

let (~%%) = test

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

open Betfair.ApiNG

Check.Quick ( test<PlaceExecutionReport> |@ "PlaceExecutionReport json serialization" )
Check.Quick ( test<PlaceInstruction>)

Check.Quick ( test<CancelationRequest>)
Check.Quick ( test<CancelExecutionReport>)
Check.Quick ( test<ReplaceRequest>)
Check.Quick ( test<ReplaceExecutionReport>)

Check.Quick ( test<CurrentOrderSummaryReport>)
Check.Quick ( test<EventTypeResult list>)
Check.Quick ( test<EventResultRequest>)
Check.Quick ( test<EventResult list>)
Check.Quick ( test<MarketCatalogueRequest>)


Check.Quick ( test<MarketCatalogue>)

Check.Quick ( test<MarketBookRequest>)
Check.Quick ( test<MarketBook list>)