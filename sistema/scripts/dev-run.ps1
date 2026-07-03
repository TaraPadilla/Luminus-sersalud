# Desarrollo local: SerSalud (:5000), AVM (:5001) o ambos a la vez.
param(
    [ValidateSet("SerSalud", "AVM", "Both")]
    [string]$Profile = "SerSalud",
    [switch]$NoBuild
)

$ErrorActionPreference = "Stop"
$projectRoot = Split-Path $PSScriptRoot -Parent
Set-Location $projectRoot

function Test-FarmamestRunning {
    return $null -ne (Get-Process -Name farmamest -ErrorAction SilentlyContinue | Select-Object -First 1)
}

function Start-FarmamestProfile {
    param(
        [Parameter(Mandatory = $true)][string]$LaunchProfile,
        [switch]$SkipBuild
    )

    $runArgs = @("run", "--launch-profile", $LaunchProfile)
    if ($SkipBuild) { $runArgs += "--no-build" }

    $url = if ($LaunchProfile -eq "AVM") { "http://localhost:5001" } else { "http://localhost:5000" }
    Write-Host "Iniciando $LaunchProfile en $url..." -ForegroundColor Cyan
    & dotnet @runArgs
    exit $LASTEXITCODE
}

if ($Profile -eq "Both") {
    Write-Host "Compilando una vez..." -ForegroundColor Yellow
    & dotnet build -v q
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    $runner = Join-Path $PSScriptRoot "dev-run.ps1"
    Write-Host "Abriendo SerSalud (5000) y AVM (5001) en ventanas separadas..." -ForegroundColor Green
    Start-Process powershell -ArgumentList @(
        "-NoExit", "-ExecutionPolicy", "Bypass", "-File", $runner, "-Profile", "SerSalud", "-NoBuild"
    ) | Out-Null
    Start-Sleep -Seconds 4
    Start-Process powershell -ArgumentList @(
        "-NoExit", "-ExecutionPolicy", "Bypass", "-File", $runner, "-Profile", "AVM", "-NoBuild"
    ) | Out-Null
    exit 0
}

if ($NoBuild -or (Test-FarmamestRunning)) {
    if (-not $NoBuild) {
        Write-Host "Otra instancia de farmamest ya corre; se usa --no-build." -ForegroundColor Yellow
    }
    Start-FarmamestProfile -LaunchProfile $Profile -SkipBuild
}

Write-Host "Compilando..." -ForegroundColor Yellow
& dotnet build -v q
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Start-FarmamestProfile -LaunchProfile $Profile
