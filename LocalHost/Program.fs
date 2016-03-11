// Дополнительные сведения о F# см. на http://fsharp.org
// Дополнительную справку см. в проекте "Учебник по F#".

[<EntryPoint>]
let main argv = 
    WebSharper.Warp.RunAndWaitForInput( CentBet.Site.Main )
