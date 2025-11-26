param(
  [switch]$SkipSDK
)
$ErrorActionPreference = "Stop"
Write-Host "Detecting .NET Desktop Runtime 6..."
function Test-Dotnet {
  try { dotnet --info | Out-Null; return $true } catch { return $false }
}
if (-not (Test-Dotnet)) {
  Write-Host "Installing .NET SDK 6..."
  $url = "https://download.visualstudio.microsoft.com/download/pr/48e4d51a-3b8f-4a7a-9bc7-602d3435b84f/9bfc86a9aee42b9b36a90b48dd85b167/dotnet-sdk-6.0.428-win-x64.exe"
  $tmp = Join-Path $env:TEMP "dotnet-sdk-6.exe"
  Invoke-WebRequest -Uri $url -OutFile $tmp
  Start-Process -FilePath $tmp -ArgumentList "/install /quiet /norestart" -Wait
}
Write-Host "Installing .NET Desktop Runtime 6..."
$rtUrl = "https://download.visualstudio.microsoft.com/download/pr/1b09c0ae-e16c-4d1c-bb37-3f3f74b97f80/8f42d9d3583adce935ff707240b2f5ba/windowsdesktop-runtime-6.0.36-win-x64.exe"
$rt = Join-Path $env:TEMP "windowsdesktop-runtime-6.exe"
Invoke-WebRequest -Uri $rtUrl -OutFile $rt
Start-Process -FilePath $rt -ArgumentList "/install /quiet /norestart" -Wait
Write-Host "Setup completed"
