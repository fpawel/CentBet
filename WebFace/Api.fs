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

let fail x = async{ return x |> Left  |> serialize }

type Action = 
    | LoginBetfair of string * string * string
    | GetCoupon of ((GameId * int) list) * bool
    
let private return'<'a> : Async<'a> -> Async<Json>  = 
    Either.mapAsync ( serialize<'a> )
    
   
let callAction = function
    | LoginBetfair (adminpass, betuser, betpass) ->
        if not <| Remote.isValidPassword adminpass then fail Remote.``access denied`` else
        Betfair.Login.login betuser betpass
        |> return'
    | GetCoupon (x,y) -> 
        Coupon.getCoupon (x,y)
        |> return'

let processInput (inputStream : System.IO.Stream) = 
    use tr = new System.IO.StreamReader(inputStream)
    let input = tr.ReadToEnd()
    match Json.parse input with
    | Left error -> 
        sprintf "reest api can't parse json from %A"  input
        |> fail
    | Right inputJson -> 
        match deserialize<Action> inputJson with
        | Left error -> 
            sprintf "reest api can't deserialize action from %A"  input
            |> fail
        | Right action -> callAction action
    |> Either.mapAsync( Json.formatWith JsonFormattingOptions.Compact )
    

let loginBetfair apiurl (adminpass, betuser, betpass) = async{    
    let _,send = RestApi.makeRequest apiurl (serialize <| LoginBetfair (adminpass, betuser, betpass) )
    let! x,_ = send
    match x with
    | Left error -> return Left error 
    | Right x -> 
        match deserialize< Either<string, RestApi.Auth> > x with
        | Left error -> return Left error 
        | Right auth -> return  auth

        }
    