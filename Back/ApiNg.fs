namespace Betfair.ApiNG

open System
open System.ComponentModel

type MarketId = { marketId : int }

type MarketType = string
type Venue = string
type SelectionId = int
type Handicap = decimal
type EventId = string
type EventTypeId = int
type CountryCode = string
type ExchangeId = string
type CompetitionId = string
type Price = decimal
type Size = decimal
type BetId = int64
type MatchId = int

type GetAccauntFundsResponse = {
    availableToBetBalance : decimal }



type MarketProjection =
    | [<Description("If not selected then the competition will not be returned with marketCatalogue")>]
        COMPETITION
    | [<Description("If not selected then the event will not be returned with marketCatalogue")>]
        EVENT
    | [<Description("If not selected then the eventType will not be returned with marketCatalogue")>]
        EVENT_TYPE
    | [<Description("If not selected then the start time will not be returned with marketCatalogue")>]
        MARKET_START_TIME
    | [<Description("If not selected then the description will not be returned with marketCatalogue")>]
        MARKET_DESCRIPTION
    | [<Description("If not selected then the runners will not be returned with marketCatalogue")>]
        RUNNER_DESCRIPTION
    | [<Description("If not selected then the runner metadata will not be returned with marketCatalogue. If selected then RUNNER_DESCRIPTION will also be returned regardless of whether it is included as a market projection.")>]
        RUNNER_METADATA

type PriceData =
    | SP_AVAILABLE
    | SP_TRADED
    | EX_BEST_OFFERS
    | [<Description("EX_ALL_OFFERS trumps EX_BEST_OFFERS if both settings are present")>]
        EX_ALL_OFFERS
    | EX_TRADED

type MatchProjection =
    | NO_ROLLUP
    | ROLLED_UP_BY_PRICE
    | ROLLED_UP_BY_AVG_PRICE

type OrderProjection =
    | ALL
    | EXECUTABLE
    | EXECUTION_COMPLETE

type MarketStatus =
    | [<Description("Inactive Market")>]
        INACTIVE
    | [<Description("Open Market")>]
        OPEN
    | [<Description("Suspended Market")>]
        SUSPENDED
    | [<Description("Closed Market")>]
        CLOSED



type RunnerStatus =
    | [<Description("ACTIVE")>]
        ACTIVE
    | [<Description("WINNER")>]
        WINNER
    | [<Description("LOSER")>]
        LOSER
    | [<Description("REMOVED_VACANT applies to Greyhounds. Greyhound markets always return a fixed number of runners (traps). If a dog has been removed, the trap is shown as vacant.")>]
        REMOVED_VACANT
    | [<Description("REMOVED")>]
        REMOVED

type TimeGranularity =
    | DAYS
    | HOURS
    | MINUTES

type Side =
    | [<Description("To back a team, horse or outcome is to bet on the selection to win.")>]
        BACK
    | [<Description("To lay a team, horse, or outcome is to bet on the selection to lose.")>]
        LAY    
    member x.What = match x with BACK -> "B" | _ -> "L"
    member x.WhatRu = match x with BACK -> "ЗА" | _ -> "ПРОТИВ"

type OrderStatus =
    | [<Description("An order that does not have any remaining unmatched portion.")>]
        EXECUTION_COMPLETE
    | [<Description("An order that has a remaining unmatched portion.")>]
        EXECUTABLE

type OrderBy =
    | [<Description("@Deprecated Use BY_PLACE_TIME instead. Order by placed time, then bet id.")>]
        BY_BET
    | [<Description("Order by market id, then placed time, then bet id.")>]
        BY_MARKET
    | [<Description("Order by time of last matched fragment (if any), then placed time, then bet id. Filters out orders which have no matched date. The dateRange filter (if specified) is applied to the matched date.")>]
        BY_MATCH_TIME
    | [<Description("Order by placed time, then bet id. This is an alias of to be deprecated BY_BET. The dateRange filter (if specified) is applied to the placed date.")>]
        BY_PLACE_TIME
    | [<Description("Order by time of last settled fragment (if any due to partial market settlement), then by last match time, then placed time, then bet id. Filters out orders which have not been settled. The dateRange filter (if specified) is applied to the settled date.")>]
        BY_SETTLED_TIME
    | [<Description("Order by time of last voided fragment (if any), then by last match time, then placed time, then bet id. Filters out orders which have not been voided. The dateRange filter (if specified) is applied to the voided date.")>]
        BY_VOID_TIME

type SortDir =
    | [<Description("Order from earliest value to latest e.g. lowest betId is first in the results.")>]
        EARLIEST_TO_LATEST
    | [<Description("Order from the latest value to the earliest e.g. highest betId is first in the results.")>]
        LATEST_TO_EARLIEST

type OrderType =
    | [<Description("A normal exchange limit order for immediate execution")>]
        LIMIT
    | [<Description("Limit order for the auction (SP)")>]
        LIMIT_ON_CLOSE
    | [<Description("Market order for the auction (SP)")>]
        MARKET_ON_CLOSE

type MarketSort =
    | [<Description("Minimum traded volume")>]
        MINIMUM_TRADED
    | [<Description("Maximum traded volume")>]
        MAXIMUM_TRADED
    | [<Description("Minimum available to match")>]
        MINIMUM_AVAILABLE
    | [<Description("Maximum available to match")>]
        MAXIMUM_AVAILABLE
    | [<Description("The closest markets based on their expected start time")>]
        FIRST_TO_START
    | [<Description("The most distant markets based on their expected start time")>]
        LAST_TO_START

type MarketBettingType =
    | [<Description("Odds Market")>]
        ODDS
    | [<Description("Line Market")>]
        LINE
    | [<Description("Range Market")>]
        RANGE
    | [<Description("Asian Handicap Market")>]
        ASIAN_HANDICAP_DOUBLE_LINE
    | [<Description("Asian Single Line Market")>]
        ASIAN_HANDICAP_SINGLE_LINE
    | [<Description("Sportsbook Odds Market. This type is deprecated and will be removed in future releases, when Sportsbook markets will be represented as ODDS market but with a different product type.")>]
        FIXED_ODDS

