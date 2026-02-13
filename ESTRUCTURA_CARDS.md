# 🎨 Estructura Visual de las Cards - Referencia Detallada

## Estructura HTML Genérica de una Card

```html
<div class="card" style="transition: all 0.3s ease;">
  
  <!-- HEADER CON GRADIENTE -->
  <div style="background: linear-gradient(135deg, #PRIMARIO 0%, #SECUNDARIO 100%); 
              padding: 24px; 
              color: white; 
              display: flex; 
              align-items: center; 
              gap: 16px;">
    
    <!-- ICONO EN CUADRADO SEMI-TRANSPARENTE -->
    <div style="background: rgba(255,255,255,0.2); 
                border-radius: 12px; 
                width: 56px; 
                height: 56px; 
                display: flex; 
                align-items: center; 
                justify-content: center;">
      <md-icon style="font-size: 32px;">ICONO_PRINCIPAL</md-icon>
    </div>
    
    <!-- TEXTO DEL HEADER -->
    <div>
      <h3 class="md-typescale-title-large" style="margin: 0; color: white;">
        Título del Reporte
      </h3>
      <p style="margin: 4px 0 0 0; opacity: 0.9; font-size: 14px;">
        Descripción corta del reporte
      </p>
    </div>
  </div>

  <!-- CONTENIDO CON FORMULARIO -->
  <div style="padding: 24px;">
    <form action="/Reportes/ACTION" method="get" target="_blank" 
          style="display: flex; flex-direction: column; gap: 16px;">
      
      <!-- FILA DE FILTROS 1 -->
      <div class="form-row">
        <div style="flex: 1;">
          <md-filled-select label="Filtro 1">
            <md-select-option value=""></md-select-option>
            <md-select-option value="op1">Opción 1</md-select-option>
            <md-select-option value="op2">Opción 2</md-select-option>
          </md-filled-select>
        </div>
      </div>

      <!-- FILA DE FILTROS 2 (si aplica) -->
      <div class="form-row">
        <div style="flex: 1;">
          <md-filled-select label="Filtro 2">
            <md-select-option value=""></md-select-option>
            <!-- opciones aquí -->
          </md-filled-select>
        </div>
      </div>

      <!-- BOTÓN CENTRADO -->
      <div style="display: flex; justify-content: center; margin-top: 8px;">
        <md-filled-button type="submit" style="background-color: #COLOR;">
          <md-icon slot="icon">picture_as_pdf</md-icon>
          Generar PDF
        </md-filled-button>
      </div>
    </form>
  </div>
</div>
```

---

## 📐 Dimensiones y Espaciado

### Card Principal
- **Min-width**: 500px
- **Max-width**: auto (2 columnas)
- **Altura**: auto (según contenido)
- **Border-radius**: 12px
- **Box-shadow**: 0 2px 8px rgba(0,0,0,0.1)
- **Box-shadow (hover)**: 0 8px 16px rgba(0,0,0,0.15)

### Header
- **Padding**: 24px
- **Height**: auto
- **Display**: flex
- **Align-items**: center
- **Gap**: 16px

### Icon Box
- **Width**: 56px
- **Height**: 56px
- **Background**: rgba(255,255,255,0.2)
- **Border-radius**: 12px
- **Icon size**: 32px

### Content Area
- **Padding**: 24px
- **Gap**: 16px (entre elementos)

### Form Row
- **Display**: flex
- **Gap**: 16px
- **Flex-wrap**: wrap

### Inputs
- **Width**: 100%

### Submit Button
- **Max-width**: 200px
- **Width**: 100%

---

## 🎨 Esquema Cromático de Cada Card

### Card 1: Inventario General
```
Header Gradient: #047394 → #0589ab
Text Color: white
Icon Background: rgba(255,255,255,0.2)
Button Color: #047394

Visual:
┌─────────────────────────┐
│ [Icon] Inventario General │
│        Reporte completo   │
├─────────────────────────┤
│ [Estado]                │
│ [Categoría]             │
│ [Sucursal]              │
│      [Generar PDF]      │
└─────────────────────────┘
```

