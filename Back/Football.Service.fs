module Betfair.Football.Services

open System
//open System.Collections.ObjectModel
open System.Net
open System.Text

[<AutoOpen>]
module private Helpers  =

    let downloadUrl url f = async{

        let webClient = 
            {   new WebClient(Encoding = UTF8Encoding.UTF8) with
                    override x.GetWebRequest( address : Uri ) = 
                        let request =  base.GetWebRequest(address) 
                        (request :?> HttpWebRequest).AutomaticDecompression <- DecompressionMethods.GZip + DecompressionMethods.Deflate            
                        request }        
        webClient.Headers.[HttpRequestHeader.AcceptLanguage] <- "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4"
        webClient.Headers.[HttpRequestHeader.Accept] <- "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8"
        webClient.Headers.[HttpRequestHeader.UserAgent ] <- "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.143 Safari/537.36"
        webClient.Headers.[HttpRequestHeader.Cookie] <- """__utma=1.70504096.1402649363.1402649363.1402649363.1; __utmz=1.1402649363.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none); bfj=RU; UI=0; spi=0; pref_pg_pers_0="{\"*\":{\"exp\":\"e\"}}"; Betex_7989563=upSAF%3A%3Atrue; clog=7989563; bfsd=ts=1407859553103|st=reg; PI=3013; StickyTags=rfr=3013; TrackingTags=rfr=3013; pi=partner3013; rfr=3013; sess=active; NSC_mc-80-qpsubm.efgbvmu=ffffffff0920b70a45525d5f4f58455e445a4a4229a0; BFS=; bspLP=false; wsid=5d2e6400-2398-11e4-93c7-90e2ba0fa318; mEWJSESSIONID=4DBB8A1F167111E2E2B783208827C834; pref_md_0="{\"betslip\":{\"s\":\"0\"},\"cashout-minimarketview\":{\"expanded-OVER_UNDER_15\":\"0\",\"expanded-HALF_TIME_SCORE\":\"0\",\"expanded-OVER_UNDER_35\":\"0\",\"expanded-CORRECT_SCORE\":\"0\",\"expanded-OVER_UNDER_05\":\"0\",\"expanded-NEXT_GOAL\":\"0\",\"expanded-MATCH_ODDS\":\"0\",\"expanded-OVER_UNDER_45\":\"0\",\"expanded-OVER_UNDER_25\":\"1\",\"expanded-HALF_TIME\":\"0\",\"expanded-HALF_TIME_FULL_TIME\":\"0\"}}"; _ga=GA1.2.70504096.1402649363; exp=ex; pref_md_pers_0="{\"com-es-info\":{\"spainRedirectNotification\":\"false\"},\"favourites\":{\"expanded\":\"false\"},\"marketgroups\":{\"linearView\":\"true\",\"selectedTab\":\"1\"}}"; bucket=3~0~master; homepageredirect=TRUE; BETEX_ESD=accountservices; betexPtk=betexCurrency%3DUSD%7EbetexTimeZone%3DEET%7EbetexRegion%3DGBR%7EbetexLocale%3Dru; betexPtkSess=betexCurrencySessionCookie%3DUSD%7EbetexRegionSessionCookie%3DGBR%7EbetexTimeZoneSessionCookie%3DEET%7EbetexLocaleSessionCookie%3Dru%7EbetexSkin%3Dstandard%7EbetexBrand%3Dbetfair; userhistory=15478133831402649357792|8|Y|140814|130814|home|Y; bftim=1408123269895; geoIpCountryCode=DE; vid=5bb350e9-f6a7-4962-bf00-8de7f46f648d"""
        webClient.Headers.[HttpRequestHeader.Referer] <- "http://www.betfair.com/ru/"
        webClient.Headers.[HttpRequestHeader.AcceptEncoding] <- "gzip,deflate,sdch"       
        
        return! catchInetErrors <| async{ 
            let! x =  webClient.AsyncDownloadString ( Uri( url ) ) 
            return f x }  }

    let downloadCouponPage couponId npage = 
        let url = 
            #if DEBUG 
            sprintf "http://0s.o53xo.mjsxiztbnfzc4y3pnu.cmle.ru/exchange/football/coupon?id=%d&goingInPlay=true&fdcPage=%d" couponId npage
            #else
            sprintf "http://sports.betfair.com/football/coupon?id=%d&goingInPlay=true&fdcPage=%d" couponId npage
            #endif
        downloadUrl url (Parsing.coupon npage )

    let readGamesList couponId isAllToday =
        let rec loop acc npage = asyncEiter{
            let! (newPortion, xnextPage ) = downloadCouponPage couponId npage 
            let games = acc @ newPortion
            match xnextPage with
            | None -> return (games, None)
            | Some nextPage -> 
                if isAllToday then return! loop games nextPage else
                match List.rev newPortion with
                | ( _,{playMinute = Some _ } )::_ -> return! loop games nextPage
                | _ -> return (games, xnextPage)}
        loop []

    
    let couponIdUrl =         
        #if DEBUG 
        "http://0s.o53xo.mjsxiztbnfzc4y3pnu.cmle.ru/exchange/football"
        #else
        "https://www.betfair.com/exchange/football"
        #endif 

module CouponId = 
    open Concurency
    let status = Status ("COUPON-ID")
    let state = todayRef()

    let private get() = async{                     
        match state.Value with
        | Some x -> return Right x
        | _ ->
            let! x = downloadUrl couponIdUrl (Parsing.firstPageId)
            match x with
            | Left x -> 
                status.Set Logging.Error "%s" x
                return Left x
            | Right x -> 
                status.Set Logging.Info "%d" x
                state.Set x
                return Right x }

    let do' f = async {
        let! couponId = get() 
        match couponId with 
        | Left x -> return Logging.Error, sprintf "can't get coupon id : %s" x
        | Right couponId -> return! f couponId }

module Coupon = 
    open Concurency    
    let state = Status ("WEB-COUPON")
    let inplay = atom []
    let foreplay = atom []    
    let nextPageId = atom None
    
    let ``no next id``   = Logging.Warn, "no next id"
    let ``ok`` = Logging.Info, "Ok"


        
    let updateInplay = CouponId.do' <| fun couponId -> async {
        let! gs = readGamesList couponId false 1
        match gs with
        | Right ((games, nextPageId') as x) ->                 
            inplay.Set games
            nextPageId.Set nextPageId'
            return ``ok``
        | Left error -> return Logging.Error, error }

    let updateForeplay = CouponId.do' <| fun couponId -> async {
        match nextPageId.Value with 
        | None -> return ``no next id``
        | Some nextPage ->
            let! games = readGamesList couponId true nextPage 
            match games with
            | Right (today2,n) -> 
                foreplay.Set today2  
                return ``ok``
            | Left error -> return Logging.Error, error  }
        
        
