open System
open System.IO
open System.Configuration

[<EntryPoint>]
do    
    AppConfig.initialize()
    
//    Betfair.Football.Coupon.start()
//    Betfair.Football.Coupon.LocalHostTesting.loginBetfair usps
//    |> Async.map (printfn "%A")    
//    |> Async.Start
    
    Betfair.Football.Coupon.LocalHostTesting.run1 ()
    |> Async.map (printfn "%A")    
    |> Async.Start
    
    WebSharper.Warp.RunAndWaitForInput( CentBet.Site.Main ) |> ignore  
