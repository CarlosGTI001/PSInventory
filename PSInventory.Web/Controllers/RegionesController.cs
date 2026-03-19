using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;
using PSInventory.Web.Models.ViewModels;

namespace PSInventory.Web.Controllers
{
    [AuthorizeRole("Administrador")]
    public class RegionesController : Controller
    {
        private readonly PSDatos _context;

        public RegionesController(PSDatos context)
        {
            _context = context;
        }

        // GET: Regiones
        public async Task<IActionResult> Index(string q = "", int page = 1, int pageSize = 30)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 10 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var query = _context.Regiones
                .Where(r => !r.Eliminado)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(r =>
                    r.Nombre.ToLower().Contains(term) ||
                    (r.Descripcion != null && r.Descripcion.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var regiones = await query
                .OrderBy(r => r.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Query = q;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;
            return View(regiones);
        }

        // GET: Regiones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Regiones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion")] Region region)
        {
            if (ModelState.IsValid)
            {
                _context.Add(region);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Región creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            return View(region);
        }

        // GET: Regiones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var region = await _context.Regiones
                .Where(r => !r.Eliminado)
                .FirstOrDefaultAsync(r => r.RegionId == id);
            if (region == null)
            {
                return NotFound();
            }
            return View(region);
        }

        // POST: Regiones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RegionId,Nombre,Descripcion")] Region region)
        {
            if (id != region.RegionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(region);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Región actualizada exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegionExists(region.RegionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(region);
        }

        // POST: Regiones/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var region = await _context.Regiones
                .Where(r => !r.Eliminado)
                .FirstOrDefaultAsync(r => r.RegionId == id);
            if (region == null)
            {
                return Json(new { success = false, message = "Región no encontrada" });
            }

            // Verificar si tiene sucursales asociadas activas
            var tieneSucursales = await _context.Sucursales
                .AnyAsync(s => s.RegionId == id && !s.Eliminado);
            if (tieneSucursales)
            {
                return Json(new { success = false, message = "No se puede eliminar la región porque tiene sucursales asociadas" });
            }

            // Soft delete
            region.Eliminado = true;
            region.FechaEliminacion = DateTime.Now;
            region.UsuarioEliminacion = User.Identity?.Name ?? "Sistema";

            _context.Update(region);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Región eliminada exitosamente" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearRapido([FromBody] QuickCreateLocationInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Nombre))
            {
                return Json(new { success = false, message = "Debe ingresar el nombre de la región." });
            }

            var nombre = input.Nombre.Trim();
            var existe = await _context.Regiones
                .AnyAsync(r => !r.Eliminado && r.Nombre.ToLower() == nombre.ToLower());
            if (existe)
            {
                return Json(new { success = false, message = "Ya existe una región con ese nombre." });
            }

            var region = new Region
            {
                Nombre = nombre,
                Activo = true
            };
            _context.Regiones.Add(region);
            await _context.SaveChangesAsync();

            return Json(new { success = true, option = new { value = region.RegionId.ToString(), text = region.Nombre } });
        }

        private bool RegionExists(int id)
        {
            return _context.Regiones.Any(e => e.RegionId == id && !e.Eliminado);
        }
    }
}
