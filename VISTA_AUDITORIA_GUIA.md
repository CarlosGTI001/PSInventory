# 📊 Vista de Auditoría - Guía de Implementación (Opcional)

## 🎯 Objetivo

Esta guía explica cómo crear una vista de administración para ver y restaurar registros eliminados (soft delete).

---

## 🏗️ Arquitectura

```
┌─────────────────────────────────────────────────┐
│          Vista de Auditoría                     │
│  /Auditoria/Index                               │
├─────────────────────────────────────────────────┤
│  📋 Filtros:                                    │
│    • Tipo de Entidad (Artículos, Compras, etc) │
│    • Rango de Fechas                            │
│    • Usuario que eliminó                        │
├─────────────────────────────────────────────────┤
│  📊 Tabla de Registros Eliminados:              │
│    • ID │ Nombre │ Fecha Elim. │ Usuario │ ⚙️  │
│    • 5  │ Cat-X  │ 2026-02-13  │ Admin   │ 🔄  │
│    • 12 │ Reg-Y  │ 2026-02-12  │ Jefe    │ 🔄  │
└─────────────────────────────────────────────────┘
```

---

## 📝 Paso 1: Crear Modelo de Vista (ViewModel)

**Ubicación**: `PSInventory.Web/ViewModels/RegistroEliminadoViewModel.cs`

```csharp
namespace PSInventory.Web.ViewModels
{
    public class RegistroEliminadoViewModel
    {
        public string TipoEntidad { get; set; }  // "Artículo", "Categoría", etc.
        public string Id { get; set; }  // ID del registro (puede ser int o string)
        public string Nombre { get; set; }  // Nombre/descripción del registro
        public DateTime? FechaEliminacion { get; set; }
        public string UsuarioEliminacion { get; set; }
        public string Detalles { get; set; }  // Info adicional del registro
    }

    public class AuditoriaIndexViewModel
    {
        public List<RegistroEliminadoViewModel> Registros { get; set; }
        public string TipoEntidadFiltro { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string UsuarioFiltro { get; set; }
    }
}
```

---

## 🎮 Paso 2: Crear Controlador