type ExecutionReportStatus =
    | SUCCESS
    | FAILURE
    | PROCESSED_WITH_ERRORS
    | TIMEOUT

type InstructionReportStatus = ExecutionReportStatus 

type ExecutionReportErrorCode =
    | [<Description("The matcher is not healthy")>]
        ERROR_IN_MATCHER
    | [<Description("The order itself has been accepted, but at least one (possibly all) actions have generated errors")>]
        PROCESSED_WITH_ERRORS
    | [<Description("There is an error with an action that has caused the entire order to be rejected")>]
        BET_ACTION_ERROR
    | [<Description("Order rejected due to the account's status (suspended, inactive, dup cards)")>]
        INVALID_ACCOUNT_STATE
    | [<Description("Order rejected due to the account's wallet's status")>]
        INVALID_WALLET_STATUS
    | [<Description("Account has exceeded its exposure limit or available to bet limit")>]
        INSUFFICIENT_FUNDS
    | [<Description("The account has exceed the self imposed loss limit")>]
        LOSS_LIMIT_EXCEEDED
    | [<Description("Market is suspended")>]
        MARKET_SUSPENDED
    | [<Description("Market is not open for betting. It is either not yet active, suspended or closed awaiting settlement.")>]
        MARKET_NOT_OPEN_FOR_BETTING
    | [<Description("duplicate customer referece data submitted")>]
        DUPLICATE_TRANSACTION
    | [<Description("Order cannot be accepted by the matcher due to the combination of actions. For example, bets being edited are not on the same market, or order includes both edits and placement")>]
        INVALID_ORDER
    | [<Description("Market doesn't exist")>]
        INVALID_MARKET_ID
    | [<Description("Business rules do not allow order to be placed")>]
        PERMISSION_DENIED
    | [<Description("duplicate bet ids found")>]
        DUPLICATE_BETIDS
    | [<Description("Order hasn't been passed to matcher as system detected there will be no state change")>]
        NO_ACTION_REQUIRED
    | [<Description("The requested service is unavailable")>]
        SERVICE_UNAVAILABLE
    | [<Description("The regulator rejected the order. On the Italian Exchange this error will occur if more than 50 bets are sent in a single placeOrders request.")>]
        REJECTED_BY_REGULATOR

type PersistenceType =
    | [<Description("Lapse the order when the market is turned in-play")>]
        LAPSE
    | [<Description("Persist the order to in-play. The bet will be place automatically into the in-play market at the start of the event.")>]
        PERSIST
    | [<Description("Put the order into the auction (SP) at turn-in-play")>]
        MARKET_ON_CLOSE

type InstructionReportErrorCode =
    | [<Description("bet size is invalid for your currency or your regulator")>]
        INVALID_BET_SIZE
    | [<Description("Runner does not exist, includes vacant traps in greyhound racing")>]
        INVALID_RUNNER
    | [<Description("Bet cannot be cancelled or modified as it has already been taken or has lapsed Includes attempts to cancel/modify market on close BSP bets and cancelling limit on close BSP bets")>]
        BET_TAKEN_OR_LAPSED
    | [<Description("No result was received from the matcher in a timeout configured for the system")>]
        BET_IN_PROGRESS
    | [<Description("Runner has been removed from the event")>]
        RUNNER_REMOVED
    | [<Description("Attempt to edit a bet on a market that has closed.")>]
        MARKET_NOT_OPEN_FOR_BETTING
    | [<Description("The action has caused the account to exceed the self imposed loss limit")>]
        LOSS_LIMIT_EXCEEDED
    | [<Description("Market now closed to bsp betting. Turned in-play or has been reconciled")>]
        MARKET_NOT_OPEN_FOR_BSP_BETTING
    | [<Description("Attempt to edit down the price of a bsp limit on close lay bet, or edit up the price of a limit on close back bet")>]
        INVALID_PRICE_EDIT
    | [<Description("Odds not on price ladder - either edit or placement")>]
        INVALID_ODDS
    | [<Description("Insufficient funds available to cover the bet action. Either the exposure limit or available to bet limit would be exceeded")>]
        INSUFFICIENT_FUNDS
    | [<Description("Invalid persistence type for this market, e.g. KEEP for a non bsp market")>]
        INVALID_PERSISTENCE_TYPE
    | [<Description("A problem with the matcher prevented this action completing successfully")>]
        ERROR_IN_MATCHER
    | [<Description("The order contains a back and a lay for the same runner at overlapping prices. This would guarantee a self match. This also applies to BSP limit on close bets")>]
        INVALID_BACK_LAY_COMBINATION
    | [<Description("The action failed because the parent order failed")>]
        ERROR_IN_ORDER
    | [<Description("Bid type is mandatory")>]
        INVALID_BID_TYPE
    | [<Description("Bet for id supplied has not been found")>]
        INVALID_BET_ID
    | [<Description("Bet cancelled but replacement bet was not placed")>]
        CANCELLED_NOT_PLACED
    | [<Description("Action failed due to the failure of a action on which this action is dependent")>]
        RELATED_ACTION_FAILED
    | [<Description("the action does not result in any state change. eg changing a persistence to it's current value")>]
        NO_ACTION_REQUIRED

type RollupModel =
    | [<Description("The volumes will be rolled up to the minimum value which is >= rollupLimit.")>]
        STAKE
    | [<Description("The volumes will be rolled up to the minimum value where the payout( price * volume ) is >= rollupLimit.")>]
        PAYOUT
    | [<Description("The volumes will be rolled up to the minimum value which is >= rollupLimit, until a lay price threshold. There after, the volumes will be rolled up to the minimum value such that the liability >= a minimum liability. Not supported as yet.")>]
        MANAGED_LIABILITY
    | [<Description("No rollup will be applied. However the volumes will be filtered by currency specific minimum stake unless overridden specifically for the channel.")>]
        NONE

