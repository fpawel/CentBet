﻿module RestApi

open System
open System.Net
open System.Text
open System.IO

open Json

type ApiService = 
    {   ApiUrl : string   
        ApiMethod : string }
    static member what x = 
        sprintf "url : %A, method : %A" x.ApiUrl x.ApiMethod

[<AutoOpen>]
module private Helpers =
    let formatPrety = Json.formatWith JsonFormattingOptions.Pretty
    let formatCompact = Json.formatWith JsonFormattingOptions.Compact
    let (|P|_|) k = prop k

    let makeArgs service args =        
        Json.obj 
            [   "jsonrpc", String "2.0"
                "method", String service.ApiMethod
                "params", args 
                "id", Number 1m ]

    let (|ApiError|_|) = function 
        | P "error" 
           (P "data" 
             (P "exceptionname" 
               (String exceptionname) & 
                 (P "errorCode" ( String errorCode) ) &
                 (P "errorDetails" ( String errorDetails) ) )) ->
            Some(exceptionname, errorCode, errorDetails)
        | _ -> None

type Auth1 = { 
    SessionToken : string
    AppKey : string option }
        
    
let private callUntyped auth service requestArgsJson =
    let requestJson = 
        Json.obj         
            [   "jsonrpc", String "2.0"
                "method", String service.ApiMethod
                "params", requestArgsJson
                "id", Number 1m ]

    WebUtils.read <| fun () -> async {        
        ServicePointManager.Expect100Continue <- false
        let request = HttpWebRequest.Create service.ApiUrl :?> HttpWebRequest
        request.Method <- "POST"
        request.ContentType <- "application/json"
        //request.UserAgent <- "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)"    
        request.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8")
        request.Accept <- "application/json"
        match auth.AppKey with
        | None -> ()
        | Some x -> request.Headers.Add("X-Application", x)
        request.Headers.Add("X-Authentication", auth.SessionToken)

        let requestString = formatCompact requestJson
        //let whatReq = strPrms |> sprintf "sessionToken=%A X-Application=%A %s" sessionToken appKey 
        let requestData = Encoding.UTF8.GetBytes requestString
        request.ContentLength <- int64 requestData.Length 
        
        use stream = request.GetRequestStream()    
        let! n = stream.AsyncWrite(requestData, 0, requestData.Length)
        
        let response = request.GetResponse()
        use stream = response.GetResponseStream()
        use reader = new StreamReader(stream, Encoding.UTF8)
        let! responseString = Async.AwaitTask ( reader.ReadToEndAsync() )
        let! canDebugLogging = Config.enableApiNgDebugLogs.Get()

        let xresp = Json.parse responseString
        match xresp with 
        | Left error -> Some(Logging.Error, "no answer")
        | Right json -> Some( Logging.Debug, formatPrety json)
        |> function 
            | None -> ()
            | Some (level, s) -> 
                Logging.write level "rest api - %A, %A, %s -> %s" auth service (formatPrety requestJson) s

        return 
            xresp 
            |> Either.bindRight ( fun responseJson -> 
                match responseJson with 
                | ApiError (exceptionname, errorCode, errorDetails) ->                     
                    Left <| sprintf "%A, exceptionname - %A, errorCode - %A " errorDetails exceptionname errorCode
                | P "result" json -> Right json
                | _ -> Left "missing property \"result\" in response" )            
            |> Either.mapLeft (fun error -> 
                sprintf "rest api error - %s, service %s" (ApiService.what service) error ) }

let callWithoutAppKey sessionToken service requestArgsJson =
    callUntyped { SessionToken = sessionToken; AppKey = None} service requestArgsJson

type Auth = { 
    SessionToken : string
    AppKey : string }


let callWithAppKey auth service requestArgsJson =
    callUntyped { SessionToken = auth.SessionToken; AppKey = Some auth.AppKey} service requestArgsJson

let call auth service (parseResult : _ -> Either<string,_> ) (request : _ ) : Async<_> = async{
    let! x = callUntyped {SessionToken = auth.SessionToken; AppKey = Some auth.AppKey} service (Json.Serialization.serialize request)     
    return x |> Either.bindRight ( fun json -> 
        Json.Serialization.deserialize json
        |> Either.mapLeft ( sprintf "rest api : error deserealizing response, %A, %A - %s" auth service )
        |> Either.bindRight( fun r -> 
            parseResult r
            |> Either.mapLeft 
                ( sprintf "rest api : error parsing result, %A, %A, %s - %s" auth service (formatPrety json ) ) ) ) }  

let callForType<'a> auth service request : Async<Either<string,'a>> = call auth service Right request

    
    

