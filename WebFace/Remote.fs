module CentBet.Remote

open System
open System.Threading
open System.Globalization

open WebSharper

open Betfair
open Betfair.Football
open Betfair.Football.Services

[<NamedUnionCases "result">]
type Result<'T> =
    | [<CompiledName "success">] Success of 'T
    | [<CompiledName "failure">] Failure of message: string

[<Rpc>]
let getCoupon ( (reqGames,inplayOnly) as request)  = async{
    let reqIds, reqGames = List.ids reqGames fst
    let! games = async{ 
        let! inplay = Coupon.Inplay.Get()
        if inplayOnly then return inplay else
        let! today1 = Coupon.Today1.Get()
        let! today2 = Coupon.Today2.Get()
        return inplay @ today1 @ today2 } 
    let extIds, extGames = List.ids games ( fst >> Game.id )
    let newIds = Set.difference extIds reqIds
    let outIds = Set.difference reqIds extIds 
    let updIds = Set.intersect reqIds extIds        
    let newGames =
        newIds |> Seq.map ( fun id -> 
            let game, gameInfo = extGames.[id] 
            game,  gameInfo, gameInfo.GetHash() )
        |> Seq.toList
    let update = 
        updIds
        |> Seq.map( fun id -> 
            let _, gameInfo = extGames.[id] 
            id, gameInfo, gameInfo.GetHash()  )
        |> Seq.filter( fun(gameId, gameInfo, hash) -> 
            let _,reqHash = reqGames.[gameId]     
            hash <> reqHash  )
        |> Seq.toList
    return newGames, update, outIds }

let isAdminCtx (ctx : Web.IContext) = async{
    let! user = ctx.UserSession.GetLoggedInUser()
    return 
        match user with 
        | Some "admin" -> true
        | _ -> false }

let passwordKey = "E018CB561EE1DB0EF3892AE22FCCDD5C" 

let authorizeAdmin reqPass (ctx : Web.IContext)   =  async {
    if md5hash reqPass = passwordKey then
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

    let map2<'a> (x : _) = async{
        let! x = x
        return 
            match x with
            | Left x -> Failure x
            | Right (x : 'a) -> Success <| sprintf "%A" x }

    let fobj1<'a> = ( format1 ( fun (y, x : 'a) -> y, sprintf "%A" x ) )

    let ``bad request`` = Failure "bad request"
    let ``access denied`` = Failure "access denied"
    
let consoleCommands = [ 
    "-login-betfair", 2, fun [user;pass] -> Betfair.Login.login user pass |> map2
    "-atoms-names", 0, fun [] -> Atom.Trace.getNames() |> map1 
    "-atom", 1, fun [x] -> Atom.Trace.getAtomValue x |> map1  ] |> List.map( fun (x,y,z) -> x, (y,z)) 
                                                                                 |> Map.ofList
let (|Cmd|_|) = consoleCommands.TryFind 

let protected' ctx work = async {
    let! admin = isAdminCtx ctx
    if admin then 
        return! work
    else
        return ``access denied`` }


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
            return! protected' ctx <| f args 
        | _ -> return  ``bad request`` }



let shortCountries = 
    [   "Боливарийская Республика Венесуэла", "Венесуэла"
        "Чешская республика", "Чехия"
        "Китайская Народная Республика", "Китай"
        "Македония (Бывшая Югославская Республика Македония)", "Македония"
        "Сербия и Черногория (бывшая)", "Сербия" ]
    |> Map.ofList

let countries1 = 
    [ "GI", "Гибралтар" ]
    |> Map.ofList


let getCountryFromRegionInfo (countryCode : string) = 
    // Change current culture
    let culture = CultureInfo("ru-RU")      
    Thread.CurrentThread.CurrentCulture <- culture
    Thread.CurrentThread.CurrentUICulture <- culture

    let i = Globalization.RegionInfo(countryCode)
    if i=null then countryCode else 
    shortCountries.TryFind i.DisplayName 
    |> Option.getWith i.DisplayName 

let countryFromEvent (e: ApiNG.Event) = 
    e.countryCode |> Option.map( fun countryCode ->
        countries1.TryFind countryCode
        |> Option.getBy ( fun () -> 
            try
                getCountryFromRegionInfo(countryCode)
            with _ -> 
                countryCode ) )
    
[<Rpc>]
let getEventsCatalogue ids =     
    Betfair.Football.Coupon.getExistedEvents ids
    |> Either.mapAsync ( fun (rdds, msng) -> 
        rdds 
        |> Map.toList
        |> List.map(fun (gameId,{event = e}) ->
            let country = 
                try
                    countryFromEvent e
                with exn -> 
                    Logging.error "%A"  exn
                    None
            gameId, e.name, country ) )

let getToltalMatched xs = 
    let xs = xs |> List.choose( fun (x : ApiNG.MarketCatalogue )-> x.totalMatched ) 
    if xs.IsEmpty then None else Some <| List.sum xs 
    |> Option.map int

[<Rpc>]
let getEventTotalMatched gameId = async{
    let! m' = Betfair.Football.Coupon.getMarketCatalogue gameId    
    return Either.mapRight getToltalMatched m'}
        
[<Rpc>]
let getMarketCatalogue gameId = async{    
    let! m = Betfair.Football.Coupon.getMarketCatalogue gameId
    return m |> Either.mapRight ( fun m ->       
        m |> List.map( fun x ->             
            let runners = x.runners |> List.map( fun rnr -> rnr.runnerName, rnr.selectionId)
            x.marketId.marketId, x.marketName, runners, Option.map int x.totalMatched ) ) }


module Api = 
    open WebSharper.Sitelets

    type LoginAdmin = { password : string }
    type LoginBetfair = { username: string; password : string }

    type Action =
        | [<Method "POST"; CompiledName "loginAdmin"; Json "data" >]
            LoginAdmin of LoginAdmin
        | [<Method "POST"; CompiledName "loginBetfair"; Json "data">]
            LoginBetfair of LoginBetfair

    
    let ApiContent context (action: Action) =
        match action with
        | LoginAdmin { password = pass } ->
            Content.Json <| authorizeAdmin pass
        | LoginBetfair { username = username; password = pass } ->
            Betfair.Login.login username pass |> map2 
            |> protected' context 
            |>  Content.Json 

    
    
    








