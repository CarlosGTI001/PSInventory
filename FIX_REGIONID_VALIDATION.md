# Fix: RegionId = 0 Error - Foreign Key Constraint

## 🔍 Problema Detectado

El error persiste porque cuando NO se selecciona ninguna región en el formulario:
1. El HTML envía `value=""` (string vacío)  
2. ASP.NET convierte `""` a `0` para el tipo `int`
3. La validación anterior solo verificaba si > 0, pero no agregaba error si era 0
4. Se intentaba guardar con `RegionId = 0` (que no existe en la BD)
5. **Resultado:** Foreign Key Constraint Error

## ✅ Solución Implementada

### **Validación Mejorada en SucursalesController.Create**

**Antes (❌ Incompleto):**
```csharp
if (sucursal.RegionId > 0)  // Solo valida si es > 0
{
    // Valida existencia
}
// Si es 0, no hace nada y deja pasar!
```

**Ahora (✅ Completo):**
```csharp
if (sucursal.RegionId <= 0)
{
    ModelState.AddModelError("RegionId", "Debe seleccionar una región");
}
else
{
    var regionExists = await _context.Regiones
        .AnyAsync(r => r.RegionId == sucursal.RegionId && !r.Eliminado);
    
    if (!regionExists)
    {
        ModelState.AddModelError("RegionId", "La región seleccionada no existe o está inactiva");
    }
}
```

**Cambio clave:** Ahora valida **AMBOS casos**:
1. ✅ Si `RegionId <= 0` → Error: "Debe seleccionar una región"  
2. ✅ Si `RegionId > 0` pero no existe en BD → Error: "No existe o está inactiva"

## 📋 IMPORTANTE: Verificar ANTES de Probar

### 1. **¿Existen regiones en la base de datos?**

Ejecuta en SQL Server Management Studio:

```sql
SELECT RegionId, Nombre, Activo, Eliminado 
FROM Regiones 
WHERE Eliminado = 0 AND Activo = 1;
```

**Si NO hay resultados (tabla vacía):**
1. Ve a la aplicación web → **Regiones → Nueva**
2. Crea al menos una región:
   - Nombre: "Ciudad de México"
   - Descripción: "Zona metropolitana CDMX"
   - ✅ Activo (checkbox marcado)
3. Guarda

### 2. **Reiniciar la aplicación**

Para que los cambios del controlador surtan efecto:
- Detén la aplicación (Ctrl+C)
- Compila: `dotnet build`
- Reinicia: `dotnet run --project PSInventory.Web`

## 🧪 Pruebas

### Test 1: Sin seleccionar región (debe fallar con validación)
1. Ve a **Sucursales → Nueva**
2. Ingresa:
   - Nombre: "Sucursal Prueba"
   - Dirección: "Calle Falsa 123"
   - Teléfono: "5551234567"
3. **NO** selecciones ninguna región (deja en "Seleccione una región")
4. Haz clic en **Crear**
5. **Resultado esperado:** ❌ Error: "Debe seleccionar una región" (NO debe llegar a la BD)

### Test 2: Con región válida (debe funcionar)
1. Ve a **Sucursales → Nueva**
2. Ingresa todos los datos
3. **Selecciona** una región del dropdown
4. Haz clic en **Crear**
5. **Resultado esperado:** ✅ "Sucursal creada exitosamente con código SUC-001"

## 🔧 Si el Error Persiste

### Opción 1: Agregar Logging Temporal

Modifica temporalmente el controlador:

```csharp
public async Task<IActionResult> Create([Bind("Nombre,Direccion,Telefono,RegionId,Activo")] Sucursal sucursal)
{
    // LOGGING TEMPORAL
    Console.WriteLine($"=== DEBUG CREATE SUCURSAL ===");
    Console.WriteLine($"RegionId recibido: {sucursal.RegionId}");
    Console.WriteLine($"Nombre: {sucursal.Nombre}");
    
    // Validar que la región sea válida
    if (sucursal.RegionId <= 0)
    {
        Console.WriteLine($"ERROR: RegionId es <= 0");
        ModelState.AddModelError("RegionId", "Debe seleccionar una región");
    }
    // ... resto del código
```

Revisa la **consola donde corre la aplicación** para ver los logs.

### Opción 2: Verificar el HTML Generado

1. Abre la página de crear sucursal
2. F12 → Elements
3. Busca el select de Región
4. Debe verse así:

```html
<select id="RegionId" name="RegionId" class="md3-select" required>
    <option value="">Seleccione una región</option>
    <option value="1">Ciudad de México</option>
    <option value="2">Guadalajara</option>
</select>
```

Si NO hay opciones con valores numéricos, significa que no hay regiones en la BD.

### Opción 3: Verificar Regiones en la BD

```sql
-- Ver TODAS las regiones
SELECT * FROM Regiones;

-- Ver solo activas
SELECT * FROM Regiones WHERE Eliminado = 0;

-- Contar regiones
SELECT COUNT(*) as Total FROM Regiones WHERE Eliminado = 0;
```

Si el count es 0, **crea regiones primero**.

## 📊 Flujo de Datos

```
Usuario NO selecciona región
↓
HTML envía: value="" (campo vacío)
↓
Model Binding ASP.NET: "" → 0 (conversión int)
↓
Controller recibe: sucursal.RegionId = 0
↓
Validación Nueva: if (RegionId <= 0) → ADD ERROR ✅
↓
ModelState.IsValid = FALSE
↓
NO se ejecuta SaveChangesAsync
↓
Vuelve a la vista con mensaje de error
```

## ✅ Archivos Modificados

- `PSInventory.Web/Controllers/SucursalesController.cs`
  - Líneas 43-53: Validación mejorada para RegionId

## 🎯 Resumen

El problema era que la validación no capturaba cuando `RegionId = 0`. Ahora:

1. ✅ Valida si RegionId es 0 o negativo
2. ✅ Valida si la región existe en la BD  
3. ✅ Valida si la región está activa (no eliminada)
4. ✅ Muestra errores ANTES de intentar guardar
5. ✅ Evita el Foreign Key Constraint Error

**Reinicia la aplicación y prueba crear una sucursal sin seleccionar región.** Debe mostrar el error de validación inmediatamente.
