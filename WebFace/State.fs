[<WebSharper.Pervasives.JavaScript>]
module CentBet.Client.State 

open WebSharper
open WebSharper.UI.Next
    
open Utils
open Betfair.Football
open CentBet.Client.Football

let meetups = ListModel.Create Meetup.id []
let varCurrentPageNumber = Var.Create 0
let varTargetPageNumber = Var.Create 0
let varPagesCount = Var.Create 1
let varColumnGpbVisible = Var.Create false
let varColumnCountryVisible = Var.Create false
let varMode = Var.Create PageModeCoupon
let varGameDetailError = Var.Create false

let isCouponMode() = 
    varMode.Value = PageModeCoupon
let notCouponMode() = not <| isCouponMode() 

let events = 
    let k = "CentBetEventsCatalogue"
    let dt = LocalStorage.checkTodayKey "CentBetEventsCatalogueCreated" k
    let x = 
        try
            ListModel.CreateWithStorage Event.id (Storage.LocalStorage k Serializer.Default)
        with e  ->
            printfn "error when restoring %A - %A" k e
            ListModel.Create Event.id []
    printfn "%A - %d, %A" k x.Length dt
    x


let tryGetEventById gameId = events.Value |> Seq.tryFind( fun {gameId = gameId'} -> gameId = gameId' )
let mtpevt = Meetup.id >> tryGetEventById
let tryGetMarkets = tryGetEventById >> Option.map( fun e -> e.markets)
let tryGetCountry = tryGetEventById >> Option.bind( fun e -> e.country)

module PageLen =
    let private localStorageKey = "pageLen"
    let private validateValue v = 
        if v < 10 then 30
        elif v > 40 then 40 
        else v

    let private var = 
        LocalStorage.getWithDef localStorageKey 30
        |> validateValue 
        |> Var.Create 

    let set value =
        let value = validateValue value
        if value <> var.Value then
            var.Value <- value 
            LocalStorage.set localStorageKey value

    let get() = validateValue var.Value

    let view = var.View

module ServerBetfairsSession = 
    let mutable private hasServerBetfairsSession = true
    let check = async{ 
        let! x = CentBet.Remote.hasServerBetfairsSession()
        hasServerBetfairsSession <- x }
    let has() = hasServerBetfairsSession
    let hasNot() = not hasServerBetfairsSession




    