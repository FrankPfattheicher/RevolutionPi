cls
IF NOT EXIST "packages\FAKE\tools\Fake.exe" ".\NuGet.exe" Install "FAKE" -OutputDirectory "packages" -ExcludeVersion
IF NOT EXIST "packages\NUnit.ConsoleRunner\tools\nunit3-console.exe" ".\NuGet.exe" Install "NUnit.Runners" -OutputDirectory "packages" -ExcludeVersion
"packages\FAKE\tools\Fake.exe" build.fsx
