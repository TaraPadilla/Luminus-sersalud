# Publicar sin sobrescribir appsettings del servidor
$ErrorActionPreference = "Stop"

$ProjectDir = Split-Path $PSScriptRoot -Parent
$PublishDir = Join-Path $ProjectDir "publish-sersalud"
$EnsurePdf = Join-Path $PSScriptRoot "ensure-wkhtmltopdf.ps1"

& $EnsurePdf
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Deteniendo farmamest local..." -ForegroundColor Yellow
Get-Process -Name "farmamest" -ErrorAction SilentlyContinue | Stop-Process -Force

Write-Host "Publicando en $PublishDir ..." -ForegroundColor Cyan
if (Test-Path $PublishDir) {
    Remove-Item $PublishDir -Recurse -Force
}

Push-Location $ProjectDir
dotnet publish -c Release -o $PublishDir
if ($LASTEXITCODE -ne 0) { Pop-Location; exit $LASTEXITCODE }
Pop-Location

# wwwroot/Rotativa (PDFs en Windows) + copia a raiz publish (motor Wkhtmltopdf.NetCore)
$rotativaPubWww = Join-Path $PublishDir "wwwroot\Rotativa\Windows\wkhtmltopdf.exe"
$rotativaPubEngine = Join-Path $PublishDir "Rotativa\Windows\wkhtmltopdf.exe"
if (-not (Test-Path $rotativaPubWww)) {
    $src = Join-Path $ProjectDir "wwwroot\Rotativa"
    if (-not (Test-Path $src)) {
        $src = Join-Path $ProjectDir "Rotativa"
    }
    if (Test-Path $src) {
        Copy-Item $src (Join-Path $PublishDir "wwwroot\Rotativa") -Recurse -Force
        Write-Host "Copiado Rotativa al paquete publish (wwwroot)." -ForegroundColor Yellow
    }
}
if ((Test-Path $rotativaPubWww) -and -not (Test-Path $rotativaPubEngine)) {
    Copy-Item (Join-Path $PublishDir "wwwroot\Rotativa") (Join-Path $PublishDir "Rotativa") -Recurse -Force
    Write-Host "Copiado Rotativa a raiz publish (motor PDF)." -ForegroundColor Yellow
}

$dllPath = Join-Path $PublishDir "farmamest.dll"
if (-not (Test-Path $dllPath)) {
    Write-Host "ERROR: publish incompleto - falta farmamest.dll en $PublishDir" -ForegroundColor Red
    exit 1
}
if (-not (Test-Path $rotativaPubEngine) -and -not (Test-Path $rotativaPubWww)) {
    Write-Host "ADVERTENCIA: falta wkhtmltopdf.exe - PDFs fallaran en Windows." -ForegroundColor Yellow
    Write-Host "  Coloque wkhtmltopdf.exe en sistema\wwwroot\Rotativa\Windows\ y vuelva a publicar." -ForegroundColor Yellow
}

# appsettings.json es obligatorio en Production (/app/appsettings.json). El target EnsureAppSettingsInPublish
# ya lo incluye; aqui solo quitamos capas de desarrollo y duplicados.
$appSettingsPublish = Join-Path $PublishDir "appsettings.json"
if (-not (Test-Path $appSettingsPublish)) {
    $estructuraSrc = Join-Path $ProjectDir "appsettings.ESTRUCTURA-SERVIDOR.json"
    if (-not (Test-Path $estructuraSrc)) {
        Write-Host "ERROR: falta appsettings.json en publish y no hay appsettings.ESTRUCTURA-SERVIDOR.json" -ForegroundColor Red
        exit 1
    }
    Copy-Item $estructuraSrc $appSettingsPublish -Force
    Write-Host "Incluido appsettings.json desde plantilla ESTRUCTURA-SERVIDOR" -ForegroundColor Yellow
}
else {
    Write-Host "appsettings.json incluido en el paquete publish" -ForegroundColor Green
}

Get-ChildItem $PublishDir -Filter "appsettings*.json" -ErrorAction SilentlyContinue | ForEach-Object {
    if ($_.Name -eq "appsettings.json") { return }
    if ($_.Name -eq "appsettings.ESTRUCTURA-SERVIDOR.json") { return }
    Remove-Item $_.FullName -Force
    Write-Host "Eliminado del paquete: $($_.Name)" -ForegroundColor Yellow
}

$nestedDeploy = Join-Path $PublishDir "server-deploy"
if (Test-Path $nestedDeploy) {
    Remove-Item $nestedDeploy -Recurse -Force
    Write-Host "Eliminado del paquete: server-deploy/" -ForegroundColor Yellow
}

$testServidor = Join-Path $PublishDir "test-servidor"
if (Test-Path $testServidor) {
    Remove-Item $testServidor -Recurse -Force
    Write-Host "Eliminado del paquete: test-servidor/" -ForegroundColor Yellow
}

$demoUploads = Join-Path $PublishDir "wwwroot\uploads\expediente"
if (Test-Path $demoUploads) {
    Get-ChildItem $demoUploads -File -ErrorAction SilentlyContinue | Remove-Item -Force
    Write-Host "Eliminados PDFs demo de wwwroot/uploads/expediente" -ForegroundColor Yellow
}

# Guia en la carpeta publish
$leeme = Join-Path (Split-Path $ProjectDir -Parent) "LEEME.md"
if (Test-Path $leeme) {
    Copy-Item $leeme (Join-Path $PublishDir "LEEME.md") -Force
}
$entrega = Join-Path (Split-Path $ProjectDir -Parent) "ENTREGA-CLIENTE.md"
if (Test-Path $entrega) {
    Copy-Item $entrega (Join-Path $PublishDir "ENTREGA-CLIENTE.md") -Force
}
$requisitos = Join-Path (Split-Path $ProjectDir -Parent) "REQUISITOS-CLIENTE.md"
if (Test-Path $requisitos) {
    Copy-Item $requisitos (Join-Path $PublishDir "REQUISITOS-CLIENTE.md") -Force
}
$verify = Join-Path $PSScriptRoot "verify-sersalud-server.ps1"
if (Test-Path $verify) {
    Copy-Item $verify (Join-Path $PublishDir "verify-sersalud-server.ps1") -Force
}
$estructura = Join-Path $ProjectDir "appsettings.ESTRUCTURA-SERVIDOR.json"
if (Test-Path $estructura) {
    Copy-Item $estructura (Join-Path $PublishDir "appsettings.ESTRUCTURA-SERVIDOR.json") -Force
}

Write-Host ""
Write-Host "=== LISTO PARA SUBIR AL SERVIDOR ===" -ForegroundColor Green
Write-Host "  Subir todo publish-sersalud (incluye appsettings.json)"
Write-Host "  Si el servidor ya tiene appsettings.json configurado, conserve el del servidor o fusione claves"
Write-Host "  Verificar log al iniciar: Cliente=SS en sersaludgt.com (Cliente=AVM solo en servidores AVM)"
Write-Host "  Verificacion: .\scripts\verify-sersalud-server.ps1"
Write-Host "  Linux: sudo apt-get install -y wkhtmltopdf"
Write-Host "  Guia: LEEME.md (seccion publicar para el cliente)"
Write-Host ""
Write-Host "Listo: $PublishDir" -ForegroundColor Green
