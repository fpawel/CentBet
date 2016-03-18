module WebUtils

open System
open System.Net
open System.Text
open System.IO


let catchInetErrors<'T> (work : Async< Either<string,'T>>) =  async {    
    try 
        return! work
    with 
    | :? System.Net.ProtocolViolationException as exn ->            
        return Left <| sprintf "protocol violation - %A" exn 
    | RootException( HttpExcpetion (message) ) -> 
        return Left <| sprintf "http error - %s" message                
    | RootException exn ->            
        return Left  <| sprintf  "unhandled exection when reading web - %A" exn    } 

open Json

[<AutoOpen>]
module private Helpers =
    let formatCompact = Json.formatWith JsonFormattingOptions.Compact
    let (|P|_|) k = prop k

let transmitJson (url:string) headers (requestJson : Json) =
    async {        
        ServicePointManager.Expect100Continue <- false
        let request = HttpWebRequest.Create url :?> HttpWebRequest
        request.Method <- "POST"
        request.ContentType <- "application/json"
        //request.UserAgent <- "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)"    
        request.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8")
        request.Accept <- "application/json"
        headers |> Map.iter( fun (header:string) (value:string) -> 
            request.Headers.Add(header, value) )
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
        return Json.parse responseString }
    |> catchInetErrors

