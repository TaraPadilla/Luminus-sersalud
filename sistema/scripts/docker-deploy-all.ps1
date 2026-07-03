# Build, export, run locally, and optionally deploy to Ubuntu server via SSH.
param(
    [string]$Tag = "sersalud:19.00",
    [string]$TarFile = "sersalud019.tar",
    [string]$ServerHost = "",
    [string]$KeyPath = "",
    [string]$ServerUser = "ubuntu",
    [int]$LocalPort = 8080,
    [int]$ServerPort = 80,
    [switch]$SkipPublish,
    [switch]$SkipLocal,
    [switch]$RemoteOnly
)

$ErrorActionPreference = "Stop"
$ScriptDir = $PSScriptRoot
$RepoRoot = Split-Path (Split-Path $ScriptDir -Parent) -Parent
$DeployDir = Join-Path $RepoRoot "server-deploy\docker"
$TarPath = Join-Path $DeployDir $TarFile
$RunSh = Join-Path $DeployDir "docker-run.sh"

function Invoke-LocalDeploy {
    if ($SkipLocal) { return }
    Write-Host "`n=== LOCAL: docker run ===" -ForegroundColor Cyan
    & (Join-Path $ScriptDir "docker-run.ps1") -TarFile $TarPath -Port $LocalPort -Tag $Tag
    Start-Sleep -Seconds 15
    $logs = docker logs sersalud 2>&1 | Out-String
    if ($logs -notmatch "Application started") {
        Write-Host "ADVERTENCIA: revise logs: docker logs sersalud" -ForegroundColor Yellow
    }
    else {
        Write-Host "OK: Application started (http://localhost:$LocalPort)" -ForegroundColor Green
    }
    docker exec sersalud test -x /app/Rotativa/Linux/wkhtmltopdf 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "OK: PDF wrapper /app/Rotativa/Linux/wkhtmltopdf" -ForegroundColor Green
    }
}

function Invoke-RemoteDeploy {
    if (-not $ServerHost) {
        Write-Host "`nOmitido deploy remoto (use -ServerHost IP -KeyPath C:\path\to\key.pem)" -ForegroundColor DarkGray
        return
    }
    if (-not (Test-Path $KeyPath)) {
        Write-Host "ERROR: no existe KeyPath: $KeyPath" -ForegroundColor Red
        exit 1
    }
    if (-not (Test-Path $TarPath)) {
        Write-Host "ERROR: no existe $TarPath" -ForegroundColor Red
        exit 1
    }

    $ssh = "ssh -i `"$KeyPath`" -o StrictHostKeyChecking=accept-new $ServerUser@$ServerHost"
    $scp = "scp -i `"$KeyPath`" -o StrictHostKeyChecking=accept-new"

    Write-Host "`n=== REMOTO: subiendo a $ServerUser@$ServerHost ===" -ForegroundColor Cyan
    Invoke-Expression "$scp `"$TarPath`" ${ServerUser}@${ServerHost}:~/sersalud019.tar"
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    Invoke-Expression "$scp `"$RunSh`" ${ServerUser}@${ServerHost}:~/docker-run.sh"
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    $remote = @"
set -e
chmod +x ~/docker-run.sh
sudo docker rm -f sersalud 2>/dev/null || true
sudo docker load -i ~/sersalud019.tar
sudo docker run -d --name sersalud -p ${ServerPort}:80 --restart unless-stopped $Tag
sleep 8
sudo docker ps --filter name=sersalud
sudo docker logs sersalud --tail 15
"@

    Write-Host "Iniciando contenedor en puerto $ServerPort ..." -ForegroundColor Cyan
    $remote | & ssh -i $KeyPath -o StrictHostKeyChecking=accept-new "$ServerUser@$ServerHost" "bash -s"
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    Write-Host "`nOK: deploy remoto completado en $ServerHost" -ForegroundColor Green
}

if (-not $RemoteOnly) {
    $exportArgs = @()
    if ($SkipPublish) { $exportArgs += "-SkipPublish" }
    & (Join-Path $ScriptDir "docker-export.ps1") -Tag $Tag -OutFile $TarFile @exportArgs
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    Invoke-LocalDeploy
}

Invoke-RemoteDeploy

Write-Host "`n=== FIN ===" -ForegroundColor Green
Write-Host "Paquete: $TarPath"
