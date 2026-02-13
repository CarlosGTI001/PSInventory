# 🗑️ Implementación de Borrado Lógico (Soft Delete)

## 📋 Resumen

Se ha implementado **borrado lógico** en todo el sistema PSInventory para mantener un historial completo de auditoría y permitir la recuperación de registros eliminados por error.

---

## 🎯 Campos Agregados a Todos los Modelos

Cada entidad principal ahora tiene estos campos:

```csharp
// Soft Delete
public bool Eliminado { get; set; } = false;
public DateTime? FechaEliminacion { get; set; }
[StringLength(100)]
public string? UsuarioEliminacion { get; set; }
```

---

## 📊 Modelos Actualizados

### ✅ Modelos con Soft Delete

| Modelo | Archivo | Estado |
|--------|---------|--------|
| **Categoria** | `PSData/Modelos/Categoria.cs` | ✅ Completado |
| **Region** | `PSData/Modelos/Region.cs` | ✅ Completado |
| **Sucursal** | `PSData/Modelos/Sucursal.cs` | ✅ Completado |
| **Usuario** | `PSData/Modelos/Usuario.cs` | ✅ Completado |
| **Articulo** | `PSData/Modelos/Articulo.cs` | ✅ Completado |
| **Departamento** | `PSData/Modelos/Departamento.cs` | ✅ Completado |
| **Compra** | `PSData/Modelos/Compra.cs` | ✅ Completado |
| **Item** | `PSData/Modelos/Item.cs` | ✅ Completado |

### ⚠️ Modelo Sin Soft Delete (por diseño)

- **MovimientoItem** - No tiene soft delete porque representa historial de auditoría

---

## 🔧 Controladores Actualizados

Todos los controladores ahora implementan el patrón de soft delete:

### 📝 Cambios Implementados en Cada Controlador

#### 1. **Index()** - Filtrar solo registros activos
```csharp
var entidades = await _context.Entidades
    .Where(e => !e.Eliminado)
    .OrderBy(e => e.Nombre)
    .ToListAsync();
```

#### 2. **Edit()** - Solo editar registros activos
```csharp
var entidad = await _context.Entidades
    .Where(e => !e.Eliminado)
    .FirstOrDefaultAsync(e => e.Id == id);
```

#### 3. **Create()** - Filtrar relaciones (ViewBag)
```csharp
ViewBag.Categorias = await _context.Categorias
    .Where(c => !c.Eliminado)
    .OrderBy(c => c.Nombre)
    .ToListAsync();
```

#### 4. **Delete()** - Borrado lógico en lugar de físico
```csharp
// ANTES (Borrado físico)
_context.Entidades.Remove(entidad);
await _context.SaveChangesAsync();

// AHORA (Borrado lógico)
entidad.Eliminado = true;
entidad.FechaEliminacion = DateTime.Now;
entidad.UsuarioEliminacion = User.Identity?.Name ?? "Sistema";

_context.Update(entidad);
await _context.SaveChangesAsync();
```

#### 5. **Exists()** - Verificar solo registros activos
```csharp
private bool EntidadExists(int id)
{
    return _context.Entidades.Any(e => e.Id == id && !e.Eliminado);
}
```

#### 6. **Validación de Relaciones** - Solo verificar relaciones activas
```csharp
// Verificar si tiene artículos asociados ACTIVOS
var tieneArticulos = await _context.Articulos
    .AnyAsync(a => a.CategoriaId == id && !a.Eliminado);
```

---

## 📦 Controladores Modificados

| Controlador | Index | Create | Edit | Delete | Exists | Estado |
|-------------|-------|--------|------|--------|--------|--------|
| **CategoriasController** | ✅ | - | ✅ | ✅ | ✅ | Completado |
| **RegionesController** | ✅ | - | ✅ | ✅ | ✅ | Completado |
| **SucursalesController** | ✅ | ✅ | ✅ | ✅ | ✅ | Completado |
| **UsuariosController** | ✅ | - | ✅ | ✅ | ✅ | Completado |
| **ArticulosController** | ✅ | ✅ | ✅ | ✅ | ✅ | Completado |
| **DepartamentosController** | ✅ | - | ✅ | ✅ | ✅ | Completado |
| **ComprasController** | ✅ | ✅ | - | ✅ | ✅ | Completado |
| **ItemsController** | ✅ | ✅ | - | ✅ | ✅ | Completado |
| **MovimientosController** | - | ✅ | - | - | - | Parcial* |

*MovimientosController solo filtra Sucursales en ViewBag, no tiene delete propio.

---

## 🗄️ Migración de Base de Datos

### Archivo de Migración

**Ubicación**: `PSData/Migrations/20260213150000_AddSoftDeleteFields.cs`

### Campos Agregados a Cada Tabla

Para cada tabla (`Articulos`, `Categorias`, `Compras`, `Departamentos`, `Regiones`, `Sucursales`, `Usuarios`, `Items`):

