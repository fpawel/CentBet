module CentBet.Admin

open WebSharper

let adminsKeys = [   "43EE17BD4E8057F1CA0201FBEBDD71F7" ]

let isAuthorized () = 
    let ctx = Web.Remoting.GetContext()
    async {
        try
            let! x = ctx.UserSession.GetLoggedInUser()
            return x.IsSome
        with _ -> 
            return false }

            
[<Rpc>]
let getIsAuthorized() = async {
    return! isAuthorized () }
    
[<Rpc>]
let authorize key = 
    let ctx = Web.Remoting.GetContext()
    async {
        Logging.write Logging.Warn "authorize {key:%A, uri:%A, environment:%A}" key ctx.RequestUri ctx.Environment 
        try
            if adminsKeys |> List.exists( (=)  (md5hash key) ) then                
                do! ctx.UserSession.LoginUser "admin"
                Logging.write Logging.Info "admin authorized" 
                return None
            else                
                Logging.write Logging.Warn "trying authorize with wrong key"
                return Some "wrong key"
        with e ->             
            return Some e.Message}

[<Rpc>]
let getLogging req = async{
    let! user = WebSharper.Web.Remoting.GetContext().UserSession.GetLoggedInUser()
    if user.IsSome then 
        let! r = Logging.getRecords req
        return Right r 
    else 
        Logging.write Logging.Warn "not authorized logging request"
        return Left "not authorized" }





