using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSInventory.Web.Filters;
using PSInventory.Web.Models.ViewModels;

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

        // GET: Movimientos
        public async Task<IActionResult> Index(string sucursalId = "", DateTime? fechaInicio = null, DateTime? fechaFin = null, string q = "", int page = 1, int pageSize = 30)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 10 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var query = _context.MovimientosItem
                .Include(m => m.Item)
                    .ThenInclude(i => i.Articulo)
                    .ThenInclude(a => a.Categoria)
                .Include(m => m.SucursalOrigen)
                .Include(m => m.SucursalDestino)
                .AsQueryable();

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

            var totalCount = await query.CountAsync();
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
            ViewBag.Query = q;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;

            return View(movimientos);
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

            var ultimoMovimientoId = await _context.MovimientosItem
                .Where(m => m.ItemId == movimiento.ItemId)
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
    }
}
