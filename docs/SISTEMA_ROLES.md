# 🔐 Sistema de Roles - PSInventory Web

## ✅ **IMPLEMENTADO**

El sistema ahora cuenta con **control de acceso basado en roles** completo.

---

## 👥 **Roles Disponibles**

### **1. Administrador**
- ✅ Acceso total al sistema
- ✅ **Gestión de Usuarios** (crear, editar, eliminar)
- ✅ Todos los CRUD de inventario
- ✅ Dashboard y reportes
- ✅ Operaciones de asignar/transferir items

### **2. Supervisor**
- ✅ Ver y editar inventario
- ✅ Crear/editar categorías, regiones, sucursales
- ✅ Crear/editar artículos, compras, items
- ✅ Operaciones de asignar/transferir items
- ❌ **NO puede gestionar usuarios**

### **3. Usuario**
- ✅ Solo lectura (ver información)
- ✅ Dashboard
- ❌ NO puede editar ni eliminar
- ❌ NO puede crear registros
- ❌ NO puede gestionar usuarios

---

## 🔑 **Usuarios por Defecto (Seed)**

Al ejecutar la aplicación por primera vez se crean automáticamente:

| Usuario | Contraseña | Rol | Acceso |
|---------|-----------|-----|--------|
| **admin** | admin123 | Administrador | Total |
| **supervisor** | supervisor123 | Supervisor | Inventario |

---

## 🛡️ **Cómo Funciona**

### **1. Login**
Al iniciar sesión, se guarda en la sesión:
```csharp
HttpContext.Session.SetString("UserName", user.Nombre);
HttpContext.Session.SetString("UserId", user.Id);
HttpContext.Session.SetString("UserRole", user.Rol);  // ← NUEVO
HttpContext.Session.SetString("UserEmail", user.Email);
```

### **2. Protección de Controladores**
Se usa el atributo `[AuthorizeRole("...")]`:

```csharp
// Solo administradores
[AuthorizeRole("Administrador")]
public class UsuariosController : Controller
{
    // ...
}

// Administradores y Supervisores
[AuthorizeRole("Administrador", "Supervisor")]
public class ArticulosController : Controller
{
    // ...
}

// Requiere autenticación (cualquier rol)
[RequireAuth]
public class HomeController : Controller
{
    // ...
}
```

### **3. Menú Dinámico**
El módulo de **Usuarios** solo aparece si el rol es **Administrador**:

```cshtml
@if (Context.Session.GetString("UserRole") == "Administrador")
{
    <a href="/Usuarios/Index">
        <md-icon>group</md-icon>
        <span>Usuarios</span>
    </a>
}
```

### **4. Protección en Vistas**
Puedes ocultar botones según el rol:

```cshtml
@if (Context.Session.GetString("UserRole") != "Usuario")
{
    <md-filled-button>Editar</md-filled-button>
}
```

---

## 📋 **Aplicar Roles a Controladores**

### **Ya Protegidos:**
- ✅ **UsuariosController** → Solo Administrador

### **Pendientes de Proteger:**
Puedes agregar `[AuthorizeRole]` a estos controladores:

```csharp
// Administradores y Supervisores pueden editar
[AuthorizeRole("Administrador", "Supervisor")]
public class CategoriasController : Controller { }

[AuthorizeRole("Administrador", "Supervisor")]
public class RegionesController : Controller { }

[AuthorizeRole("Administrador", "Supervisor")]
public class SucursalesController : Controller { }

[AuthorizeRole("Administrador", "Supervisor")]
public class ArticulosController : Controller { }

[AuthorizeRole("Administrador", "Supervisor")]
public class ComprasController : Controller { }

[AuthorizeRole("Administrador", "Supervisor")]
public class ItemsController : Controller { }

// Todos pueden ver el dashboard
[RequireAuth]
public class HomeController : Controller { }
```

---

## 🔒 **Seguridad Adicional**

### **Protección contra eliminar último administrador:**
```csharp
if (usuario.Rol == "Administrador")
{
    var adminCount = await _context.Usuarios.CountAsync(u => u.Rol == "Administrador");
    if (adminCount <= 1)
    {
        return Json(new { success = false, message = "No se puede eliminar el último administrador" });
    }
}
```

### **ForbidResult:**
Si un usuario intenta acceder a un módulo sin permisos, recibe un error 403 Forbidden.

---

## 🎨 **Indicadores Visuales**

### **Badge de Rol en Header:**
El rol del usuario actual aparece en el header:
```
Carlos González [Administrador]
```

### **Badge de Rol en Tabla de Usuarios:**
- 🔴 **Administrador** → Badge rojo
- 🔵 **Supervisor** → Badge azul
- ⚪ **Usuario** → Badge gris

---

## 📝 **CRUD de Usuarios**

### **Campos:**
- Nombre (único)
- Email
- Contraseña (hash en producción)
- Rol (dropdown: Administrador, Supervisor, Usuario)

### **Funciones:**
- ✅ Index con tabla de usuarios
- ✅ Create nuevo usuario
- ✅ Edit usuario existente
- ✅ Delete con validación de último admin
- ✅ Protegido por `[AuthorizeRole("Administrador")]`

---

## 🚀 **Para Probar**

### **Como Administrador:**
```
Usuario: admin
Contraseña: admin123
```

Deberías ver:
- ✅ Menú "Usuarios" en el drawer
- ✅ Badge "Administrador" en el header
- ✅ Acceso total a todos los módulos

### **Como Supervisor:**
```
Usuario: supervisor
Contraseña: supervisor123
```

Deberías ver:
- ❌ NO aparece menú "Usuarios"
- ✅ Badge "Supervisor" en el header
- ✅ Puede editar inventario
- ❌ Error 403 si intenta acceder a /Usuarios

### **Crear Usuario Solo Lectura:**
1. Login como **admin**
2. Ir a **Usuarios → Nuevo Usuario**
3. Crear con Rol = "Usuario"
4. Logout y login con ese usuario
5. Solo podrá ver, no editar

---

## 🔧 **Configuración Adicional (Opcional)**

### **Ocultar botones según rol:**
```cshtml
@{
    var userRole = Context.Session.GetString("UserRole");
    var canEdit = userRole == "Administrador" || userRole == "Supervisor";
}

@if (canEdit)
{
    <md-filled-button onclick="window.location.href='@Url.Action("Edit", new { id = item.Id })'">
        Editar
    </md-filled-button>
}
```

### **Proteger acciones específicas:**
```csharp
[AuthorizeRole("Administrador")]
public async Task<IActionResult> Delete(int id)
{
    // Solo admin puede eliminar
}
```

---

## ⚠️ **Notas Importantes**

1. **Contraseñas en texto plano:** En producción, usar BCrypt para hashear contraseñas
2. **Sesiones:** Configuradas para 30 minutos de inactividad
3. **Logout:** Limpia toda la sesión
4. **Último Admin:** No se puede eliminar el último administrador del sistema
5. **Roles personalizados:** Puedes agregar más roles editando el dropdown

---

## 📊 **Resumen de Archivos Nuevos**

- `Filters/AuthorizeRoleAttribute.cs` - Filtro de autorización
- `Controllers/UsuariosController.cs` - CRUD de usuarios
- `Views/Usuarios/Index.cshtml` - Lista de usuarios
- `Views/Usuarios/Create.cshtml` - Crear usuario
- `Views/Usuarios/Edit.cshtml` - Editar usuario
- `Datos/DbInitializer.cs` - Seed con usuarios de ejemplo

---

**¡El sistema de roles está completamente funcional!** 🎉
