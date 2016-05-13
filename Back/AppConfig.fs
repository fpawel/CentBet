module AppConfig

open System
open System.Configuration
open System.IO

[<AutoOpen>]
module private Helpers =
    let appsets = ConfigurationManager.AppSettings
    module K = 
        let betuser = "BET_USER"
        let betpass = "BET_PASS"
        let constr = "SQLSERVER_CONNECTION_STRING" 

let initialize() = 
    let userPass = File.ReadAllText("../../../password.txt")
    let [|_; betuser; betpass|] = userPass.Split( [|" "; "\r\n"|], StringSplitOptions.RemoveEmptyEntries )    
    appsets.[K.betuser] <- betuser
    appsets.[K.betpass] <- betpass
    appsets.[K.constr] <- File.ReadAllText("../../../constr.txt")

let betuser() = appsets.[K.betuser]
let betpass() = appsets.[K.betpass]
let constr() = appsets.[K.constr]

let dump() = 
    Logging.info "betuser : %A\nbetpass : %A\nconstr : %A\n" (betuser()) (betpass()) (constr())