### Card 2: Inventario por Sucursal
```
Header Gradient: #ff5c00 → #ff7f2c
Text Color: white
Icon Background: rgba(255,255,255,0.2)
Button Color: #ff5c00

Visual:
┌─────────────────────────┐
│ [Icon] Por Sucursal      │
│    Items detallados     │
├─────────────────────────┤
│ [Sucursal *]            │
│      [Generar PDF]      │
└─────────────────────────┘
```

### Card 3: Movimientos de Items
```
Header Gradient: #7c3aed → #a855f7
Text Color: white
Icon Background: rgba(255,255,255,0.2)
Button Color: #7c3aed

Visual:
┌─────────────────────────┐
│ [Icon] Movimientos Items│
│   Historial y cambios   │
├─────────────────────────┤
│ [Fecha Inicio] [Fin]    │
│ [Sucursal]              │
│      [Generar PDF]      │
└─────────────────────────┘
```

### Card 4: Garantía por Vencer
```
Header Gradient: #f59e0b → #fbbf24
Text Color: white
Icon Background: rgba(255,255,255,0.2)
Button Color: #f59e0b

Visual:
┌─────────────────────────┐
│ [Icon] Garantía Vencer  │
│   Alertas de vencimiento│
├─────────────────────────┤
│ [Días]                  │
│    💡 Mensaje info      │
│      [Generar PDF]      │
└─────────────────────────┘
```

### Card 5: Reporte de Compras
```
Header Gradient: #10b981 → #34d399
Text Color: white
Icon Background: rgba(255,255,255,0.2)
Button Color: #10b981

Visual:
┌─────────────────────────┐
│ [Icon] Compras          │
│   Análisis de compras   │
├─────────────────────────┤
│ [Fecha Inicio] [Fin]    │
│      [Generar PDF]      │
└─────────────────────────┘
```

### Card 6: Estadísticas Generales
```
Header Gradient: #6366f1 → #8b5cf6
Text Color: white
Icon Background: rgba(255,255,255,0.2)
Button Color: #6366f1

Visual:
┌─────────────────────────┐
│ [Icon] Estadísticas     │
│    Resumen estadístico  │
├─────────────────────────┤
│    💡 Sin filtros       │
│      [Generar PDF]      │
└─────────────────────────┘
```

---

## 🔤 Tipografía

### Títulos (Card Headers)
- **Class**: `md-typescale-title-large`
- **Color**: white
- **Margin**: 0
- **Font-size**: ~20px
- **Font-weight**: 600

### Descripción (Card Headers)
- **Font-size**: 14px
- **Opacity**: 0.9
- **Color**: white
- **Margin**: 4px 0 0 0

### Labels
- **Font-size**: 14px
- **Color**: var(--md-sys-color-on-surface-variant)
- **Associated with inputs via label attribute**

### Button Text
- **Class**: Default MD3
- **Icon**: picture_as_pdf (16px)
- **Text**: "Generar PDF"

---

## 🎯 Estados Interactivos

### Estado Normal
```css
.card {
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transform: translateY(0);
}
```

### Estado Hover
```css
.card:hover {
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.15);
  transform: translateY(-2px);
  transition: all 0.3s ease;
}
```

---

## 📱 Responsive Breakpoints

### Desktop (> 1024px)
```
┌──────────────────┬──────────────────┐
│     Card 1       │     Card 2       │
├──────────────────┼──────────────────┤
│     Card 3       │     Card 4       │
├──────────────────┼──────────────────┤
│     Card 5       │     Card 6       │
└──────────────────┴──────────────────┘
```

### Tablet (768px - 1024px)
```
┌──────────────────┬──────────────────┐
│     Card 1       │     Card 2       │
├──────────────────┼──────────────────┤
│     Card 3       │     Card 4       │
├──────────────────┼──────────────────┤
│     Card 5       │     Card 6       │
└──────────────────┴──────────────────┘
(Se ajusta según espacio disponible)
```

### Mobile (< 768px)
```
┌──────────────────┐
│     Card 1       │
├──────────────────┤
│     Card 2       │
├──────────────────┤
│     Card 3       │
├──────────────────┤
│     Card 4       │
├──────────────────┤
│     Card 5       │
├──────────────────┤
│     Card 6       │
└──────────────────┘
```

---

