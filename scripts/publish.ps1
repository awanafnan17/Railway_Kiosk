param([string]$Runtime = "win-x64", [string]$Configuration = "Release")
$ErrorActionPreference = "Stop"
Push-Location "$PSScriptRoot\.."
dotnet publish -c $Configuration -r $Runtime --self-contained true -p:PublishSingleFile=false -p:PublishTrimmed=false
Pop-Location
