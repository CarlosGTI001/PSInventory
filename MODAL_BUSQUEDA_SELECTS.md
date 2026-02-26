# Modal de Búsqueda para Selects

## 🎯 Descripción
Sistema de búsqueda modal para campos select, permitiendo buscar y seleccionar opciones de manera más eficiente cuando hay muchos elementos.

## ✅ Implementación Realizada

### 1. **JavaScript - search-dialog.js**
Archivo: `wwwroot/js/search-dialog.js`

**Funciones principales:**
- `openSearchDialog(selectId, title, searchPlaceholder)` - Abre el modal de búsqueda
- `initSearchableSelects()` - Inicializa todos los selects con búsqueda
- Auto-inicialización al cargar la página

**Características:**
- ✅ Búsqueda en tiempo real (filtra mientras escribes)
- ✅ Muestra todas las opciones del select
- ✅ Indica la opción actualmente seleccionada con check
- ✅ Mensaje "No se encontraron resultados" cuando aplica
- ✅ Estilo Material Design 3
- ✅ Focus automático en el campo de búsqueda
- ✅ Actualiza el select y dispara evento `change`

### 2. **Layout - _Layout.cshtml**
**Cambios:**
- ✅ Agregado `<script src="~/js/search-dialog.js">` 
- ✅ Estilos CSS para `.searchable-select-wrapper`
- ✅ Posicionamiento del botón de búsqueda

### 3. **Vista de Ejemplo - Sucursales/Create.cshtml**
**Estructura HTML:**
```html
<div class="searchable-select-wrapper">
    <div class="md3-select-field has-leading-icon">
        <md-icon class="md3-select-leading-icon">location_on</md-icon>
        <select asp-for="RegionId" id="RegionId" class="md3-select" required>
            <option value="">Seleccione una región</option>
            @foreach (var region in ViewBag.Regiones)
            {
                <option value="@region.Value">@region.Text</option>
            }
        </select>
        <label class="md3-select-label">Región *</label>
    </div>
    <md-icon-button 
        class="search-button" 
        type="button"
        data-title="Buscar Región"
        data-placeholder="Escriba el nombre de la región..."
        style="position: absolute; right: 8px; top: 8px;">
        <md-icon>search</md-icon>
    </md-icon-button>
</div>
```

**Atributos importantes:**
- `class="searchable-select-wrapper"` - Contenedor wrapper
- `id="RegionId"` - **REQUERIDO** en el select
- `class="search-button"` - Botón que abre el modal
- `data-title` - Título del modal
- `data-placeholder` - Placeholder del campo de búsqueda
- `type="button"` - Evita submit del formulario

## 🔧 Cómo Agregar a Otros Formularios

### Paso 1: Asegurar que el select tenga ID
```html
<select asp-for="CampoId" id="CampoId" class="md3-select" required>
```

### Paso 2: Envolver en searchable-select-wrapper
```html
<div class="searchable-select-wrapper">
    <!-- Tu select existente aquí -->
</div>
```

### Paso 3: Agregar el botón de búsqueda
```html
<md-icon-button 
    class="search-button" 
    type="button"
    data-title="Buscar [Nombre del Campo]"
    data-placeholder="Escriba para buscar..."
    style="position: absolute; right: 8px; top: 8px;">
    <md-icon>search</md-icon>
</md-icon-button>
```

## 📝 Ejemplos de Uso

### Categorías
```html
<div class="searchable-select-wrapper">
    <select id="CategoriaId" asp-for="CategoriaId" class="md3-select">
        <!-- opciones -->
    </select>
    <md-icon-button 
        class="search-button" 
        type="button"
        data-title="Buscar Categoría"
        data-placeholder="Nombre de la categoría...">
        <md-icon>search</md-icon>
    </md-icon-button>
</div>
```

### Artículos
```html
<div class="searchable-select-wrapper">
    <select id="ArticuloId" asp-for="ArticuloId" class="md3-select">
        <!-- opciones -->
    </select>
    <md-icon-button 
        class="search-button" 
        type="button"
        data-title="Buscar Artículo"
        data-placeholder="Código o nombre del artículo...">
        <md-icon>search</md-icon>
    </md-icon-button>
</div>
```

### Departamentos
```html
<div class="searchable-select-wrapper">
    <select id="DepartamentoId" asp-for="DepartamentoId" class="md3-select">
        <!-- opciones -->
    </select>
    <md-icon-button 
        class="search-button" 
        type="button"
        data-title="Buscar Departamento"
        data-placeholder="Nombre del departamento...">
        <md-icon>search</md-icon>
    </md-icon-button>
</div>
```

## 🎨 Personalización

### Cambiar ancho del modal
En `search-dialog.js`, línea con `min-width: 400px; max-width: 600px;`:
```javascript
<div slot="content" style="padding: 24px; min-width: 500px; max-width: 700px;">
```

### Cambiar altura máxima de resultados
En `search-dialog.js`, línea con `max-height: 400px;`:
```javascript
<div id="search-results-${selectId}" 
     style="max-height: 500px; overflow-y: auto; ...">
```

### Cambiar icono del botón
```html
<md-icon-button class="search-button" ...>
    <md-icon>manage_search</md-icon>  <!-- Icono diferente -->
</md-icon-button>
```

## ✅ Archivos Involucrados

- ✅ `wwwroot/js/search-dialog.js` - Lógica del modal
- ✅ `Views/Shared/_Layout.cshtml` - Script y estilos
- ✅ `Views/Sucursales/Create.cshtml` - Ejemplo implementado

## 🧪 Prueba

1. Ve a **Sucursales → Nueva**
2. En el campo **Región**, verás un icono de lupa
3. Haz clic en la lupa
4. Se abre un modal con todas las regiones
5. Escribe en el campo de búsqueda para filtrar
6. Haz clic en una región para seleccionarla
7. El modal se cierra y el select se actualiza

## 🔍 Solución de Problemas

### El botón no aparece
- ✅ Verifica que `search-dialog.js` esté en _Layout.cshtml
- ✅ Asegura que el select tenga `id` único
- ✅ Verifica que la clase sea `search-button`

### No se abre el modal
- ✅ Abre la consola del navegador (F12)
- ✅ Busca errores de JavaScript
- ✅ Verifica que el `id` del select sea correcto

### El select no se actualiza
- ✅ El select debe tener el mismo `id` usado en `openSearchDialog()`
- ✅ Verifica que no haya múltiples selects con el mismo `id`

## 🚀 Próximos Pasos

Para implementar en todos los formularios:
1. ✅ Articulos/Create.cshtml → Select de Categoría
2. ✅ Compras/Create.cshtml → Select de Departamento
3. ✅ Items/Create.cshtml → Select de Artículo y Sucursal
4. ✅ MovimientosItem → Selects de Sucursal Origen/Destino
5. ✅ Todos los Edit.cshtml que tengan selects

**Simplemente copia el patrón de Sucursales/Create.cshtml** 🎯
