# Build Docker image from repo root (same layout as server deploy).
$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$Tag = "sersalud:19.00"

Push-Location $RepoRoot
try {
    Write-Host "Building $Tag from $RepoRoot ..." -ForegroundColor Cyan
    docker build -t $Tag .
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    Write-Host ""
    Write-Host "Image ready: $Tag" -ForegroundColor Green
    Write-Host "Export for server:" -ForegroundColor Cyan
    Write-Host "  docker save $Tag -o sersalud019.tar"
    Write-Host ""
    Write-Host "Run locally (appsettings.json ya va en la imagen):" -ForegroundColor Cyan
    Write-Host "  docker run -d -p 8080:80 $Tag"
    Write-Host "  Opcional: -v `"`$PWD/sistema/appsettings.json:/app/appsettings.json:ro`" para sobreescribir"
}
finally {
    Pop-Location
}
