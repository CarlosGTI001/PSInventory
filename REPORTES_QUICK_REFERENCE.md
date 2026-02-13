# 📊 Reportes - Quick Reference

## URL y Actions

| # | Reporte | URL | Método | Parámetros |
|----|---------|-----|--------|-----------|
| 1 | Inventario General | `/Reportes/InventarioGeneral` | GET | estado?, categoriaId?, sucursalId? |
| 2 | Por Sucursal | `/Reportes/InventarioPorSucursal` | GET | sucursalId* |
| 3 | Movimientos | `/Reportes/MovimientosItems` | GET | fechaInicio?, fechaFin?, sucursalId? |
| 4 | Garantía | `/Reportes/ItemsGarantiaPorVencer` | GET | diasAnticipacion? (default:30) |
| 5 | Compras | `/Reportes/ReporteCompras` | GET | fechaInicio?, fechaFin? |
| 6 | Estadísticas | `/Reportes/EstadisticasGenerales` | GET | _(sin parámetros)_ |

*= requerido, ? = opcional

---

## 🎨 Colores y Diseño

### Card Headers (Gradientes)
```
1. Inventario General:    #047394 → #0589ab (Azul)
2. Por Sucursal:          #ff5c00 → #ff7f2c (Naranja)
3. Movimientos:           #7c3aed → #a855f7 (Púrpura)
4. Garantía:              #f59e0b → #fbbf24 (Amarillo)
5. Compras:               #10b981 → #34d399 (Verde)
6. Estadísticas:          #6366f1 → #8b5cf6 (Índigo)
```

### Iconos
```
1. inventory_2
2. store
3. swap_horiz
4. schedule
5. shopping_cart
6. analytics
```

---

## 🔧 Estructura HTML de una Card

```html
<div class="card">
  <!-- Header con gradiente -->
  <div style="background: linear-gradient(135deg, #COLOR1 0%, #COLOR2 100%); 
              padding: 24px; color: white; display: flex; align-items: center; gap: 16px;">
    <div style="background: rgba(255,255,255,0.2); border-radius: 12px; 
                width: 56px; height: 56px; display: flex; align-items: center; justify-content: center;">
      <md-icon style="font-size: 32px;">ICON</md-icon>
    </div>
    <div>
      <h3>Título Reporte</h3>
      <p>Descripción corta</p>
    </div>
  </div>

  <!-- Contenido con formulario -->
  <div style="padding: 24px;">
    <form action="/Reportes/ACTION" method="get" target="_blank">
      <!-- Filtros aquí -->
      <!-- Botón al final -->
    </form>
  </div>
</div>
```

---

## 📝 Ejemplos de Filtros

### Select (Dropdown)
```html
<md-filled-select label="Estado">
  <md-select-option value=""></md-select-option>
  <md-select-option value="Disponible">Disponible</md-select-option>
  <md-select-option value="Asignado">Asignado</md-select-option>
</md-filled-select>
```

### Select Dinámico (desde ViewBag)
```html
<md-filled-select label="Categoría">
  <md-select-option value=""></md-select-option>
  @foreach (var cat in categorias)
  {
    <md-select-option value="@cat.Id">@cat.Nombre</md-select-option>
  }
</md-filled-select>
```

### Date Input
```html
<md-filled-text-field label="Fecha Inicio" type="date" name="fechaInicio"></md-filled-text-field>
```

### Number Input con Default
```html
<md-filled-text-field label="Días" type="number" name="diasAnticipacion" value="30"></md-filled-text-field>
```

### Botón Generar PDF
```html
<md-filled-button type="submit" style="background-color: #COLOR;">
  <md-icon slot="icon">picture_as_pdf</md-icon>
  Generar PDF
</md-filled-button>
```

---

## 📱 Responsive Breakpoints

```css
/* Desktop: 2 columnas automáticas */
grid-template-columns: repeat(auto-fit, minmax(500px, 1fr));

/* Tablet/Mobile: 1 columna */
@media (max-width: 768px) {
  grid-template-columns: 1fr;
}
```

---

## 🎯 Datos Requeridos del Controller

```csharp
// En ReportesController.Index()
ViewBag.Categorias = await _context.Categorias
    .OrderBy(c => c.Nombre)
    .ToListAsync();

ViewBag.Sucursales = await _context.Sucursales
    .OrderBy(s => s.Nombre)
    .ToListAsync();
```

**Propiedades esperadas**:
- Categoria: `Id` (int), `Nombre` (string)
- Sucursal: `Id` (string), `Nombre` (string)

---

## ✅ Lista de Verificación

- [x] Breadcrumb y Page Header
- [x] 6 Cards con diseño MD3
- [x] Gradientes únicos por reporte
- [x] Iconos descriptivos
- [x] Formularios GET con target="_blank"
- [x] Dropdowns dinámicos (Categorías, Sucursales)
- [x] Filtros específicos por reporte
- [x] Grid responsivo (2 columnas → 1)
- [x] Hover effects
- [x] Estilos CSS integrados
- [x] Validación HTML5 (required en campos obligatorios)
- [x] Mensajes informativos
- [x] Valores por defecto (ej: diasAnticipacion=30)

---

## 🚀 Próximos Pasos

1. Verificar que ReportesController.Index() carga ViewBag.Categorias y ViewBag.Sucursales
2. Probar cada formulario y validar que los parámetros URL se construyan correctamente
3. Verificar que los action methods en el controller manejen los parámetros GET
4. Ajustar colores si es necesario según el branding de la aplicación
5. Agregar validaciones de lado del cliente si es necesario

---

**Última actualización**: 2024-02-13
**Estado**: ✅ Completo y listo para usar
