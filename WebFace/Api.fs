module CentBet.Api

open System
open System.Threading
open System.Globalization

open WebSharper


open Betfair
open Betfair.Football
open Betfair.Football.Services

open Json
open Json.Serialization

let (|Admin|) = function
    | Prop "password" ( Json.String password) when Remote.isValidPassword password -> true
    | _ -> false


let fail (format : _ )=    
    Printf.kprintf 
        ( fun x -> 
            ["Err", Json.String x]
            |> Map.ofList
            |> Json.Object
            |> Async.return' ) 
        format

type Action = 
    | LoginBetfair of string * string * string
    | GetCouponPage of ((GameId * int) list) * int * int
    
let callAction = 
    function
        | LoginBetfair (adminpass, betuser, betpass) ->
            if not <| Remote.isValidPassword adminpass then fail "%s" Remote.``access denied`` else
            Betfair.Login.login betuser betpass
            |> Async.map serialize 
        | GetCouponPage (x,y,z) ->
            Coupon.getCouponPage (x,y,z)
            |> Async.map serialize
    >> Async.map( fun x -> 
        ["Ok", x]
        |> Map.ofList
        |> Json.Object )
        
    

let processInput (inputStream : System.IO.Stream) = 
    use tr = new System.IO.StreamReader(inputStream)
    tr.ReadToEnd()
    |> Json.parse 
    |> Result.mapErr (sprintf "can't parse json from %A")
    |> Result.bind (
        deserialize<Action> 
        >> Result.mapErr (sprintf "can't deserialize action from %A") )
    |> function
        | Ok action -> callAction action        
        | Err x -> fail "%s" x
    |> Async.map( Json.formatWith JsonFormattingOptions.Compact )


let loginBetfair apiurl (adminpass, betuser, betpass) = 

    RestApi.makeRequest apiurl (serialize <| LoginBetfair (adminpass, betuser, betpass) )
    |> snd 
    |> Async.map fst
    |> Result.Async.bind1 ( fun x -> 
        deserialize< Result<RestApi.Auth,string> > x )
    
    

let getCoupon apiurl (games, npage, pagelen) = 
    RestApi.makeRequest apiurl (serialize <| GetCouponPage (games, npage, pagelen) )
    |> snd
    |> Async.map fst
    |> Result.Async.bind1 deserialize< Coupon.CouponResponse >