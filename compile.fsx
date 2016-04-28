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

// version info
let version = "0.1"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    trace <| sprintf "Cleaning %A..." buildDir
    CleanDirs [buildDir]
    trace "done."
)

Target "Build" (fun _ ->
    // compile all projects below src/app/
    MSBuildDebug buildDir "Build" appReferences
        |> Log "AppBuild-Output: "
)

// Build order
"Clean"
  ==> "Build"

// start build
MSBuildDefaults <- { MSBuildDefaults with Verbosity = Some MSBuildVerbosity.Minimal }
RunTargetOrDefault "Build"
