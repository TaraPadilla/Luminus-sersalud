# Publish on host (includes real appsettings.json) + build runtime Docker image + export .tar
param(
    [string]$Tag = "sersalud:19.00",
    [string]$OutFile = "sersalud019.tar",
    [switch]$SkipPublish,
    [switch]$UseFullDockerfile
)

$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$SistemaDir = Join-Path $RepoRoot "sistema"
$PublishDir = Join-Path $RepoRoot "publish"
$DeployDir = Join-Path $RepoRoot "server-deploy\docker"
$OutPath = Join-Path $DeployDir $OutFile
$AppSettingsLocal = Join-Path $SistemaDir "appsettings.json"

function Test-PlaceholderAppSettings {
    param([string]$Path)
    if (-not (Test-Path $Path)) { return $true }
    $json = Get-Content $Path -Raw
    return $json -match 'Host=\.\.\.' -or $json -match 'Server=\.\.\.'
}

function Copy-DeployScripts {
    New-Item -ItemType Directory -Force -Path $DeployDir | Out-Null
    New-Item -ItemType Directory -Force -Path (Join-Path $DeployDir "config") | Out-Null
    foreach ($script in @("docker-run.ps1", "docker-run.sh")) {
        $src = Join-Path $PSScriptRoot $script
        if (Test-Path $src) {
            Copy-Item $src (Join-Path $DeployDir $script) -Force
        }
    }
    $leeme = Join-Path $RepoRoot "server-deploy\docker\LEEME-SERVIDOR.md"
    $leemeDest = Join-Path $DeployDir "LEEME-SERVIDOR.md"
    if ((Test-Path $leeme) -and ($leeme -ne $leemeDest)) {
        Copy-Item $leeme $leemeDest -Force
    }
    $example = Join-Path $SistemaDir "appsettings.ESTRUCTURA-SERVIDOR.json"
    if (Test-Path $example) {
        Copy-Item $example (Join-Path $DeployDir "config\appsettings.ESTRUCTURA-SERVIDOR.json") -Force
    }
}

if (-not (Test-Path $AppSettingsLocal)) {
    Write-Host "ERROR: falta $AppSettingsLocal (cadena PostgreSQL real requerida)." -ForegroundColor Red
    exit 1
}
if (Test-PlaceholderAppSettings $AppSettingsLocal) {
    Write-Host "ERROR: appsettings.json contiene Host=... (plantilla). Use credenciales reales del servidor." -ForegroundColor Red
    exit 1
}

if (-not $SkipPublish) {
    Write-Host "Publicando en $PublishDir ..." -ForegroundColor Cyan
    if (Test-Path $PublishDir) { Remove-Item $PublishDir -Recurse -Force }
    Push-Location $SistemaDir
    dotnet publish farmamest.csproj -c Release -o $PublishDir
    if ($LASTEXITCODE -ne 0) { Pop-Location; exit $LASTEXITCODE }
    Pop-Location
}

$pubSettings = Join-Path $PublishDir "appsettings.json"
if (-not (Test-Path $pubSettings)) {
    Write-Host "ERROR: publish sin appsettings.json" -ForegroundColor Red
    exit 1
}
if (Test-PlaceholderAppSettings $pubSettings) {
    Write-Host "ERROR: publish contiene appsettings plantilla. Verifique EnsureAppSettingsInPublish." -ForegroundColor Red
    exit 1
}

Write-Host "appsettings.json OK -> $(Select-String -Path $pubSettings -Pattern 'Server=|Host=' | Select-Object -First 1 | ForEach-Object { $_.Line.Trim().Substring(0, [Math]::Min(80, $_.Line.Trim().Length)) })" -ForegroundColor Green

Copy-DeployScripts

Push-Location $RepoRoot
try {
    if ($UseFullDockerfile) {
        Write-Host "Building $Tag (Dockerfile multi-stage) ..." -ForegroundColor Cyan
        docker build -t $Tag .
    }
    else {
        Write-Host "Building $Tag (Dockerfile.publish, sin restore en contenedor) ..." -ForegroundColor Cyan
        docker build -f Dockerfile.publish -t $Tag .
    }
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    Write-Host "Exportando $OutPath ..." -ForegroundColor Cyan
    docker save $Tag -o $OutPath
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally {
    Pop-Location
}

Write-Host ""
Write-Host "=== PAQUETE LISTO ===" -ForegroundColor Green
Write-Host "  $OutPath"
Write-Host "  server-deploy\docker\docker-run.ps1"
Write-Host ""
Write-Host "En el servidor Ubuntu (bash):" -ForegroundColor Cyan
Write-Host "  scp server-deploy/docker/sersalud019.tar ubuntu@SERVIDOR:~/"
Write-Host "  scp server-deploy/docker/docker-run.sh ubuntu@SERVIDOR:~/"
Write-Host "  ssh ubuntu@SERVIDOR"
Write-Host "  chmod +x docker-run.sh && ./docker-run.sh"
Write-Host ""
Write-Host "  # o manual:"
Write-Host "  sudo docker load -i sersalud019.tar"
Write-Host "  sudo docker run -d --name sersalud -p 80:80 --restart unless-stopped sersalud:19.00"
