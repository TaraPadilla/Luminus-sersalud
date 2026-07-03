# Verifica que el servidor SerSalud tenga la configuracion correcta (sin tocar appsettings).
param(
    [string]$BaseUrl = "https://sersaludgt.com"
)

$ErrorActionPreference = "Continue"
Write-Host "=== Verificacion SerSalud: $BaseUrl ===" -ForegroundColor Cyan

$checks = @()

try {
    $login = Invoke-WebRequest -Uri "$BaseUrl/Identity/Account/Login" -UseBasicParsing -TimeoutSec 30
    $checks += [pscustomobject]@{ Prueba = "Login HTTP"; Resultado = $login.StatusCode }
} catch {
    $checks += [pscustomobject]@{ Prueba = "Login HTTP"; Resultado = "FAIL: $($_.Exception.Message)" }
}

Write-Host ""
Write-Host "Pruebas automaticas (sin credenciales):" -ForegroundColor Yellow
$checks | Format-Table -AutoSize

Write-Host ""
Write-Host "Verifique MANUALMENTE en el servidor:" -ForegroundColor Yellow
Write-Host "  1. appsettings.json tiene ""Cliente"": ""SS"" (no AVM ni M14)"
Write-Host "  2. ASPNETCORE_ENVIRONMENT=Production"
Write-Host "  3. Solo existe appsettings.json (sin appsettings.Development.json)"
Write-Host "  4. Log al iniciar: Cliente configurado: SS"
Write-Host "  5. Linux: wkhtmltopdf instalado (which wkhtmltopdf)"
Write-Host "  6. Windows: publish incluye Rotativa/Windows/wkhtmltopdf.exe"
Write-Host "  7. PDF: $BaseUrl/CrearPDF/ExpedienteCompletoPDF?hospitalizacionId=ID"
Write-Host "  8. Calendario: desbloquear fechas con motivo erroneo (ej. 123)"
Write-Host ""
