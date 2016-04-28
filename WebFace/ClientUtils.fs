[<AutoOpen>]
[<WebSharper.Pervasives.JavaScript>]
module CentBet.Client.Utils
open System
open WebSharper
open WebSharper.JavaScript

let rec trimEnd = function
    | "",_ -> ""
    | s, [] -> s
    | s, (c :: _ as rx) when s.[s.Length-1] = c -> trimEnd (s.[..s.Length-2],rx)
    | s, _::rx -> trimEnd (s,rx)

let formatDecimal x = 
    trimEnd (sprintf "%M" x, ['0'; '.'])

let formatDecimalOption = function
    | None -> ""
    | Some x -> formatDecimal x

[<Direct "dateTimeToString($s)">]
let dateTimeToString (s: int64) : string = failwith "n/a"

//let mkids<'T,'a when 'a:comparison> (x : 'T list)  (getid : 'T -> 'a)  =
//    let m = x |> List.map(fun g -> (getid g), g) |> Map.ofList
//    let s = x |> List.map( getid ) |> Set.ofList
//    s,m

let getDatePart (x:JavaScript.Date) = 
    let y = Date(x.GetTime())
    y.SetHours(0,0,0,0)
    y

[<Inline "createBlobFromString($arg)">]
let createBlobFromString (arg : string) : Blob = failwith "n/a"


let (|Err|Ok|) = function
    | Choice1Of2 a -> Err a
    | Choice2Of2 b -> Ok b

let Err = Choice1Of2
let Ok = Choice2Of2

module Option = 
    
    let getWith def x =
        match x with
        | Some x' -> x'
        | None -> def

module LocalStorage = 
    open WebSharper.UI.Next
    let private strg = WebSharper.JavaScript.JS.Window.LocalStorage

    let clear k = strg.RemoveItem k

    let get<'a> k = 
        try 
            let value = 
                strg.GetItem k
                |> JavaScript.JSON.Parse
                :?> 'a

            Some value
        with e -> 
            printfn "error getting from local storage, key %A : %A" k e 
            None

    let set k value = 
        try 
            let value = JavaScript.JSON.Stringify(value)
            strg.SetItem(k,value)            
        with error -> 
            failwithf "error setting to local storage, key %A, value %A : %A" k value error            

    let getWithDef k def = 
        match get k with
        | Some k -> k
        | _ -> def

    // если значение даты в ключе createdKey превышает текущую дату более чем на сутки 
    //  - удалить значение в ключе createdKey, вернуть текущую дату
    // иначе 
    //  - вернуть значение даты в ключе createdKey
    let checkTodayKey createdKey key = 
        let now = DateTime.Now
        let creationDate = getWithDef createdKey now
        if now - creationDate > TimeSpan.FromDays 1. then
            clear key
            set createdKey DateTime.Now 
            printfn "local storage %A of %A is inspired" key  creationDate
            now
        else creationDate

open WebSharper.UI.Next
open WebSharper.UI.Next.Client
open WebSharper.UI.Next.Html

let updateVarValue<'a when 'a : equality> (x:Var<'a>) (value : 'a) =
    if x.Value <> value then
        x.Value <- value


let private initializeModalVar = 
    let hs = ref []
    let onclick (e:Dom.Event) = 
        if e.Target ? className = "w3-modal" then                    
            !hs |> List.iter( fun h ->  h() )
    JS.Window.AddEventListener( "click", onclick, false)
    fun h -> 
        hs := h :: !hs


let createModal viewVisible close = 
    let doc (x :Elt) = x :> Doc
    let (~%%) = attr.``class``
    do
        initializeModalVar close

    
    

    let renderModalContent attrs content =
        doc <| divAttr[ 
            %% "w3-modal"
            attr.style "display : block;"] [
            divAttr [ 
                //attr.style "border-radius: 10px;"
                yield! attrs
                yield %% "w3-modal-content w3-animate-zoom w3-card-8"  ] content ]

    let render attrs content =
        viewVisible |> View.Map( function
            | false -> Doc.Empty
            | _ -> renderModalContent attrs content)
        |> Doc.EmbedView    

    render

let titleAndCloseButton title close =        
    let (~%%) = attr.``class``
    let doc (x :Elt) = x :> Doc
    headerAttr [ %% "w3-row w3-teal" ] [
        h4Attr [ 
            %% "w3-col s11 w3-center"
            attr.style "padding-left : 10px;" ] [ 
            text title ] 
            
        divAttr [%% "w3-col s1 w3-center"] [
            spanAttr [ 
                %% "w3-closebtn" 
                attr.style "padding-right : 5px;"
                on.click ( fun _ _ ->                     
                    close() ) ] 
                [ text "×" ] ] ]