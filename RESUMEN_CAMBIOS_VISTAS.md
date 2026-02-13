# Resumen de Cambios - Vistas de Compras para Facturas Escaneadas

## 📋 Descripción General

Se han actualizado tres vistas Razor para soportar la carga, visualización y descarga de facturas escaneadas. Todos los cambios mantienen el diseño MD3 (Material Design 3) consistente con el resto de la aplicación.

---

## 📁 Archivos Modificados

### 1. **Views/Compras/Edit.cshtml**

#### Cambios Realizados:

**1.1 - Form tag actualizado**
```html
<!-- ANTES -->
<form asp-action="Edit" method="post">

<!-- DESPUÉS -->
<form asp-action="Edit" method="post" enctype="multipart/form-data">
```

**1.2 - Nueva sección de Preview (condicional)**

Se agregó antes del input file (líneas 126-161):

```html
<!-- Mostrado solo si existe RutaFactura -->
@if (!string.IsNullOrEmpty(Model?.RutaFactura))
{
    <div class="form-row" style="margin-top: 16px;">
        <div class="form-field" style="width: 100%;">
            <div style="display: flex; align-items: center; gap: 12px; padding: 16px; 
                        background: var(--md-sys-color-surface-container-high); 
                        border-radius: 12px; border: 2px solid var(--md-sys-color-primary); 
                        margin-bottom: 16px;">
                
                <!-- Icono según tipo de archivo -->
                <div style="width: 48px; height: 48px; border-radius: 12px; 
                            background: rgba(4, 115, 148, 0.15); 
                            display: flex; align-items: center; justify-content: center; 
                            flex-shrink: 0;">
                    @{
                        var extension = System.IO.Path.GetExtension(Model.RutaFactura).ToLower();
                        var isImage = extension == ".jpg" || extension == ".jpeg" || extension == ".png";
                        var fileName = System.IO.Path.GetFileName(Model.RutaFactura);
                    }
                    @if (isImage)
                    {
                        <md-icon style="color: var(--md-sys-color-primary); font-size: 28px;">image</md-icon>
                    }
                    else
                    {
                        <md-icon style="color: var(--md-sys-color-primary); font-size: 28px;">picture_as_pdf</md-icon>
                    }
                </div>
                
                <!-- Información del archivo -->
                <div style="flex: 1;">
                    <div class="md-typescale-body-medium" style="color: var(--md-sys-color-on-surface); margin-bottom: 4px;">
                        <strong>Factura actual:</strong> @fileName
                    </div>
                </div>
                
                <!-- Link de descarga -->
                <a href="@Url.Action("DescargarFactura", new { id = Model.Id })" 
                   style="padding: 8px 16px; background: var(--md-sys-color-primary); 
                           color: white; border-radius: 8px; text-decoration: none; 
                           display: flex; align-items: center; gap: 8px; cursor: pointer; 
                           transition: all 0.2s ease;"
                   onmouseover="this.style.background='var(--md-sys-color-on-primary-container)'"
                   onmouseout="this.style.background='var(--md-sys-color-primary)'">
                    <md-icon style="font-size: 20px;">download</md-icon>
                    <span class="md-typescale-label-large">Descargar</span>
                </a>
            </div>
        </div>
    </div>
}
```

**1.3 - Input file actualizado (idéntico a Create.cshtml)**

El input file ahora muestra dinámicamente:
- "Adjuntar factura escaneada" si no existe RutaFactura
- "Cambiar factura" si ya existe RutaFactura

```html
<div id="fileLabel" class="md-typescale-body-large" style="color: var(--md-sys-color-on-surface); margin-bottom: 4px;">
    @(string.IsNullOrEmpty(Model?.RutaFactura) ? "Adjuntar factura escaneada" : "Cambiar factura")
</div>
```

**1.4 - Sección de Scripts**

Se agregó la función JavaScript `updateFileLabel()` (líneas 233-259):

```javascript
function updateFileLabel(input) {
    const label = document.getElementById('fileLabel');
    if (input.files && input.files[0]) {
        const file = input.files[0];
        const fileName = file.name;
        const fileSize = (file.size / 1024).toFixed(2); // KB
        label.innerHTML = `<md-icon style="font-size: 20px; color: var(--md-sys-color-primary);">check_circle</md-icon> ${fileName} (${fileSize} KB)`;
        
        // Cambiar estilo del contenedor
        input.closest('label').style.borderColor = 'var(--md-sys-color-primary)';
        input.closest('label').style.background = 'rgba(4, 115, 148, 0.08)';
    } else {
        label.textContent = @(string.IsNullOrEmpty(Model?.RutaFactura) ? "'Adjuntar factura escaneada'" : "'Cambiar factura'");
        input.closest('label').style.borderColor = 'var(--md-sys-color-outline)';
        input.closest('label').style.background = 'var(--md-sys-color-surface-container-high)';
    }
}
```

