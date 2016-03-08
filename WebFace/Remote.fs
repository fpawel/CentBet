module CentBet.Remote

open System
open WebSharper

open Betfair
open Betfair.Football
open Betfair.Football.Services

[<Rpc>]
let loginBetfair (buser,bpass) = 

    let ctx = Web.Remoting.GetContext()
    async{
        let! user = ctx.UserSession.GetLoggedInUser()
        match user with 
        | Some "admin" -> 
            let! r = Betfair.Login.login buser bpass
            return leftSome r
        | _ -> return Some "access denied" }

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