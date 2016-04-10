module CentBet.Remote

open System
open System.Threading
open System.Globalization

open WebSharper

open Betfair
open Betfair.Football
open Betfair.Football.Services
open Betfair.Football.Coupon

let isValidPassword = md5hash >> (=) "57545929BA6115DCAA317EEF8065F63D"

[<NamedUnionCases "result">]
type Result<'T> =
    | [<CompiledName "success">] Success of 'T
    | [<CompiledName "failure">] Failure of string

let resultOf<'a> (x : _) = async{
    let! x = x
    return 
        match x with
        | Left x -> Failure x
        | Right (x : 'a) -> Success <| sprintf "%A" x }



let isAdminCtx (ctx : Web.IContext) = async{
    let! user = ctx.UserSession.GetLoggedInUser()
    return 
        match user with 
        | Some "admin" -> true
        | _ -> false }

let ``bad request`` = "bad request"
let ``access denied`` = "access denied"

let authorizeAdmin password (ctx : Web.IContext)   =  async {
    if isValidPassword password then
        do! ctx.UserSession.LoginUser("admin")
        return Success "ok"
    else
        return Failure "access denied" }


[<AutoOpen>]
module Helpers =

    let format1 f x = async{ 
        let! x = x
        match x with
        | Left x -> return Failure x
        | Right x -> return Success <| f x }

    let map1<'a> (x : 'a) =
        sprintf "%A" x
        |> Success
        |> Async.id

    

    let fobj1<'a> = ( format1 ( fun (y, x : 'a) -> y, sprintf "%A" x ) )

    
    
    let consoleCommands = [ 
        "-login-betfair", 2, fun [user;pass] -> async{
            let! x  = Betfair.Login.login user pass 
            match x with
            | Left x -> return Failure x
            | Right auth -> 
                Coupon.adminBetafirAuth.Set <| Some auth
                return Success <| sprintf "%A" auth }

        "-atoms-names", 0, fun [] -> 
            Concurency.Status.getNames() 
            |> map1
        "-atom", 1, fun [x] -> Concurency.Status.getValue |> map1  ] |> List.map( fun (x,y,z) -> x, (y,z)) 
                                                                                     |> Map.ofList
    let (|Cmd|_|) = consoleCommands.TryFind 

    let webprotected ctx work = async {
        let! admin = isAdminCtx ctx
        if admin then 
            return! work
        else
            return Failure ``access denied`` }

[<Rpc>]
let getCouponPage (games,npage,pagelen) = Betfair.Football.Coupon.getCouponPage (games,npage,pagelen)

[<Rpc>]
let getGame gameId = Async.id <| Betfair.Football.Coupon.getGame gameId
  
[<Rpc>]
let perform (request : string )= 
    let ctx = Web.Remoting.GetContext()
    async{         
        let xs = 
            request.Split([|" "|], StringSplitOptions.RemoveEmptyEntries )
            |> Array.toList
        match xs with
        | "-login" :: pass :: _ ->  
            let! r = authorizeAdmin pass ctx 
            return r         
        | Cmd (n,f)::args when args.Length = n -> 
            return! webprotected ctx <| f args 
        | _ -> return  Failure ``bad request`` }

[<Rpc>]
let hasServerBetfairsSession() = async{
        return adminBetafirAuth.Value.IsSome
    }
    
[<Rpc>]
let getEventsCatalogue ids = Async.id <| Events.get ids 

[<Rpc>]
let getMarketsCatalogue gameId = 
    Betfair.Football.Coupon.MarketsCatalogue.get gameId |> Option.map ( 
        List.map( fun x ->             
            let runners = x.runners |> List.map( fun rnr -> rnr.runnerName, rnr.selectionId)
            x.marketId.marketId, x.marketName, runners, Option.bind decimalToInt32Safety x.totalMatched ) ) 
    |> Async.id
    

[<Rpc>]
let getTotalMatched gameId = 
    Football.Coupon.TotalMatched.get gameId    

[<AutoOpen>]
module private Helpers1 =
    type R = ApiNG.Runner
    type S = ApiNG.Side
    type W = Price | Size
    let get (s,w) x = 
        R.exchangePrice s x
        |> Option.bind (match w with Price -> fst | Size -> snd)

    let mk k = 
        List.choose( fun r -> 
            get k r
            |> Option.map( fun v -> r.selectionId,v) )
        >> Map.ofList 

[<Rpc>]
let getMarketsBook marketsIds = 
    Football.Coupon.MarketBook.get marketsIds
    |> Async.map( Map.map(fun _ x -> 
        let (~%%) k  = mk k x.runners
        %% (S.BACK,Price), %% (S.BACK, Size), %% (S.LAY,Price), %% (S.LAY, Size) ) )
        



    








