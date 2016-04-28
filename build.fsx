// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"
open System.Diagnostics
open Fake

// Directories
let buildDir  = "./build/"

// Filesets
let appReferences  =
    !! "/**/*.csproj"
      ++ "/**/*.fsproj"
      -- "/LocalTest1/*.fsproj"

// version info
let version = "0.1"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir]
)

Target "Build" (fun _ ->
    // compile all projects below src/app/
    MSBuildDebug buildDir "Build" appReferences
        |> Log "AppBuild-Output: "
)

Target "Run" (fun _ ->
    let dir = System.Environment.CurrentDirectory 
    
    let p = new Process();    
    let i = p.StartInfo    
    i.UseShellExecute <- false
    i.FileName <- "build/LocalHost.exe"
    i.Arguments <- ""
    i.ErrorDialog <- false
    i.WindowStyle <- ProcessWindowStyle.Minimized
    i.WorkingDirectory <- "build"
    if p.Start() then 
        p.WaitForExit()
    else
        eprintf "cant start localhost.exe - %d" p.ExitCode    
)


// Build order
"Clean"
  ==> "Build"
  ==> "Run"

// start build
RunTargetOrDefault "Run"