---

### 2. **Views/Compras/Details.cshtml**

#### Cambios Realizados:

**2.1 - Nueva fila de Factura (condicional)**

Se agregó después de Observaciones (líneas 49-85):

```html
@if (!string.IsNullOrEmpty(Model.RutaFactura))
{
    <div class="detail-row">
        <div class="detail-label">Factura:</div>
        <div class="detail-value" style="display: flex; align-items: center; gap: 12px;">
            @{
                var extension = System.IO.Path.GetExtension(Model.RutaFactura).ToLower();
                var isImage = extension == ".jpg" || extension == ".jpeg" || extension == ".png";
                var fileName = System.IO.Path.GetFileName(Model.RutaFactura);
            }
            
            <!-- Icono según tipo -->
            @if (isImage)
            {
                <md-icon style="color: var(--md-sys-color-secondary); font-size: 20px;">image</md-icon>
            }
            else
            {
                <md-icon style="color: var(--md-sys-color-secondary); font-size: 20px;">picture_as_pdf</md-icon>
            }
            
            <!-- Link de descarga -->
            <a href="@Url.Action("DescargarFactura", new { id = Model.Id })" 
               style="color: var(--md-sys-color-primary); text-decoration: none; cursor: pointer; font-weight: 500;"
               onmouseover="this.style.textDecoration='underline'"
               onmouseout="this.style.textDecoration='none'">
                @fileName
            </a>
        </div>
    </div>

    <!-- Vista previa si es imagen -->
    @if (isImage)
    {
        <div style="margin-top: 20px; padding-top: 20px; border-top: 1px solid var(--md-sys-color-outline-variant);">
            <h3 class="md-typescale-title-medium" style="margin-bottom: 12px;">Vista Previa</h3>
            <img src="@Url.Action("DescargarFactura", new { id = Model.Id })" 
                 alt="Factura" 
                 style="max-width: 300px; max-height: 300px; border-radius: 12px; 
                         border: 1px solid var(--md-sys-color-outline); 
                         box-shadow: var(--md-sys-elevation-level1);">
        </div>
    }
}
```

**Características:**
- ✅ Se muestra solo si `Model.RutaFactura` no es nulo
- ✅ Icono dinámico según tipo de archivo
- ✅ Link de descarga con hover effect
- ✅ Vista previa solo para imágenes (JPG, JPEG, PNG)
- ✅ Estilos MD3 completamente consistentes

---

### 3. **Views/Compras/Index.cshtml**

#### Cambios Realizados:

**3.1 - Nueva sección "Factura adjunta"**

Se agregó después de la sección de Número de Factura (líneas 154-182):

```html
@if (!string.IsNullOrEmpty(compra.RutaFactura))
{
    <div style="background: var(--md-sys-color-primary-container); border-radius: 8px; 
                padding: 12px; display: flex; align-items: center; gap: 10px; 
                margin-bottom: 16px;">
        
        <!-- Determinación de tipo de archivo -->
        @{
            var extension = System.IO.Path.GetExtension(compra.RutaFactura).ToLower();
            var isImage = extension == ".jpg" || extension == ".jpeg" || extension == ".png";
            var fileName = System.IO.Path.GetFileName(compra.RutaFactura);
        }
        
        <!-- Icono según tipo -->
        @if (isImage)
        {
            <md-icon style="color: var(--md-sys-color-on-primary-container); font-size: 20px;">image</md-icon>
        }
        else
        {
            <md-icon style="color: var(--md-sys-color-on-primary-container); font-size: 20px;">picture_as_pdf</md-icon>
        }
        
        <!-- Contenido -->
        <div style="flex: 1;">
            <div class="md-typescale-label-small" style="color: var(--md-sys-color-on-primary-container); margin-bottom: 4px;">
                Factura adjunta
            </div>
            <!-- Link de descarga -->
            <a href="@Url.Action("DescargarFactura", new { id = compra.Id })" 
               style="color: var(--md-sys-color-on-primary-container); font-weight: 500; 
                       text-decoration: none; cursor: pointer; display: flex; align-items: center; gap: 4px;"
               onclick="event.stopPropagation()"
               onmouseover="this.style.textDecoration='underline'"
               onmouseout="this.style.textDecoration='none'">
                <span class="md-typescale-body-medium">@fileName</span>
                <md-icon style="font-size: 18px;">download</md-icon>
            </a>
        </div>
    </div>
}
```

**Características:**
- ✅ Se muestra solo si `compra.RutaFactura` no es nulo
- ✅ Background usando `var(--md-sys-color-primary-container)`
- ✅ Icono dinámico según tipo de archivo
- ✅ Nombre del archivo + icono de descarga
- ✅ `event.stopPropagation()` para evitar navegar a Details
- ✅ Hover effects con subrayado
- ✅ Flujo horizontal flexible

---

