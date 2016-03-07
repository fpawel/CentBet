module Betfair.Football.Parsing

open System
open System.Text
open System.Text.RegularExpressions

open HtmlAgilityPack

[<AutoOpen>]
module private Helpers =

    let (|InplayInfo|) = GameInfo.inplayInfo


    let htmlDecode (s:string) = System.Web.HttpUtility.HtmlDecode(s.Trim())
    

    // для поиска целых по рег.выр.
    let parseInt pattern input = 
        let mtchsList (m:Match) = List.tail [for g in m.Groups -> g.Value ]
        let result = Regex.Match( input, pattern )        
        if not result.Success then None 
        else
            match mtchsList result with
            | hd :: [] -> 
                let f,v = Int32.TryParse hd
                if f then Some( v )  else None          
            | _ -> None
    let (|ParseInt|_|) = parseInt
    
let time source = 
    let rmatch pat = Regex.Match( source, pat)

    let m = rmatch "^[^$]*[П|п]ериод[^$]*$"
    if m.Success then Inplay, Some 45, None else
    let m = rmatch "^[^$]*HZ[^$]*$"
    if m.Success then Inplay, Some 45, None else
    let m = rmatch "^[^$]*[М|м]атч[^$]*$"
    if m.Success then GameOver, None, None else        
    let m = rmatch "^[^\\d]*(\\d\\d):(\\d\\d)[^$]*$"
    if m.Success then Forpaly, None, Some ( Int32.Parse m.Groups.[1].Value, Int32.Parse m.Groups.[2].Value ) else
    let m = rmatch "^[^\\d]*([0-9]+)[`'\"][^$]*$"
    let h3, time = Int32.TryParse m.Groups.[1].Value
    if m.Success && h3 then Inplay, Some time, None else 
    TimeUnknown, None, None

let score source =
    let m = Regex.Match( source, "^[^\\d]*(\\d+)[^-]*-[^\\d]*(\\d+)[^$]*$" )
    if m.Success  && m.Groups.Count=3 |> not then None else
    let h1, home = Int32.TryParse m.Groups.[1].Value
    let h2, away = Int32.TryParse m.Groups.[2].Value
    if (h1 && h2) |> not then None else Some(home, away)

let roundD2 x = 
    let x0, x1, x2 =  Decimal.Ceiling x, Decimal.Round(x,1), Decimal.Round(x,2) 
    if x=x0 then x0
    elif x=x1 then x1
    else x2

let selectionId s = 
    let m = Regex.Match( s, "m(?:\\d+)-sel[\\d]_(?:\\d+)-(\\d+)")
    if not m.Success then -1 else
    Int32.Parse( m.Groups.[1].Value ) 

let price (x:HtmlNode) = 
    let x = x.SelectSingleNode( ".//span[@class=\"price\"]"  )
    if x=null then None else
    let s = x.InnerHtml |> htmlDecode
    let decimalSeparator = Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator
    let s = Regex.Replace( s, "[,.]", decimalSeparator)
    let good, v = System.Decimal.TryParse( s )
    if not good then None else Some (roundD2 v )

let event (e:HtmlNode) = 
    if e=null then None else
    let homeTeam =  let x = e.SelectSingleNode( ".//span[@class=\"home-team\"]"  )
                    if x=null then "" else htmlDecode x.InnerText
    let awayTeam =  let x = e.SelectSingleNode( ".//span[@class=\"away-team\"]"  )
                    if x=null then "" else htmlDecode x.InnerText
    let stime =  
        match   e.SelectNodes( ".//span[@class]" ) 
                |> Seq.tryFind( fun x -> x.Attributes.["class"].Value.Contains("start-time") ) with
        | Some x -> htmlDecode x.InnerText
        | _ -> ""
    let sscore =    
        let x = e.SelectSingleNode( ".//span[@class=\"result\"]"  )
        if x=null then "" else htmlDecode x.InnerText
    let xprices = e.SelectNodes(".//button[@id and @class and @data-bettype]") |> Seq.toList
    if List.length xprices<6 then None else
    let att = e.Attributes.["data-eventid"]
    if att=null then None else
    match Int32.TryParse att.Value with
    | false, _ -> None
    | _, eventId ->
        let att = e.Attributes.["data-marketid"]
        if att=null then None else
        let m = Regex.Match( att.Value, "1.(\\d+)" )
        if not m.Success then None else 
        let v = m.Groups.[1].Value
        match Int32.TryParse v with
        | false, _ -> None
        | _, marketId ->
            let sel = 0::2::4::[] |> List.map( fun n -> 
                let e = xprices.[n]
                let att = e.Attributes.["id"]
                if att=null then (-1) else 
                selectionId att.Value )
            let prices = List.map price xprices
            let kef n = prices.[n]
            let game = 
                {   gameId = eventId, marketId
                    winId = sel.[0] 
                    loseId = sel.[2] 
                    drawId = sel.[1] 
                    home = homeTeam
                    away = awayTeam }

            let score = score sscore 
            let playStatus, playMinute, forplay = time stime
            let gameInfo = 
                {   summary = sscore
                    status = stime
                    playStatus = sprintf "%A" playStatus
                    playMinute = playMinute
                    forplay = forplay
                    score = score
                    order = 0, 0  
                    winBack = kef 0
                    winLay = kef 1
                    loseBack = kef 4
                    loseLay = kef 5
                    drawBack = kef 2
                    drawLay = kef 3  }

            Some (game, gameInfo)

let events npage (htmlEvents:HtmlNodeCollection) = 
    htmlEvents 
    |> Seq.choose event        
    |> Seq.mapi( fun n (g, gi)  -> g, { gi with order = npage,n }  )
    |> Seq.toList
  
let coupon npage source  = 
    let doc = new HtmlDocument()    
    doc.LoadHtml(source)
    if doc.DocumentNode=null then Left "Ошибка формата купона 1" else
        
    let htmlEvents = doc.DocumentNode.SelectNodes("//tbody[@data-marketid and @data-eventid]") 
    if htmlEvents=null then Left "Ошибка формата купона 2" 
    elif Seq.isEmpty htmlEvents then Left "Ошибка формата купона 3" else
        let events = events npage htmlEvents
        let nextPageUrl = 
            let x = doc.DocumentNode.SelectSingleNode( "//a[@class=\"next-page\"]"  ) 
            if x=null then None else 
            let x = x.Attributes.["href"]
            if x=null then None else
            parseInt "fdcPage=(\\d+)" x.Value 
        Right(events,nextPageUrl)

let firstPageId source  = 
        
    (*
    <a class="i13n-ltxt-MoreMkts i13n-SP-1 more-events" 
        href="/exchange/football/coupon?id=7&amp;goingInPlay=true" id="yui_3_3_0_2_144930181157714402">Mehr In-Play Fußball</a>
    *)

    let doc = new HtmlDocument()    
    //doc.Load(@"d:\Projects\betfair\bethlpr\website.html")
    doc.LoadHtml(source)
    if doc.DocumentNode=null then Left "Ошибка формата купона 4" else


    let x = doc.DocumentNode.SelectSingleNode( "//a[contains(@class,'more-events')]"  ) 
    if x=null then Left "не удалось получить id первой страницы купона 1" else 
    let x = x.Attributes.["href"]
    if x=null then Left "не удалось получить id первой страницы купона 2" else
    match  x.Value with
    | ParseInt "id=(\\d+)" id -> Right id
    | _ -> Left "не удалось получить id первой страницы купона 3"
    