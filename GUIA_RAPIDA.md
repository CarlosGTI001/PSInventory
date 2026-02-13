# Guía Rápida - Facturas Escaneadas en Compras

## 🚀 Start Here

Las vistas han sido 100% actualizadas y están listas. Necesitas completar el backend.

### Archivos a Consultar

1. **Vistas (YA COMPLETADAS):**
   - ✅ `/Views/Compras/Edit.cshtml`
   - ✅ `/Views/Compras/Details.cshtml`
   - ✅ `/Views/Compras/Index.cshtml`

2. **Documentación:**
   - 📄 `IMPLEMENTACION_FACTURAS_BACKEND.md` ← COPIA EL CÓDIGO
   - 📄 `RESUMEN_CAMBIOS_VISTAS.md` ← Lee los detalles
   - 📄 `CHECKLIST_IMPLEMENTACION.md` ← Sigue el plan

---

## ⚡ Implementación Rápida (80 minutos)

### 1. Modelo (5 min)

**Archivo:** `Models/Compra.cs`

```csharp
public string RutaFactura { get; set; }  // AGREGAR ESTA LÍNEA
```

### 2. Base de Datos (5 min)

```bash
dotnet ef migrations add AddRutaFacturaToCompra
dotnet ef database update
```

### 3. Carpeta (2 min)

```bash
mkdir wwwroot/facturas
```

### 4. Controller (30 min)

**Archivo:** `Controllers/ComprasController.cs`

Copia el código completo de `IMPLEMENTACION_FACTURAS_BACKEND.md`:

- Actualiza `Edit(POST)` ← Procesar archivo
- Actualiza `Create(POST)` ← Procesar archivo  
- Crea `DescargarFactura()` ← Nueva acción
- Actualiza `DeleteConfirmed()` ← Limpiar archivo

### 5. Testing (20 min)

- [ ] Crear compra con factura
- [ ] Editar y cambiar factura
- [ ] Descargar factura
- [ ] Ver preview en Details
- [ ] Eliminar compra

### 6. Validaciones (10 min)

Agregar `[Authorize]` si es necesario (seguridad).

### 7. Documentar (5 min)

Actualizar README.md con instrucciones.

---

## 📌 Cambios en Vistas (Reference)

### Edit.cshtml
- Línea 27: Form con `enctype="multipart/form-data"`
- Líneas 126-161: Preview de factura actual
- Líneas 163-186: Input file
- Líneas 233-259: JavaScript updateFileLabel()

### Details.cshtml
- Líneas 49-85: Nueva fila "Factura" con preview

### Index.cshtml
- Líneas 154-182: Sección "Factura adjunta" en tarjetas

---

## 🔗 Links Importantes

### URLs que usan las vistas:
```html
@Url.Action("DescargarFactura", new { id = Model.Id })
@Url.Action("DescargarFactura", new { id = compra.Id })
```

**Necesitas implementar esta acción en el controller.**

---

## 💾 Archivos Afectados

```
PSInventory/
├── Views/Compras/
│   ├── Edit.cshtml          ← MODIFICADO
│   ├── Details.cshtml       ← MODIFICADO
│   ├── Index.cshtml         ← MODIFICADO
│   └── Create.cshtml        ← NO MODIFICADO (referencia)
│
├── Models/
│   └── Compra.cs            ← AGREGA RutaFactura
│
├── Controllers/
│   └── ComprasController.cs ← IMPLEMENTA LOGICA
│
├── wwwroot/
│   └── facturas/            ← CREAR CARPETA
│
└── Documentación/
    ├── IMPLEMENTACION_FACTURAS_BACKEND.md
    ├── RESUMEN_CAMBIOS_VISTAS.md
    ├── CHECKLIST_IMPLEMENTACION.md
    └── GUIA_RAPIDA.md (este archivo)
```

---

## ✅ Checklist de Implementación Rápida

### Fase 1: Configuración Base (15 min)
- [ ] Agregar RutaFactura a modelo
- [ ] Crear migración EF Core
- [ ] Crear carpeta /wwwroot/facturas/

### Fase 2: Controller (30 min)
- [ ] Copiar código de Edit(POST)
- [ ] Copiar código de Create(POST)
- [ ] Copiar código de DescargarFactura()
- [ ] Copiar código de DeleteConfirmed()

### Fase 3: Testing (20 min)
- [ ] Crear compra CON factura
- [ ] Descargar factura
- [ ] Editar y cambiar factura
- [ ] Eliminar y verificar archivo

### Fase 4: Polish (15 min)
- [ ] Agregar validaciones extra
- [ ] Agregar autorización si es necesario
- [ ] Actualizar documentación

---

## 🐛 Troubleshooting

### "No such file or directory" en descarga
→ Verifica que `/wwwroot/facturas/` existe

### "Archivo no encontrado"
→ Verifica que RutaFactura se guardó en BD

### "El form no envía archivo"
→ Verifica `enctype="multipart/form-data"` en Edit.cshtml

### Links de descarga no funcionan
→ Verifica que `DescargarFactura()` está implementada en controller

---

## 📚 Referencia de Código

### Copiar de IMPLEMENTACION_FACTURAS_BACKEND.md:

1. **Edit (POST)** - líneas ~50-120
2. **Create (POST)** - líneas ~120-200
3. **DescargarFactura()** - líneas ~200-250
4. **DeleteConfirmed()** - líneas ~250-280

---

## 🎯 Objetivo Final

Cuando termines, tendrás:

✅ Poder cargar facturas en Create
✅ Poder reemplazar facturas en Edit
✅ Ver factura actual con preview en Edit
✅ Ver factura con preview en Details
✅ Ver factura en lista (Index)
✅ Descargar facturas desde cualquier vista
✅ Archivos se eliminan al eliminar compra

---

## 💡 Tips

- **RutaFactura** almacena la ruta relativa: `/facturas/1_20240115_abc123.pdf`
- **Nombres únicos** se generan con: `{CompraId}_{timestamp}_{GUID}`
- **Archivos anteriores** se eliminan al actualizar
- **Carpeta** debe ser `/wwwroot/facturas/` (no otro lugar)

---

## 📞 Preguntas Frecuentes

**P: ¿Puedo cambiar la ruta de almacenamiento?**
R: Sí, pero debe ser dentro de `/wwwroot/` para que sean accesibles

**P: ¿Qué tamaño máximo de archivo?**
R: 5MB recomendado (configurable en código)

**P: ¿Puedo agregar más extensiones?**
R: Sí, modifica `accept=".pdf,.jpg,.jpeg,.png"` y la validación en C#

**P: ¿Necesito hacer backup de las facturas?**
R: Sí, la carpeta `/wwwroot/facturas/` debe estar en tu estrategia de backup

---

## 🔄 Próximas Mejoras (Futuro)

- [ ] Thumbnails automáticos para imágenes
- [ ] Validación de MIME type real
- [ ] Historial de cambios de factura
- [ ] Escaneo de virus (ClamAV)
- [ ] Almacenamiento en Azure Blob
- [ ] Compresión automática de PDFs

---

Última actualización: Hoy
Estado: ✅ 100% Listo para implementar