type GroupBy =
    | [<Description("A roll up of settled P&L, commission paid and number of bet orders, on a specified event type")>]
        EVENT_TYPE
    | [<Description("A roll up of settled P&L, commission paid and number of bet orders, on a specified event")>]
        EVENT
    | [<Description("A roll up of settled P&L, commission paid and number of bet orders, on a specified market")>]
        MARKET
    | [<Description("An averaged roll up of settled P&L, and number of bets, on the specified side of a specified selection within a specified market, that are either settled or voided")>]
        SIDE
    | [<Description("The P&L, commission paid, side and regulatory information etc, about each individual bet order")>]
        BET

type  BetStatus =
    | [<Description("A matched bet that was settled normally")>]
        SETTLED
    | [<Description("A matched bet that was subsequently voided by Betfair, before, during or after settlement")>]
        VOIDED
    | [<Description("Unmatched bet that was cancelled by Betfair (for example at turn in play).")>]
        LAPSED
    | [<Description("Unmatched bet that was cancelled by an explicit customer action.")>]
        CANCELLED

type PlaceOrderRequest =
    {   marketId : MarketId
        instructions : PlaceInstruction list }
and PlaceOrderResult = 
    {   BetId : int64
        AveragePriceMatched : decimal 
        SizeMatched : decimal  }
and CancelationRequest = 
    {   marketId : MarketId
        instructions : CancelInstruction list  }
and ReplaceRequest =
    {   marketId : MarketId
        instructions : ReplaceInstruction list  }
and MarketFilter =
    {   [<Description("Restrict markets by any text associated with the market such as the Name, Event, Competition, etc. You can include a wildcard (*) character as long as it is not the first character.")>]
        textQuery : string option
        [<Description("Restrict markets by the Exchange where the market operates. Not currently in use, requests for Australian markets should be sent to the Aus Exchange endpoint.")>]
        exchangeIds : int list option
        [<Description("Restrict markets by event type associated with the market. (i.e., Football, Hockey, etc)")>]
        eventTypeIds : int list option
        [<Description("Restrict markets by the event id associated with the market.")>]
        eventIds : int list option
        [<Description("Restrict markets by the competitions associated with the market.")>]
        competitionIds : string list option
        [<Description("Restrict markets by the market id associated with the market.")>]
        marketIds : string list option
        [<Description("Restrict markets by the venue associated with the market. Currently only Horse Racing markets have venues.")>]
        venues : string list option
        [<Description("Restrict to bsp markets only, if True or non-bsp markets if False. If not specified then returns both BSP and non-BSP markets")>]
        bspOnly : bool option 
        [<Description("Restrict to markets that will turn in play if True or will not turn in play if false. If not specified, returns both.")>]
        turnInPlayEnabled : bool option
        [<Description("Restrict to markets that are currently in play if True or are not currently in play if false. If not specified, returns both.")>]
        inPlayOnly : bool option
        [<Description("Restrict to markets that match the betting type of the market (i.e. Odds, Asian Handicap Singles, or Asian Handicap Doubles")>]
        marketBettingTypes : MarketBettingType list option
        [<Description("Restrict to markets that are in the specified country or countries")>]
        marketCountries : string list option
        [<Description("Restrict to markets that match the type of the market (i.e., MATCH_ODDS, HALF_TIME_SCORE). You should use this instead of relying on the market name as the market type codes are the same in all locales")>]
        marketTypeCodes : string list option
        [<Description("Restrict to markets with a market start time before or after the specified date")>]
        marketStartTime : TimeRange option 
        [<Description("Restrict to markets that I have one or more orders in these status.")>]
        withOrders : OrderStatus list option}
    static member None = 
        {   textQuery  = None                        
            exchangeIds = None                        
            eventTypeIds = None                     
            eventIds = None                        
            competitionIds = None                        
            marketIds = None                        
            venues = None                        
            bspOnly = None                        
            turnInPlayEnabled = None                        
            inPlayOnly = None                        
            marketBettingTypes = None                        
            marketCountries = None                        
            marketTypeCodes = None                        
            marketStartTime = None                        
            withOrders = None }
        

///Information about a market
and MarketCatalogue =
    {   [<Description("The unique identifier for the market. MarketId's are prefixed with '1.' or '2.' 1. = UK Exchange 2. = AUS Exchange.")>]
        marketId : MarketId
        [<Description("The name of the market")>]
        marketName : string 
        [<Description("The time this market starts at, only returned when the MARKET_START_TIME enum is passed in the marketProjections")>]
        marketStartTime : DateTime option
        [<Description("Details about the market")>]
        description : MarketDescription option
        [<Description("The total amount of money matched on the market")>]
        totalMatched : decimal option
        [<Description("The runners (selections) contained in the market")>]
        runners : RunnerCatalog list
        [<Description("The Event Type the market is contained within")>]
        eventType : EventType option
        [<Description("The competition the market is contained within. Usually only applies to Football competitions")>]
        competition : Competition option
        [<Description("The event the market is contained within")>]
        event : Event option}

