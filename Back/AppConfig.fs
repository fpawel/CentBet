module AppConfig

module EnableBetfairApiNGDebugLogs = 
    let get,set,_ = createAtom false

type AppKey'SessionToken = 
    {   SessionToken : string
        AppKey : string }
    member x.Auth = 
        x.SessionToken, x.AppKey

module Betfair =    
    let get,set,_ = createAtom<AppKey'SessionToken option> None
    let apply f = async{
        let! x = get()
        match x with
        | Some x -> do! f x
        | _ -> () }