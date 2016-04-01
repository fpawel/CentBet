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

    let map1<'a> (x : Async<'a>) = async{
        let! (x : 'a) = x
        return Success <| sprintf "%A" x }

    

    let fobj1<'a> = ( format1 ( fun (y, x : 'a) -> y, sprintf "%A" x ) )

    
    
    let consoleCommands = [ 
        "-login-betfair", 2, fun [user;pass] -> Betfair.Login.login user pass |> resultOf
        "-atoms-names", 0, fun [] -> Atom.Trace.getNames() |> map1 
        "-atom", 1, fun [x] -> Atom.Trace.getAtomValue x |> map1  ] |> List.map( fun (x,y,z) -> x, (y,z)) 
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
let getGame gameId = Betfair.Football.Coupon.getGame gameId
  
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
let hasServerBetfairsSession() = 
    Betfair.Login.auth.Get()
    |> Async.map Option.isSome
    
[<Rpc>]
let getEventsCatalogue ids = Events.get ids

[<Rpc>]
let getMarketsCatalogue gameId = async{    
    try
        let! m = Betfair.Football.Coupon.MarketsCatalogue.get gameId
        return m |> Option.map ( fun m ->       
            m |> List.map( fun x ->             
                let runners = x.runners |> List.map( fun rnr -> rnr.runnerName, rnr.selectionId)
                x.marketId.marketId, x.marketName, runners, Option.bind decimalToInt32Safety x.totalMatched ) ) 
    with e ->
        Logging.error "getMarketsCatalogue - %A" e 
        return None }

[<Rpc>]
let getTotalMatched gameId = 
    Football.Coupon.TotalMatched.get gameId    


        



    