## 🎨 Estilos y Colores MD3 Utilizados

| Elemento | Color MD3 | Uso |
|----------|-----------|-----|
| Icono Preview (Edit) | `--md-sys-color-primary` | Indica archivo actual |
| Fondo Preview (Edit) | `--md-sys-color-surface-container-high` | Fondo del contenedor |
| Borde Preview (Edit) | `--md-sys-color-primary` (2px) | Destacar contenedor |
| Botón Descargar (Edit) | `--md-sys-color-primary` | Primary button |
| Icono (Details) | `--md-sys-color-secondary` | Diferenciación visual |
| Link (Details) | `--md-sys-color-primary` | Click action |
| Fondo (Index) | `--md-sys-color-primary-container` | Contenedor destacado |
| Texto (Index) | `--md-sys-color-on-primary-container` | Alto contraste |

---

## 📱 Responsive Design

Todas las nuevas secciones son completamente responsivas:
- ✅ Flexbox para layouts flexibles
- ✅ `gap` property para espaciado consistente
- ✅ Max-width en imágenes de preview
- ✅ Padding adaptativo

---

## 🔗 Puntos de Integración con Backend

### Links de Descarga
Todos los links utilizan la acción `DescargarFactura`:
```html
@Url.Action("DescargarFactura", new { id = Model.Id })
@Url.Action("DescargarFactura", new { id = compra.Id })
```

### Propiedades del Modelo
- `Model.RutaFactura` → String con la ruta relativa del archivo
- `Model.Id` → ID de la compra para identificar la factura

### Variables Locales (Razor)
```csharp
var extension = System.IO.Path.GetExtension(Model.RutaFactura).ToLower();
var isImage = extension == ".jpg" || extension == ".jpeg" || extension == ".png";
var fileName = System.IO.Path.GetFileName(Model.RutaFactura);
```

---

## ✅ Validaciones en Frontend

### Input File (Create y Edit)
```html
<input type="file" 
       name="facturaFile" 
       id="facturaFile" 
       accept=".pdf,.jpg,.jpeg,.png"
       onchange="updateFileLabel(this)">
```

**Archivos permitidos:**
- `.pdf` - Documentos PDF
- `.jpg` - Imágenes JPEG
- `.jpeg` - Imágenes JPEG
- `.png` - Imágenes PNG

---

## 🔄 Flujo de Interacción

### En Create.cshtml
1. Usuario selecciona archivo
2. `updateFileLabel()` actualiza el label con nombre y tamaño
3. Estilos cambian (borde azul, fondo aclarado)
4. Se muestra icono de check_circle

### En Edit.cshtml
1. Si existe factura: se muestra preview con botón descargar
2. Usuario puede:
   - Descargar la factura actual
   - Seleccionar una nueva factura (reemplaza la anterior)
3. Mismo comportamiento que Create en el input file

### En Details.cshtml
1. Se muestra fila "Factura:" con link de descarga
2. Si es imagen: se muestra thumbnail debajo

### En Index.cshtml
1. Se muestra sección "Factura adjunta" en cada tarjeta
2. Link de descarga sin navegar a Details
3. Mismo icono dinámico según tipo

---

## 📝 Notas Técnicas

- Los nombres de archivos NO se guardan sin procesar por seguridad
- Se utiliza `Path.GetFileName()` para extraer el nombre sin ruta
- Se detectan solo las primeras 3 extensiones (.jpg, .jpeg, .png) como imágenes
- El resto se trata como PDF
- Los hover effects se manejan con `onmouseover` / `onmouseout` de forma inline
- Se usa `event.stopPropagation()` en Index para evitar navegaciones no deseadas
- `flex: 1` se utiliza para layouts responsivos

---

## 🐛 Casos Edge

| Caso | Manejo |
|------|--------|
| `RutaFactura == null` | No se muestra la sección |
| `RutaFactura == ""` | Se trata como nulo |
| Extensión desconocida | Se muestra icono PDF |
| Archivo de imagen grande | Se limita max-width/height a 300px |
| Usuario cancela selección | El label vuelve a su estado original |

---

## 🚀 Próximos Pasos Recomendados

1. ✅ [COMPLETADO] Crear las vistas
2. ⏳ Actualizar modelo Compra (agregar `RutaFactura`)
3. ⏳ Implementar lógica en ComprasController
4. ⏳ Crear carpeta `/wwwroot/facturas/`
5. ⏳ Ejecutar migraciones EF Core
6. ⏳ Realizar pruebas de carga/descarga
7. ⏳ Implementar seguridad (authorization)
8. ⏳ Optimizar rendimiento (thumbnails)

---

## 📖 Referencias

- Material Design 3 Colors: [Material Design Spec](https://m3.material.io)
- ASP.NET Razor Helpers: [Microsoft Docs](https://docs.microsoft.com/aspnet/core)
- HTML File Input: [MDN Web Docs](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input/file)
