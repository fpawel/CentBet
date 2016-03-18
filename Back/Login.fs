module Betfair.Login

open System
open System.Net
open System.Web
open System.Diagnostics
open System.Text
open System.Text.RegularExpressions
open System.IO


let auth = Atom.withLogsByValue "BETFAIR-SESSION-TOKEN-APP-KEY" None 

let login user pass = 
    let uri = 
        sprintf "https://identitysso.betfair.com/api/login?username=%s&password=%s&login=true&redirectMethod=POST&product=home.betfair.int&url=https://www.betfair.com/" user pass
    let req = WebRequest.Create(uri) :?> HttpWebRequest
    req.Method <- "POST"
    req.Timeout <- 15000
    WebUtils.catchInetErrors <| async{
        let! resp = req.GetResponseAsync() |> Async.AwaitTask        
        if resp=null then return Left "no response from betfair.com" 
        elif resp.Headers=null then return Left "no headers in response" else
        Trace.WriteLine "login betfair.com : has got response"
        let x = resp.Headers.GetValues("Set-Cookie") |> Array.toList |> List.choose( fun x -> 
            let m = Regex.Match(x, "ssoid=([^;]+);")
            if m.Success then Some m.Groups.[1].Value else None )
        match x with
        | [] -> 
            use stream = resp.GetResponseStream()
            use reader = new StreamReader(stream, Encoding.UTF8)
            let! sresp = Async.AwaitTask (reader.ReadToEndAsync())
            [   yield "login betfair.com : no sessoin token in headers"
                yield sprintf "Type:%s" resp.ContentType 
                yield sprintf "Length:%d" resp.ContentLength  
                for x in resp.Headers do
                    yield "Header: " + x
                yield sresp ]
            |> List.iter Trace.TraceError
            return Left "no sessoin token in headers"
        | x::_ -> return  Right x  }
    |> Either.bindRightAsyncR ( fun sessionToken -> async{ 
        let! appKey = ApiNG.Services.requestDevelopersAppKey sessionToken
        match appKey with
        | Left error -> return Left <| sprintf "error reciving app key : %s" error
        | Right appKey -> 
            let x = { RestApi.Auth.SessionToken = sessionToken; RestApi.Auth.AppKey = appKey }
            do! auth.Set (Some x)
            return Right x } )
    

