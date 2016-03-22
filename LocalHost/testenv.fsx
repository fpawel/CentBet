#I "bin/Debug/"
#r "Utils.dll"
#r "Back.dll"
#r "WebFace.dll"

#I @"..\packages"
#r @"FsCheck.2.2.4\lib\net45\FsCheck.dll"

//#I "../packages/Owin.1.0/lib/net40"
//#I "../packages/Microsoft.Owin.3.0.1/lib/net45"
//#I "../packages/Microsoft.Owin.Host.HttpListener.3.0.1/lib/net45"
//#I "../packages/Microsoft.Owin.Hosting.3.0.1/lib/net45"
//#I "../packages/Microsoft.Owin.FileSystems.3.0.1/lib/net45"
//#I "../packages/Microsoft.Owin.StaticFiles.3.0.1/lib/net45"
//#I "../packages/WebSharper.3.6.10.230/lib/net40"
//#I "../packages/WebSharper.Compiler.3.6.10.230/lib/net40"
//#I "../packages/WebSharper.Owin.3.6.9.124/lib/net45"
#load "../packages/WebSharper.Warp.3.6.13.93/tools/reference.fsx"




open System
Async.Start <| async{
    WebSharper.Warp.RunAndWaitForInput( CentBet.Site.Main ) 
    }


open Betfair.Football

open FsCheck





    
let genChar = 
    let chars = 
        [ 'a'..'z' ] @ ['A' .. 'Z'] 

    gen {     
        let! i = Gen.choose (0, List.length chars - 1)     
        return chars.[i] }

let rnd = Random()

type MyGenerators =
    static member String() =
      {new Arbitrary<String>() with
          override x.Generator = gen{
            let! xs = Gen.arrayOfLength 20 genChar
            return new String(xs) }
          override x.Shrinker t = Seq.empty }
    static member Int32() =
      {new Arbitrary<Int32>() with
          override x.Generator = Gen.choose (1, Int32.MaxValue)
          override x.Shrinker t = Seq.empty }

    static member Decimal() =
      {new Arbitrary<Decimal>() with
          override x.Generator = gen{
            let value = rnd.Next(101,1101)
            return (decimal value) / 100m }
          override x.Shrinker t = Seq.empty }





Async.Start<| async {
    Arb.register<MyGenerators>() |> ignore
    
    do! 
        Gen.sample 40 40 Arb.generate<Game * GameInfo >
        |> Betfair.Football.Services.Coupon.Inplay.Set

    let! xs = Betfair.Football.Services.Coupon.Inplay.Get()
    xs |> List.iter (printfn "%A")

    }