**Ubicación**: `PSInventory.Web/Controllers/AuditoriaController.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSInventory.Web.Filters;
using PSInventory.Web.ViewModels;

namespace PSInventory.Web.Controllers
{
    [AuthorizeRole("Administrador")]
    public class AuditoriaController : Controller
    {
        private readonly PSDatos _context;

        public AuditoriaController(PSDatos context)
        {
            _context = context;
        }

        // GET: Auditoria
        public async Task<IActionResult> Index(
            string tipoEntidad = "",
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null,
            string usuario = "")
        {
            var registros = new List<RegistroEliminadoViewModel>();

            // Obtener registros eliminados según filtros
            if (string.IsNullOrEmpty(tipoEntidad) || tipoEntidad == "Articulos")
            {
                var articulos = await _context.Articulos
                    .Include(a => a.Categoria)
                    .Where(a => a.Eliminado)
                    .ApplyFilters(fechaInicio, fechaFin, usuario)
                    .Select(a => new RegistroEliminadoViewModel
                    {
                        TipoEntidad = "Artículo",
                        Id = a.Id.ToString(),
                        Nombre = $"{a.Marca} {a.Modelo}",
                        FechaEliminacion = a.FechaEliminacion,
                        UsuarioEliminacion = a.UsuarioEliminacion,
                        Detalles = $"Categoría: {a.Categoria.Nombre}"
                    })
                    .ToListAsync();
                registros.AddRange(articulos);
            }

            if (string.IsNullOrEmpty(tipoEntidad) || tipoEntidad == "Categorias")
            {
                var categorias = await _context.Categorias
                    .Where(c => c.Eliminado)
                    .ApplyFilters(fechaInicio, fechaFin, usuario)
                    .Select(c => new RegistroEliminadoViewModel
                    {
                        TipoEntidad = "Categoría",
                        Id = c.Id.ToString(),
                        Nombre = c.Nombre,
                        FechaEliminacion = c.FechaEliminacion,
                        UsuarioEliminacion = c.UsuarioEliminacion,
                        Detalles = c.Descripcion
                    })
                    .ToListAsync();
                registros.AddRange(categorias);
            }

            if (string.IsNullOrEmpty(tipoEntidad) || tipoEntidad == "Compras")
            {
                var compras = await _context.Compras
                    .Where(c => c.Eliminado)
                    .ApplyFilters(fechaInicio, fechaFin, usuario)
                    .Select(c => new RegistroEliminadoViewModel
                    {
                        TipoEntidad = "Compra",
                        Id = c.Id.ToString(),
                        Nombre = $"Compra #{c.Id} - {c.Proveedor}",
                        FechaEliminacion = c.FechaEliminacion,
                        UsuarioEliminacion = c.UsuarioEliminacion,
                        Detalles = $"Fecha: {c.FechaCompra:dd/MM/yyyy} - Total: {c.Total:C}"
                    })
                    .ToListAsync();
                registros.AddRange(compras);
            }

            if (string.IsNullOrEmpty(tipoEntidad) || tipoEntidad == "Departamentos")
            {
                var departamentos = await _context.Departamentos
                    .Where(d => d.Eliminado)
                    .ApplyFilters(fechaInicio, fechaFin, usuario)
                    .Select(d => new RegistroEliminadoViewModel
                    {
                        TipoEntidad = "Departamento",
                        Id = d.Id.ToString(),
                        Nombre = d.Nombre,
                        FechaEliminacion = d.FechaEliminacion,
                        UsuarioEliminacion = d.UsuarioEliminacion,
                        Detalles = $"Responsable: {d.Responsable}"
                    })
                    .ToListAsync();
                registros.AddRange(departamentos);
            }

            if (string.IsNullOrEmpty(tipoEntidad) || tipoEntidad == "Items")
            {
                var items = await _context.Items
                    .Include(i => i.Articulo)
                    .Where(i => i.Eliminado)
                    .ApplyFilters(fechaInicio, fechaFin, usuario)
                    .Select(i => new RegistroEliminadoViewModel
                    {
                        TipoEntidad = "Item",
                        Id = i.Serial,
                        Nombre = $"{i.Serial} - {i.Articulo.Marca} {i.Articulo.Modelo}",
                        FechaEliminacion = i.FechaEliminacion,
                        UsuarioEliminacion = i.UsuarioEliminacion,
                        Detalles = $"Estado: {i.Estado} - Costo: {i.Costo:C}"
                    })
                    .ToListAsync();
                registros.AddRange(items);
            }

            if (string.IsNullOrEmpty(tipoEntidad) || tipoEntidad == "Regiones")
            {
                var regiones = await _context.Regiones
                    .Where(r => r.Eliminado)
                    .ApplyFilters(fechaInicio, fechaFin, usuario)
                    .Select(r => new RegistroEliminadoViewModel
                    {
                        TipoEntidad = "Región",
                        Id = r.Id.ToString(),
                        Nombre = r.Nombre,
                        FechaEliminacion = r.FechaEliminacion,
                        UsuarioEliminacion = r.UsuarioEliminacion,
                        Detalles = r.Descripcion
                    })
                    .ToListAsync();
                registros.AddRange(regiones);
            }

            if (string.IsNullOrEmpty(tipoEntidad) || tipoEntidad == "Sucursales")
            {
                var sucursales = await _context.Sucursales
                    .Include(s => s.Region)
                    .Where(s => s.Eliminado)
                    .ApplyFilters(fechaInicio, fechaFin, usuario)
                    .Select(s => new RegistroEliminadoViewModel
                    {
                        TipoEntidad = "Sucursal",
                        Id = s.Id,
                        Nombre = s.Nombre,
                        FechaEliminacion = s.FechaEliminacion,
                        UsuarioEliminacion = s.UsuarioEliminacion,
                        Detalles = $"Región: {s.Region.Nombre} - {s.Direccion}"
                    })
                    .ToListAsync();
                registros.AddRange(sucursales);
            }

            if (string.IsNullOrEmpty(tipoEntidad) || tipoEntidad == "Usuarios")
            {
                var usuarios = await _context.Usuarios
                    .Where(u => u.Eliminado)
                    .ApplyFilters(fechaInicio, fechaFin, usuario)
                    .Select(u => new RegistroEliminadoViewModel
                    {
                        TipoEntidad = "Usuario",
                        Id = u.Id,
                        Nombre = u.Nombre,
                        FechaEliminacion = u.FechaEliminacion,
                        UsuarioEliminacion = u.UsuarioEliminacion,
                        Detalles = $"Email: {u.Email} - Rol: {u.Rol}"
                    })
                    .ToListAsync();
                registros.AddRange(usuarios);
            }

            var viewModel = new AuditoriaIndexViewModel
            {
                Registros = registros.OrderByDescending(r => r.FechaEliminacion).ToList(),
                TipoEntidadFiltro = tipoEntidad,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                UsuarioFiltro = usuario
            };

            // ViewBag para filtros
            ViewBag.TiposEntidad = new List<string> 
            { 
                "Articulos", "Categorias", "Compras", "Departamentos", 
                "Items", "Regiones", "Sucursales", "Usuarios" 
            };

            return View(viewModel);
        }

        // POST: Auditoria/Restaurar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restaurar(string tipoEntidad, string id)
        {
            try
            {
                switch (tipoEntidad)
                {
                    case "Artículo":
                        var articulo = await _context.Articulos
                            .FirstOrDefaultAsync(a => a.Id.ToString() == id && a.Eliminado);
                        if (articulo != null)
                        {
                            articulo.Eliminado = false;
                            articulo.FechaEliminacion = null;
                            articulo.UsuarioEliminacion = null;
                            _context.Update(articulo);
                        }
                        break;

                    case "Categoría":
                        var categoria = await _context.Categorias
                            .FirstOrDefaultAsync(c => c.Id.ToString() == id && c.Eliminado);
                        if (categoria != null)
                        {
                            categoria.Eliminado = false;
                            categoria.FechaEliminacion = null;
                            categoria.UsuarioEliminacion = null;
                            _context.Update(categoria);
                        }
                        break;

                    case "Compra":
                        var compra = await _context.Compras
                            .FirstOrDefaultAsync(c => c.Id.ToString() == id && c.Eliminado);
                        if (compra != null)
                        {
                            compra.Eliminado = false;
                            compra.FechaEliminacion = null;
                            compra.UsuarioEliminacion = null;
                            _context.Update(compra);
                        }
                        break;

                    case "Departamento":
                        var departamento = await _context.Departamentos
                            .FirstOrDefaultAsync(d => d.Id.ToString() == id && d.Eliminado);
                        if (departamento != null)
                        {
                            departamento.Eliminado = false;
                            departamento.FechaEliminacion = null;
                            departamento.UsuarioEliminacion = null;
                            _context.Update(departamento);
                        }
                        break;

                    case "Item":
                        var item = await _context.Items
                            .FirstOrDefaultAsync(i => i.Serial == id && i.Eliminado);
                        if (item != null)
                        {
                            item.Eliminado = false;
                            item.FechaEliminacion = null;
                            item.UsuarioEliminacion = null;
                            _context.Update(item);
                        }
                        break;

                    case "Región":
                        var region = await _context.Regiones
                            .FirstOrDefaultAsync(r => r.Id.ToString() == id && r.Eliminado);
                        if (region != null)
                        {
                            region.Eliminado = false;
                            region.FechaEliminacion = null;
                            region.UsuarioEliminacion = null;
                            _context.Update(region);
                        }
                        break;

                    case "Sucursal":
                        var sucursal = await _context.Sucursales
                            .FirstOrDefaultAsync(s => s.Id == id && s.Eliminado);
                        if (sucursal != null)
                        {
                            sucursal.Eliminado = false;
                            sucursal.FechaEliminacion = null;
                            sucursal.UsuarioEliminacion = null;
                            _context.Update(sucursal);
                        }
                        break;

                    case "Usuario":
                        var usuario = await _context.Usuarios
                            .FirstOrDefaultAsync(u => u.Id == id && u.Eliminado);
                        if (usuario != null)
                        {
                            usuario.Eliminado = false;
                            usuario.FechaEliminacion = null;
                            usuario.UsuarioEliminacion = null;
                            _context.Update(usuario);
                        }
                        break;

                    default:
                        return Json(new { success = false, message = "Tipo de entidad no reconocido" });
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Registro restaurado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error al restaurar: {ex.Message}" });
            }
        }
    }

    // Método de extensión para filtros
    public static class QueryExtensions
    {
        public static IQueryable<T> ApplyFilters<T>(
            this IQueryable<T> query,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            string usuario) where T : class
        {
            if (fechaInicio.HasValue)
            {
                var prop = typeof(T).GetProperty("FechaEliminacion");
                if (prop != null)
                {
                    query = query.Where(e => 
                        EF.Property<DateTime?>(e, "FechaEliminacion") >= fechaInicio);
                }
            }

            if (fechaFin.HasValue)
            {
                var prop = typeof(T).GetProperty("FechaEliminacion");
                if (prop != null)
                {
                    query = query.Where(e => 
                        EF.Property<DateTime?>(e, "FechaEliminacion") <= fechaFin);
                }
            }

            if (!string.IsNullOrEmpty(usuario))
            {
                var prop = typeof(T).GetProperty("UsuarioEliminacion");
                if (prop != null)
                {
                    query = query.Where(e => 
                        EF.Property<string>(e, "UsuarioEliminacion").Contains(usuario));
                }
            }

            return query;
        }
    }
}
```

