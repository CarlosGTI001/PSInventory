# Documentación: Vista Views/Reportes/Index.cshtml

## 📋 Resumen General

Se ha creado la vista `Index.cshtml` para la sección de Reportes con un diseño Material Design 3 profesional. La vista presenta 6 cards interactivas, cada una representa un tipo de reporte PDF diferente con sus filtros específicos.

## 🎨 Estructura Visual

### 1. Breadcrumb y Page Header
```
Inicio / Reportes PDF
────────────────────
Reportes PDF
Genera reportes profesionales del inventario
```

### 2. Grid Responsivo
- **Layout**: 2 columnas (automático)
- **Ancho mínimo por card**: 500px
- **Gap entre cards**: 24px
- **Comportamiento móvil**: 1 columna en pantallas < 768px
- **Efecto hover**: Elevación y sombra

## 📊 Los 6 Reportes Implementados

### 1️⃣ Inventario General (Azul)
**Código de color**: #047394 → #0589ab (gradiente)
**Ícono**: `inventory_2`
**Filtros**:
- Estado (select): [Todos], Disponible, Asignado, En Reparación, Baja
- Categoría (select dinamizado): Poblado desde ViewBag.Categorias
- Sucursal (select dinamizado): Poblado desde ViewBag.Sucursales

**Todos los filtros son opcionales**

**Action URL**: `/Reportes/InventarioGeneral`
**Método**: GET
**Target**: `_blank` (nueva pestaña)

```html
<form action="/Reportes/InventarioGeneral" method="get" target="_blank">
```

---

### 2️⃣ Inventario por Sucursal (Naranja)
**Código de color**: #ff5c00 → #ff7f2c (gradiente)
**Ícono**: `store`
**Filtros**:
- Sucursal (select required): Poblado desde ViewBag.Sucursales

**El filtro de sucursal es OBLIGATORIO**

**Action URL**: `/Reportes/InventarioPorSucursal`
**Método**: GET
**Target**: `_blank`

```html
<md-filled-select label="Sucursal *" required>
```

---

### 3️⃣ Movimientos de Items (Púrpura)
**Código de color**: #7c3aed → #a855f7 (gradiente)
**Ícono**: `swap_horiz`
**Filtros**:
- Fecha Inicio (date input): `name="fechaInicio"`
- Fecha Fin (date input): `name="fechaFin"`
- Sucursal (select opcional): Poblado desde ViewBag.Sucursales

**Los campos de fecha son inputs HTML5 type="date"**
**El filtro de sucursal es opcional**

**Action URL**: `/Reportes/MovimientosItems`
**Método**: GET
**Target**: `_blank`

---

### 4️⃣ Items Garantía por Vencer (Amarillo)
**Código de color**: #f59e0b → #fbbf24 (gradiente)
**Ícono**: `schedule`
**Filtros**:
- Días de Anticipación (number input): `name="diasAnticipacion"`, `value="30"`

**Incluye un mensaje explicativo sobre el funcionamiento**

**Action URL**: `/Reportes/ItemsGarantiaPorVencer`
**Método**: GET
**Target**: `_blank`

---

### 5️⃣ Reporte de Compras (Verde)
**Código de color**: #10b981 → #34d399 (gradiente)
**Ícono**: `shopping_cart`
**Filtros**:
- Fecha Inicio (date input): `name="fechaInicio"`
- Fecha Fin (date input): `name="fechaFin"`

**Permite análisis de compras en un rango de fechas**

**Action URL**: `/Reportes/ReporteCompras`
**Método**: GET
**Target**: `_blank`

---

### 6️⃣ Estadísticas Generales (Índigo)
**Código de color**: #6366f1 → #8b5cf6 (gradiente)
**Ícono**: `analytics`
**Filtros**: Sin filtros

**Incluye un mensaje informativo explicando que no requiere filtros**

**Action URL**: `/Reportes/EstadisticasGenerales`
**Método**: GET
**Target**: `_blank`

---

## 🛠️ Estructura Técnica

### Variables ViewBag Requeridas (del Controller)
```csharp
ViewBag.Categorias = await _context.Categorias
    .OrderBy(c => c.Nombre)
    .ToListAsync();

ViewBag.Sucursales = await _context.Sucursales
    .OrderBy(s => s.Nombre)
    .ToListAsync();
```

### Componentes Material Design 3 Utilizados
1. **md-filled-select**: Para dropdowns de filtros
2. **md-select-option**: Para opciones dentro de selects
3. **md-filled-text-field**: Para inputs de fecha y número
4. **md-filled-button**: Botón "Generar PDF"
5. **md-icon**: Para iconos en headers y dentro de botones
6. **md-divider**: Podría usarse en futuras mejoras

