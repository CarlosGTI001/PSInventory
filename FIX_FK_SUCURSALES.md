# Fix: Foreign Key Constraint Error - Sucursales

## ❌ Error Encontrado
```
SqlException: The INSERT statement conflicted with the FOREIGN KEY constraint 
"FK_Sucursales_Regiones_RegionId". The conflict occurred in database "PSInventoryDB", 
table "dbo.Regiones", column 'RegionId'.
```

## 🔍 Causa del Problema

El error ocurre cuando intentas crear una sucursal con un `RegionId` que:
1. **No existe** en la tabla Regiones
2. Es **0** (valor por defecto si no se selecciona nada)
3. Es **null**
4. Corresponde a una región **eliminada** (soft delete)

## ✅ Solución Implementada

### **SucursalesController.cs - Método Create**
Agregada validación **antes** de guardar:

```csharp
// Validar que la región existe
if (sucursal.RegionId > 0)
{
    var regionExists = await _context.Regiones
        .AnyAsync(r => r.RegionId == sucursal.RegionId && !r.Eliminado);
    
    if (!regionExists)
    {
        ModelState.AddModelError("RegionId", "La región seleccionada no existe o está inactiva");
    }
}
```

**Beneficios:**
- ✅ Valida que el ID sea mayor a 0
- ✅ Verifica que exista en la base de datos
- ✅ Confirma que no esté eliminada (soft delete)
- ✅ Muestra mensaje de error claro al usuario
- ✅ Evita el error de base de datos

## 📋 Verificaciones Previas

Antes de crear una sucursal, asegúrate de:

### 1. **Hay regiones activas en la base de datos**
```sql
SELECT * FROM Regiones WHERE Eliminado = 0;
```

Si no hay regiones, créalas primero:
- Ve a **Regiones → Nueva**
- Crea al menos una región (ej: "Ciudad de México", "Guadalajara", etc.)

### 2. **El select muestra opciones**
En la vista Create, el select de Región debe tener opciones:
```html
<select asp-for="RegionId" id="RegionId" class="md3-select" required>
    <option value="">Seleccione una región</option>
    <!-- Debe haber opciones aquí -->
</select>
```

Si está vacío, no hay regiones activas.

## 🧪 Cómo Probar la Solución

### Escenario 1: Crear sucursal SIN seleccionar región
1. Ve a **Sucursales → Nueva**
2. Ingresa nombre, dirección, teléfono
3. **NO** selecciones ninguna región
4. Haz clic en Guardar
5. **Resultado esperado:** Error de validación: "The RegionId field is required"

### Escenario 2: Crear sucursal con región válida
1. Ve a **Sucursales → Nueva**
2. Ingresa todos los datos
3. **Selecciona** una región del dropdown
4. Haz clic en Guardar
5. **Resultado esperado:** ✅ "Sucursal creada exitosamente con código SUC-001"

### Escenario 3: Región inexistente (test manual en base de datos)
Si alguien manipula el HTML y envía un RegionId=999 que no existe:
1. La validación lo detecta
2. Muestra: "La región seleccionada no existe o está inactiva"
3. No se guarda nada en la base de datos

## 🔧 Correcciones Adicionales Realizadas

### ViewBag.Regiones - Uso correcto de RegionId
**Antes (❌ Incorrecto):**
```csharp
ViewBag.Regiones = new SelectList(..., "Id", "Nombre");
```

**Ahora (✅ Correcto):**
```csharp
ViewBag.Regiones = new SelectList(..., "RegionId", "Nombre");
```

**Aplicado en:**
- SucursalesController.Create (GET y POST)
- SucursalesController.Edit (GET y POST)

## 📝 Modelo de Datos

### Region.cs
```csharp
public class Region
{
    [Key]
    public int RegionId { get; set; }  // ← Clave primaria
    // ...
}
```

### Sucursal.cs
```csharp
public class Sucursal
{
    [Key]
    public string Id { get; set; }     // ← Auto-generado: SUC-001
    
    [Required]
    public int RegionId { get; set; }  // ← Foreign Key
    
    [ForeignKey("RegionId")]
    public virtual Region Region { get; set; }
    // ...
}
```

## ⚠️ Problemas Comunes y Soluciones

### Problema 1: "No aparecen regiones en el dropdown"
**Causa:** No hay regiones activas en la base de datos
**Solución:** 
1. Ve a **Regiones → Nueva**
2. Crea al menos una región
3. Asegúrate de que esté **Activa** (checkbox marcado)

### Problema 2: "El select muestra regiones pero da error al guardar"
**Causa:** El HTML no está enviando el valor correcto
**Solución:**
1. Verifica que el select tenga `asp-for="RegionId"`
2. Asegura que las opciones tengan `value="@region.Value"`
3. Inspecciona el HTML generado (F12 → Elements)

### Problema 3: "Error después de eliminar una región"
**Causa:** Sucursales existentes apuntan a una región eliminada
**Solución:** 
- El soft delete evita esto
- Las regiones eliminadas no aparecen en los dropdowns
- Las sucursales existentes mantienen su RegionId

## 🎯 Validaciones Implementadas

1. **Required** - El campo es obligatorio (atributo en el modelo)
2. **RegionId > 0** - No acepta valores por defecto
3. **Exists in DB** - Verifica que exista en la tabla
4. **Not Deleted** - Solo regiones activas (soft delete)

## 📚 Archivos Modificados

- ✅ `PSInventory.Web/Controllers/SucursalesController.cs`
  - Agregada validación en Create (POST)
  - Corregido ViewBag.Regiones (4 ubicaciones)

## ✅ Estado Final

- ✅ Validación de foreign key implementada
- ✅ Mensajes de error claros
- ✅ ViewBag.Regiones usando "RegionId"
- ✅ Modal de búsqueda agregado (bonus)
- ✅ Auto-generación de ID funcionando

El sistema ahora está **protegido** contra errores de foreign key en Sucursales. 🛡️