```sql
ALTER TABLE [tabla] ADD 
    [Eliminado] bit NOT NULL DEFAULT 0,
    [FechaEliminacion] datetime2 NULL,
    [UsuarioEliminacion] nvarchar(100) NULL;

CREATE INDEX IX_[tabla]_Eliminado ON [tabla]([Eliminado]);
```

### Índices Creados

Se crearon índices en el campo `Eliminado` de todas las tablas para mejorar el performance de las consultas:

- `IX_Articulos_Eliminado`
- `IX_Categorias_Eliminado`
- `IX_Compras_Eliminado`
- `IX_Departamentos_Eliminado`
- `IX_Regiones_Eliminado`
- `IX_Sucursales_Eliminado`
- `IX_Usuarios_Eliminado`

---

## 🔍 Ventajas del Soft Delete

✅ **Auditoría Completa**
   - Se mantiene registro de quién eliminó cada registro
   - Se guarda la fecha exacta de eliminación
   - Historial completo para consultas de auditoría

✅ **Recuperación de Datos**
   - Posibilidad de restaurar registros eliminados por error
   - No se pierde información valiosa
   - Ideal para cumplir con regulaciones de retención de datos

✅ **Integridad Referencial**
   - Las relaciones entre tablas se mantienen intactas
   - No hay errores de claves foráneas rotas
   - Se puede consultar el historial completo de relaciones

✅ **Reportes Históricos**
   - Se pueden generar reportes incluyendo datos eliminados
   - Análisis temporal completo
   - Estadísticas precisas de todo el historial

---

## 📝 Pasos para Aplicar

### 1. Detener la Aplicación (si está corriendo)

### 2. Desde Windows PowerShell:

```powershell
cd C:\Users\CarlosMasterPC1\Documents\PSInventory
.\update-database.ps1
```

### 3. Verificar que se ejecutaron las migraciones:

Las siguientes migraciones deberían aplicarse:
- `20260213142943_AddDepartamentosAndUpdateCompras`
- `20260213150000_AddSoftDeleteFields`

### 4. Reiniciar la aplicación

---

## 🎓 Uso del Sistema Después de Implementar

### Para los Usuarios

- ✅ **No hay cambios visibles** en la interfaz
- ✅ Al eliminar un registro, simplemente desaparece de las listas
- ✅ Los registros eliminados no afectan la operación normal

### Para los Administradores

- ✅ Los datos eliminados se mantienen en la base de datos
- ✅ Se puede consultar directamente en SQL para ver registros eliminados:

```sql
-- Ver categorías eliminadas
SELECT * FROM Categorias WHERE Eliminado = 1;

-- Ver quién eliminó artículos en febrero 2026
SELECT * FROM Articulos 
WHERE Eliminado = 1 
  AND FechaEliminacion >= '2026-02-01'
  AND FechaEliminacion < '2026-03-01';
```

---

## 🔮 Funcionalidades Futuras (Opcional)

### Vista de Auditoría

Se podría crear una vista especial para administradores que muestre:
- Todos los registros eliminados
- Filtros por fecha de eliminación
- Filtros por usuario que eliminó
- Botón para "restaurar" (cambiar `Eliminado` a `false`)

### Código de Ejemplo para Restaurar:

```csharp
[HttpPost]
public async Task<IActionResult> Restaurar(int id)
{
    var categoria = await _context.Categorias
        .Where(c => c.Eliminado)
        .FirstOrDefaultAsync(c => c.Id == id);
        
    if (categoria != null)
    {
        categoria.Eliminado = false;
        categoria.FechaEliminacion = null;
        categoria.UsuarioEliminacion = null;
        
        _context.Update(categoria);
        await _context.SaveChangesAsync();
        
        return Json(new { success = true });
    }
    return Json(new { success = false });
}
```

---

## ✅ Checklist de Implementación

- [x] Agregar campos Soft Delete a modelos
- [x] Crear migración `AddSoftDeleteFields`
- [x] Actualizar CategoriasController
- [x] Actualizar RegionesController
- [x] Actualizar SucursalesController
- [x] Actualizar UsuariosController
- [x] Actualizar ArticulosController
- [x] Actualizar DepartamentosController
- [x] Actualizar ComprasController
- [x] Actualizar ItemsController
- [x] Actualizar MovimientosController (filtros ViewBag)
- [ ] **Aplicar migraciones a la base de datos**
- [ ] **Probar eliminación en cada módulo**
- [ ] **Verificar que los filtros funcionan correctamente**

---

## 🚀 Resumen Final

Se ha implementado exitosamente el **borrado lógico** en todo el sistema PSInventory:

- ✅ **8 modelos** actualizados con campos de soft delete
- ✅ **8 controladores** completamente actualizados
- ✅ **1 migración** creada con todos los cambios de base de datos
- ✅ **Índices** agregados para optimizar performance
- ✅ **Validaciones** de relaciones actualizadas

**Estado**: ✅ **LISTO PARA APLICAR MIGRACIÓN**

El sistema ahora mantiene un historial completo de auditoría y permite la recuperación de datos eliminados accidentalmente. 🎉
