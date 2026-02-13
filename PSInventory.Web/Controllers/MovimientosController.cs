using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSInventory.Web.Filters;

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
        public async Task<IActionResult> Index(string sucursalId = "", DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
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

            var movimientos = await query
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();

            // Datos para filtros - solo sucursales activas
            ViewBag.Sucursales = await _context.Sucursales
                .Where(s => !s.Eliminado)
                .OrderBy(s => s.Nombre)
                .ToListAsync();
            ViewBag.SucursalFiltro = sucursalId;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

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
    }
}
