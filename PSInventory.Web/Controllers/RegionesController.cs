using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;

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
        public async Task<IActionResult> Index()
        {
            var regiones = await _context.Regiones
                .Where(r => !r.Eliminado)
                .OrderBy(r => r.Nombre)
                .ToListAsync();
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

        private bool RegionExists(int id)
        {
            return _context.Regiones.Any(e => e.RegionId == id && !e.Eliminado);
        }
    }
}
