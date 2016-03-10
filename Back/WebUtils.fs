module WebUtils

open System
open System.Net
open System.Text
open System.IO


let read<'T> (work : unit -> Async< Either<string,'T>>) =  async {    
    try 
        return! work()        
    with 
    | :? System.Net.ProtocolViolationException as exn ->            
        return Left <| sprintf "protocol violation - %A" exn 
    | RootException( HttpExcpetion (message) ) -> 
        return Left <| sprintf "http error - %s" message                
    | RootException exn ->            
        return Left  <| sprintf  "unhandled exection when reading web - %A" exn    } 

