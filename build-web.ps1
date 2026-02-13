# Script para limpiar procesos y compilar
# Ejecuta desde PowerShell en Windows

Write-Host "Deteniendo procesos de dotnet..." -ForegroundColor Yellow
Get-Process | Where-Object {$_.ProcessName -like "*dotnet*"} | Stop-Process -Force -ErrorAction SilentlyContinue

Start-Sleep -Seconds 2

Write-Host "Compilando proyecto..." -ForegroundColor Yellow
cd C:\Users\CarlosMasterPC1\Documents\PSInventory
dotnet build PSInventory.Web\PSInventory.Web.csproj

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Compilación exitosa" -ForegroundColor Green
} else {
    Write-Host "✗ Error en compilación" -ForegroundColor Red
}
