# Ensures wwwroot/Rotativa/Windows/wkhtmltopdf.exe exists (required for PDFs on Windows servers).
$ErrorActionPreference = "Stop"

$ProjectDir = Split-Path $PSScriptRoot -Parent
$DestDir = Join-Path $ProjectDir "wwwroot\Rotativa\Windows"
$DestExe = Join-Path $DestDir "wkhtmltopdf.exe"
$MinBytes = 100000

if ((Test-Path $DestExe) -and ((Get-Item $DestExe).Length -ge $MinBytes)) {
    Write-Host "wkhtmltopdf.exe OK en $DestExe" -ForegroundColor Green
    exit 0
}

Write-Host "Descargando wkhtmltopdf (necesario para PDFs)..." -ForegroundColor Yellow
New-Item -ItemType Directory -Path $DestDir -Force | Out-Null

$7zUrl = "https://github.com/wkhtmltopdf/packaging/releases/download/0.12.6-1/wkhtmltox-0.12.6-1.mxe-cross-win64.7z"
$7zPath = Join-Path $env:TEMP "wkhtmltox.7z"
$extractDir = Join-Path $env:TEMP "wkhtmltox-extract"

Invoke-WebRequest -Uri $7zUrl -OutFile $7zPath -UseBasicParsing

$7za = Join-Path $env:TEMP "7za.exe"
if (-not (Test-Path $7za)) {
    $7zaZip = Join-Path $env:TEMP "7za920.zip"
    Invoke-WebRequest -Uri "https://www.7-zip.org/a/7za920.zip" -OutFile $7zaZip -UseBasicParsing
    $7zaDir = Join-Path $env:TEMP "7za-portable"
    if (Test-Path $7zaDir) { Remove-Item $7zaDir -Recurse -Force }
    Expand-Archive $7zaZip $7zaDir -Force
    $7za = (Get-ChildItem $7zaDir -Recurse -Filter "7za.exe" | Select-Object -First 1).FullName
}

if (Test-Path $extractDir) { Remove-Item $extractDir -Recurse -Force }
New-Item -ItemType Directory -Path $extractDir -Force | Out-Null
& $7za x $7zPath "-o$extractDir" -y | Out-Null

$binDir = Join-Path $extractDir "wkhtmltox\bin"
if (-not (Test-Path $binDir)) { throw "No se encontro wkhtmltox\bin en el archivo descargado." }

Copy-Item (Join-Path $binDir "wkhtmltopdf.exe") $DestDir -Force
Copy-Item (Join-Path $binDir "wkhtmltox.dll") $DestDir -Force -ErrorAction SilentlyContinue
Copy-Item (Join-Path $binDir "wkhtmltoimage.exe") $DestDir -Force -ErrorAction SilentlyContinue

if (-not (Test-Path $DestExe) -or (Get-Item $DestExe).Length -lt $MinBytes) {
    throw "No se pudo instalar wkhtmltopdf.exe en $DestDir"
}

Write-Host "wkhtmltopdf instalado en $DestExe" -ForegroundColor Green