---

## 🎨 Paso 3: Crear Vista

**Ubicación**: `PSInventory.Web/Views/Auditoria/Index.cshtml`

```html
@model PSInventory.Web.ViewModels.AuditoriaIndexViewModel
@{
    ViewData["Title"] = "Auditoría de Registros Eliminados";
}

<style>
    .audit-container {
        padding: 24px;
        max-width: 1400px;
        margin: 0 auto;
    }

    .filters-card {
        background: white;
        border-radius: 16px;
        padding: 24px;
        margin-bottom: 24px;
        box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .filters-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
        gap: 16px;
        margin-bottom: 16px;
    }

    .audit-table {
        background: white;
        border-radius: 16px;
        overflow: hidden;
        box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .audit-table table {
        width: 100%;
        border-collapse: collapse;
    }

    .audit-table th {
        background: var(--md-sys-color-primary);
        color: white;
        padding: 16px;
        text-align: left;
        font-weight: 600;
    }

    .audit-table td {
        padding: 12px 16px;
        border-bottom: 1px solid #eee;
    }

    .audit-table tr:hover {
        background: #f5f5f5;
    }

    .entity-badge {
        display: inline-block;
        padding: 4px 12px;
        border-radius: 12px;
        font-size: 12px;
        font-weight: 600;
        background: var(--md-sys-color-secondary-container);
        color: var(--md-sys-color-on-secondary-container);
    }

    .restore-btn {
        background: var(--md-sys-color-tertiary);
        color: white;
        border: none;
        padding: 8px 16px;
        border-radius: 8px;
        cursor: pointer;
        font-weight: 500;
    }

    .restore-btn:hover {
        background: var(--md-sys-color-tertiary-container);
        color: var(--md-sys-color-on-tertiary-container);
    }
</style>

<div class="audit-container">
    <!-- Breadcrumb -->
    <nav class="breadcrumb" style="margin-bottom: 24px;">
        <a href="/" class="breadcrumb-item">
            <md-icon>home</md-icon>
            <span>Inicio</span>
        </a>
        <md-icon class="breadcrumb-separator">chevron_right</md-icon>
        <span class="breadcrumb-item active">
            <md-icon>history</md-icon>
            <span>Auditoría</span>
        </span>
    </nav>

    <!-- Header -->
    <div class="page-header" style="margin-bottom: 32px;">
        <h1 class="display-small">📊 Auditoría de Registros Eliminados</h1>
        <p class="body-large">Consulta y restaura registros eliminados del sistema</p>
    </div>

    <!-- Filtros -->
    <div class="filters-card">
        <h3 style="margin-bottom: 16px;">🔍 Filtros</h3>
        <form asp-action="Index" method="get">
            <div class="filters-grid">
                <div class="form-group">
                    <label for="tipoEntidad">Tipo de Entidad</label>
                    <select name="tipoEntidad" id="tipoEntidad" class="form-control">
                        <option value="">Todas</option>
                        @foreach (var tipo in ViewBag.TiposEntidad)
                        {
                            <option value="@tipo" selected="@(tipo == Model.TipoEntidadFiltro)">@tipo</option>
                        }
                    </select>
                </div>
                <div class="form-group">
                    <label for="fechaInicio">Fecha Inicio</label>
                    <input type="date" name="fechaInicio" id="fechaInicio" class="form-control" 
                           value="@Model.FechaInicio?.ToString("yyyy-MM-dd")" />
                </div>
                <div class="form-group">
                    <label for="fechaFin">Fecha Fin</label>
                    <input type="date" name="fechaFin" id="fechaFin" class="form-control" 
                           value="@Model.FechaFin?.ToString("yyyy-MM-dd")" />
                </div>
                <div class="form-group">
                    <label for="usuario">Usuario</label>
                    <input type="text" name="usuario" id="usuario" class="form-control" 
                           value="@Model.UsuarioFiltro" placeholder="Buscar por usuario..." />
                </div>
            </div>
            <div style="text-align: right;">
                <md-text-button type="button" onclick="window.location.href='@Url.Action("Index")'">
                    Limpiar
                </md-text-button>
                <md-filled-button type="submit">
                    <md-icon slot="icon">search</md-icon>
                    Buscar
                </md-filled-button>
            </div>
        </form>
    </div>

    <!-- Tabla de Resultados -->
    <div class="audit-table">
        <table>
            <thead>
                <tr>
                    <th>Tipo</th>
                    <th>ID</th>
                    <th>Nombre</th>
                    <th>Detalles</th>
                    <th>Fecha Eliminación</th>
                    <th>Usuario</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody>
                @if (Model.Registros.Any())
                {
                    @foreach (var registro in Model.Registros)
                    {
                        <tr>
                            <td><span class="entity-badge">@registro.TipoEntidad</span></td>
                            <td><strong>@registro.Id</strong></td>
                            <td>@registro.Nombre</td>
                            <td style="max-width: 300px; overflow: hidden; text-overflow: ellipsis;">@registro.Detalles</td>
                            <td>@registro.FechaEliminacion?.ToString("dd/MM/yyyy HH:mm")</td>
                            <td>@registro.UsuarioEliminacion</td>
                            <td>
                                <button type="button" class="restore-btn" 
                                        onclick="restaurarRegistro('@registro.TipoEntidad', '@registro.Id')">
                                    🔄 Restaurar
                                </button>
                            </td>
                        </tr>
                    }
                }
                else
                {
                    <tr>
                        <td colspan="7" style="text-align: center; padding: 40px;">
                            <md-icon style="font-size: 48px; color: #999;">inbox</md-icon>
                            <p style="color: #666; margin-top: 16px;">No se encontraron registros eliminados</p>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </div>

    <!-- Stats -->
    <div style="margin-top: 24px; text-align: center; color: #666;">
        <p><strong>Total de registros eliminados:</strong> @Model.Registros.Count</p>
    </div>
</div>

@section Scripts {
    <script>
        async function restaurarRegistro(tipoEntidad, id) {
            if (!confirm(`¿Está seguro de restaurar este registro?\n\nTipo: ${tipoEntidad}\nID: ${id}`)) {
                return;
            }

            try {
                const formData = new FormData();
                formData.append('tipoEntidad', tipoEntidad);
                formData.append('id', id);

                const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
                if (token) {
                    formData.append('__RequestVerificationToken', token);
                }

                const response = await fetch('@Url.Action("Restaurar", "Auditoria")', {
                    method: 'POST',
                    body: formData
                });

                const result = await response.json();

                if (result.success) {
                    alert('✅ ' + result.message);
                    window.location.reload();
                } else {
                    alert('❌ ' + result.message);
                }
            } catch (error) {
                alert('❌ Error al restaurar: ' + error.message);
            }
        }
    </script>
}

@Html.AntiForgeryToken()
```

