module Betfair.Login

open System
open System.Net
open System.Web
open System.Diagnostics
open System.Text
open System.Text.RegularExpressions
open System.IO

[<AutoOpen>]
module private Helpers = 
    type A = RestApi.Auth
    let tryGetSessiontken x = 
        let m = Regex.Match(x, "ssoid=([^;]+);")
        if m.Success then Some m.Groups.[1].Value else None

    let getResponse user pass = 
        let uri = 
            sprintf 
                "https://identitysso.betfair.com/api/login?username=%s&password=%s&login=true&redirectMethod=POST&product=home.betfair.int&url=https://www.betfair.com/" 
                user pass
        let req = WebRequest.Create(uri) :?> HttpWebRequest
        req.Method <- "POST"
        req.Timeout <- 15000    
        req.GetResponseAsync() |> Async.AwaitTask 

let login user pass = 
    async{
        let! resp = getResponse user pass
        if resp=null then return Err "no response from betfair.com" 
        elif resp.Headers=null then return Err "no headers in response" else
        Trace.WriteLine "login betfair.com : has got response"
        return 
            resp.Headers.GetValues("Set-Cookie") 
            |> Array.tryPick tryGetSessiontken
            |> Option.map Ok
            |> Option.getWith (Err "no sessoin token in headers") }
    |> catchInetErrors
    |> Result.Async.bind ( fun sessionToken ->
        ApiNG.Services.requestDevelopersAppKey sessionToken
        |> Result.Async.map( fun appKey -> 
            {   A.SessionToken = sessionToken
                A.AppKey = appKey } )
        |> Result.Async.Err.map (sprintf "error reciving app key : %s") )
    