///The dynamic data in a market
and MarketBook =
    {   [<Description("The unique identifier for the market. MarketId's are prefixed with '1.' or '2.' 1. = UK Exchange 2. = AUS Exchange.")>]
        marketId : MarketId
        [<Description("True if the data returned by listMarketBook will be delayed. The data may be delayed because you are not logged in with a funded account or you are using an Application Key that does not allow up to date data.")>]
        isMarketDataDelayed : bool option
        [<Description("The status of the market, for example ACTIVE, SUSPENDED, SETTLED, etc.")>]
        status : MarketStatus
        [<Description("The number of seconds an order is held until it is submitted into the market. Orders are usually delayed when the market is in-play")>]
        betDelay : int
        [<Description("True if the market starting price has been reconciled")>]
        bspReconciled : bool
        [<Description("If false, runners may be added to the market")>]
        complete : bool
        [<Description("True if the market is currently in play")>]
        inplay : bool
        [<Description("The number of selections that could be settled as winners")>]
        numberOfWinners : int
        [<Description("The number of runners in the market")>]
        numberOfRunners : int
        [<Description("The number of runners that are currently active. An active runner is a selection available for betting")>]
        numberOfActiveRunners : int
        [<Description("The most recent time an order was executed")>]
        lastMatchTime : DateTime option
        [<Description("The total amount matched")>]
        totalMatched : decimal 
        [<Description("The total amount of orders that remain unmatched")>]
        totalAvailable : decimal
        [<Description("True if cross matching is enabled for this market.")>]
        crossMatching : bool
        [<Description("True if runners in the market can be voided")>]
        runnersVoidable : bool
        [<Description("The version of the market. The version increments whenever the market status changes, for example, turning in-play, or suspended when a goal score.")>]
        version : int
        [<Description("Information about the runners (selections) in the market.")>]
        runners : Runner list}

///Information about the Runners (selections) in a market
and RunnerCatalog =
    {   [<Description("The unique id for the selection.")>]
        selectionId : int 
        [<Description("The name of the runner")>]
        runnerName : string 
        [<Description("The handicap")>]
        handicap : decimal option
        [<Description("The sort priority of this runner")>]
        sortPriority : int option
        [<Description("Metadata associated with the runner.  For a description of this data for Horse Racing, please see Runner Metadata Description")>]
        metadata : Map<String,String>}

///The dynamic data about runners in a market
and Runner =
    {   [<Description("The unique id of the runner (selection)")>]
        selectionId : int
        [<Description("The handicap.  Enter the specific handicap value (returned by RUNNER in listMaketBook) if the market is an Asian handicap market.")>]
        handicap : decimal option
        [<Description("The status of the selection (i.e., ACTIVE, REMOVED, WINNER, LOSER, HIDDEN) Runner status information is available for 90 days following market settlement.")>]
        status : RunnerStatus option
        [<Description("The adjustment factor applied if the selection is removed")>]
        adjustmentFactor : decimal option
        [<Description("The price of the most recent bet matched on this selection")>]
        lastPriceTraded : decimal option
        [<Description("The total amount matched on this runner")>]
        totalMatched : decimal option
        [<Description("If date and time the runner was removed")>]
        removalDate : DateTime option
        [<Description("The BSP related prices for this runner")>]
        sp : StartingPrices option
        [<Description("The Exchange prices available for this runner")>]
        ex : ExchangePrices option
        [<Description("List of orders in the market")>]
        orders : Order list
        [<Description("List of matches (i.e, orders that have been fully or partially executed)")>]
        matches : Match list}
    
    static member bestPriceBack x = 
        match x.ex with
        | None -> None
        | Some ex ->
            match ex.availableToBack with
            | {price=Some price}::_ -> Some price
            | _ -> None
    static member bestPriceLay x = 
        match x.ex with
        | None -> None
        | Some ex ->
            match ex.availableToLay with
            | {price=Some price}::_ -> Some price
            | _ -> None
    static member bestSizeBack x = 
        match x.ex with
        | None -> None
        | Some ex ->
            match ex.availableToBack with
            | {size=Some size}::_ -> Some size
            | _ -> None
    static member bestSizeLay x = 
        match x.ex with
        | None -> None
        | Some ex ->
            match ex.availableToLay with
            | {size=Some size}::_ -> Some size
            | _ -> None


///Information about the Betfair Starting Price. Only available in BSP markets
and StartingPrices =
    {   [<Description("What the starting price would be if the market was reconciled now taking into account the SP bets as well as unmatched exchange bets on the same selection in the exchange.")>]
        nearPrice : decimal
        [<Description("What the starting price would be if the market was reconciled now taking into account only the currently place SP bets. The Far Price is not as complicated but not as accurate and only accounts for money on the exchange at SP.")>]
        farPrice : decimal
        [<Description("The back bets matched at the actual Betfair Starting Price")>]
        backStakeTaken : PriceSize list
        [<Description("The lay amount matched at the actual Betfair Starting Price")>]
        layLiabilityTaken : PriceSize list
        [<Description("The final BSP price for this runner. Only available for a BSP market that has been reconciled.")>]
        actualSP : decimal}

and ExchangePrices =
    {   availableToBack : PriceSize list
        availableToLay : PriceSize list
        tradedVolume : PriceSize list}

///Event
and Event =
    {   [<Description("The unique id for the event")>]
        id : int
        [<Description("The name of the event")>]
        name : string
        [<Description("The ISO-2 code for the event.  A list of ISO-2 codes is available via http://en.wikipedia.org/wiki/ISO_3166-1_alpha-2")>]
        countryCode : string option
        [<Description("This is timezone in which the event is taking place.")>]
        timezone : string option
        [<Description("venue")>]
        venue : string option
        [<Description("The scheduled start date and time of the event. This is Europe/London (GMT) by default")>]
        openDate : DateTime option}

///Event Result
and EventResult =
    {   [<Description("Event")>]
        event : Event
        [<Description("Count of markets associated with this event")>]
        marketCount : int}
    

///Competition
and Competition =
    {   [<Description("id")>]
        id : string
        [<Description("name")>]
        name : string}

///Competition Result
and CompetitionResult =
    {   [<Description("Competition")>]
        competition : Competition
        [<Description("Count of markets associated with this competition")>]
        marketCount : int
        [<Description("Region in which this competition is happening")>]
        competitionRegion : string}

