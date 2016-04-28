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

let fail x = async{ return x |> Err  |> serialize }




type Action = 
    | LoginBetfair of string * string * string
    | GetCouponPage of ((GameId * int) list) * int * int

    //| TestSet1 of ((GameId * int) list) * bool
    
    
    
let callAction = function
    | LoginBetfair (adminpass, betuser, betpass) ->
        if not <| Remote.isValidPassword adminpass then fail Remote.``access denied`` else
        Betfair.Login.login betuser betpass
        |> Async.map serialize 
    | GetCouponPage (x,y,z) ->
        Coupon.getCouponPage (x,y,z)
        |> Async.map serialize
    
let processInput (inputStream : System.IO.Stream) = 
    use tr = new System.IO.StreamReader(inputStream)
    let input = tr.ReadToEnd()
    match Json.parse input with
    | Err error -> 
        sprintf "reest api can't parse json from %A"  input
        |> fail
    | Ok inputJson -> 
        match deserialize<Action> inputJson with
        | Err error -> 
            sprintf "reest api can't deserialize action from %A"  input
            |> fail
        | Ok action -> callAction action
    |> Async.map( Json.formatWith JsonFormattingOptions.Compact )
    

let loginBetfair apiurl (adminpass, betuser, betpass) = async{    
    let _,send = RestApi.makeRequest apiurl (serialize <| LoginBetfair (adminpass, betuser, betpass) )
    let! x,_ = send
    match x with
    | Err error -> return Err error 
    | Ok x -> 
        match deserialize< Result<RestApi.Auth,string> > x with
        | Err error -> return Err error 
        | Ok auth -> return  auth }
    

let getCoupon apiurl (games, npage, pagelen) = async{    
    let _,send = RestApi.makeRequest apiurl (serialize <| GetCouponPage (games, npage, pagelen) )
    let! x,_ = send
    return 
        match x with
        | Err error -> Err error 
        | Ok x -> deserialize< Coupon.CouponResponse > x }
