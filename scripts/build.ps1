param([string]$Configuration = "Release")
$ErrorActionPreference = "Stop"
Push-Location "$PSScriptRoot\.."
dotnet restore
dotnet build -c $Configuration
dotnet test -c $Configuration
Pop-Location