///EventType
and EventType =
    {   [<Description("id")>]
        id : int
        [<Description("name")>]
        name : string}

///EventType Result
and EventTypeResult =
    {   [<Description("The ID identifying the Event Type")>]
        eventType : EventType
        [<Description("Count of markets associated with this eventType")>]
        marketCount : int}

///MarketType Result
and MarketTypeResult =
    {   [<Description("Market Type")>]
        marketType : string
        [<Description("Count of markets associated with this marketType")>]
        marketCount : int}

///CountryCode Result
and CountryCodeResult =
    {   [<Description("The ISO-2 code for the event.  A list of ISO-2 codes is available via http://en.wikipedia.org/wiki/ISO_3166-1_alpha-2")>]
        countryCode : string
        [<Description("Count of markets associated with this Country Code")>]
        marketCount : int}

///Venue Result
and VenueResult =
    {   [<Description("Venue")>]
        venue : string
        [<Description("Count of markets associated with this Venue")>]
        marketCount : int}

///TimeRange
and TimeRange =
    {   [<Description("from")>]
        from : DateTime
        [<Description("to")>]
        ``to`` : DateTime}

///TimeRange Result
and TimeRangeResult =
    {   [<Description("TimeRange")>]
        timeRange : TimeRange
        [<Description("Count of markets associated with this TimeRange")>]
        marketCount : int}

and Order =
    {   betId : string option
        [<Description("BSP Order type.")>]
        orderType : OrderType option
        [<Description("Either EXECUTABLE (an unmatched amount remains) or EXECUTION_COMPLETE (no unmatched amount remains).")>]
        status : OrderStatus option
        [<Description("What to do with the order at turn-in-play")>]
        persistenceType : PersistenceType option
        [<Description("Indicates if the bet is a Back or a LAY")>]
        side : Side option
        [<Description("The price of the bet.")>]
        price : decimal option
        [<Description("The size of the bet.")>]
        size : decimal option
        [<Description("Not to be confused with size. This is the liability of a given BSP bet.")>]
        bspLiability : decimal option
        [<Description("The date, to the second, the bet was placed.")>]
        placedDate : DateTime option
        [<Description("The average price matched at. Voided match fragments are removed from this average calculation. For MARKET_ON_CLOSE BSP bets this reports the matched SP price following the SP reconciliation process.")>]
        avgPriceMatched : decimal
        [<Description("The current amount of this bet that was matched.")>]
        sizeMatched : decimal
        [<Description("The current amount of this bet that is unmatched.")>]
        sizeRemaining : decimal
        [<Description("The current amount of this bet that was lapsed.")>]
        sizeLapsed : decimal
        [<Description("The current amount of this bet that was cancelled.")>]
        sizeCancelled : decimal
        [<Description("The current amount of this bet that was voided.")>]
        sizeVoided : decimal}

///An individual bet Match, or rollup by price or avg price. Rollup depends on the requested MatchProjection
and Match =
    {   [<Description("Only present if no rollup")>]
        betId : string
        [<Description("Only present if no rollup")>]
        matchId : string
        [<Description("Indicates if the bet is a Back or a LAY")>]
        side : Side option
        [<Description("Either actual match price or avg match price depending on rollup.")>]
        price : decimal option
        [<Description("Size matched at in this fragment, or at this price or avg price depending on rollup")>]
        size : decimal option
        [<Description("Only present if no rollup")>]
        matchDate : DateTime}

///Market definition
and MarketDescription =
    {   [<Description("If 'true' the market supports 'Keep' bets if the market is to be turned in-play")>]
        persistenceEnabled : bool option
        [<Description("If 'true' the market supports Betfair SP betting")>]
        bspMarket : bool option
        [<Description("The market start time")>]
        marketTime : DateTime 
        [<Description("The market suspend time")>]
        suspendTime : DateTime option
        [<Description("settled time")>]
        settleTime : DateTime option
        [<Description("See MarketBettingType")>]
        bettingType : MarketBettingType option
        [<Description("If 'true' the market is set to turn in-play")>]
        turnInPlayEnabled : bool option
        [<Description("Market base type")>]
        marketType : string option
        [<Description("The market regulator")>]
        regulator : string option
        [<Description("The commission rate applicable to the market")>]
        marketBaseRate : decimal option
        [<Description("Indicates whether or not the user's discount rate is taken into account on this market. If �false� all users will be charged the same commission rate, regardless of discount rate.")>]
        discountAllowed : bool option
        [<Description("The wallet to which the market belongs (UK/AUS)")>]
        wallet : string
        [<Description("The market rules.")>]
        rules : string option
        rulesHasDate : bool
        [<Description("Any additional information regarding the market")>]
        clarifications : string option}
    static member BettingType x = 
        match x.bettingType with
        | Some x -> 
            match tryGetCaseAttribute x with
            | Some x -> x
            | _ -> sprintf "%A" x
        | _ -> ""

///Market Rates
and MarketRates =
    {   [<Description("marketBaseRate")>]
        marketBaseRate : decimal option
        [<Description("discountAllowed")>]
        discountAllowed : bool option}

///Market Licence
and MarketLicence =
    {   [<Description("The wallet from which funds will be taken when betting on this market")>]
        wallet : string option
        [<Description("The rules for this market")>]
        rules : string
        [<Description("The market's start date and time are relevant to the rules.")>]
        rulesHasDate : bool
        [<Description("Clarifications to the rules for the market")>]
        clarifications : string}

///Market Line and Range Info
and MarketLineRangeInfo =
    {   [<Description("maxPrice")>]
        maxUnitValue : decimal option
        [<Description("minPrice")>]
        minUnitValue : decimal option
        [<Description("interval")>]
        interval : decimal option
        [<Description("unit")>]
        marketUnit : string option}

and PriceSize =
    {   price : decimal option
        size : decimal option}

