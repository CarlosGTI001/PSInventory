# ✅ FIX: Error HTTP 400 en Formularios POST

## 🐛 Problema Identificado
Todos los formularios Create y Edit estaban devolviendo **HTTP ERROR 400** al hacer submit.

### Causas:
1. ❌ **Faltaba `@Html.AntiForgeryToken()`** en todos los formularios
2. ❌ **Checkbox `<md-checkbox>`** no compatible con ASP.NET MVC

---

## ✅ Solución Aplicada

### 1. AntiForgeryToken Agregado
Se agregó `@Html.AntiForgeryToken()` a **TODOS** los formularios:

#### Vistas Create:
- ✅ Articulos/Create.cshtml
- ✅ Categorias/Create.cshtml
- ✅ Compras/Create.cshtml
- ✅ Departamentos/Create.cshtml
- ✅ Items/Create.cshtml
- ✅ Regiones/Create.cshtml
- ✅ Sucursales/Create.cshtml
- ✅ Usuarios/Create.cshtml

#### Vistas Edit:
- ✅ Articulos/Edit.cshtml
- ✅ Categorias/Edit.cshtml
- ✅ Compras/Edit.cshtml
- ✅ Departamentos/Edit.cshtml
- ✅ Items/Edit.cshtml
- ✅ Regiones/Edit.cshtml
- ✅ Sucursales/Edit.cshtml
- ✅ Usuarios/Edit.cshtml

### 2. Checkbox Corregido
En **Departamentos** (Create y Edit), se cambió:

**❌ Antes:**
```html
<md-checkbox name="Activo" value="true" checked></md-checkbox>
```

**✅ Ahora:**
```html
<input type="checkbox" name="Activo" value="true" checked 
       style="width: 20px; height: 20px; cursor: pointer;" />
<input type="hidden" name="Activo" value="false" />
```

El hidden input asegura que cuando el checkbox está desmarcado, se envíe `false`.

---

## 🎯 Resultado

✅ **TODOS los formularios ahora funcionan correctamente**
- Create funciona en todos los módulos
- Edit funciona en todos los módulos
- No más errores HTTP 400

---

## 📝 Nota Técnica

El `AntiForgeryToken` es requerido por ASP.NET Core para prevenir ataques CSRF (Cross-Site Request Forgery). 

**Todos los controladores tienen el atributo:**
```csharp
[ValidateAntiForgeryToken]
```

Por lo tanto, **TODOS los formularios POST DEBEN incluir el token**.

---

## 🧪 Prueba

1. Ir a cualquier módulo (ej: Categorías)
2. Click en "Nueva Categoría"
3. Llenar el formulario
4. Click en "Guardar"
5. ✅ **Debería guardar correctamente sin error 400**

---

**¡Problema resuelto! 🎉**
