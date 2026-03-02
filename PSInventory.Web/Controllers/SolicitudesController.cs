using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;

namespace PSInventory.Web.Controllers
{
    [RequireAuth]
    public class SolicitudesController : Controller
    {
        private readonly PSDatos _context;
        private string UsuarioActual => HttpContext.Session.GetString("UserName") ?? "Sistema";

        public SolicitudesController(PSDatos context) => _context = context;

        // GET: Solicitudes
        public async Task<IActionResult> Index()
        {
            var solicitudes = await _context.SolicitudesCompra
                .Where(s => !s.Eliminado)
                .Include(s => s.Detalles)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();
            return View(solicitudes);
        }

        // GET: Solicitudes/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Articulos = await _context.Articulos
                .Where(a => !a.Eliminado)
                .OrderBy(a => a.Marca).ThenBy(a => a.Modelo)
                .Select(a => new { a.Id, Nombre = a.Marca + " " + a.Modelo })
                .ToListAsync();
            return View();
        }

        // POST: Solicitudes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string titulo,
            string? observaciones,
            List<int?> articuloIds,
            List<string?> descripciones,
            List<int> cantidades,
            List<string?> obsDetalles)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                TempData["Error"] = "El título es requerido.";
                return RedirectToAction(nameof(Create));
            }

            bool hayLineas = false;
            for (int i = 0; i < articuloIds.Count; i++)
            {
                if (articuloIds[i].HasValue || !string.IsNullOrWhiteSpace(descripciones.ElementAtOrDefault(i)))
                { hayLineas = true; break; }
            }
            if (!hayLineas)
            {
                TempData["Error"] = "Agrega al menos un artículo a la solicitud.";
                return RedirectToAction(nameof(Create));
            }

            var solicitud = new SolicitudCompra
            {
                Titulo         = titulo.Trim(),
                Solicitante    = UsuarioActual,
                FechaSolicitud = DateTime.Now,
                Estado         = "Pendiente",
                Observaciones  = observaciones?.Trim()
            };
            _context.SolicitudesCompra.Add(solicitud);
            await _context.SaveChangesAsync();

            for (int i = 0; i < articuloIds.Count; i++)
            {
                var artId = articuloIds.ElementAtOrDefault(i);
                var desc  = descripciones.ElementAtOrDefault(i)?.Trim();
                var cant  = cantidades.ElementAtOrDefault(i);
                if (cant < 1) cant = 1;

                if (!artId.HasValue && string.IsNullOrWhiteSpace(desc)) continue;

                _context.DetallesSolicitudCompra.Add(new DetalleSolicitudCompra
                {
                    SolicitudId      = solicitud.Id,
                    ArticuloId       = artId,
                    DescripcionLibre = artId.HasValue ? null : desc,
                    Cantidad         = cant,
                    Observaciones    = obsDetalles.ElementAtOrDefault(i)?.Trim()
                });
            }
            await _context.SaveChangesAsync();

            TempData["Success"] = "Solicitud creada exitosamente.";
            return RedirectToAction(nameof(Details), new { id = solicitud.Id });
        }

        // GET: Solicitudes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var solicitud = await _context.SolicitudesCompra
                .Where(s => !s.Eliminado && s.Id == id)
                .Include(s => s.Detalles).ThenInclude(d => d.Articulo)
                .FirstOrDefaultAsync();
            if (solicitud == null) return NotFound();
            return View(solicitud);
        }

        // POST: Solicitudes/CambiarEstado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado, string? notasRevision)
        {
            var solicitud = await _context.SolicitudesCompra.FindAsync(id);
            if (solicitud == null || solicitud.Eliminado) return NotFound();

            var estadosValidos = new[] { "Pendiente", "En Revisión", "Aprobada", "Rechazada" };
            if (!estadosValidos.Contains(nuevoEstado)) return BadRequest();

            solicitud.Estado        = nuevoEstado;
            solicitud.UsuarioRevisor = UsuarioActual;
            solicitud.FechaRevision  = DateTime.Now;
            solicitud.NotasRevision  = notasRevision?.Trim();

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Estado actualizado a «{nuevoEstado}».";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Solicitudes/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var solicitud = await _context.SolicitudesCompra.FindAsync(id);
            if (solicitud == null) return NotFound();
            if (solicitud.Estado != "Pendiente")
            {
                TempData["Error"] = "Solo se pueden eliminar solicitudes en estado Pendiente.";
                return RedirectToAction(nameof(Details), new { id });
            }
            solicitud.Eliminado = true;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Solicitud eliminada.";
            return RedirectToAction(nameof(Index));
        }
    }
}
