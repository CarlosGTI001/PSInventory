# 🎨 Iconos y Logo - PSInventory Web

## 🖼️ Logo

El logo de **Presidente Sports** está integrado en:

### **Ubicaciones:**
- ✅ **Login:** `/wwwroot/images/logo.png` (200px width)
- ✅ **Navigation Drawer:** `/wwwroot/images/logo.png` (150px width)

### **Usar en otras vistas:**
```html
<img src="~/images/logo.png" alt="Presidente Sports" style="width: 150px;" />
```

---

## 🎯 Iconos Material Design 3

El proyecto usa **dos sistemas de iconos de Google**:

### **1. Material Symbols (Moderno - MD3)**

Iconos modernos con peso y relleno variable.

**Uso en HTML:**
```html
<!-- Con md-icon (Material Web Components) -->
<md-icon>dashboard</md-icon>
<md-icon>inventory</md-icon>
<md-icon>shopping_cart</md-icon>

<!-- Directamente en HTML -->
<span class="material-symbols-outlined">
    dashboard
</span>
```

**Galería completa:**
👉 https://fonts.google.com/icons

### **2. Material Icons (Clásico - Compatibilidad)**

Iconos clásicos de Material Design.

**Uso en HTML:**
```html
<md-icon>home</md-icon>
<md-icon>person</md-icon>
<md-icon>settings</md-icon>
```

---

## 📋 Iconos Usados en el Proyecto

### **Navegación:**
- `dashboard` - Dashboard/Inicio
- `category` - Categorías
- `inventory` - Artículos
- `shopping_cart` - Compras
- `qr_code_scanner` - Items
- `location_on` - Regiones
- `store` - Sucursales

### **Operaciones:**
- `assignment` - Asignar Item
- `swap_horiz` - Transferir Item
- `description` - Reportes

### **Autenticación:**
- `person` - Usuario
- `lock` - Contraseña
- `login` - Iniciar sesión
- `logout` - Cerrar sesión

### **Acciones:**
- `add` - Agregar
- `edit` - Editar
- `delete` - Eliminar
- `search` - Buscar
- `filter_list` - Filtrar
- `download` - Descargar
- `upload` - Subir

### **Estados:**
- `check_circle` - Completado/OK
- `error` - Error
- `warning` - Advertencia
- `info` - Información

### **Otros:**
- `menu` - Menú hamburguesa
- `close` - Cerrar
- `arrow_back` - Volver
- `more_vert` - Más opciones

---

## 🎨 Personalización de Iconos

### **Cambiar tamaño:**
```html
<md-icon style="font-size: 32px;">dashboard</md-icon>
```

### **Cambiar color:**
```html
<md-icon style="color: var(--md-sys-color-primary);">dashboard</md-icon>
```

### **Material Symbols con relleno:**
```html
<span class="material-symbols-outlined" style="font-variation-settings: 'FILL' 1;">
    favorite
</span>
```

### **Material Symbols con peso:**
```html
<span class="material-symbols-outlined" style="font-variation-settings: 'wght' 700;">
    star
</span>
```

---

## 🔍 Buscar Iconos

**Sitio oficial:**
https://fonts.google.com/icons

**Ejemplo de búsqueda:**
- Buscar: "warehouse" → `warehouse`
- Buscar: "box" → `inventory_2`
- Buscar: "truck" → `local_shipping`
- Buscar: "computer" → `computer`

---

## 💡 Consejos

1. **Usa Material Symbols** para iconos nuevos (más modernos)
2. **Material Icons** funciona como fallback
3. Todos los iconos son **vectoriales** (escalan perfectamente)
4. Los iconos cargan desde **CDN de Google** (sin descargas necesarias)
5. Puedes usar **cualquier icono** de la galería oficial

---

## 📦 Iconos del Proyecto Escritorio

Los iconos PNG del proyecto de escritorio están en:
`/PSInventory/Iconos/`

**No se usan en la web** porque Material Design provee versiones vectoriales superiores.

**Si necesitas un icono personalizado:**
1. Colócalo en `/wwwroot/images/icons/`
2. Úsalo así: `<img src="~/images/icons/mi-icono.png" />`

---

**¡Todos los iconos de Material Design están disponibles sin configuración adicional!** 🎉
