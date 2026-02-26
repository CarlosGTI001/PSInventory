# 🔧 Fix: Error al Crear Sucursal - ID Null

## ❌ Problema

Al intentar crear una nueva Sucursal, aparecía el siguiente error:

```
InvalidOperationException: Unable to track an entity of type 'Sucursal' 
because its primary key property 'Id' is null.
```

### Causa Raíz

A diferencia de otros modelos que usan `int` como Id (auto-generado), el modelo **Sucursal** usa `string` como Id, que **debe ser proporcionado manualmente** por el usuario.

```csharp
public class Sucursal
{
    [Key]
    [StringLength(50)]
    public string Id { get; set; }  // ⚠️ String, no auto-generado
    
    // ... otros campos
}
```

El problema tenía dos partes:

1. **Controlador**: El `[Bind]` no incluía el campo `Id`
2. **Vista**: Faltaba el campo de entrada para el `Id`

---

## ✅ Solución Aplicada

### 1. Actualización del Controlador

**Archivo**: `PSInventory.Web/Controllers/SucursalesController.cs`

**Antes**:
```csharp
public async Task<IActionResult> Create(
    [Bind("Nombre,Direccion,Telefono,RegionId")] Sucursal sucursal)
{
    if (ModelState.IsValid)
    {
        _context.Add(sucursal);  // ❌ Id es null aquí
        await _context.SaveChangesAsync();
        // ...
    }
}
```

**Después**:
```csharp
public async Task<IActionResult> Create(
    [Bind("Id,Nombre,Direccion,Telefono,RegionId,Activo")] Sucursal sucursal)
{
    if (ModelState.IsValid)
    {
        // Validar que el Id no esté vacío
        if (string.IsNullOrWhiteSpace(sucursal.Id))
        {
            ModelState.AddModelError("Id", "El código de la sucursal es obligatorio");
            ViewBag.Regiones = await _context.Regiones
                .Where(r => !r.Eliminado)
                .OrderBy(r => r.Nombre)
                .ToListAsync();
            return View(sucursal);
        }
        
        // Validar que el Id sea único
        var existe = await _context.Sucursales.AnyAsync(s => s.Id == sucursal.Id);
        if (existe)
        {
            ModelState.AddModelError("Id", "Ya existe una sucursal con este código");
            ViewBag.Regiones = await _context.Regiones
                .Where(r => !r.Eliminado)
                .OrderBy(r => r.Nombre)
                .ToListAsync();
            return View(sucursal);
        }
        
        _context.Add(sucursal);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Sucursal creada exitosamente";
        return RedirectToAction(nameof(Index));
    }
    // ...
}
```

**Cambios**:
- ✅ Agregado `Id` al `[Bind]`
- ✅ Agregado `Activo` al `[Bind]`
- ✅ Validación: Id no puede estar vacío
- ✅ Validación: Id debe ser único
- ✅ Mensajes de error claros

---

### 2. Actualización de la Vista

**Archivo**: `PSInventory.Web/Views/Sucursales/Create.cshtml`

**Agregado** el campo Id como primer campo del formulario:

```html
<div class="form-field">
    <md-filled-text-field 
        label="Código/ID *" 
        name="Id" 
        required
        value="@Model?.Id"
        supporting-text="Código único de la sucursal (ej: SUC001, CDMX01)"
        pattern="[A-Za-z0-9]{1,50}"
        maxlength="50">
        <md-icon slot="leading-icon">tag</md-icon>
    </md-filled-text-field>
    <span asp-validation-for="Id" class="field-error"></span>
</div>
```

**Características del campo**:
- ✅ Campo obligatorio (`required`)
- ✅ Máximo 50 caracteres (`maxlength="50"`)
- ✅ Pattern: Solo letras y números (`pattern="[A-Za-z0-9]{1,50}"`)
- ✅ Ícono de etiqueta/tag
- ✅ Texto de ayuda con ejemplos
- ✅ Validación de errores

---

## 🧪 Cómo Probar

### 1. Reinicia la Aplicación
```bash
# Detén la aplicación (Ctrl + C)
# Inicia nuevamente
cd PSInventory.Web
dotnet run
```

### 2. Crea una Nueva Sucursal

