
#r @"packages\FAKE\tools\FakeLib.dll"
open Fake
open Fake.AssemblyInfoFile
open System.IO

let NUnit = Fake.Testing.NUnit3.NUnit3

let release =
    ReadFile "ReleaseNotes.md"
    |> ReleaseNotesHelper.parseReleaseNotes

// Properties
let buildDir = @".\builds46"
let testsDir = @".\tests"
let artifactsDir = @".\Artifacts"

let CreateDirs dirs = for dir in dirs do CreateDir dir

Target "Clean" (fun _ ->
    CreateDirs [artifactsDir]
    CleanDirs [artifactsDir]
    CleanDirs [testsDir]
    CleanDirs [buildDir]
)

Target "CreatePackage" (fun _ ->
    // Copy all the package files into a package folder
    let libFile = buildDir </> @"IctBaden.RevolutionPi.dll"
    if Fake.FileHelper.TestFile libFile
    then CleanDir @".\nuget"
         CreateDir @".\nuget\lib" 
         CreateDir @".\nuget\lib\net4" 
         CopyFiles @".\nuget\lib\net4" [ libFile; 
                                         buildDir </> @"IctBaden.RevolutionPi.pdb"
                                       ]
         NuGet (fun p -> 
        {p with
            Authors = [ "Frank Pfattheicher" ]
            Project = "IctBaden.RevolutionPi"
            Description = "RevolutionPi .NET Library"
            OutputPath = @".\nuget"
            Summary = "RevolutionPi .NET Library"
            WorkingDir = @".\nuget"
            Version = release.NugetVersion
            ReleaseNotes = release.Notes.Head
            Files = [ 
                      (@"lib/net4/IctBaden.RevolutionPi.dll", Some "lib/net4", None)
                      (@"lib/net4/IctBaden.RevolutionPi.pdb", Some "lib/net4", None) 
                    ]
            ReferencesByFramework = [ { FrameworkVersion  = "net4"; References = [ "IctBaden.RevolutionPi.dll"
                                                                                 ] } ]
            DependenciesByFramework = [ { FrameworkVersion  = "net4"; Dependencies = [ "Newtonsoft.Json", "10.0.3" 
                                                                                     ] } ]
            Publish = false }) // using website for upload
            @"RevolutionPi.nuspec"
    else
        printfn "*****************************************************" 
        printfn "Output file missing. Package built with RELEASE only." 
        printfn "*****************************************************" 
)

Target "Build4" (fun _ ->
     !! @".\**\*.csproj" 
     -- @".\**\*Test.csproj"
      |> MSBuildRelease buildDir "Build"
      |> Log "Build-Output: "
)

Target "Build4Tests" (fun _ ->
     !! @".\**\*Test.csproj"
      |> MSBuildRelease testsDir "Build"
      |> Log "TestBuild-Output: "
)

Target "Run4Tests" (fun _ ->
    !! (testsDir + @"\*Test.dll")
      |> NUnit (fun p -> 
      {p with 
        ShadowCopy = false
        WorkingDir = artifactsDir
      })
)

Target "AssemblyVersion" (fun _ ->
    CreateCSharpAssemblyInfo @".\RevPi-AssemblyInfo.cs"
        [Attribute.Copyright "Copyright ©2017 ICT Baden GmbH"
         Attribute.Company "ICT Baden GmbH"
         Attribute.Product "RevolutionPi .NET Library"
         Attribute.Version release.AssemblyVersion
         Attribute.FileVersion release.AssemblyVersion]
)

// Dependencies
"Clean"
  ==> "AssemblyVersion"
  ==> "Build4"
  ==> "Build4Tests"
  ==> "Run4Tests"
  ==> "CreatePackage"

RunTargetOrDefault "CreatePackage"
