#I "../packages"
#r "../Utils/bin/Release/Utils.dll"
#r "FsCheck.2.2.4/lib/net45/FsCheck.dll"

#load "Prelude.fs"
#load "ApiNg.fs"

open System
open FsCheck
open Json
open Json.Serialization



open Betfair.ApiNG

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

deserialize<MarketId>( Json.Number 1.456m )

let validcahrs = 
    [ 'a'..'z' ] @ ['A' .. 'Z'] @ ['0' .. '9']
    
let genJsonChar = gen {     
    let! i = Gen.choose (0, List.length validcahrs - 1)     
    return validcahrs.[i] }

type MyGenerators =
    static member String() =
      {new Arbitrary<String>() with
          override x.Generator = gen{
            let! xs = Gen.arrayOfLength 50 genJsonChar
            return new String(xs) }
          override x.Shrinker t = Seq.empty }
    static member Int32() =
      {new Arbitrary<Int32>() with
          override x.Generator = Gen.choose (1, Int32.MaxValue)
          override x.Shrinker t = Seq.empty } 

Arb.register<MyGenerators>()

test {MarketId.marketId = 100}

Check.Quick ( test<Map<string,string>> |@ "Map<string,string>" )
Check.Quick ( test<MarketId> )
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