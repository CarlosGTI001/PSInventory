# 📚 PSInventory - Sistema de Gestión de Inventario

## 🎯 ¿Qué es PSInventory?

PSInventory es un sistema completo de gestión de inventario desarrollado en **ASP.NET Core** con **Material Design 3**. Permite gestionar artículos, items, compras, movimientos entre sucursales, y mucho más.

---

## 🚀 Características Principales

✅ **Gestión de Artículos y Categorías**
✅ **Control de Items con Números de Serie**
✅ **Sistema de Solicitudes de Compra con Departamentos**
✅ **Movimientos entre Sucursales**
✅ **Reportes y Estadísticas**
✅ **Borrado Lógico (Soft Delete)** - Nueva funcionalidad ⭐
✅ **Sistema de Auditoría Completa**
✅ **Interfaz Material Design 3 Estandarizada**

---

## 📖 Documentación Disponible

### 🔥 **Comienza Aquí**

| Documento | Descripción | Tiempo |
|-----------|-------------|--------|
| **[RESUMEN_SOFT_DELETE.md](RESUMEN_SOFT_DELETE.md)** ⭐ | Resumen del borrado lógico | 5 min |
| **[GUIA_RAPIDA.md](GUIA_RAPIDA.md)** | Guía rápida del sistema | 10 min |

### 🗑️ **Sistema de Borrado Lógico (Soft Delete)**

| Documento | Descripción |
|-----------|-------------|
| **[RESUMEN_SOFT_DELETE.md](RESUMEN_SOFT_DELETE.md)** | Resumen ejecutivo - **INICIO AQUÍ** |
| **[SOFT_DELETE_IMPLEMENTACION.md](SOFT_DELETE_IMPLEMENTACION.md)** | Guía técnica completa |
| **[VISTA_AUDITORIA_GUIA.md](VISTA_AUDITORIA_GUIA.md)** | Crear vista de auditoría (opcional) |

### 🛒 **Sistema de Compras y Departamentos**

| Documento | Descripción |
|-----------|-------------|
| **[INSTRUCCIONES_SOLICITUDES_COMPRA.md](INSTRUCCIONES_SOLICITUDES_COMPRA.md)** | Sistema de solicitudes de compra |
| **[IMPLEMENTACION_FACTURAS_BACKEND.md](IMPLEMENTACION_FACTURAS_BACKEND.md)** | Sistema de facturas escaneadas |

### 📊 **Reportes**

| Documento | Descripción |
|-----------|-------------|
| **[REPORTES_VIEW_DOCUMENTACION.md](REPORTES_VIEW_DOCUMENTACION.md)** | Documentación completa de reportes |
| **[REPORTES_QUICK_REFERENCE.md](REPORTES_QUICK_REFERENCE.md)** | Referencia rápida |
| **[RESUMEN_FINAL_REPORTES.md](RESUMEN_FINAL_REPORTES.md)** | Resumen final |

### 🎨 **Diseño e Interfaz**

| Documento | Descripción |
|-----------|-------------|
| **[ESTRUCTURA_CARDS.md](ESTRUCTURA_CARDS.md)** | Guía de Material Design 3 |
| **[RESUMEN_CAMBIOS_VISTAS.md](RESUMEN_CAMBIOS_VISTAS.md)** | Cambios en vistas |

### 🔧 **Solución de Problemas**

| Documento | Descripción |
|-----------|-------------|
| **[FIX_HTTP_400.md](FIX_HTTP_400.md)** | Solución a errores HTTP 400 |

---

## 📋 Pasos para Comenzar

### 1️⃣ **Aplicar Migraciones** (REQUERIDO)

Desde **Windows PowerShell**:

```powershell
cd C:\Users\CarlosMasterPC1\Documents\PSInventory
.\update-database.ps1
```

Esto aplicará:
- ✅ Migración de Departamentos
- ✅ Migración de Soft Delete

### 2️⃣ **Iniciar la Aplicación**

```powershell
cd PSInventory.Web
dotnet run
```

### 3️⃣ **Probar el Sistema**

1. Navega a `https://localhost:5001`
2. Inicia sesión
3. Prueba eliminar un registro
4. Verifica que funcione correctamente

---

## 🏗️ Arquitectura del Proyecto

```
PSInventory/
├── PSData/                          # Capa de Datos
│   ├── Datos/PSDatos.cs            # DbContext
│   ├── Modelos/                     # Modelos con Soft Delete
│   └── Migrations/                  # Migraciones de BD
│
├── PSInventory.Web/                 # Aplicación Web
│   ├── Controllers/                 # Controladores con Soft Delete
│   ├── Views/                       # Vistas Material Design 3
│   └── wwwroot/js/                  # JavaScript Components
│       ├── searchable-select.js    # Select con búsqueda
│       └── confirmation-dialogs.js # Diálogos de confirmación
│
└── Documentación/                   # 📚 Documentos
    ├── README.md                    # ⭐ Este archivo
    ├── RESUMEN_SOFT_DELETE.md      # ⭐ Soft Delete - Inicio aquí
    └── ... (otros documentos)
```

