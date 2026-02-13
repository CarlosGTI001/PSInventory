# Script para agregar columna RutaFactura a la base de datos
# Ejecutar desde PowerShell como Administrador

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "Actualización Base de Datos - RutaFactura" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

# Ruta del script SQL
$scriptPath = Join-Path $PSScriptRoot "add-rutafactura-column.sql"

# Verificar que existe el script
if (-not (Test-Path $scriptPath)) {
    Write-Host "ERROR: No se encontró el archivo add-rutafactura-column.sql" -ForegroundColor Red
    exit 1
}

# Configuración de conexión (ajustar si es necesario)
$serverInstance = "(localdb)\MSSQLLocalDB"
$database = "PSInventoryDB"

Write-Host "Conectando a: $serverInstance" -ForegroundColor Yellow
Write-Host "Base de datos: $database" -ForegroundColor Yellow
Write-Host ""

try {
    # Ejecutar script SQL
    Write-Host "Ejecutando script SQL..." -ForegroundColor Yellow
    
    Invoke-Sqlcmd -ServerInstance $serverInstance `
                  -Database $database `
                  -InputFile $scriptPath `
                  -ErrorAction Stop
    
    Write-Host ""
    Write-Host "✅ Columna RutaFactura agregada exitosamente" -ForegroundColor Green
    Write-Host ""
    Write-Host "Ahora puedes ejecutar la aplicación web sin errores." -ForegroundColor Green
    Write-Host ""
}
catch {
    Write-Host ""
    Write-Host "❌ ERROR al ejecutar el script:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Soluciones:" -ForegroundColor Yellow
    Write-Host "1. Verificar que SQL Server LocalDB esté corriendo" -ForegroundColor White
    Write-Host "2. Verificar el nombre de la instancia y base de datos" -ForegroundColor White
    Write-Host "3. Ejecutar el script manualmente desde SSMS:" -ForegroundColor White
    Write-Host "   $scriptPath" -ForegroundColor Cyan
    Write-Host ""
    exit 1
}

Write-Host "Presiona cualquier tecla para continuar..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
