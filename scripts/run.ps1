param([string]$Configuration = "Debug")
$ErrorActionPreference = "Stop"
$exe = Join-Path "$PSScriptRoot\..\bin\$Configuration\net6.0-windows" "RailwayKiosk.exe"
if (Test-Path $exe) {
  Start-Process -FilePath $exe -WindowStyle Normal
} else {
  Write-Error "Executable not found: $exe"
}
