#I "../packages"
#r "../Utils/bin/Debug/Utils.dll"
#r "FsCheck.2.2.4/lib/net45/FsCheck.dll"
#r "FSharpx.Collections.1.14.0/lib/net40/FSharpx.Collections.dll"

#load "Prelude.fs"
#load "Logging.fs"

open System
open FsCheck

open System.Diagnostics

let chars = [ 'a'..'z' ] @ ['A' .. 'Z'] @ ['0' .. '9']

let rnd = Random()
let sleeprnd() =
    Async.Sleep(rnd.Next(1000,3000))

#time "on"
async{
    printfn "start..."
    let! _ = 
        [0..3000] |> List.map( fun _ ->  async{ 
            Logging.info "%s" (getUniqueString 50)
            do! sleeprnd()
            Logging.info "%s" (getUniqueString 50) } )
        |> Async.Parallel
    let! xs = Logging.getRecords []
    printfn "ready! %d records" xs.Length
    return ()
} |> Async.Start


Logging.info "%s" "some 17" 

async{ 
    let! xs = Logging.getRecords []
    printfn "%d records" xs.Length   } 
|> Async.Start


async{ 
    let! xs = Logging.getRecords []

    let xs' = xs |> List.sortBy( fun {Dates = dates} -> 
        if dates.IsEmpty then None else List.max dates |> Some )
    let last = xs' |> List.rev |> List.head
    let dates = last.Dates |> List.map( fun d -> 
        TimeZone.CurrentTimeZone.ToLocalTime(DateTime.fromJavaScriptMilliseconds(d)) )
    printfn "%A %A %A" dates last.Level last.Text } 
|> Async.Start


