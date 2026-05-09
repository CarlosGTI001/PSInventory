using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;
using PSInventory.Web.Models.ViewModels;
using PSInventory.Web.Services;

namespace PSInventory.Web.Controllers
{
    [RequireAuth]
    public class MovimientosController : Controller
    {
        private readonly PSDatos _context;

        public MovimientosController(PSDatos context)
        {
            _context = context;
        }

        private static IQueryable<MovimientoItem> ExcluirEliminadosLogicos(IQueryable<MovimientoItem> query)
        {
            return query.Where(m =>
                m.Observaciones == null ||
                !m.Observaciones.Contains(MovimientoLogicoService.MarcaEliminadoLogico));
        }

        private static bool EsMotivoEnvio(string? motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo))
            {
                return false;
            }

            return motivo.StartsWith("Despacho", StringComparison.OrdinalIgnoreCase)
                   || motivo.StartsWith("Salida Sin Registro", StringComparison.OrdinalIgnoreCase);
        }

        // GET: Movimientos
        public async Task<IActionResult> Index(
            string sucursalId = "",
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null,
            string motivo = "",
            string usuario = "",
            string estadoItem = "",
            string q = "",
            bool incluirEliminados = false,
            int page = 1,
            int pageSize = 30)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 10 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var query = _context.MovimientosItem
                .Include(m => m.Item)
                    .ThenInclude(i => i.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(m => m.SucursalOrigen)
                .Include(m => m.SucursalDestino)
                .AsNoTracking()
                .AsQueryable();

            if (!incluirEliminados)
            {
                query = ExcluirEliminadosLogicos(query);
            }

            // Filtros
            if (!string.IsNullOrEmpty(sucursalId))
            {
                query = query.Where(m => m.SucursalOrigenId == sucursalId || m.SucursalDestinoId == sucursalId);
            }

            if (fechaInicio.HasValue)
            {
                query = query.Where(m => m.FechaMovimiento >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                query = query.Where(m => m.FechaMovimiento <= fechaFin.Value.AddDays(1).AddSeconds(-1));
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(m =>
                    (m.Item != null && m.Item.Serial != null && m.Item.Serial.ToLower().Contains(term)) ||
                    (m.Item != null && m.Item.Articulo != null && m.Item.Articulo.Marca.ToLower().Contains(term)) ||
                    (m.Item != null && m.Item.Articulo != null && m.Item.Articulo.Modelo.ToLower().Contains(term)) ||
                    (m.Motivo != null && m.Motivo.ToLower().Contains(term)) ||
                    (m.Observaciones != null && m.Observaciones.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(motivo))
            {
                query = query.Where(m => m.Motivo == motivo);
            }

            if (!string.IsNullOrWhiteSpace(usuario))
            {
                var usuarioTerm = usuario.Trim().ToLower();
                query = query.Where(m => m.UsuarioResponsable != null && m.UsuarioResponsable.ToLower().Contains(usuarioTerm));
            }

            if (!string.IsNullOrWhiteSpace(estadoItem))
            {
                query = query.Where(m => m.Item != null && m.Item.Estado == estadoItem);
            }

            var hoy = DateTime.Today;
            var totalCount = await query.CountAsync();
            var totalAsignaciones = await query.CountAsync(m => m.Motivo == "Asignación Inicial");
            var totalTransferencias = await query.CountAsync(m => m.Motivo == "Transferencia");
            var totalHoy = await query.CountAsync(m => m.FechaMovimiento >= hoy && m.FechaMovimiento < hoy.AddDays(1));
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var movimientos = await query
                .OrderByDescending(m => m.FechaMovimiento)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Datos para filtros - solo sucursales activas
            ViewBag.Sucursales = await _context.Sucursales
                .Where(s => !s.Eliminado)
                .OrderBy(s => s.Nombre)
                .ToListAsync();
            ViewBag.SucursalFiltro = sucursalId;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");
            ViewBag.MotivoFiltro = motivo;
            ViewBag.UsuarioFiltro = usuario;
            ViewBag.EstadoItemFiltro = estadoItem;
            ViewBag.Query = q;
            ViewBag.IncluirEliminados = incluirEliminados;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalAsignaciones = totalAsignaciones;
            ViewBag.TotalTransferencias = totalTransferencias;
            ViewBag.TotalHoy = totalHoy;
            ViewBag.Motivos = await _context.MovimientosItem
                .AsNoTracking()
                .Where(m => incluirEliminados
                    || m.Observaciones == null
                    || !m.Observaciones.Contains(MovimientoLogicoService.MarcaEliminadoLogico))
                .Where(m => !string.IsNullOrWhiteSpace(m.Motivo))
                .Select(m => m.Motivo)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();

            return View(movimientos);
        }

        // GET: Movimientos/ExportarCsv
        public async Task<IActionResult> ExportarCsv(
            string sucursalId = "",
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null,
            string motivo = "",
            string usuario = "",
            string estadoItem = "",
            string q = "",
            bool incluirEliminados = false)
        {
            var query = _context.MovimientosItem
                .Include(m => m.Item)
                    .ThenInclude(i => i.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(m => m.SucursalOrigen)
                .Include(m => m.SucursalDestino)
                .AsNoTracking()
                .AsQueryable();

            if (!incluirEliminados)
            {
                query = ExcluirEliminadosLogicos(query);
            }

            if (!string.IsNullOrEmpty(sucursalId))
            {
                query = query.Where(m => m.SucursalOrigenId == sucursalId || m.SucursalDestinoId == sucursalId);
            }

            if (fechaInicio.HasValue)
            {
                query = query.Where(m => m.FechaMovimiento >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                query = query.Where(m => m.FechaMovimiento <= fechaFin.Value.AddDays(1).AddSeconds(-1));
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(m =>
                    (m.Item != null && m.Item.Serial != null && m.Item.Serial.ToLower().Contains(term)) ||
                    (m.Item != null && m.Item.Articulo != null && m.Item.Articulo.Marca.ToLower().Contains(term)) ||
                    (m.Item != null && m.Item.Articulo != null && m.Item.Articulo.Modelo.ToLower().Contains(term)) ||
                    (m.Motivo != null && m.Motivo.ToLower().Contains(term)) ||
                    (m.Observaciones != null && m.Observaciones.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(motivo))
            {
                query = query.Where(m => m.Motivo == motivo);
            }

            if (!string.IsNullOrWhiteSpace(usuario))
            {
                var usuarioTerm = usuario.Trim().ToLower();
                query = query.Where(m => m.UsuarioResponsable != null && m.UsuarioResponsable.ToLower().Contains(usuarioTerm));
            }

            if (!string.IsNullOrWhiteSpace(estadoItem))
            {
                query = query.Where(m => m.Item != null && m.Item.Estado == estadoItem);
            }

            var movimientos = await query
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();

            var headers = new[]
            {
                "Fecha", "Hora", "ItemId", "Serial", "Articulo", "Categoria", "Origen", "Destino", "Motivo", "Usuario", "Cantidad", "EstadoItem", "ResponsableRecepcion", "FechaRecepcion", "Observaciones"
            };

            var rows = movimientos.Select(m => new[]
            {
                m.FechaMovimiento.ToString("dd/MM/yyyy"),
                m.FechaMovimiento.ToString("HH:mm"),
                m.ItemId.ToString(),
                m.Item?.Serial ?? string.Empty,
                $"{m.Item?.Articulo?.Marca} {m.Item?.Articulo?.Modelo}".Trim(),
                m.Item?.Articulo?.Categoria?.Nombre ?? string.Empty,
                m.SucursalOrigen?.Nombre ?? "Almacen",
                m.SucursalDestino?.Nombre ?? string.Empty,
                m.Motivo ?? string.Empty,
                m.UsuarioResponsable ?? string.Empty,
                m.Cantidad.ToString(),
                m.Item?.Estado ?? string.Empty,
                m.ResponsableRecepcion ?? string.Empty,
                m.FechaRecepcion?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                m.Observaciones ?? string.Empty
            });

            var bytes = CsvExportService.BuildCsv(headers, rows);
            return File(bytes, "text/csv; charset=utf-8", $"movimientos_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }

        // GET: Movimientos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimiento = await _context.MovimientosItem
                .Include(m => m.Item)
                    .ThenInclude(i => i.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(m => m.SucursalOrigen)
                .Include(m => m.SucursalDestino)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movimiento == null)
            {
                return NotFound();
            }

            return View(movimiento);
        }

        // GET: Movimientos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimiento = await _context.MovimientosItem
                .Include(m => m.Item)
                    .ThenInclude(i => i.Articulo)
                .Include(m => m.SucursalDestino)
                .FirstOrDefaultAsync(m => m.Id == id.Value);

            if (movimiento == null)
            {
                return NotFound();
            }

            if (MovimientoLogicoService.EsEliminadoLogico(movimiento.Observaciones))
            {
                TempData["Error"] = "No se puede editar una salida anulada lógicamente.";
                return RedirectToAction(nameof(Index), new { incluirEliminados = true });
            }

            var ultimoMovimientoId = await _context.MovimientosItem
                .Where(m => m.ItemId == movimiento.ItemId)
                .Where(m => m.Observaciones == null || !m.Observaciones.Contains(MovimientoLogicoService.MarcaEliminadoLogico))
                .OrderByDescending(m => m.FechaMovimiento)
                .ThenByDescending(m => m.Id)
                .Select(m => m.Id)
                .FirstOrDefaultAsync();

            var model = new MovimientoEditViewModel
            {
                Id = movimiento.Id,
                ItemId = movimiento.ItemId,
                ItemCodigo = movimiento.Item?.Serial ?? $"ID: {movimiento.ItemId}",
                ArticuloNombre = movimiento.Item?.Articulo != null
                    ? $"{movimiento.Item.Articulo.Marca} {movimiento.Item.Articulo.Modelo}"
                    : "Sin artículo",
                SucursalDestinoId = movimiento.SucursalDestinoId,
                ResponsableRecepcion = movimiento.ResponsableRecepcion ?? string.Empty,
                Motivo = movimiento.Motivo,
                Observaciones = movimiento.Observaciones ?? string.Empty,
                FechaRecepcion = movimiento.FechaRecepcion,
                FechaMovimiento = movimiento.FechaMovimiento,
                EsUltimoMovimientoDelItem = ultimoMovimientoId == movimiento.Id
            };

            ViewBag.Sucursales = new SelectList(
                await _context.Sucursales
                    .Where(s => !s.Eliminado)
                    .OrderBy(s => s.Nombre)
                    .ToListAsync(),
                "Id",
                "Nombre",
                model.SucursalDestinoId);

            return View(model);
        }

        // POST: Movimientos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovimientoEditViewModel model)
        {
            var movimiento = await _context.MovimientosItem
                .Include(m => m.Item)
                .FirstOrDefaultAsync(m => m.Id == model.Id);

            if (movimiento == null)
            {
                return NotFound();
            }

            if (MovimientoLogicoService.EsEliminadoLogico(movimiento.Observaciones))
            {
                TempData["Error"] = "No se puede editar una salida anulada lógicamente.";
                return RedirectToAction(nameof(Index), new { incluirEliminados = true });
            }

            if (string.IsNullOrWhiteSpace(model.SucursalDestinoId))
            {
                ModelState.AddModelError(nameof(model.SucursalDestinoId), "Debe seleccionar una sucursal destino.");
            }
            else
            {
                var sucursalValida = await _context.Sucursales
                    .AnyAsync(s => !s.Eliminado && s.Id == model.SucursalDestinoId);
                if (!sucursalValida)
                {
                    ModelState.AddModelError(nameof(model.SucursalDestinoId), "La sucursal destino no es válida.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Sucursales = new SelectList(
                    await _context.Sucursales.Where(s => !s.Eliminado).OrderBy(s => s.Nombre).ToListAsync(),
                    "Id",
                    "Nombre",
                    model.SucursalDestinoId);
                return View(model);
            }

            movimiento.SucursalDestinoId = model.SucursalDestinoId;
            movimiento.ResponsableRecepcion = string.IsNullOrWhiteSpace(model.ResponsableRecepcion)
                ? null
                : model.ResponsableRecepcion.Trim();
            movimiento.Observaciones = string.IsNullOrWhiteSpace(model.Observaciones)
                ? null
                : model.Observaciones.Trim();
            movimiento.FechaRecepcion = model.FechaRecepcion;

            var ultimoMovimientoId = await _context.MovimientosItem
                .Where(m => m.ItemId == movimiento.ItemId)
                .Where(m => m.Observaciones == null || !m.Observaciones.Contains(MovimientoLogicoService.MarcaEliminadoLogico))
                .OrderByDescending(m => m.FechaMovimiento)
                .ThenByDescending(m => m.Id)
                .Select(m => m.Id)
                .FirstOrDefaultAsync();

            if (ultimoMovimientoId == movimiento.Id && movimiento.Item != null)
            {
                movimiento.Item.SucursalId = model.SucursalDestinoId;
                if (!string.IsNullOrWhiteSpace(model.ResponsableRecepcion))
                {
                    movimiento.Item.ResponsableEmpleado = model.ResponsableRecepcion.Trim();
                }
                _context.Items.Update(movimiento.Item);
            }

            _context.MovimientosItem.Update(movimiento);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Salida actualizada exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarLogicoEnvio(int id, string motivoAnulacion = "")
        {
            var movimiento = await _context.MovimientosItem
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movimiento == null)
            {
                return Json(new { success = false, message = "No se encontró el registro de envío." });
            }

            if (!EsMotivoEnvio(movimiento.Motivo))
            {
                return Json(new { success = false, message = "Solo se pueden anular registros de envío (despacho o salida sin registro)." });
            }

            if (MovimientoLogicoService.EsEliminadoLogico(movimiento.Observaciones))
            {
                return Json(new { success = false, message = "Este envío ya está anulado lógicamente." });
            }

            var usuario = HttpContext.Session.GetString("UserName") ?? "Sistema";
            movimiento.Observaciones = MovimientoLogicoService.ConstruirObservacionEliminado(
                movimiento.Observaciones,
                usuario,
                motivoAnulacion);

            _context.MovimientosItem.Update(movimiento);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Registro de envío anulado lógicamente." });
        }
    }
}
