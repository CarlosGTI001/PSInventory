# Auto-Generación de ID de Sucursal

## 📋 Resumen
El ID de las sucursales ahora se genera **automáticamente** con el formato `SUC-###` (ejemplo: SUC-001, SUC-002, etc.)

## ✅ Cambios Realizados

### 1. **SucursalesController.cs - Método Create**
- ✅ Removido `Id` del atributo `[Bind]` - ya no se recibe del formulario
- ✅ Eliminadas validaciones manuales de ID vacío y duplicado
- ✅ **Nueva lógica de auto-generación:**
  ```csharp
  // Busca el último ID que comienza con "SUC-"
  var ultimoId = await _context.Sucursales
      .Where(s => s.Id.StartsWith("SUC-"))
      .OrderByDescending(s => s.Id)
      .Select(s => s.Id)
      .FirstOrDefaultAsync();
  
  int siguienteNumero = 1;
  if (!string.IsNullOrEmpty(ultimoId))
  {
      // Extrae el número: SUC-001 -> 001 -> 1
      var numeroStr = ultimoId.Substring(4);
      if (int.TryParse(numeroStr, out int numero))
      {
          siguienteNumero = numero + 1;
      }
  }
  
  // Genera el nuevo ID con 3 dígitos
  sucursal.Id = $"SUC-{siguienteNumero:D3}"; // SUC-001, SUC-002, etc.
  ```
- ✅ Mensaje de éxito incluye el código generado

### 2. **Sucursales/Create.cshtml**
- ✅ **Removido completamente** el campo "Código/ID" del formulario
- ✅ El usuario solo ingresa: Nombre, Región, Dirección, Teléfono, Activo
- ✅ El ID se asigna automáticamente en el servidor

### 3. **ViewBag.Regiones - Correcciones**
- ✅ Cambiado de `"Id"` a `"RegionId"` en todos los SelectList
- ✅ Aplica en: Create (GET/POST) y Edit (GET/POST)

## 🔢 Formato de IDs

| Primera Sucursal | Segunda | Tercera | ... | Sucursal 999 |
|-----------------|---------|---------|-----|--------------|
| SUC-001         | SUC-002 | SUC-003 | ... | SUC-999      |

- **Prefijo fijo:** `SUC-`
- **Número secuencial:** 3 dígitos con ceros a la izquierda
- **Rango:** SUC-001 hasta SUC-999 (999 sucursales)

## 🎯 Beneficios

1. **Sin errores de usuario:** Imposible ingresar IDs duplicados o inválidos
2. **Consistencia:** Todos los IDs siguen el mismo formato
3. **Secuencial:** Fácil identificar el orden de creación
4. **Profesional:** Formato estándar tipo código de barras

## 🧪 Prueba

1. Ve a **Sucursales → Nueva**
2. Verás que **NO hay campo de ID**
3. Ingresa solo:
   - Nombre: "Sucursal Centro"
   - Región: (selecciona una)
   - Dirección y Teléfono
4. Al guardar verás: **"Sucursal creada exitosamente con código SUC-001"**
5. Crea otra, obtendrá: **SUC-002**

## 📝 Notas Técnicas

- Si eliminas sucursales (soft delete), los números no se reutilizan
- El último ID se busca entre **todas** las sucursales (incluso eliminadas)
- Si cambias el prefijo en el futuro, modifica `"SUC-"` en el código
- Para más de 999 sucursales, cambia `D3` a `D4` (4 dígitos)

## 🔧 Archivos Modificados

- `PSInventory.Web/Controllers/SucursalesController.cs`
- `PSInventory.Web/Views/Sucursales/Create.cshtml`
