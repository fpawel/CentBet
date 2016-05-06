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
    | [<CompiledName "ok">] Ok of 'T
    | [<CompiledName "err">] Err of string





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
        return Ok "ok"
    else
        return Err "access denied" }


[<AutoOpen>]
module Helpers =

    let map1<'a> (x : 'a) =
        sprintf "%A" x
        |> Ok
        |> Async.return'

    let fromUtilsResult<'a> (f : 'a -> string) = function
        | Utils.Ok x -> Ok (f x)
        | Utils.Err (x:string) -> Err x

    let consoleCommands = [ 
        "-login-betfair", 2, fun [user;pass] -> 
            Betfair.Login.login user pass                 
            |> Result.Async.map ( fun auth -> 
                Some auth |> Coupon.adminBetafirAuth.Set 
                auth )
            |> Async.map(  fromUtilsResult (sprintf "%A") )

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
            return Err ``access denied`` }

[<Rpc>]
let getCouponPage (games,npage,pagelen) = Betfair.Football.Coupon.getCouponPage (games,npage,pagelen)

[<Rpc>]
let getGame gameId = Async.return' <| Betfair.Football.Coupon.getGame gameId
  
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
        | _ -> return  Err ``bad request`` }

[<Rpc>]
let hasServerBetfairsSession() = async{
        return adminBetafirAuth.Value.IsSome
    }
    
[<Rpc>]
let getEventsCatalogue ids = Async.return' <| Events.get ids 

[<Rpc>]
let getMarketsCatalogue gameId = 
    Betfair.Football.Coupon.MarketsCatalogue.get gameId |> Option.map ( 
        List.map( fun x ->             
            let runners = x.runners |> List.map( fun rnr -> rnr.runnerName, rnr.selectionId)
            x.marketId.marketId, x.marketName, runners, Option.bind decimalToInt32Safety x.totalMatched ) ) 
    |> Async.return'
    

[<Rpc>]
let getTotalMatched gameId = 
    Football.Coupon.TotalMatched.get gameId    

[<AutoOpen>]
module private Helpers1 =
    type R = ApiNG.Runner
    type S = ApiNG.Side
    type W = Price | Size
    
[<Rpc>]
let getMarketsBook marketsIds = 
    Football.Coupon.MarketBook.get marketsIds
    |> Async.map( Map.map(fun _ x ->
        x.runners 
        |> List.map( fun r -> r.selectionId, (R.exchangePrices S.BACK r, R.exchangePrices S.LAY r) )
        |> Map.ofList ) )        