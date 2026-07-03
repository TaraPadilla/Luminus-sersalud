# Elimina artefactos de build y carpetas duplicadas (no codigo fuente).
$ErrorActionPreference = "Stop"

$ProjectDir = Split-Path $PSScriptRoot -Parent
$RepoRoot = Split-Path $ProjectDir -Parent

Write-Host "Deteniendo farmamest..." -ForegroundColor Yellow
Get-Process -Name "farmamest" -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Milliseconds 500

$toRemove = @(
    (Join-Path $ProjectDir "bin"),
    (Join-Path $ProjectDir "obj"),
    (Join-Path $ProjectDir "test-servidor"),
    (Join-Path $ProjectDir "publish-sersalud"),
    (Join-Path $ProjectDir "server-deploy"),
    (Join-Path $RepoRoot "Database.Shared\bin"),
    (Join-Path $RepoRoot "Database.Shared\obj")
)

foreach ($path in $toRemove) {
    if (Test-Path $path) {
        Remove-Item $path -Recurse -Force
        Write-Host "Eliminado: $path" -ForegroundColor Green
    }
}

$demoUploads = Join-Path $ProjectDir "wwwroot\uploads\expediente"
if (Test-Path $demoUploads) {
    Get-ChildItem $demoUploads -File -ErrorAction SilentlyContinue | Remove-Item -Force
    Write-Host "Limpiado: wwwroot/uploads/expediente (demo)" -ForegroundColor Green
}

$obsoleteScripts = @(
    (Join-Path $PSScriptRoot "fix-hosp-vm-json.ps1"),
    (Join-Path $PSScriptRoot "prepare-client-package.ps1")
)
foreach ($script in $obsoleteScripts) {
    if (Test-Path $script) {
        Remove-Item $script -Force
        Write-Host "Eliminado script obsoleto: $(Split-Path $script -Leaf)" -ForegroundColor DarkGray
    }
}

$total = (Get-ChildItem $RepoRoot -Recurse -File -ErrorAction SilentlyContinue | Measure-Object).Count
Write-Host ""
Write-Host "Limpieza completa. Archivos restantes en repo: $total" -ForegroundColor Cyan
Write-Host "Para ejecutar: cd sistema; dotnet run" -ForegroundColor Cyan
