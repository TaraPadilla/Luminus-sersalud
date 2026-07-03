# Prueba local igual que el servidor del cliente (Production + solo appsettings.json).
param(
    [ValidateSet("SerSalud", "AVM")]
    [string]$Perfil = "SerSalud"
)

$ErrorActionPreference = "Stop"
$projectRoot = Split-Path $PSScriptRoot -Parent
Set-Location $projectRoot

Write-Host "Deteniendo farmamest..." -ForegroundColor Yellow
Get-Process -Name "farmamest" -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Milliseconds 800

$extras = Get-ChildItem $projectRoot -Filter "appsettings.*.json" -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -notin @("appsettings.json", "appsettings.ESTRUCTURA-SERVIDOR.json") }
if ($extras) {
    Write-Host "ADVERTENCIA: archivos de configuracion extra (no usados en Production):" -ForegroundColor Yellow
    $extras | ForEach-Object { Write-Host "  - $($_.Name)" -ForegroundColor Yellow }
    Write-Host "  En el servidor del cliente solo debe existir appsettings.json" -ForegroundColor DarkYellow
}

if (-not (Test-Path (Join-Path $projectRoot "appsettings.json"))) {
    Write-Host "ERROR: falta appsettings.json en $projectRoot" -ForegroundColor Red
    exit 1
}

$url = if ($Perfil -eq "AVM") { "http://localhost:5001" } else { "http://localhost:5000" }
Write-Host "Iniciando $Perfil en $url (ASPNETCORE_ENVIRONMENT=Production)..." -ForegroundColor Cyan
Write-Host "Config activa: appsettings.json + variables del perfil" -ForegroundColor DarkGray
dotnet run -c Release --launch-profile $Perfil
