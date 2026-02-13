# 📖 Guía de Usuario - PSInventory Web

## Índice
- [Inicio de Sesión](#inicio-de-sesión)
- [Dashboard Principal](#dashboard-principal)
- [Gestión de Items](#gestión-de-items)
- [Gestión de Compras](#gestión-de-compras)
- [Movimientos de Items](#movimientos-de-items)
- [Reportes PDF](#reportes-pdf)
- [Gestión de Catálogos](#gestión-de-catálogos)
- [Administración](#administración)
- [Roles y Permisos](#roles-y-permisos)

---

## Inicio de Sesión

### Acceso al Sistema
1. Abrir navegador y navegar a la URL del sistema
2. Ingresar **Usuario** y **Contraseña**
3. Click en **Iniciar Sesión**

### Roles Disponibles
- **Administrador**: Acceso completo al sistema
- **Jefe**: Gestión de inventario, compras y reportes
- **Usuario**: Consulta de información (futuro)

---

## Dashboard Principal

### Vista General
Al iniciar sesión, verás el **Dashboard** con:

#### 📊 Tarjetas de Resumen
- **Total Items**: Cantidad total de items en inventario
- **Items Disponibles**: Items sin asignar
- **Items Asignados**: Items en uso en sucursales
- **Total Compras**: Número de compras registradas

#### 📈 Gráficas Interactivas
1. **Items por Estado** (Dona)
   - Visualiza distribución: Disponible, Asignado, En Reparación, Baja
   - Hover para ver cantidades y porcentajes

2. **Top 5 Categorías** (Barras)
   - Categorías con más items
   - Identifica qué equipos predominan

3. **Items por Sucursal** (Barras Horizontales)
   - Distribución de items en cada sucursal
   - Visualiza carga de trabajo

4. **Compras Últimos 6 Meses** (Líneas)
   - Cantidad de compras mensuales
   - Monto total invertido
   - Dual eje Y para mejor comparación

#### 🚀 Acciones Rápidas
- **+ Nuevo Item**: Registrar equipo
- **📦 Nueva Compra**: Registrar compra
- **📄 Ver Reportes**: Generar PDFs
- **🔄 Ver Movimientos**: Historial

---

## Gestión de Items

### Listar Items
**Ruta:** Inventario > Items

#### Filtros Disponibles
- **Por Estado**: Todos, Disponible, Asignado, En Reparación, Baja

#### Información Mostrada
- Serial (identificador único)
- Artículo (categoría + marca + modelo)
- Estado actual
- Sucursal asignada (si aplica)
- Garantía vigente/vencida
- Acciones disponibles

### Crear Nuevo Item
1. Click en **+ Nuevo Item**
2. Completar formulario:
   - **Serial**: Número único del equipo
   - **Artículo**: Seleccionar de catálogo
   - **Estado**: Disponible (por defecto)
   - **Fecha Inicio Garantía**: Fecha de compra/activación
   - **Meses de Garantía**: Duración (ej: 12, 24, 36)
   - **Observaciones**: Notas adicionales (opcional)
3. Click en **Guardar**

### Editar Item
1. Click en ícono **✏️ Editar** del item
2. Modificar campos necesarios
3. **Guardar Cambios**

### Asignar Item a Sucursal
**Solo items con estado "Disponible"**

1. Click en **📍 Asignar** en el item
2. Completar formulario:
   - **Sucursal Destino**: Seleccionar sucursal
   - **Usuario Responsable**: Nombre del responsable
   - **Observaciones**: Motivo o detalles
3. **Confirmar Asignación**

**Efecto:**
- Estado cambia a "Asignado"
- Item vinculado a sucursal
- Se crea registro en historial de movimientos

### Transferir Item Entre Sucursales
**Solo items con estado "Asignado"**

1. Click en **🔄 Transferir** en el item
2. Completar formulario:
   - **Sucursal Origen**: Se muestra automáticamente
   - **Sucursal Destino**: Seleccionar nueva sucursal
   - **Motivo**: Transferencia, Reubicación, Préstamo, etc.
   - **Usuario Responsable**: Quien autoriza
   - **Observaciones**: Detalles adicionales
3. **Confirmar Transferencia**

**Efecto:**
- Item cambia de sucursal
- Se crea registro de movimiento con origen y destino

### Eliminar Item
1. Click en **🗑️ Eliminar**
2. Confirmar acción

⚠️ **Precaución**: No se puede eliminar si hay registros relacionados.

---

## Gestión de Compras

### Listar Compras
**Ruta:** Finanzas > Compras

#### Información Mostrada
- Fecha de compra
- Proveedor
- Costo total
- Número de factura
- Estado (Recibida, Pendiente, Parcial)
- Factura adjunta (PDF/Imagen)

### Registrar Nueva Compra
1. Click en **+ Nueva Compra**
2. Completar formulario:
   - **Fecha Compra**: Fecha de la transacción
   - **Proveedor**: Nombre del vendedor
   - **Costo Total**: Monto en $
   - **Número Factura**: Referencia fiscal
   - **Estado**: Recibida/Pendiente/Parcial
   - **Factura**: Adjuntar PDF o imagen escaneada
   - **Observaciones**: Detalles adicionales
3. Click en **Guardar**

#### 📎 Adjuntar Factura
- **Formatos permitidos**: PDF, JPG, JPEG, PNG
- **Tamaño máximo**: 10 MB
- **Recomendación**: Escanear a 150 DPI en PDF

### Editar Compra
1. Click en **✏️ Editar**
2. Modificar campos
3. **Cambiar Factura**: Subir nuevo archivo (reemplaza anterior)
4. **Guardar Cambios**

### Ver Detalles de Compra
1. Click en **👁️ Ver**
2. Ver toda la información
3. **Descargar Factura**: Click en enlace si existe

---

## Movimientos de Items

### Historial de Movimientos
**Ruta:** Inventario > Historial Movimientos

#### Filtros Disponibles
- **Sucursal**: Ver movimientos de una sucursal específica
- **Fecha Inicio**: Desde cuándo buscar
- **Fecha Fin**: Hasta cuándo buscar

#### Información Mostrada
- **Fecha**: Cuándo ocurrió el movimiento
- **Item**: Serial y artículo movido
- **Origen**: Sucursal de donde salió (o "Almacén")
- **Destino**: Sucursal a donde llegó
- **Motivo**: Asignación, Transferencia, Reubicación, etc.
- **Usuario**: Quien registró el movimiento
- **Observaciones**: Detalles adicionales

#### 🔍 Casos de Uso
- Auditar movimientos de equipos
- Rastrear ubicación histórica de un item
- Generar reportes de transferencias
- Identificar responsables de movimientos

---

## Reportes PDF

### Generar Reportes
**Ruta:** Análisis > Reportes PDF

### Tipos de Reportes Disponibles

#### 1. 📦 Inventario General
**Descripción**: Listado completo de items con filtros.

**Filtros:**
- Estado (Disponible, Asignado, En Reparación, Baja)
- Categoría (Laptops, Monitores, etc.)
- Sucursal (Central, Norte, Sur, etc.)

**Contenido:**
- Serial, Artículo, Estado, Sucursal, Garantía
- Totales por estado
- Resumen de cantidad

**Uso:** Inventario físico, auditorías, conteos.

---

#### 2. 🏢 Inventario por Sucursal
**Descripción**: Items asignados a una sucursal específica.

**Filtros:**
- Sucursal (requerido)

**Contenido:**
- Información de la sucursal
- Listado de items asignados
- Total de items
- Breakdown por categoría

**Uso:** Control local, responsabilidades por sucursal.

---

#### 3. 🔄 Movimientos de Items
**Descripción**: Historial de transferencias y asignaciones.

**Filtros:**
- Sucursal (opcional)
- Fecha Inicio
- Fecha Fin

**Contenido:**
- Fecha, Serial, Artículo, Origen, Destino, Motivo, Usuario
- Total de movimientos
- Distribución por tipo de movimiento

**Uso:** Auditorías, rastreo de equipos, análisis de flujo.

---

#### 4. ⏰ Items con Garantía por Vencer
**Descripción**: Items cuya garantía está próxima a expirar.

**Filtros:**
- Días de anticipación (default: 30)

**Contenido:**
- Serial, Artículo, Fecha Inicio, Fecha Vencimiento, Días Restantes
- Items ordenados por urgencia
- Total de items a gestionar

**Uso:** Planificar renovaciones, reclamaciones de garantía.

---

#### 5. 💰 Reporte de Compras
**Descripción**: Resumen financiero de compras.

**Filtros:**
- Fecha Inicio (default: primer día del mes)
- Fecha Fin (default: último día del mes)

**Contenido:**
- Fecha, Proveedor, Número Factura, Costo, Estado
- Total invertido
- Cantidad de compras
- Promedio por compra

**Uso:** Control financiero, análisis de gastos, presupuestos.

---

#### 6. 📊 Estadísticas Generales
**Descripción**: Dashboard completo en PDF.

**Sin Filtros**: Genera snapshot del sistema.

**Contenido:**
- Totales por estado
- Totales por categoría
- Totales por sucursal
- Total de compras
- Monto total invertido
- Resumen ejecutivo

**Uso:** Presentaciones, reportes ejecutivos, snapshots.

---

### Cómo Generar un Reporte
1. Ir a **Análisis > Reportes PDF**
2. Seleccionar el tipo de reporte (tarjeta)
3. Aplicar filtros deseados
4. Click en **📄 Generar PDF**
5. El PDF se abre en nueva pestaña
6. **Descargar** o **Imprimir** desde el navegador

### 💡 Consejos
- Los reportes tienen **estilo SAP profesional**
- Incluyen **encabezado** con fecha y usuario
- Tienen **pie de página** con numeración
- Los filtros aplicados se muestran en el reporte
- Ideales para auditorías y presentaciones

---

## Gestión de Catálogos

### Categorías
**Ruta:** Catálogos > Categorías

**Ejemplos**: Laptops, Monitores, Impresoras, Teclados, Mouse

1. **Crear**: Click en **+ Nueva Categoría**, ingresar nombre
2. **Editar**: Modificar nombre existente
3. **Eliminar**: Solo si no tiene artículos asociados

---

### Artículos
**Ruta:** Catálogos > Artículos

**Información**: Categoría + Marca + Modelo + Especificaciones

1. **Crear**: 
   - Seleccionar **Categoría**
   - Ingresar **Marca** (ej: Dell, HP, Logitech)
   - Ingresar **Modelo** (ej: Latitude 7490, ProBook 450)
   - Agregar **Especificaciones** (ej: i5-8350U, 16GB RAM, 256GB SSD)
2. **Editar**: Modificar cualquier campo
3. **Eliminar**: Solo si no tiene items asociados

---

### Regiones y Sucursales
**Acceso:** Solo **Administradores**

#### Regiones
**Ruta:** Catálogos > Regiones

**Ejemplos**: Norte, Sur, Centro, Occidente

1. **Crear**: Ingresar nombre de región
2. **Editar**: Modificar nombre
3. **Eliminar**: Solo si no tiene sucursales

#### Sucursales
**Ruta:** Catálogos > Sucursales

**Información**: Nombre, Dirección, Región

1. **Crear**:
   - Nombre de sucursal
   - Dirección completa
   - Seleccionar **Región**
2. **Editar**: Modificar datos
3. **Eliminar**: Solo si no tiene items asignados

---

## Administración

### Usuarios
**Ruta:** Administración > Usuarios  
**Acceso:** Solo **Administradores**

#### Crear Usuario
1. Click en **+ Nuevo Usuario**
2. Completar:
   - **ID**: Username único
   - **Nombre**: Nombre completo
   - **Email**: Correo electrónico
   - **Contraseña**: Mínimo 6 caracteres
   - **Rol**: Administrador, Jefe, o Usuario
3. **Guardar**

#### Editar Usuario
- Modificar nombre, email, rol
- **Nota**: No se puede cambiar el ID

#### Eliminar Usuario
- Click en **Eliminar**
- Confirmar acción

⚠️ **Seguridad**: Las contraseñas están encriptadas.

---

## Roles y Permisos

### Tabla de Permisos

| Módulo | Administrador | Jefe | Usuario |
|--------|:-------------:|:----:|:-------:|
| **Dashboard** | ✅ | ✅ | ✅ |
| **Items (Ver/Crear/Editar)** | ✅ | ✅ | ❌ |
| **Items (Asignar/Transferir)** | ✅ | ✅ | ❌ |
| **Compras** | ✅ | ✅ | ❌ |
| **Movimientos (Ver)** | ✅ | ✅ | ✅ |
| **Reportes PDF** | ✅ | ✅ | ✅ |
| **Categorías/Artículos** | ✅ | ✅ | ❌ |
| **Regiones/Sucursales** | ✅ | ❌ | ❌ |
| **Usuarios** | ✅ | ❌ | ❌ |

### Descripción de Roles

#### 👑 Administrador
- Control total del sistema
- Gestión de usuarios
- Configuración de catálogos completos
- Todas las operaciones de inventario

#### 🎯 Jefe
- Gestión completa de inventario
- Registro de compras
- Asignación y transferencia de items
- Generación de reportes
- No puede gestionar usuarios ni regiones/sucursales

#### 📋 Usuario (Futuro)
- Consulta de información
- Visualización de reportes
- Sin permisos de modificación

---

## Preguntas Frecuentes

### ¿Cómo recupero una contraseña?
Contactar al administrador del sistema para reseteo.

### ¿Puedo eliminar un item asignado?
No directamente. Primero cambiar estado a "Baja" o desasignar.

### ¿Las gráficas se actualizan automáticamente?
Sí, cada vez que recargas el dashboard se actualizan con datos en tiempo real.

### ¿Los reportes PDF incluyen imágenes?
Sí, tienen el logo corporativo y formato profesional.

### ¿Puedo exportar datos a Excel?
Actualmente solo PDF. Considera generar reporte y convertir si necesario.

### ¿Cómo sé si una garantía está por vencer?
Usa el reporte "Items con Garantía por Vencer" con 30 días de anticipación.

---

## Soporte Técnico

Para asistencia técnica, contactar al administrador del sistema o equipo de TI.

**Versión del Sistema**: PSInventory Web 1.0  
**Última Actualización**: Febrero 2026  
**Tecnología**: ASP.NET Core 8 MVC + Material Design 3

---

© 2026 PSInventory. Todos los derechos reservados.
