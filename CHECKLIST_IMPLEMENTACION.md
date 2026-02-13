# Checklist de Implementación - Facturas Escaneadas en Compras

## ✅ FASE 1: VISTAS (COMPLETADO)

### Views/Compras/Edit.cshtml
- [x] Agregar `enctype="multipart/form-data"` al form
- [x] Crear sección preview de factura actual
- [x] Agregar input file idéntico a Create.cshtml
- [x] Implementar función JavaScript `updateFileLabel()`
- [x] Cambio dinámico de label: "Adjuntar" vs "Cambiar factura"
- [x] Estilos MD3 completamente consistentes

### Views/Compras/Details.cshtml
- [x] Agregar fila "Factura:" en Información General
- [x] Link de descarga con icono dinámico
- [x] Vista previa para imágenes (max 300x300px)
- [x] Mostrado solo si RutaFactura != null

### Views/Compras/Index.cshtml
- [x] Agregar sección "Factura adjunta" en tarjetas
- [x] Icono dinámico según tipo (imagen/PDF)
- [x] Link con event.stopPropagation()
- [x] Estilos usando var(--md-sys-color-primary-container)

---

## ⏳ FASE 2: MODELO (PENDIENTE)

### Actualizar Modelo Compra
- [ ] Agregar propiedad: `public string RutaFactura { get; set; }`
- [ ] Ubicación: `Models/Compra.cs`

**Código a agregar:**
```csharp
public string RutaFactura { get; set; }  // Nueva propiedad
```

---

## ⏳ FASE 3: BASE DE DATOS (PENDIENTE)

### Crear Migración
- [ ] Ejecutar: `dotnet ef migrations add AddRutaFacturaToCompra`
- [ ] Ejecutar: `dotnet ef database update`
- [ ] Verificar que la columna `RutaFactura` (VARCHAR/String) existe en tabla `Compras`

---

## ⏳ FASE 4: CONTROLLER (PENDIENTE)

### ComprasController.cs

#### Acción Edit (GET)
- [ ] Verificar que devuelve la compra con todos los datos
- [ ] Incluir Items si es necesario

#### Acción Edit (POST)
- [ ] Procesar parámetro `facturaFile`
- [ ] Validar extensión (.pdf, .jpg, .jpeg, .png)
- [ ] Validar tamaño (máximo 5MB recomendado)
- [ ] Crear carpeta `/wwwroot/facturas/` si no existe
- [ ] Generar nombre único para el archivo
- [ ] Guardar archivo en servidor
- [ ] Guardar ruta relativa en `Model.RutaFactura`
- [ ] Eliminar archivo anterior si existe
- [ ] Actualizar registro en BD

#### Acción Create (POST)
- [ ] Procesar parámetro `facturaFile`
- [ ] Aplicar mismas validaciones que Edit
- [ ] Guardar compra PRIMERO (para obtener ID)
- [ ] DESPUÉS guardar archivo (usando el ID de la compra)
- [ ] Guardar ruta relativa en BD

#### Acción DescargarFactura (NUEVA)
- [ ] Crear nueva acción `[HttpGet] public IActionResult DescargarFactura(int id)`
- [ ] Validar que la factura existe
- [ ] Validar que el archivo existe en el filesystem
- [ ] Retornar archivo con Content-Type apropiado
- [ ] Implementar método privado `GetContentType(string path)`
- [ ] Considerar agregar `[Authorize]` para seguridad

#### Acción Delete
- [ ] Modificar para eliminar archivo antes de eliminar registro
- [ ] Manejar excepciones si el archivo no existe
- [ ] Continuar con la eliminación aunque falle el archivo

### Métodos Helper
- [ ] Crear método `GetContentType(string path)` que retorne:
  - `application/pdf` para .pdf
  - `image/jpeg` para .jpg, .jpeg
  - `image/png` para .png
  - `application/octet-stream` por defecto

---

## ⏳ FASE 5: ESTRUCTURA DE ARCHIVOS (PENDIENTE)

### Crear Carpeta
- [ ] Crear directorio: `/wwwroot/facturas/`
- [ ] (Opcional) Crear archivo `.gitkeep` en la carpeta

### Estructura esperada
```
wwwroot/
└── facturas/
    ├── .gitkeep
    ├── 1_20240115141530_a7f2c9e1.pdf
    ├── 2_20240115141545_b8d3e4f2.jpg
    └── ...
```

---

## ⏳ FASE 6: CONFIGURACIÓN (PENDIENTE)

### Program.cs / Startup.cs
- [ ] Verificar que `app.UseStaticFiles()` está configurado
- [ ] Verificar que el middleware para archivos estáticos está habilitado
- [ ] Considerar agregar límite de tamaño si es necesario

**Código requerido en Program.cs:**
```csharp
app.UseStaticFiles();  // Para acceder a /wwwroot/facturas/
```

---

## ⏳ FASE 7: VALIDACIONES Y SEGURIDAD (PENDIENTE)

### Frontend (en vistas)
- [x] Validar extensiones en `accept` del input file
- [x] Mostrar solo archivos permitidos

### Backend (en controller)
- [ ] Validar extensiones en C#
- [ ] Validar tamaño de archivo
- [ ] Validar que el archivo es realmente del tipo indicado
- [ ] Sanitizar nombres de archivo
- [ ] Considerar agregar autenticación en `DescargarFactura`
- [ ] Considerar agregar autenticación en `Edit` y `Create`

### Seguridad adicional (RECOMENDADA)
- [ ] Agregar `[Authorize]` a acciones que manejan archivos
- [ ] Implementar validación de MIME type real
- [ ] Considerar escaneo de virus (ClamAV)
- [ ] Limitar tamaño total de descarga por sesión

