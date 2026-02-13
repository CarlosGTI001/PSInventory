# Implementación Backend - Soporte de Facturas Escaneadas

## 1. Actualizar Modelo Compra

```csharp
// Models/Compra.cs
public class Compra
{
    public int Id { get; set; }
    public DateTime FechaCompra { get; set; }
    public string Proveedor { get; set; }
    public decimal CostoTotal { get; set; }
    public string Estado { get; set; } // "Pendiente", "Completada", "Cancelada"
    public string NumeroFactura { get; set; }
    public string RutaFactura { get; set; }  // ✅ NUEVA PROPIEDAD
    public string Observaciones { get; set; }
    
    // Navigation property
    public ICollection<CompraItem> Items { get; set; }
}
```

---

## 2. Actualizar ComprasController

### 2.1 Acción Edit (GET)
```csharp
[HttpGet]
public async Task<IActionResult> Edit(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var compra = await _context.Compras
        .Include(c => c.Items)
        .FirstOrDefaultAsync(m => m.Id == id);
        
    if (compra == null)
    {
        return NotFound();
    }

    return View(compra);
}
```

### 2.2 Acción Edit (POST) - Con soporte de archivos
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, [Bind("Id,FechaCompra,Proveedor,CostoTotal,Estado,NumeroFactura,Observaciones,RutaFactura")] Compra compra, IFormFile facturaFile)
{
    if (id != compra.Id)
    {
        return NotFound();
    }

    if (ModelState.IsValid)
    {
        try
        {
            // Procesar archivo si se seleccionó uno nuevo
            if (facturaFile != null && facturaFile.Length > 0)
            {
                // Validar extensión
                var extensionesPermitidas = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(facturaFile.FileName).ToLower();
                
                if (!extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError("facturaFile", "Solo se permiten archivos PDF, JPG, JPEG o PNG");
                    return View(compra);
                }

                // Validar tamaño (máximo 5MB)
                if (facturaFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("facturaFile", "El archivo no puede exceder 5MB");
                    return View(compra);
                }

                // Eliminar archivo anterior si existe
                if (!string.IsNullOrEmpty(compra.RutaFactura))
                {
                    var rutaAnterior = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", compra.RutaFactura.TrimStart('/'));
                    if (System.IO.File.Exists(rutaAnterior))
                    {
                        System.IO.File.Delete(rutaAnterior);
                    }
                }

                // Crear carpeta si no existe
                var carpetaFacturas = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "facturas");
                if (!Directory.Exists(carpetaFacturas))
                {
                    Directory.CreateDirectory(carpetaFacturas);
                }

                // Generar nombre único
                var nombreUnico = $"{compra.Id}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString()[..8]}{extension}";
                var rutaArchivo = Path.Combine(carpetaFacturas, nombreUnico);

                // Guardar archivo
                using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                {
                    await facturaFile.CopyToAsync(stream);
                }

                // Guardar ruta relativa
                compra.RutaFactura = $"/facturas/{nombreUnico}";
            }

            _context.Update(compra);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Compra actualizada exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CompraExists(compra.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
    }

    return View(compra);
}
```

### 2.3 Acción Create (POST) - Con soporte de archivos
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("FechaCompra,Proveedor,CostoTotal,Estado,NumeroFactura,Observaciones")] Compra compra, IFormFile facturaFile)
{
    if (ModelState.IsValid)
    {
        try
        {
            // Procesar archivo si se seleccionó uno
            if (facturaFile != null && facturaFile.Length > 0)
            {
                // Validar extensión
                var extensionesPermitidas = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(facturaFile.FileName).ToLower();
                
                if (!extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError("facturaFile", "Solo se permiten archivos PDF, JPG, JPEG o PNG");
                    return View(compra);
                }

                // Validar tamaño
                if (facturaFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("facturaFile", "El archivo no puede exceder 5MB");
                    return View(compra);
                }

                // Crear carpeta si no existe
                var carpetaFacturas = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "facturas");
                if (!Directory.Exists(carpetaFacturas))
                {
                    Directory.CreateDirectory(carpetaFacturas);
                }

                // Se guardará después de crear la compra (para usar su ID)
                // Por ahora solo pasamos el archivo
                _context.Add(compra);
                await _context.SaveChangesAsync();

                // Ahora sí guardamos el archivo con el ID de la compra
                var nombreUnico = $"{compra.Id}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString()[..8]}{extension}";
                var rutaArchivo = Path.Combine(carpetaFacturas, nombreUnico);

                using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                {
                    await facturaFile.CopyToAsync(stream);
                }

                compra.RutaFactura = $"/facturas/{nombreUnico}";
                _context.Update(compra);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Compra creada exitosamente con factura adjunta";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _context.Add(compra);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Compra creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error al guardar: {ex.Message}");
        }
    }

    return View(compra);
}
```

### 2.4 Acción DescargarFactura - NUEVA
```csharp
[HttpGet]
public async Task<IActionResult> DescargarFactura(int id)
{
    var compra = await _context.Compras.FindAsync(id);
    
    if (compra == null || string.IsNullOrEmpty(compra.RutaFactura))
    {
        return NotFound("Factura no encontrada");
    }

    var rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", compra.RutaFactura.TrimStart('/'));
    
    if (!System.IO.File.Exists(rutaArchivo))
    {
        return NotFound("Archivo no existe");
    }

    var stream = System.IO.File.OpenRead(rutaArchivo);
    var nombreArchivo = Path.GetFileName(rutaArchivo);
    var contentType = GetContentType(rutaArchivo);

    return File(stream, contentType, nombreArchivo);
}

private string GetContentType(string path)
{
    var extension = Path.GetExtension(path).ToLower();
    return extension switch
    {
        ".pdf" => "application/pdf",
        ".jpg" => "image/jpeg",
        ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        _ => "application/octet-stream"
    };
}
```

### 2.5 Acción Delete - Eliminar también el archivo
```csharp
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var compra = await _context.Compras.FindAsync(id);
    
    if (compra != null)
    {
        // Eliminar archivo si existe
        if (!string.IsNullOrEmpty(compra.RutaFactura))
        {
            var rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", compra.RutaFactura.TrimStart('/'));
            if (System.IO.File.Exists(rutaArchivo))
            {
                try
                {
                    System.IO.File.Delete(rutaArchivo);
                }
                catch (Exception ex)
                {
                    // Log error pero continúa con la eliminación
                    Console.WriteLine($"Error al eliminar archivo: {ex.Message}");
                }
            }
        }

        _context.Compras.Remove(compra);
        await _context.SaveChangesAsync();
    }

    return RedirectToAction(nameof(Index));
}
```

---

## 3. Crear la Carpeta de Facturas

En la raíz del proyecto, crear:
```
/wwwroot/facturas/
```

Opcionalmente, agregar un `.gitkeep` para que Git rastree la carpeta:
```bash
touch wwwroot/facturas/.gitkeep
```

---

## 4. Actualizar Startup/Program.cs (si es necesario)

Asegurar que el middleware para servir archivos estáticos esté configurado:

```csharp
// Program.cs (ASP.NET Core 6+)
var builder = WebApplicationBuilder.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure middleware
app.UseStaticFiles(); // ✅ IMPORTANTE para acceder a /wwwroot/facturas/
app.UseRouting();
app.MapControllers();

app.Run();
```

---

## 5. Considerar en DbContext

Asegúrate que el DbContext tiene la propiedad para Compras:

```csharp
public DbSet<Compra> Compras { get; set; }
```

---

## 6. Crear Migración (si usas EF Core)

```bash
# En la carpeta del proyecto
dotnet ef migrations add AddRutaFacturaToCompra
dotnet ef database update
```

Esto agregará la columna `RutaFactura` a la tabla `Compras`.

---

## 7. Validaciones Incluidas

✅ **Extensiones permitidas:** .pdf, .jpg, .jpeg, .png
✅ **Tamaño máximo:** 5MB
✅ **Nombres únicos:** Usa ID + timestamp + GUID para evitar conflictos
✅ **Gestión de archivos anteriores:** Elimina el archivo anterior al actualizar
✅ **Limpieza en eliminación:** Elimina el archivo cuando se elimina la compra

---

## 8. Estructura de Archivos Resultante

```
/wwwroot/
└── facturas/
    ├── 1_20240115141530_a7f2c9e1.pdf
    ├── 2_20240115141545_b8d3e4f2.jpg
    ├── 3_20240115141600_c9e4f5g3.png
    └── 4_20240115141615_d0f5g6h4.pdf
```

---

## 9. Pruebas Recomendadas

- [ ] Crear compra con factura
- [ ] Descargar factura desde Details
- [ ] Editar compra y cambiar factura
- [ ] Verificar que el archivo anterior se elimina
- [ ] Descargar factura desde Index
- [ ] Eliminar compra y verificar que el archivo se elimina
- [ ] Probar con diferentes tipos de archivo

---

## 10. Seguridad Adicional (Recomendada)

```csharp
// Proteger acceso a DescargarFactura (opcional)
[Authorize] // Requiere autenticación
[HttpGet]
public async Task<IActionResult> DescargarFactura(int id)
{
    // ... código igual ...
}
```

---

## Notas

- Los archivos se guardan con nombres únicos para evitar sobrescrituras
- Se utiliza la ruta relativa `/facturas/` para mejor portabilidad
- Los tipos MIME se detectan automáticamente según la extensión
- La carpeta de facturas puede ser configurada como variable de entorno en producción