///Summary of a cleared order.
and ClearedOrderSummary =
    {   [<Description("The id of the event type bet on. Available at EVENT_TYPE groupBy level or lower.")>]
        eventTypeId : EventTypeId
        [<Description("The id of the event bet on. Available at EVENT groupBy level or lower.")>]
        eventId : EventId
        [<Description("The id of the market bet on. Available at MARKET groupBy level or lower.")>]
        marketId : MarketId
        [<Description("The id of the selection bet on. Available at RUNNER groupBy level or lower.")>]
        selectionId : SelectionId
        [<Description("The id of the market bet on. Available at MARKET groupBy level or lower.")>]
        handicap : Handicap
        [<Description("The id of the bet. Available at BET groupBy level.")>]
        betId : BetId
        [<Description("The date the bet order was placed by the customer. Only available at BET groupBy level.")>]
        placedDate : DateTime
        [<Description("The turn in play persistence state of the order at bet placement time. This field will be empty or omitted on true SP bets. Only available at BET groupBy level.")>]
        persistenceType : PersistenceType
        [<Description("The type of bet (e.g standard limited-liability Exchange bet (LIMIT), a standard BSP bet (MARKET_ON_CLOSE), or a minimum-accepted-price BSP bet (LIMIT_ON_CLOSE)). If the bet has a OrderType of MARKET_ON_CLOSE and a persistenceType of MARKET_ON_CLOSE then it is a bet which has transitioned from LIMIT to MARKET_ON_CLOSE. Only available at BET groupBy level.")>]
        orderType : OrderType
        [<Description("Whether the bet was a back or lay bet. Available at SIDE groupBy level or lower.")>]
        side : Side
        [<Description("A container for all the ancillary data and localised text valid for this Item")>]
        itemDescription : ItemDescription
        [<Description("The average requested price across all settled bet orders under this Item. Available at SIDE groupBy level or lower.")>]
        priceRequested : Price
        [<Description("The date and time the bet order was settled by Betfair. Available at SIDE groupBy level or lower.")>]
        settledDate : DateTime
        [<Description("The number of actual bets within this grouping (will be 1 for BET groupBy)")>]
        betCount : int
        [<Description("The cumulative amount of commission paid by the customer across all bets under this Item, in the account currency. Available at EXCHANGE, EVENT_TYPE, EVENT and MARKET level groupings only.")>]
        commission : Size
        [<Description("The average matched price across all settled bets or bet fragments under this Item. Available at SIDE groupBy level or lower.")>]
        priceMatched : Price
        [<Description("If true, then the matched price was affected by a reduction factor due to of a runner removal from this Horse Racing market.")>]
        priceReduced : bool
        [<Description("The cumulative bet size that was settled as matched or voided under this Item, in the account currency. Available at SIDE groupBy level or lower.")>]
        sizeSettled : Size
        [<Description("The profit or loss (negative profit) gained on this line, in the account currency")>]
        profit : Size
        [<Description("The amount of the bet that was available to be matched, before cancellation or lapsing, in the account currency")>]
        sizeCancelled : Size}

///A container representing search results.
and ClearedOrderSummaryReport =
    {   [<Description("The list of cleared orders returned by your query. This will be a valid list (i.e. empty or non-empty but never 'null').")>]
        clearedOrders : ClearedOrderSummary list option
        [<Description("Indicates whether there are further result items beyond this page. Note that underlying data is highly time-dependent and the subsequent search orders query might return an empty result.")>]
        moreAvailable : bool option}

///This object contains some text which may be useful to render a betting history view. It offers no long-term warranty as to the correctness of the text.
and ItemDescription =
    {   [<Description("The event type name, translated into the requested locale. Available at EVENT_TYPE groupBy or lower.")>]
        eventTypeDesc : string
        [<Description("The eventName, or openDate + venue, translated into the requested locale. Available at EVENT groupBy or lower.")>]
        eventDesc : string
        [<Description("The market name or racing market type (\"Win\", \"To Be Placed (2 places)\", \"To Be Placed (5 places)\" etc) translated into the requested locale. Available at MARKET groupBy or lower.")>]
        marketDesc : string
        [<Description("The start time of the market (in ISO-8601 format, not translated). Available at MARKET groupBy or lower.")>]
        marketStartTime : DateTime
        [<Description("The runner name, maybe including the handicap, translated into the requested locale. Available at BET groupBy.")>]
        runnerDesc : string
        [<Description("The numberOfWinners on a market. Available at BET groupBy.")>]
        numberOfWinners : int}

///This object contains the unique identifier for a runner
and RunnerId =
    {   [<Description("The id of the market bet on")>]
        marketId : MarketId option
        [<Description("The id of the selection bet on")>]
        selectionId : SelectionId option
        [<Description("The handicap associated with the runner in case of asian handicap markets, null otherwise.")>]
        handicap : Handicap option}

///A container representing search results.
and CurrentOrderSummaryReport =
    {   [<Description("The list of current orders returned by your query. This will be a valid list (i.e. empty or non-empty but never 'null').")>]
        currentOrders : CurrentOrderSummary list
        [<Description("Indicates whether there are further result items beyond this page. Note that underlying data is highly time-dependent and the subsequent search orders query might return an empty result.")>]
        moreAvailable : bool}