---

## ⏳ FASE 8: TESTING (PENDIENTE)

### Pruebas Funcionales
- [ ] Crear compra SIN factura → ✓ Funciona
- [ ] Crear compra CON factura PDF → ✓ Funciona
- [ ] Crear compra CON factura JPG → ✓ Funciona
- [ ] Crear compra CON factura PNG → ✓ Funciona
- [ ] Descargar factura desde Create → ✓ Funciona
- [ ] Editar compra SIN factura anterior → ✓ Funciona
- [ ] Editar compra REEMPLAZANDO factura → ✓ Funciona
- [ ] Descargar factura desde Edit → ✓ Funciona
- [ ] Ver detalle CON factura → ✓ Funciona
- [ ] Ver detalle SIN factura → ✓ Funciona
- [ ] Descargar factura desde Details → ✓ Funciona
- [ ] Ver vista previa de imagen → ✓ Funciona
- [ ] Descargar factura desde Index → ✓ Funciona
- [ ] Eliminar compra CON factura → ✓ Funciona

### Pruebas de Validación
- [ ] Intentar cargar archivo > 5MB → ✓ Rechazado
- [ ] Intentar cargar archivo .exe → ✓ Rechazado
- [ ] Intentar cargar archivo .doc → ✓ Rechazado
- [ ] Cargar archivo con nombre especial → ✓ Funciona

### Pruebas de UI
- [ ] Label se actualiza al seleccionar archivo → ✓ Funciona
- [ ] Colores cambian correctamente → ✓ Funciona
- [ ] Hover effects funcionan → ✓ Funciona
- [ ] Links de descarga son clicables → ✓ Funciona
- [ ] Vista previa se muestra correctamente → ✓ Funciona
- [ ] Responsive design funciona en móvil → ✓ Funciona

---

## ⏳ FASE 9: DOCUMENTACIÓN (PENDIENTE)

- [ ] Actualizar README.md con instrucciones de configuración
- [ ] Documentar el endpoint `DescargarFactura`
- [ ] Documentar límites de tamaño de archivo
- [ ] Documentar tipos de archivo permitidos
- [ ] Crear guía de uso para administradores
- [ ] Documentar procedimiento de backup de facturas

---

## ⏳ FASE 10: OPTIMIZACIÓN (PENDIENTE - FUTURO)

### Performance
- [ ] Implementar generación de thumbnails
- [ ] Considerar caché de archivos frecuentes
- [ ] Optimizar consultas a BD para RutaFactura
- [ ] Comprimir PDFs si es necesario

### UX Improvements
- [ ] Drag and drop en input file
- [ ] Progress bar para descarga de archivos
- [ ] Preview modal para facturas
- [ ] Historial de cambios de factura

### Escalabilidad
- [ ] Considerar almacenamiento en Azure Blob
- [ ] Implementar sincronización con servidor de backup
- [ ] Considerar CDN para descargas frecuentes

---

## 📋 RESUMEN DE PROGRESO

| Fase | Estado | % |
|------|--------|---|
| 1. Vistas | ✅ Completado | 100% |
| 2. Modelo | ⏳ Pendiente | 0% |
| 3. BD | ⏳ Pendiente | 0% |
| 4. Controller | ⏳ Pendiente | 0% |
| 5. Carpetas | ⏳ Pendiente | 0% |
| 6. Configuración | ⏳ Pendiente | 0% |
| 7. Validaciones | ⏳ Pendiente | 0% |
| 8. Testing | ⏳ Pendiente | 0% |
| 9. Documentación | ⏳ Pendiente | 0% |
| 10. Optimización | ⏳ Futuro | 0% |

**TOTAL: 10% Completado**

---

## 🎯 Orden Recomendado de Ejecución

1. **Modelo** (5 min) - Agregar propiedad
2. **Base de Datos** (5 min) - Crear migración
3. **Carpeta** (2 min) - Crear /wwwroot/facturas/
4. **Controller** (30 min) - Implementar lógica
5. **Testing** (20 min) - Probar todos los casos
6. **Validaciones** (10 min) - Agregar seguridad
7. **Documentación** (10 min) - Documentar
8. **Optimización** (futuro) - Mejorar

**Tiempo estimado total: ~80-100 minutos**

---

## 📞 Puntos de Contacto del Código

### Vistas
- Archivo: `Views/Compras/Edit.cshtml`
- Archivo: `Views/Compras/Details.cshtml`
- Archivo: `Views/Compras/Index.cshtml`
- Acción del controller requerida: `DescargarFactura`

### Modelo
- Archivo: `Models/Compra.cs`
- Propiedad a agregar: `RutaFactura`

### Controller
- Archivo: `Controllers/ComprasController.cs`
- Acciones a modificar: `Edit` (POST), `Create` (POST), `Delete`
- Acción a crear: `DescargarFactura`

### Base de Datos
- Tabla: `Compras`
- Columna a agregar: `RutaFactura` (VARCHAR/String)

---

## 🔗 Documentación de Referencia

Archivos de documentación generados:
1. `IMPLEMENTACION_FACTURAS_BACKEND.md` - Código del controller
2. `RESUMEN_CAMBIOS_VISTAS.md` - Detalles de las vistas

---

## ✋ Notas Importantes

- ⚠️ Las vistas ya están 100% funcionales a nivel HTML/CSS
- ⚠️ El backend DEBE implementarse para que funcione completamente
- ⚠️ La carpeta `/wwwroot/facturas/` es CRÍTICA
- ⚠️ Las migraciones de BD son OBLIGATORIAS
- ⚠️ Considerar seguridad desde el inicio (validaciones, autorización)
