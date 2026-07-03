# Load image and run container (appsettings.json ya va dentro de la imagen).
param(
    [string]$TarFile = "sersalud019.tar",
    [string]$Tag = "sersalud:19.00",
    [string]$ContainerName = "sersalud",
    [int]$Port = 8080,
    [string]$AppSettingsPath = ""
)

$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$DeployDir = Join-Path $RepoRoot "server-deploy\docker"
if (-not (Test-Path $DeployDir)) {
    $DeployDir = $PSScriptRoot
}

$TarPath = if ([System.IO.Path]::IsPathRooted($TarFile)) { $TarFile } else { Join-Path $DeployDir $TarFile }

if (-not (Test-Path $TarPath)) {
    Write-Host "ERROR: no existe $TarPath" -ForegroundColor Red
    Write-Host "Ejecute primero: cd sistema\scripts; .\docker-export.ps1" -ForegroundColor Yellow
    exit 1
}

Write-Host "Cargando imagen desde $TarPath ..." -ForegroundColor Cyan
docker load -i $TarPath
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

docker rm -f $ContainerName 2>$null | Out-Null

$runArgs = @(
    "run", "-d",
    "--name", $ContainerName,
    "-p", "${Port}:80",
    "--restart", "unless-stopped"
)

if ($AppSettingsPath -and (Test-Path $AppSettingsPath)) {
    $resolved = (Resolve-Path $AppSettingsPath).Path
    $runArgs += @("-v", "${resolved}:/app/appsettings.json:ro")
    Write-Host "Montando appsettings externo: $resolved" -ForegroundColor Yellow
}

$runArgs += $Tag

Write-Host "Iniciando contenedor en http://localhost:$Port ..." -ForegroundColor Cyan
docker @runArgs
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host ""
Write-Host "Listo. Logs: docker logs -f $ContainerName" -ForegroundColor Green
