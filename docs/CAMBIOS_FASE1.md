# Cambios en Modelos - Fase 1 Completada ✅

## 📋 Archivos Modificados

### ✅ **Item.cs** - Modelo Principal de Inventario
**Cambios:**
- ✅ Arregladas relaciones FK (removido `[NotMapped]`)
- ✅ Agregada relación con `Articulo`, `Compra`, `Sucursal`
- ✅ Nuevos campos:
  - `Ubicacion` - Ubicación física del item
  - `ResponsableEmpleado` - A quién está asignado
  - `FechaAsignacion` - Cuándo se asignó
  - `FechaUltimaTransferencia` - Última vez que se movió
  - `FechaGarantiaInicio`, `MesesGarantia`, `FechaGarantiaVencimiento`
  - `Observaciones`
- ✅ Propiedades calculadas:
  - `GarantiaVigente` - Si la garantía está vigente
  - `Disponible` - Si está disponible en almacén

**Estados válidos:**
- "Disponible" (en almacén, `SucursalId = NULL`)
- "En Uso" (asignado a empleado en sucursal)
- "En Reparación"
- "Dañado"
- "Dado de Baja"

---

### ✅ **Compra.cs** - Registro de Compras a Proveedores
**Cambios:**
- ✅ `Tienda` → `Proveedor` (renombrado)
- ✅ `costoTotal` → `CostoTotal` (nomenclatura correcta)
- ✅ Nuevos campos:
  - `FechaCompra` - Cuándo se realizó la compra
  - `NumeroFactura` - Número de factura
  - `Estado` - "Pendiente", "Recibida", "Parcial"
  - `Observaciones`
- ✅ Relación inversa: `ICollection<Item> Items`

---

### ✅ **Articulo.cs** - Catálogo de Productos
**Cambios:**
- ✅ `marca`, `modelo` → `Marca`, `Modelo` (nomenclatura correcta)
- ✅ Agregada relación FK con `Categoria`
- ✅ Nuevos campos:
  - `Descripcion` - Descripción del artículo
  - `StockMinimo` - Para alertas de stock bajo
  - `Especificaciones` - JSON con especificaciones técnicas
- ✅ Relación inversa: `ICollection<Item> Items`

---

### ✅ **Categoria.cs** - Categorías de Artículos
**Cambios:**
- ✅ Agregado campo `RequiereNumeroSerie`
  - `true` para PCs, laptops (requieren serial único)
  - `false` para mouse, cables (no requieren serial)
- ✅ Relación inversa: `ICollection<Articulo> Articulos`

**Ejemplos de categorías:**
- Computadoras (RequiereNumeroSerie = true)
- Periféricos (RequiereNumeroSerie = false)
- Repuestos (RequiereNumeroSerie = false)
- Impresoras (RequiereNumeroSerie = true)
- Networking (RequiereNumeroSerie = true)
- Mobiliario (RequiereNumeroSerie = false)

---

### ✅ **Sucursal.cs** - Sucursales/Puntos de Servicio
**Cambios:**
- ✅ Agregada relación FK con `Region`
- ✅ Nuevo campo `Activo` - Para dar de baja sucursales
- ✅ Relaciones inversas:
  - `ICollection<Item> Items` - Items asignados a la sucursal
  - `ICollection<MovimientoItem> MovimientosOrigen`
  - `ICollection<MovimientoItem> MovimientosDestino`

---

### ✅ **Region.cs** - Regiones Geográficas
**Cambios:**
- ✅ Nuevo campo `Descripcion`
- ✅ Nuevo campo `Activo`
- ✅ Relación inversa: `ICollection<Sucursal> Sucursales`

---

### ✅ **MovimientoItem.cs** - NUEVO Modelo
**Propósito:** Auditoría completa de movimientos de items

**Campos:**
- `ItemSerial` - FK al Item que se mueve
- `SucursalOrigenId` - De dónde viene (NULL = almacén central)
- `SucursalDestinoId` - A dónde va
- `FechaMovimiento` - Cuándo se registró
- `UsuarioResponsable` - Quién autorizó el movimiento
- `Motivo` - Por qué se movió
- `Observaciones`
- `ResponsableRecepcion` - Quién recibió en destino
- `FechaRecepcion` - Cuándo se recibió

