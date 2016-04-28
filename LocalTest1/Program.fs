open System
open FsCheck
    
let genChar = 
    let chars = 
        [ 'a'..'z' ] @ ['A' .. 'Z'] 

    gen {     
        let! i = Gen.choose (0, List.length chars - 1)     
        return chars.[i] }

let rnd = Random()

open Betfair.Football

let genKef = gen{
    let! value = Gen.choose(101,1101)
    return! 
        [ None; Some( (decimal value) / 100m ) ]
        |> List.map Gen.constant 
        |> Gen.oneof }

let genStr = gen{
    let! xs = Gen.arrayOfLength 20 genChar
    return new String(xs)
    }

let genMarket : Gen<Betfair.ApiNG.MarketCatalogue> = gen{
    let (~%%) = Gen.constant
    let! marketId = Gen.choose(1,Int32.MaxValue)
    let! marketName = genStr
    let! tm = Gen.choose(1,10)
    let rnr id s : Betfair.ApiNG.RunnerCatalog = 
        {   selectionId  = id
            runnerName  = s
            handicap  = None
            sortPriority  = None
            metadata = Map.empty }
    return
        {   marketId = {marketId = marketId}
        
            marketName  = marketName
                        
            marketStartTime = None                        
            description = None
                        
            totalMatched = Some(decimal tm)
                        
            runners = [ rnr 0 "Победа";  rnr 1 "Ничья"; rnr 0 "Поражение" ]
            eventType  = None
            competition = None
                        
            event = None} }

let genEvent : Gen<Betfair.ApiNG.EventResult> = gen {
    let (~%%) = Gen.constant
    let! id = Gen.choose(1,Int32.MaxValue)
    let! cc = 
        [ "ru"; "en"; "che"; "ua"]
        |> List.map ( Some >> Gen.constant )
        |> Gen.oneof
    return
        {   event = 
                {   id = id
                    name  = ""
                    countryCode  = cc
                    timezone  = None
                    venue = None
                    openDate = None }
            marketCount = 0 } }

type FsCheckGenerators =
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

    static member DecimalOption() =
      {new Arbitrary<Decimal option>() with
          override x.Generator = genKef
          override x.Shrinker t = Seq.empty }

    static member GameInfo() =
      { new Arbitrary<GameInfo>() with
            
            override x.Generator = gen{
                let (~%%) = Gen.constant
                let! score,summary,playMinute,forplay, palystatus = gen {
                    let! h = Gen.choose (1, 5)
                    let! a = Gen.choose (1, 5)


                    let! m' = Gen.choose (1, 90)
                    let! ho = Gen.choose (0, 23)
                    let! mi = Gen.choose (0, 45)

                    return! 
                        [   None, "начало скоро", None, Some (ho,mi), sprintf "%d:%d" ho mi ; 
                            Some(h,a), sprintf "%d - %d" h a, Some m', None, sprintf "%d'" m' ] 
                        |> List.map Gen.constant
                        |> Gen.oneof }

                
                let! k = Gen.arrayOfLength 6 genKef
                return
                    {   score       = score
                        playMinute  = playMinute
                        forplay     = forplay
                        playStatus  = palystatus
                        status      = palystatus
                        summary     = summary
                        order       = 1, 1
                        winBack     = k.[0]
                        winLay      = k.[1]
                        drawBack    = k.[2]
                        drawLay     = k.[3]
                        loseBack    = k.[4]
                        loseLay     = k.[5] }
                }
            override x.Shrinker t = Seq.empty }



[<EntryPoint>]
do
    Async.Start <| async{
        Arb.register<FsCheckGenerators>() |> ignore
        let xs1,xs2 = 
            Gen.sample 100 100 Arb.generate<Game * GameInfo >
            |> List.mapi( fun n (g,i) -> 
                g , { i with GameInfo.order = n / 40 + 1,  n % 40 + 1 
                             playMinute = 
                                if n > 50 then None else
                                Gen.sample 1 1 Arb.generate<int> |> List.head |> Some } )
            |> List.partition( fun (_,x) -> x.playMinute.IsSome )
        Betfair.Football.Services.Coupon.inplay.Set xs1
        Betfair.Football.Services.Coupon.foreplay.Set xs2
        xs1 @ xs2 
        |> List.map( fun (g,_) -> 
            g.gameId, Gen.sample 10 10 genMarket  )        
        |> Map.ofList
        |> Betfair.Football.Coupon.MarketsCatalogue.state.Set        
        xs1 @ xs2 
        |> List.map( fun (g,_) -> 
            let [x] = Gen.sample 1 1 genEvent
            g.gameId, x  )        
        |> Map.ofList
        |> Betfair.Football.Coupon.Events.state.Set

        let events = Betfair.Football.Coupon.Events.state.Value
        match events with
        | None ->
            printfn "ERROR!"
        | Some events ->
            printfn "EVENTS - %d" events.Count
            
        let userPass = IO.File.ReadAllText("../password.txt")
        
        printfn "%A" <| userPass.Split( [|" "; "\n"|], StringSplitOptions.RemoveEmptyEntries )
        //let [|_; betUser; betPass|] = userPass.Split( [|" "; "\n"|], StringSplitOptions.RemoveEmptyEntries )
        
        //Betfair.Login.login betUser betPass   
        Betfair.Football.Coupon.adminBetafirAuth.Set (Some { RestApi.Auth.SessionToken = ""; RestApi.Auth.AppKey = "" }) }
    //Betfair.Football.Coupon.start()
    try        
        WebSharper.Warp.RunAndWaitForInput( CentBet.Site.Main )         
        |> ignore
    with e -> 
        printfn  "%A" e
        System.Console.ReadKey()
        |> ignore
    
