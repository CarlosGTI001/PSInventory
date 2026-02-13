# Script alternativo usando EF Core Migrations
# Ejecutar desde PowerShell en la carpeta PSData

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "Crear Migración EF Core - RutaFactura" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

$psdataPath = Join-Path $PSScriptRoot "PSData"

if (-not (Test-Path $psdataPath)) {
    Write-Host "ERROR: No se encontró la carpeta PSData" -ForegroundColor Red
    exit 1
}

Set-Location $psdataPath
Write-Host "Ubicación: $psdataPath" -ForegroundColor Yellow
Write-Host ""

try {
    Write-Host "Paso 1: Crear migración..." -ForegroundColor Yellow
    dotnet ef migrations add AddRutaFacturaToCompras --verbose
    
    Write-Host ""
    Write-Host "Paso 2: Aplicar migración a la base de datos..." -ForegroundColor Yellow
    dotnet ef database update --verbose
    
    Write-Host ""
    Write-Host "✅ Migración completada exitosamente" -ForegroundColor Green
    Write-Host ""
}
catch {
    Write-Host ""
    Write-Host "❌ ERROR:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Si el error es 'dotnet ef no se reconoce', instalar con:" -ForegroundColor Yellow
    Write-Host "dotnet tool install --global dotnet-ef" -ForegroundColor Cyan
    Write-Host ""
    exit 1
}

Set-Location $PSScriptRoot
Write-Host "Presiona cualquier tecla para continuar..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
