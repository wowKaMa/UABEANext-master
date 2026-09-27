$url = "https://github.com/microsoft/DirectXTex/releases/download/feb2024/texconv.exe"
$output = "e:\UABEANext-master\UABEANext-master\UABEANext4\Tools\texconv.exe"
$dir = Split-Path $output
if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force }
Invoke-WebRequest -Uri $url -OutFile $output
Write-Host "Download complete: $output"