**Motivos válidos:**
- "Asignación Inicial" (almacén → sucursal)
- "Transferencia" (sucursal → sucursal)
- "Retorno para Reparación" (sucursal → almacén)
- "Devolución a Almacén" (sucursal → almacén)

---

### ✅ **PSDatos.cs** - DbContext
**Cambios:**
- ✅ Agregado `DbSet<MovimientoItem> MovimientosItem`
- ✅ Nuevos índices:
  - `IX_Item_Estado` - Para filtrar por estado
  - `IX_Articulo_CategoriaId` - Para joins con categoría
  - `IX_MovimientoItem_ItemSerial` - Para historial
  - `IX_MovimientoItem_FechaMovimiento` - Para reportes por fecha
- ✅ Configuración de relaciones de `MovimientoItem`:
  - `SucursalOrigen` - Opcional (puede ser NULL si viene de almacén)
  - `SucursalDestino` - Requerido
  - `WillCascadeOnDelete(false)` - Prevenir eliminación en cascada

---

### ✅ **PSData.csproj**
**Cambios:**
- ✅ Agregada referencia a `Modelos\MovimientoItem.cs`

---

## 📝 SIGUIENTE PASO - EN VISUAL STUDIO

### 1️⃣ **Compilar el Proyecto**
```
Build > Build Solution (Ctrl+Shift+B)
```
**Verificar:** 0 errores

---

### 2️⃣ **Generar Migración**
Abrir **Package Manager Console**:
```
Tools > NuGet Package Manager > Package Manager Console
```

Ejecutar:
```powershell
Add-Migration RedisenoModelosSuministros -Project PSData
```

**Esto generará:**
- Nuevas columnas en tablas existentes
- Nueva tabla `MovimientosItem`
- Índices para mejorar performance
- Foreign Keys correctas

---

### 3️⃣ **Revisar la Migración**
Abrir el archivo generado en:
```
PSData/Migrations/[timestamp]_RedisenoModelosSuministros.cs
```

**Verificar que incluya:**
- ✅ `AddColumn` para nuevos campos en `Items`, `Compras`, `Articulos`, etc.
- ✅ `CreateTable` para `MovimientosItem`
- ✅ `CreateIndex` para todos los índices
- ✅ `AddForeignKey` para relaciones

---

### 4️⃣ **Aplicar Migración**
```powershell
Update-Database -Project PSData
```

**Advertencia:** Si tienes datos existentes, puede haber errores porque:
- `Articulo.CategoriaId` es `[Required]` pero registros viejos no lo tienen
- `Compra.FechaCompra` es `[Required]` pero registros viejos no lo tienen

**Soluciones:**
- **Opción A:** Borrar la BD y empezar de cero (si no hay datos importantes)
- **Opción B:** Modificar la migración para hacer campos opcionales temporalmente
- **Opción C:** Seed data para asignar valores por defecto

---

### 5️⃣ **Verificar en SQL Server**
Abrir **SQL Server Object Explorer** en Visual Studio y verificar:
- ✅ Nuevas columnas en `Items`
- ✅ Tabla `MovimientosItem` creada
- ✅ Índices creados
- ✅ Foreign Keys configuradas

---

## ⚠️ POSIBLES ERRORES Y SOLUCIONES

### Error: "Cannot insert NULL into CategoriaId"
**Causa:** Artículos existentes no tienen categoría

**Solución:**
```sql
-- Crear categoría por defecto
INSERT INTO Categorias (Nombre, Descripcion, RequiereNumeroSerie) 
VALUES ('Sin Categoría', 'Temporal', 0)

-- Asignar a todos los artículos sin categoría
UPDATE Articulos SET CategoriaId = 1 WHERE CategoriaId IS NULL
```

### Error: "Cannot insert NULL into FechaCompra"
**Causa:** Compras existentes no tienen fecha

**Solución:**
```sql
UPDATE Compras SET FechaCompra = GETDATE(), Estado = 'Recibida' 
WHERE FechaCompra IS NULL
```

---

## 🎯 LO QUE SIGUE - FASE 2

Una vez la migración esté aplicada, podemos empezar con:
1. Form de Compras (recepción de suministros)
2. Form de Items (dar de alta con serial)
3. Form de Artículos (catálogo)
4. Form de Asignación (almacén → sucursal)

**¿Listo para ejecutar la migración?** 🚀
