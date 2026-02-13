# PSInventory.Web - Script de Setup Rápido
# Ejecuta este script desde PowerShell en Windows

Write-Host "==================================" -ForegroundColor Cyan
Write-Host " PSInventory.Web - Quick Setup" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host ""

# Verificar que estamos en la carpeta correcta
$projectPath = "C:\Users\CarlosMasterPC1\Documents\PSInventory"
if (!(Test-Path $projectPath)) {
    Write-Host "ERROR: No se encontró el proyecto en $projectPath" -ForegroundColor Red
    exit 1
}

Set-Location $projectPath\PSInventory.Web

Write-Host "[1/2] Aplicando migración a la base de datos..." -ForegroundColor Yellow
dotnet ef database update --project ..\PSData\PSData.csproj --context PSDatos
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Falló la migración" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Base de datos creada exitosamente" -ForegroundColor Green
Write-Host ""

Write-Host "[2/2] Iniciando aplicación..." -ForegroundColor Yellow
Write-Host ""
Write-Host "==================================" -ForegroundColor Cyan
Write-Host " La aplicación creará automáticamente:" -ForegroundColor Cyan
Write-Host "==================================" -ForegroundColor Cyan
Write-Host "  • Usuario: admin" -ForegroundColor Green
Write-Host "  • Contraseña: admin123" -ForegroundColor Green
Write-Host "  • Categorías de ejemplo" -ForegroundColor Green
Write-Host "  • Regiones de ejemplo" -ForegroundColor Green
Write-Host ""
Write-Host "Accede en: https://localhost:5001" -ForegroundColor Cyan
Write-Host ""
Write-Host "Presiona Ctrl+C para detener el servidor" -ForegroundColor Yellow
Write-Host ""

# Ejecutar la aplicación
dotnet run