## 🔗 Flujo de Datos

### Form GET → URL → Controller → PDF
```
User Input
   ↓
<form method="get" action="/Reportes/Action">
   ↓
URL: /Reportes/Action?param1=val1&param2=val2
   ↓
ReportesController.Action(param1, param2)
   ↓
Genera PDF
   ↓
File() → PDF en nueva pestaña (target="_blank")
```

### Ejemplo: Inventario General
```
Selecciona:
- Estado: "Disponible"
- Categoría: 5
- Sucursal: "SUC001"

↓

Form genera URL:
/Reportes/InventarioGeneral?estado=Disponible&categoriaId=5&sucursalId=SUC001

↓

Abre en nueva pestaña (target="_blank")

↓

PDF descargado/visualizado
```

---

## ✅ Checklist de Estructura por Card

### Cada Card debe tener:
- [ ] Div con clase "card" y transición
- [ ] Header con gradiente lineal a 135°
- [ ] Icon box (56x56) con fondo semi-transparente
- [ ] Título en md-typescale-title-large
- [ ] Descripción corta (14px, 90% opacity)
- [ ] Content div con padding 24px
- [ ] Form action="/Reportes/..." method="get" target="_blank"
- [ ] Form display flex, flex-direction column, gap 16px
- [ ] Form-rows para agrupar filtros
- [ ] Inputs/Selects 100% width
- [ ] Submit button centrado, max-width 200px
- [ ] Icon picture_as_pdf en botón
- [ ] Texto "Generar PDF" en botón

---

## 🎓 Ejemplo Completo: Card Inventario General

```html
<div class="card" style="transition: all 0.3s ease;">
  
  <!-- Header -->
  <div style="background: linear-gradient(135deg, #047394 0%, #0589ab 100%); 
              padding: 24px; 
              color: white; 
              display: flex; 
              align-items: center; 
              gap: 16px;">
    
    <div style="background: rgba(255,255,255,0.2); 
                border-radius: 12px; 
                width: 56px; 
                height: 56px; 
                display: flex; 
                align-items: center; 
                justify-content: center;">
      <md-icon style="font-size: 32px;">inventory_2</md-icon>
    </div>
    
    <div>
      <h3 class="md-typescale-title-large" style="margin: 0; color: white;">
        Inventario General
      </h3>
      <p style="margin: 4px 0 0 0; opacity: 0.9; font-size: 14px;">
        Reporte completo de todos los items
      </p>
    </div>
  </div>

  <!-- Content -->
  <div style="padding: 24px;">
    <form action="/Reportes/InventarioGeneral" 
          method="get" 
          target="_blank" 
          style="display: flex; flex-direction: column; gap: 16px;">
      
      <div class="form-row">
        <div style="flex: 1;">
          <md-filled-select label="Estado">
            <md-select-option value=""></md-select-option>
            <md-select-option value="Disponible">Disponible</md-select-option>
            <md-select-option value="Asignado">Asignado</md-select-option>
            <md-select-option value="En Reparación">En Reparación</md-select-option>
            <md-select-option value="Baja">Baja</md-select-option>
          </md-filled-select>
        </div>
      </div>

      <div class="form-row">
        <div style="flex: 1;">
          <md-filled-select label="Categoría">
            <md-select-option value=""></md-select-option>
            @foreach (var cat in categorias) {
              <md-select-option value="@cat.Id">@cat.Nombre</md-select-option>
            }
          </md-filled-select>
        </div>
      </div>

      <div class="form-row">
        <div style="flex: 1;">
          <md-filled-select label="Sucursal">
            <md-select-option value=""></md-select-option>
            @foreach (var suc in sucursales) {
              <md-select-option value="@suc.Id">@suc.Nombre</md-select-option>
            }
          </md-filled-select>
        </div>
      </div>

      <div style="display: flex; justify-content: center; margin-top: 8px;">
        <md-filled-button type="submit" style="background-color: #047394;">
          <md-icon slot="icon">picture_as_pdf</md-icon>
          Generar PDF
        </md-filled-button>
      </div>
    </form>
  </div>
</div>
```

---

**Última actualización**: 2024-02-13
**Compatibilidad**: Material Design 3, ASP.NET Core Razor
