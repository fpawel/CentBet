﻿
[<EntryPoint>]
let main argv = 
    //Json.Serialization.deserialize<Betfair.ApiNG.MarketId> (Json.String "1.123123")
    WebSharper.Warp.RunAndWaitForInput( CentBet.Site.Main )
