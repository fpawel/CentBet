module Betfair.ApiNG.Services

open System
open System.Text.RegularExpressions

open Json

[<AutoOpen>]
module private Helpers = 
    
    open RestApi

    let service url methodUrl methodName = 
        {   ApiUrl = url
            ApiMethod = methodUrl + methodName }

    module Betting =         
        let private (~%%) = 
            service "https://api.betfair.com/exchange/betting/json-rpc/v1" "SportsAPING/v1.0/" 

        let listEventTypes = %% "listEventTypes"
        let listEvents = %% "listEvents"
        let placeOrders = %% "placeOrders"
        let cancelOrders = %% "cancelOrders"
        let replaceOrders = %% "replaceOrders"
        let listCurrentOrders = %% "listCurrentOrders"
        let listMarketCatalogue = %% "listMarketCatalogue"
        let listMarketBook = %% "listMarketBook"
        
    module Accaunt = 
        let private (~%%) = 
            service "https://api.betfair.com/exchange/account/json-rpc/v1" "AccountAPING/v1.0/"

        let getAccountFunds =   %% "getAccountFunds"
        let getAccountDetails = %% "getAccountDetails"
        let getDeveloperAppKeys = %% "getDeveloperAppKeys"
        let createDeveloperAppKeys = %% "createDeveloperAppKeys"

    let (|JsonAppKey|_|) = 
        let rec jappkey = function    
            | [] -> None
            | (Object x) :: rest -> 
                match x |> Map.tryFind "version", x |> Map.tryFind "applicationKey"  with
                | Some ( String "1.0" ), Some (String y) -> Some y
                | _ -> jappkey rest
            | _::rest -> jappkey rest
        let (|JAppKey|_|) = function
            | Prop "appVersions" (Array y ) -> jappkey y
            | _ -> None

        function
        | Array ( (JAppKey y)::_)  ->  Some y
        | JAppKey y ->  Some y
        | _ ->  None 

    let ``bad format`` : Either<_,_> = Left "bad format"
    let ``no instruction report`` : Either<_,_> = Left "no instruction report"
    let ``missing id`` : Either<_,_>  = Left "missing id"

    
let requestDevelopersAppKey sessionToken =
    let createNewDeveloperAppKey() = async{
        let args = Json.Object <| Map.ofList ["appName", Json.String (System.Guid.NewGuid().ToString()) ]
        let! r = RestApi.callWithoutAppKey  sessionToken Accaunt.createDeveloperAppKeys args
        return r |> Either.bindRight( function
            | JsonAppKey appKey -> Right appKey
            | json -> Left <| sprintf "there is no developer app key in response %A" ( Json.formatWith JsonFormattingOptions.Pretty json) ) }
    async{
        let! x = RestApi.callWithoutAppKey sessionToken Accaunt.getDeveloperAppKeys Json.empty 
        match x with        
        | Right (JsonAppKey y) -> return Right y
        | _ -> return! createNewDeveloperAppKey()  }           


let placeOrder auth (marketId,selectionId,side,price,size) =    
    {   PlaceOrderRequest.marketId = { marketId = marketId }
        instructions = 
            [   {   orderType = Some LIMIT
                    selectionId = selectionId
                    side = side
                    handicap = None
                    limitOnCloseOrder = None
                    marketOnCloseOrder = None
                    limitOrder = 
                        {   size = Some size
                            price = Some price
                            persistenceType = Some LAPSE } } ] }     
    |> RestApi.call auth Betting.placeOrders ( function
        | { PlaceExecutionReport.errorCode = Some errorCode } -> 
            errorCode |> caseDescr |> Left
        | { status = Some x } when x<> SUCCESS -> x |> caseDescr |> Left
        | { instructionReports = [] } -> ``no instruction report``

        | { instructionReports = { errorCode = Some errorCode }::_ } -> errorCode |> caseDescr |> Left
        | { instructionReports = { status = x }::_ } when x<>Some SUCCESS -> x |> caseDescr |> Left
        | { instructionReports = { betId = None }::_ } -> ``missing id``
        | { status = Some SUCCESS ; errorCode = None 
            instructionReports = {  status = Some SUCCESS; betId = Some betId 
                                    averagePriceMatched = Some averagePriceMatched 
                                    sizeMatched = Some sizeMatched}::_ } ->
            {   BetId  = betId
                AveragePriceMatched = averagePriceMatched
                SizeMatched = sizeMatched } 
            |> Right
                
        | _ -> ``bad format``)
    
     

