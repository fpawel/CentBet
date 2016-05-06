open System
open System.IO

[<EntryPoint>]
do    
    let userPass = File.ReadAllText("../../../password.txt")
    let [|_; betUser; betPass|] = userPass.Split( [|" "; "\r\n"|], StringSplitOptions.RemoveEmptyEntries )
    let usps = betUser, betPass    

//    Betfair.Football.Coupon.start()
//    Betfair.Football.Coupon.LocalHostTesting.loginBetfair usps
//    |> Async.map (printfn "%A")    
//    |> Async.Start
    
    Betfair.Football.Coupon.LocalHostTesting.run1 (betUser, betPass)
    |> Async.map (printfn "%A")    
    |> Async.Start

    WebSharper.Warp.RunAndWaitForInput( CentBet.Site.Main ) |> ignore  