---

## ✨ Novedades de la Versión 2.0

### 🗑️ **Borrado Lógico (Soft Delete)**

Ahora cuando eliminas un registro:
- ✅ **NO se borra físicamente** de la base de datos
- ✅ Se marca como `Eliminado = true`
- ✅ Se guarda la **fecha** y el **usuario** que lo eliminó
- ✅ Se puede **restaurar** si fue un error
- ✅ Ideal para **auditoría** y **cumplimiento**

### 🏢 **Módulo de Departamentos**

- ✅ Gestión completa de departamentos
- ✅ Asociación con solicitudes de compra
- ✅ Seguimiento de responsables

### 🔍 **Searchable Select**

- ✅ Búsqueda en tiempo real en selectores
- ✅ Auto-activación para selects con >10 opciones
- ✅ Material Design Dialog

### ✅ **Confirmaciones Mejoradas**

- ✅ Diálogos de confirmación para eliminar
- ✅ Comparación de cambios al editar
- ✅ Información detallada antes de confirmar

---

## 🎓 Rutas de Aprendizaje

### Para Nuevos Desarrolladores

1. [README.md](README.md) ← **Estás aquí**
2. [GUIA_RAPIDA.md](GUIA_RAPIDA.md) - Entender el sistema
3. [ESTRUCTURA_CARDS.md](ESTRUCTURA_CARDS.md) - Diseño Material Design 3
4. [SOFT_DELETE_IMPLEMENTACION.md](SOFT_DELETE_IMPLEMENTACION.md) - Borrado lógico

### Para Implementar en Producción

1. [RESUMEN_SOFT_DELETE.md](RESUMEN_SOFT_DELETE.md) - Aplicar soft delete
2. [INSTRUCCIONES_SOLICITUDES_COMPRA.md](INSTRUCCIONES_SOLICITUDES_COMPRA.md) - Departamentos
3. [FIX_HTTP_400.md](FIX_HTTP_400.md) - Solucionar problemas

### Para Agregar Funcionalidades

1. [VISTA_AUDITORIA_GUIA.md](VISTA_AUDITORIA_GUIA.md) - Vista de auditoría
2. [REPORTES_QUICK_REFERENCE.md](REPORTES_QUICK_REFERENCE.md) - Nuevos reportes

---

## �� Estado del Proyecto

| Componente | Estado |
|------------|--------|
| **Soft Delete** | ✅ Completado - Pendiente migración |
| **Departamentos** | ✅ Completado - Pendiente migración |
| **Searchable Select** | ✅ Completado |
| **Material Design 3** | ✅ Completado |
| **HTTP 400 Fix** | ✅ Completado |
| **Confirmation Dialogs** | 🚧 Creado - No integrado |
| **Vista de Auditoría** | 📋 Opcional - Documentado |

---

## 📞 Soporte

### Preguntas Frecuentes

**P: ¿Cómo aplico las migraciones?**  
R: Ejecuta `.\update-database.ps1` desde Windows PowerShell

**P: ¿Puedo restaurar registros eliminados?**  
R: Sí, ver [VISTA_AUDITORIA_GUIA.md](VISTA_AUDITORIA_GUIA.md) o SQL directo

**P: ¿Cómo funcionan los departamentos?**  
R: Ver [INSTRUCCIONES_SOLICITUDES_COMPRA.md](INSTRUCCIONES_SOLICITUDES_COMPRA.md)

**P: Tengo un error HTTP 400 en los formularios**  
R: Ver [FIX_HTTP_400.md](FIX_HTTP_400.md)

---

## 📊 Estadísticas del Proyecto

- **8 Modelos** con Soft Delete
- **8 Controladores** actualizados
- **17 Vistas** estandarizadas
- **3 Migraciones** pendientes de aplicar
- **2 JavaScript Components** nuevos
- **15+ Documentos** de guía

---

## 🎉 ¡Gracias!

El sistema PSInventory ahora cuenta con:
- ✅ Borrado lógico completo
- ✅ Sistema de departamentos
- ✅ Interfaz estandarizada
- ✅ Auditoría mejorada

**Versión**: 2.0  
**Última actualización**: 13 de Febrero 2026  
**Estado**: ✅ Listo para aplicar migraciones

---

**📚 [Ver Índice Completo](INDICE_DOCUMENTACION.md)** | **⭐ [Comenzar con Soft Delete](RESUMEN_SOFT_DELETE.md)**