### CSS Classes Utilizadas
```css
.card {
    background: var(--md-sys-color-surface);
    border-radius: 12px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    overflow: hidden;
}

.card:hover {
    box-shadow: 0 8px 16px rgba(0, 0, 0, 0.15);
    transform: translateY(-2px);
}

.form-row {
    display: flex;
    gap: 16px;
    flex-wrap: wrap;
}
```

### Tipografía
- Titles: `md-typescale-title-large` (card headers)
- Page Title: `md-typescale-display-small` (main header)
- Descripción: `md-typescale-body-large`
- Labels: Texto pequeño (14px) con opacidad

## 📱 Responsive Design

### Desktop (> 768px)
- Grid de 2 columnas automáticas
- Mínimo 500px por column
- Cards distribuidas en 2 filas

### Tablet (768px)
- Transición suave a 1 columna

### Mobile (< 768px)
- 1 columna
- Full width con padding
- Inputs y selects con 100% de ancho

## 🎯 Convenciones Implementadas

### Nombres de Parámetros URL
- `estado`: Para filtro de estado
- `categoriaId`: Para categoría (como ID)
- `sucursalId`: Para sucursal (como ID)
- `fechaInicio` / `fechaFin`: Rango de fechas
- `diasAnticipacion`: Número de días

### Métodos HTTP
- Todos usan **GET** para permitir bookmarks y compartir URLs
- Parámetros en query string (?param=value)

### target="_blank"
- Todos los formularios abren PDFs en nueva pestaña
- Permite seguir navegando en la aplicación

## 🎨 Sistema de Colores

| Reporte | Primario | Secundario | Uso |
|---------|----------|-----------|-----|
| Inventario General | #047394 | #0589ab | Color base de la app |
| Inventario por Sucursal | #ff5c00 | #ff7f2c | Accent naranja |
| Movimientos | #7c3aed | #a855f7 | Púrpura |
| Garantía | #f59e0b | #fbbf24 | Amarillo (advertencia) |
| Compras | #10b981 | #34d399 | Verde (positivo) |
| Estadísticas | #6366f1 | #8b5cf6 | Índigo |

Cada color usa un **gradiente lineal a 135°** para mayor dimensionalidad.

## 🔧 Integración con el Controller

El ReportesController.cs Index() action:
```csharp
public async Task<IActionResult> Index()
{
    ViewBag.Categorias = await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync();
    ViewBag.Sucursales = await _context.Sucursales.OrderBy(s => s.Nombre).ToListAsync();
    return View();
}
```

Ya proporciona los datos necesarios para los dropdowns.

## ✨ Características Destacadas

1. ✅ **Headers Decorativos**: Cada card tiene un header con gradiente único
2. ✅ **Iconografía Clara**: Ícono representativo para cada reporte
3. ✅ **Formularios Inline**: Los filtros están dentro de cada card
4. ✅ **Validación HTML5**: Algunos campos tienen atributos required
5. ✅ **Mensajes Informativos**: Descripción de cada reporte
6. ✅ **Diseño Accesible**: Labels claros y color de texto accesible
7. ✅ **Hover Effects**: Transiciones suaves en las cards
8. ✅ **Responsive**: Adapta a cualquier tamaño de pantalla

## 📝 Notas Técnicas

### Formato de Nombres en Selects Dinámicos
```html
<md-select-option value="@cat.Id">@cat.Nombre</md-select-option>
```
- Usa `@cat.Id` como value (pasará como query string)
- Muestra `@cat.Nombre` al usuario

### Campos de Fecha
```html
<md-filled-text-field label="Fecha Inicio" type="date" name="fechaInicio"></md-filled-text-field>
```
- HTML5 date input proporciona date picker nativo
- Valor se envía como: `2024-02-13`

### Valores por Defecto
```html
<md-filled-text-field label="Días de Anticipación" type="number" name="diasAnticipacion" value="30"></md-filled-text-field>
```
- El atributo `value` establece el valor por defecto en 30

## 🚀 Uso

1. Navegar a `/Reportes` en la aplicación
2. Seleccionar el tipo de reporte deseado
3. Completar los filtros específicos (si aplican)
4. Hacer click en "Generar PDF"
5. El PDF se abre en una nueva pestaña

## 🔍 Debugging

Si los dropdowns no muestran datos:
1. Verificar que `ViewBag.Categorias` y `ViewBag.Sucursales` estén poblados en el controller
2. Revisar la consola del navegador para errores de JavaScript
3. Validar que los valores de ID sean correctos (string vs int)

---

**Archivo creado**: `/PSInventory.Web/Views/Reportes/Index.cshtml`
**Líneas de código**: 333
**Tamaño**: 16KB
**Compatibilidad**: ASP.NET Core 6+, Material Design 3
