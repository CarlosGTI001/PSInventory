# 🌐 PSInventory.Web - Instrucciones de Setup

## ✅ Estado Actual

- ✅ **PSData** migrado a EF Core 8.0
- ✅ **PSInventory.Web** compilado correctamente
- ✅ Migración de base de datos creada
- ✅ **Seed de datos** configurado (crea usuario admin automáticamente)
- ⏳ **Falta:** Aplicar migración y ejecutar (solo desde Windows)

---

## 🚀 Setup Completo en 2 Pasos

### **Paso 1: Aplicar Migración**

**Abre PowerShell** en Windows y ejecuta:

```powershell
cd C:\Users\CarlosMasterPC1\Documents\PSInventory\PSInventory.Web

dotnet ef database update --project ..\PSData\PSData.csproj --context PSDatos
```

### **Paso 2: Ejecutar la Aplicación**

```powershell
dotnet run
```

**¡Eso es todo!** La primera vez que ejecutes la aplicación, se creará automáticamente:

- ✅ Usuario: **admin** / Contraseña: **admin123**
- ✅ 4 Categorías de ejemplo (Computadoras, Periféricos, etc.)
- ✅ 3 Regiones de ejemplo (Norte, Sur, Centro)

---

## 🔐 Credenciales de Acceso

- **Usuario:** `admin`
- **Contraseña:** `admin123`

---

## 🌐 Acceder a la Aplicación

Después de `dotnet run`, abre tu navegador en:

**https://localhost:5001**

O la URL que aparezca en la consola.

---

## 📊 Funcionalidades Actuales

### ✅ Implementado:
- **Autenticación** con sesiones
- **Dashboard** con 6 estadísticas en tiempo real
- **Alertas de stock** (items bajo mínimo)
- **Diseño Material Design 3** completo
- **Navegación responsiva** (drawer colapsable)

### ⏳ Pendiente de Implementar:
- **CRUD Categorías** (Create, Read, Update, Delete)
- **CRUD Regiones**
- **CRUD Sucursales**
- **CRUD Artículos**
- **CRUD Compras**
- **CRUD Items** (con serial number, garantía)
- **Operaciones:** Asignar Item a Sucursal
- **Operaciones:** Transferir Item entre Sucursales
- **Reportes en Excel**
- **Gráficas con Chart.js**

---

## 🗂️ Estructura del Proyecto

```
PSInventory.Web/
├── Controllers/
│   ├── AuthController.cs       # Login/Logout
│   └── HomeController.cs       # Dashboard con estadísticas
├── Views/
│   ├── Auth/
│   │   └── Login.cshtml        # Página de login
│   ├── Home/
│   │   └── Index.cshtml        # Dashboard
│   └── Shared/
│       └── _Layout.cshtml      # Layout principal con MD3
├── wwwroot/
│   ├── css/
│   │   ├── material-theme.css  # Tema MD3 personalizado
│   │   └── site.css            # Estilos del sitio
│   └── js/
│       └── site.js             # Funciones JS (loading, snackbar)
├── Program.cs                  # Configuración de la app
└── appsettings.json           # Connection string
```

---

## 🎨 Material Design 3

El proyecto usa **Material Web Components** oficiales de Google:

- Componentes desde CDN: `@material/web`
- Importmap configurado en `_Layout.cshtml`
- Tema personalizado con colores corporativos (#0061A8)
- Responsive design con breakpoints móviles

### Componentes disponibles:

```html
<!-- Botones -->
<md-filled-button>Guardar</md-filled-button>
<md-outlined-button>Cancelar</md-outlined-button>
<md-text-button>Ver más</md-text-button>

<!-- Campos de texto -->
<md-filled-text-field label="Nombre"></md-filled-text-field>

<!-- Iconos -->
<md-icon>add</md-icon>

<!-- Cards -->
<md-elevation></md-elevation>

<!-- Y más... -->
```

---

## 🔧 Configuración de Conexión

**Archivo:** `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PSInventoryDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

**Si usas SQL Server:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PSInventoryDB;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

---

## ⚠️ Problemas Comunes

### Error: "Cannot access the file 'PSData.dll'"

**Solución:** Cierra Visual Studio y todos los procesos de dotnet:

```powershell
Get-Process | Where-Object {$_.Name -like "*dotnet*"} | Stop-Process -Force
```

Luego vuelve a compilar.

### Error: "LocalDB is not supported"

Estás en WSL/Linux. Ejecuta los comandos desde **PowerShell en Windows**.

### La página no carga estilos

Verifica que `wwwroot/css/material-theme.css` y `wwwroot/css/site.css` existan.

---

## 📝 Próximos Pasos

1. ✅ Aplicar migración → Crear usuario → Ejecutar app
2. ⏳ Implementar **CRUD de Categorías** (siguiente prioridad)
3. ⏳ Implementar **CRUD de Regiones y Sucursales**
4. ⏳ Implementar **CRUD de Artículos** (con FK a Categoría)
5. ⏳ Implementar **CRUD de Compras** (con ítems múltiples)
6. ⏳ Implementar **CRUD de Items** (con estado, garantía)
7. ⏳ Implementar **Asignar Item** (operación)
8. ⏳ Implementar **Transferir Item** (operación)
9. ⏳ Agregar **gráficas** al Dashboard con Chart.js
10. ⏳ Implementar **exportación a Excel**

---

## 🤝 ¿Necesitas ayuda?

Si encuentras errores o tienes preguntas:

1. Verifica que la migración se aplicó correctamente
2. Revisa que exista al menos un usuario en la BD
3. Revisa la consola del navegador (F12) por errores JS
4. Revisa los logs de la app en la terminal

---

**¡La aplicación web está lista para funcionar!** 🎉

Solo falta aplicar la migración desde Windows y crear un usuario de prueba.
