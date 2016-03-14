module CentBet.Remote

open System
open System.Threading
open System.Globalization

open WebSharper

open Betfair
open Betfair.Football
open Betfair.Football.Services

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

let authorizeAdmin (ctx : Web.IContext)  reqPass =  async {
    if md5hash reqPass = passwordKey then
        do! ctx.UserSession.LoginUser("admin")
        return true, "admin is authorized"
    else
        return false, "access denied" }

[<AutoOpen>]
module Helpers =

    let format1 f x = async{ 
        let! x = x
        match x with
        | Left x -> return false, x
        | Right x -> return true, f x }

    let map1<'a> (x : Async<'a>) = async{
        let! (x : 'a) = x
        return true, sprintf "%A" x }

    let map2<'a> (x : _) = async{
        let! x = x
        return 
            match x with
            | Left x -> false, x
            | Right (x : 'a) -> true, sprintf "%A" x }

    let fobj1<'a> = ( format1 ( fun (y, x : 'a) -> y, sprintf "%A" x ) )

    let ``bad request`` = false, "bad request"
    let ``access denied`` = false, "access denied"
    
let consoleCommands = [ 
    "-login-betfair", 2, fun [user;pass] -> Betfair.Login.login user pass |> map2
    "-atoms-names", 0, fun [] -> Atom.Trace.getNames() |> map1 
    "-atom", 1, fun [x] -> Atom.Trace.getAtomValue x |> map1  ] |> List.map( fun (x,y,z) -> x, (y,z)) 
                                                                                 |> Map.ofList
let (|Cmd|_|) = consoleCommands.TryFind 

[<Rpc>]
let perform (request : string )= 
    let ctx = Web.Remoting.GetContext()
    async{         
        let xs = 
            request.Split([|" "|], StringSplitOptions.RemoveEmptyEntries )
            |> Array.toList
        let! isAdmin = isAdminCtx ctx
        match isAdmin,xs with
        | _, "-login" :: pass :: _ ->  
            let! r = authorizeAdmin ctx pass
            return r         
        | false, _ -> return ``access denied``
        | true, Cmd (n,f)::args when args.Length = n -> 
            return! f args 
        | _ -> return ``bad request`` }



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
let getApiNgEvents ids =     
    Betfair.Football.Coupon.getExistedEvents ids
    |> Either.mapAsync ( fun (rdds, msng) -> 

        
        // Change current culture
        let culture = CultureInfo("ru-RU")      
        Thread.CurrentThread.CurrentCulture <- culture
        Thread.CurrentThread.CurrentUICulture <- culture
        let response =
            rdds 
            |> Map.toList
            |> List.map(fun (gameId,{event = e}) ->
                let country = 
                    try
                        countryFromEvent e
                    with exn -> 
                        Logging.error "%A"  exn
                        None
                gameId, e.name, country, e.openDate)
        response )
        

    
    