---

## 🔗 Paso 4: Agregar al Menú

Editar `_Layout.cshtml` para agregar el link:

```html
<!-- Dentro del menú de Administrador -->
<div class="menu-section">
    <div class="menu-section-title">Sistema</div>
    <a href="/Usuarios" class="menu-item">
        <md-icon>group</md-icon>
        <span>Usuarios</span>
    </a>
    <a href="/Auditoria" class="menu-item">
        <md-icon>history</md-icon>
        <span>Auditoría</span>
    </a>
</div>
```

---

## ✅ Checklist de Implementación

- [ ] Crear carpeta `ViewModels` en `PSInventory.Web`
- [ ] Crear `RegistroEliminadoViewModel.cs` y `AuditoriaIndexViewModel.cs`
- [ ] Crear `AuditoriaController.cs`
- [ ] Crear carpeta `Views/Auditoria`
- [ ] Crear `Views/Auditoria/Index.cshtml`
- [ ] Agregar link al menú en `_Layout.cshtml`
- [ ] Probar la vista de auditoría
- [ ] Probar la restauración de registros

---

## 🎉 Resultado Final

Tendrás una vista completa de auditoría donde podrás:

✅ Ver todos los registros eliminados
✅ Filtrar por tipo de entidad, fechas, y usuario
✅ Restaurar registros con un solo clic
✅ Interfaz consistente con el diseño Material Design 3

---

**Nota**: Esta vista es completamente opcional. El sistema de soft delete funciona perfectamente sin ella.