///Summary of a current order.
and CurrentOrderSummary =
    {   [<Description("The bet ID of the original place order.")>]
        betId : int64
        [<Description("The market id the order is for.")>]
        marketId : MarketId
        [<Description("The selection id the order is for.")>]
        selectionId : int 
        [<Description("BACK/LAY")>]
        side : Side 
        [<Description("The handicap of the bet.")>]
        handicap : decimal option
        [<Description("The price and size of the bet.")>]
        priceSize : PriceSize 
        [<Description("Not to be confused with size. This is the liability of a given BSP bet.")>]
        bspLiability : decimal option        
        [<Description("Either EXECUTABLE (an unmatched amount remains) or EXECUTION_COMPLETE (no unmatched amount remains).")>]
        status : OrderStatus option
        [<Description("What to do with the order at turn-in-play.")>]
        persistenceType : PersistenceType option
        [<Description("BSP Order type.")>]
        orderType : OrderType option
        [<Description("The date, to the second, the bet was placed.")>]
        placedDate : DateTime option
        [<Description("The date, to the second, of the last matched bet fragment (where applicable)")>]
        matchedDate : DateTime option
        [<Description("The average price matched at. Voided match fragments are removed from this average calculation.")>]
        averagePriceMatched : decimal option
        [<Description("The current amount of this bet that was matched.")>]
        sizeMatched : decimal option
        [<Description("The current amount of this bet that is unmatched.")>]
        sizeRemaining : decimal option
        [<Description("The current amount of this bet that was lapsed.")>]
        sizeLapsed : decimal option
        [<Description("The current amount of this bet that was cancelled.")>]
        sizeCancelled : decimal option
        [<Description("The current amount of this bet that was voided.")>]
        sizeVoided : decimal option
        [<Description("The regulator authorisation code.")>]
        regulatorAuthCode : string option
        [<Description("The regulator Code.")>]
        regulatorCode : string option}
    member x.IsUnmathed = 
        match x.sizeMatched with 
        | Some x when x>0m -> false
        |  _-> true
        
    static member simple betId marketId selectionId side price size averagePriceMatched sizeMatched = 
        {   betId = betId            
            marketId = { marketId = marketId }            
            selectionId  = selectionId
            side  = side            
            handicap  = None            
            priceSize = { price = Some price; size = Some size} 
            bspLiability  = None            
            status  = None            
            persistenceType  = None            
            orderType  = None            
            placedDate = Some DateTime.Now            
            matchedDate = Some DateTime.Now
            averagePriceMatched = Some averagePriceMatched
            sizeMatched = Some sizeMatched
            sizeRemaining  = None            
            sizeLapsed  = None            
            sizeCancelled  = None            
            sizeVoided = None            
            regulatorAuthCode = None
            regulatorCode = None }

///Instruction to place a new order
and PlaceInstruction =
    {   orderType : OrderType option
        [<Description("The selection_id.")>]
        selectionId : int
        [<Description("The handicap applied to the selection, if on an asian-style market.")>]
        handicap : decimal option
        [<Description("Back or Lay")>]
        side : Side
        [<Description("A simple exchange bet for immediate execution")>]
        limitOrder : LimitOrder
        [<Description("Bets are matched if, and only if, the returned starting price is better than a specified price. In the case of back bets, LOC bets are matched if the calculated starting price is greater than the specified price. In the case of lay bets, LOC bets are matched if the starting price is less than the specified price. If the specified limit is equal to the starting price, then it may be matched, partially matched, or may not be matched at all, depending on how much is needed to balance all bets against each other (MOC, LOC and normal exchange bets)")>]
        limitOnCloseOrder : LimitOnCloseOrder option
        [<Description("Bets remain unmatched until the market is reconciled. They are matched and settled at a price that is representative of the market at the point the market is turned in-play. The market is reconciled to find a starting price and MOC bets are settled at whatever starting price is returned. MOC bets are always matched and settled, unless a starting price is not available for the selection. Market on Close bets can only be placed before the starting price is determined")>]
        marketOnCloseOrder : MarketOnCloseOrder option}

and PlaceExecutionReport =
    {   [<Description("Echo of the customerRef if passed.")>]
        customerRef : string option
        status : ExecutionReportStatus option
        errorCode : ExecutionReportErrorCode option
        [<Description("Echo of marketId passed")>]
        marketId : MarketId
        instructionReports : PlaceInstructionReport list}

///Place a new LIMIT order (simple exchange bet for immediate execution)
and LimitOrder =
    {   [<Description("The size of the bet.")>]
        size : decimal option
        [<Description("The limit price")>]
        price : decimal option
        [<Description("What to do with the order at turn-in-play")>]
        persistenceType : PersistenceType option}

///Place a new LIMIT_ON_CLOSE bet
and LimitOnCloseOrder =
    {   [<Description("The size of the bet.")>]
        liability : decimal option
        [<Description("The limit price of the bet if LOC")>]
        price : decimal option}

///Place a new MARKET_ON_CLOSE bet
and MarketOnCloseOrder =
    {   [<Description("The size of the bet.")>]
        liability : decimal option}

///Response to a PlaceInstruction
and PlaceInstructionReport =
    {   [<Description("whether the command succeeded or failed")>]
        status : InstructionReportStatus option
        [<Description("cause of failure, or null if command succeeds")>]
        errorCode : InstructionReportErrorCode option
        [<Description("The instruction that was requested")>]
        instruction : PlaceInstruction option
        [<Description("The bet ID of the new bet. May be null on failure.")>]
        betId : int64 option
        [<Description("The date on which the bet was placed")>]
        placedDate : DateTime option
        [<Description("The average price matched")>]
        averagePriceMatched : decimal option
        [<Description("The size matched.")>]
        sizeMatched : decimal option}

///Instruction to fully or partially cancel an order (only applies to LIMIT orders)
and CancelInstruction =
    {   [<Description("The betId")>]
        betId : int64
        [<Description("If supplied then this is a partial cancel.  Should be set to 'null' if no size reduction is required.")>]
        sizeReduction : decimal option}

and CancelExecutionReport =
    {   [<Description("Echo of the customerRef if passed.")>]
        customerRef : string option
        status : ExecutionReportStatus option
        errorCode : ExecutionReportErrorCode option
        [<Description("Echo of marketId passed")>]
        marketId : MarketId
        instructionReports : CancelInstructionReport list}

