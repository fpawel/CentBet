namespace CentBet.Client
open System
open WebSharper
open WebSharper.JavaScript

[<JavaScript>]
module Utils = 

    let rec trimEnd = function
        | "",_ -> ""
        | s, [] -> s
        | s, (c :: _ as rx) when s.[s.Length-1] = c -> trimEnd (s.[..s.Length-2],rx)
        | s, _::rx -> trimEnd (s,rx)

    let formatDecimal x = 
        trimEnd (sprintf "%M" x, ['0'; '.'])

    let formatDecimalOption = function
        | None -> ""
        | Some x -> formatDecimal x

    [<Direct "dateTimeToString($s)">]
    let dateTimeToString (s: int64) : string = failwith "n/a"

    

    let mkids<'T,'a when 'a:comparison> (x : 'T list)  (getid : 'T -> 'a)  =
        let m = x |> List.map(fun g -> (getid g), g) |> Map.ofList
        let s = x |> List.map( getid ) |> Set.ofList
        s,m

    let getDatePart (x:JavaScript.Date) = 
        let y = Date(x.GetTime())
        y.SetHours(0,0,0,0)
        y

    [<Inline "createBlobFromString($arg)">]
    let createBlobFromString (arg : string) : Blob = failwith "n/a"

    type Level =
        | Debug 
        | Info
        | Warn
        | Error
        | Fatal  

        static member color = function
            | Info -> "white", "navy"
            | Error -> "lightgrey", "red"
            | Warn -> "white", "green"
            | Fatal -> "black", "yellow"
            | Debug -> "lightgrey", "gray"