1. Ve a **Administración → Sucursales**
2. Haz clic en **Nueva Sucursal**
3. **Verás** ahora el campo **Código/ID** como primer campo
4. Ingresa los datos:
   - **Código/ID**: `SUC001` (o cualquier código único)
   - **Nombre**: `Sucursal Centro`
   - **Región**: Selecciona una
   - **Dirección**: `Av. Principal 123`
   - **Teléfono**: `5551234567` (opcional)
   - **Activo**: ✅ (checked por defecto)

5. Haz clic en **Guardar**

### 3. Verifica que Funcione

✅ **Resultado esperado**:
- La sucursal se crea exitosamente
- Redirige a la lista de sucursales
- Muestra mensaje: "Sucursal creada exitosamente"
- La nueva sucursal aparece en la lista

❌ **Si el código ya existe**:
- Muestra error: "Ya existe una sucursal con este código"
- No se crea la sucursal
- Puedes cambiar el código e intentar de nuevo

❌ **Si dejas el código vacío**:
- Muestra error: "El código de la sucursal es obligatorio"
- No se crea la sucursal

---

## 📋 Validaciones Implementadas

| Validación | Descripción | Mensaje de Error |
|------------|-------------|------------------|
| **Required** | El Id no puede estar vacío | "El código de la sucursal es obligatorio" |
| **Unique** | El Id debe ser único en la BD | "Ya existe una sucursal con este código" |
| **Pattern** | Solo letras y números | Validación del navegador |
| **MaxLength** | Máximo 50 caracteres | Validación del navegador |

---

## 🎯 Por Qué Sucursal Usa String como Id

### Ventajas del String Id

1. **Códigos Legibles**: `SUC001`, `CDMX01`, `MTY01`
2. **Significado de Negocio**: El código puede tener significado
3. **Compatibilidad**: Facilita integración con otros sistemas
4. **Auditoría**: Más fácil de rastrear en logs

### Comparación

```csharp
// Otros modelos (int auto-generado)
public class Categoria
{
    public int Id { get; set; }  // ✅ Auto-generado: 1, 2, 3, ...
}

// Sucursal (string manual)
public class Sucursal
{
    public string Id { get; set; }  // ⚠️ Manual: "SUC001", "CDMX01"
}
```

---

## 🔍 Otros Modelos con String Id

Si hay otros modelos con `string Id`, verifica que:

1. ✅ El campo `Id` esté en el `[Bind]` del controlador
2. ✅ Exista un campo de entrada en la vista Create
3. ✅ Se valide que no esté vacío
4. ✅ Se valide que sea único

**Ejemplo de búsqueda**:
```bash
# Buscar modelos con string Id
grep -r "public string Id" PSData/Modelos/*.cs
```

---

## ✅ Checklist de Validación

Después de aplicar el fix:

- [ ] La aplicación compila sin errores
- [ ] Puedes acceder a Sucursales → Nueva
- [ ] Ves el campo "Código/ID" como primer campo
- [ ] Puedes crear una sucursal con un código único
- [ ] Si intentas crear con código duplicado, muestra error
- [ ] Si dejas el código vacío, muestra error
- [ ] La sucursal se crea correctamente en la base de datos
- [ ] Aparece en la lista de sucursales

---

## 📚 Archivos Modificados

| Archivo | Cambios |
|---------|---------|
| `Controllers/SucursalesController.cs` | Agregado `Id` y `Activo` al Bind, validaciones |
| `Views/Sucursales/Create.cshtml` | Agregado campo Id con validaciones |

---

## 🎓 Lecciones Aprendidas

1. **String Ids requieren atención especial** - No se auto-generan
2. **Siempre validar unicidad** - Para evitar claves duplicadas
3. **UX clara** - Indicar formato esperado (ej: SUC001)
4. **Validaciones en backend y frontend** - Doble capa de seguridad

---

## 🚀 Estado Actual

✅ **Problema Resuelto**

Ahora puedes crear sucursales sin problemas. El campo Id es:
- Visible y accesible en el formulario
- Validado (no vacío, único, formato correcto)
- Guardado correctamente en la base de datos

**¡Problema solucionado!** 🎉