///Instruction to replace a LIMIT or LIMIT_ON_CLOSE order at a new price. Original order will be cancelled and a new order placed at the new price for the remaining stake.
and ReplaceInstruction =
    {   [<Description("Unique identifier for the bet")>]
        betId : int64
        [<Description("The price to replace the bet at")>]
        newPrice : decimal }

and ReplaceExecutionReport =
    {   [<Description("Echo of the customerRef if passed.")>]
        customerRef : string option
        status : ExecutionReportStatus option
        errorCode : ExecutionReportErrorCode option
        [<Description("Echo of marketId passed")>]
        marketId : MarketId
        instructionReports : ReplaceInstructionReport list}

and ReplaceInstructionReport =
    {   [<Description("whether the command succeeded or failed")>]
        status : InstructionReportStatus option
        [<Description("cause of failure, or null if command succeeds")>]
        errorCode : InstructionReportErrorCode option
        [<Description("Cancelation report for the original order")>]
        cancelInstructionReport : CancelInstructionReport
        [<Description("Placement report for the new order")>]
        placeInstructionReport : PlaceInstructionReport option}

and CancelInstructionReport =
    {   [<Description("whether the command succeeded or failed")>]
        status : InstructionReportStatus option
        [<Description("cause of failure, or null if command succeeds")>]
        errorCode : InstructionReportErrorCode option
        [<Description("The instruction that was requested")>]
        instruction : CancelInstruction
        sizeCancelled : decimal option
        cancelledDate : DateTime option}

///Instruction to update LIMIT bet's persistence of an order that do not affect exposure
and UpdateInstruction =
    {   [<Description("Unique identifier for the bet")>]
        betId : string option
        [<Description("The new persistence type to update this bet to")>]
        newPersistenceType : PersistenceType option}

and UpdateExecutionReport =
    {   [<Description("Echo of the customerRef if passed.")>]
        customerRef : string option
        status : ExecutionReportStatus option
        errorCode : ExecutionReportErrorCode option
        [<Description("Echo of marketId passed")>]
        marketId : MarketId
        instructionReports : UpdateInstructionReport list}

and UpdateInstructionReport =
    {   [<Description("whether the command succeeded or failed")>]
        status : InstructionReportStatus option
        [<Description("cause of failure, or null if command succeeds")>]
        errorCode : InstructionReportErrorCode option
        [<Description("The instruction that was requested")>]
        instruction : UpdateInstruction option}

///Selection criteria of the returning price data
and PriceProjection =
    {   [<Description("The basic price data you want to receive in the response.")>]
        priceData : PriceData list
        [<Description("Options to alter the default representation of best offer prices Applicable to EX_BEST_OFFERS priceData selection")>]
        exBestOffersOverrides : ExBestOffersOverrides option
        [<Description("Indicates if the returned prices should include virtual prices. Applicable to EX_BEST_OFFERS and EX_ALL_OFFERS priceData selections, default value is false.")>]
        virtualise : bool option
        [<Description("Indicates if the volume returned at each price point should be the absolute value or a cumulative sum of volumes available at the price and all better prices. If unspecified defaults to false. Applicable to EX_BEST_OFFERS and EX_ALL_OFFERS price projections. Not supported as yet.")>]
        rolloverStakes : bool option}
    static member Default = 
        {   priceData = []
            exBestOffersOverrides = None
            virtualise = None
            rolloverStakes = None }

///Options to alter the default representation of best offer prices
and ExBestOffersOverrides =
    {   [<Description("The maximum number of prices to return on each side for each runner. If unspecified defaults to 3.")>]
        bestPricesDepth : int
        [<Description("The model to use when rolling up available sizes. If unspecified defaults to STAKE rollup model with rollupLimit of minimum stake in the specified currency.")>]
        rollupModel : RollupModel
        [<Description("The volume limit to use when rolling up returned sizes. The exact definition of the limit depends on the rollupModel. If no limit is provided it will use minimum stake as default the value. Ignored if no rollup model is specified.")>]
        rollupLimit : int option
        [<Description("Only applicable when rollupModel is MANAGED_LIABILITY. The rollup model switches from being stake based to liability based at the smallest lay price which is >= rollupLiabilityThreshold.service level default (TBD). Not supported as yet.")>]
        rollupLiabilityThreshold : decimal option
        [<Description("Only applicable when rollupModel is MANAGED_LIABILITY. (rollupLiabilityFactor * rollupLimit) is the minimum liabilty the user is deemed to be comfortable with. After the rollupLiabilityThreshold price subsequent volumes will be rolled up to minimum value such that the liability >= the minimum liability.service level default (5). Not supported as yet.")>]
        rollupLiabilityFactor : int option}

///Profit and loss in a market
and MarketProfitAndLoss =
    {   [<Description("The unique identifier for the market")>]
        marketId : string option
        [<Description("The commission rate applied to P&L values. Only returned if netOfCommision option is requested")>]
        commissionApplied : decimal option
        [<Description("Calculated profit and loss data.")>]
        profitAndLosses : RunnerProfitAndLoss list option}

///
and RunnerProfitAndLoss =
    {   [<Description("The unique identifier for the selection")>]
        selectionId : SelectionId option
        [<Description("Profit and loss for the market if this selection is the winner")>]
        ifWin : decimal option
        [<Description("Profit and loss for the market if this selection is the loser. Only returned for multi-winner odds markets.")>]
        ifLose : decimal option}

type MarketBookRequest = {   
    marketIds : MarketId list
    priceProjection : PriceProjection option
    orderProjection : OrderProjection option
    matchProjection : MatchProjection option
    currencyCode : string option
    locale : string }

type ListEventTypesRequest = {
    locale : string
    }

type MarketCatalogueRequest =
    {   locale : string
        marketProjection : MarketProjection list
        filter : MarketFilter
        maxResults : int }

type EventResultRequest =
    {   locale : string
        filter : MarketFilter  }