let cancelOrder auth (marketId,betId,sizeReduction) =
    let instruction = { betId = betId; sizeReduction = sizeReduction }
    {   CancelationRequest.marketId = { marketId = marketId }
        instructions = [ instruction ] }         
    |> RestApi.call auth Betting.cancelOrders (function
        | { CancelExecutionReport.errorCode = Some errorCode } -> errorCode |> caseDescr |> Left
        | { status = Some x } when x<>SUCCESS -> x |> caseDescr |> Left
        | { instructionReports = [] } -> ``no instruction report``
        | { instructionReports = { sizeCancelled = sizeCancelled }::_ } -> 
            Right sizeCancelled )
    
        
let replaceOrder auth (marketId,betId : int64,newPrice) =

    {   ReplaceRequest.marketId = { marketId = marketId }
        instructions = [ { betId = betId; newPrice = newPrice } ] } 
    |> RestApi.call auth Betting.replaceOrders ( function
        | { ReplaceExecutionReport.errorCode = Some x } ->  x |> caseDescr |> Left
        | { status = Some x } when x<>SUCCESS ->  x |> caseDescr |> Left                
        | { instructionReports = [] } ->  ``no instruction report``
        | { instructionReports = {  placeInstructionReport = Some { betId = Some betId 
                                                                    errorCode = None
                                                                    averagePriceMatched = Some averagePriceMatched
                                                                    sizeMatched = Some sizeMatched
                                                                    status = Some SUCCESS} }::_ } ->
                {   BetId  = betId
                    AveragePriceMatched = averagePriceMatched
                    SizeMatched = sizeMatched } |> Right
        | _ -> ``bad format`` )


let (>>==) x f  = Either.bindLeftAsync f x
let (==>>) x f = Either.bindRightAsync f x
let (==>>!) x f = Either.bindRightAsyncR f x

let placeCentBet acc ((marketId,selectionId,betType,price,size : decimal) as betInfo)  = 
    let size = System.Math.Round(size, 2)
    let unmatchingPrice = if betType=BACK then 1000m else 1.01m    
    placeOrder acc (marketId,selectionId,betType,unmatchingPrice, 4m)
    >>== sprintf "placing cent-bet error : %A - %s" betInfo  
    ==>>! fun { BetId=betId } -> 
        cancelOrder acc (marketId, betId, Some (4m-size) )        
        >>== (sprintf "canceling placed cent-bet error : %A - %s" betInfo)
        ==>>! fun _ -> 
            replaceOrder acc (marketId, betId, price) 
            |> Either.bindLeftAsync( sprintf "replacing cent-bet error : %A - %s" betInfo ) 


let placeBet acc ((marketId,selectionId,betType,price,size) as betInfo)  = 
    if size>=4m then 
        placeOrder acc betInfo >>== sprintf "placing bet error : %A - %s" betInfo  
    else
        placeCentBet acc betInfo
             

let getAccauntFunds auth = 
    RestApi.call auth Accaunt.getAccountFunds  
        ( fun {availableToBetBalance = availableToBetBalance} -> 
            Right availableToBetBalance ) ()



let getCurentBets auth = 
    let rec loop acc = 
        RestApi.callForType<CurrentOrderSummaryReport> auth Betting.listCurrentOrders () 
        ==>>! function 
            | {moreAvailable=true; currentOrders=currentOrders} -> loop ( acc @ currentOrders  )
            | {moreAvailable=false; currentOrders=currentOrders} -> async { return Right (acc @ currentOrders)  }
    loop []    



let listEventTypes auth = 
    RestApi.callForType<EventTypeResult list> auth Betting.listEventTypes {ListEventTypesRequest.locale = "ru"}

let listEvents auth eventIds = 
    {   EventResultRequest.locale = "ru"
        filter = { MarketFilter.None with eventIds = Some eventIds } } 
    |> RestApi.callForType<EventResult list> auth Betting.listEvents 



let listMarketCatalogue auth eventId =
    {   MarketCatalogueRequest.locale = "ru"
        marketProjection = [ MARKET_START_TIME; MARKET_DESCRIPTION; RUNNER_DESCRIPTION; RUNNER_METADATA; MarketProjection.EVENT ]
        filter = { MarketFilter.None with eventIds = Some [eventId] } 
        maxResults = 1000 } 
    |> RestApi.callForType<MarketCatalogue list> auth Betting.listMarketCatalogue 



let listMarketBook auth marketsIds =    
    {   marketIds = marketsIds |> List.map( fun marketId -> {marketId = marketId } )
        priceProjection = 
            Some{   PriceProjection.Default with priceData = [ EX_BEST_OFFERS ]; virtualise = Some true }
        orderProjection = None
        matchProjection = None
        currencyCode = None
        locale = "ru" } 
    |> RestApi.callForType<MarketBook list> auth Betting.listMarketBook 

    