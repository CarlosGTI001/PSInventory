using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;

namespace PSInventory.Web.Controllers
{
    [AuthorizeRole("Administrador")]
    public class SucursalesController : Controller
    {
        private readonly PSDatos _context;

        public SucursalesController(PSDatos context)
        {
            _context = context;
        }

        // GET: Sucursales
        public async Task<IActionResult> Index()
        {
            var sucursales = await _context.Sucursales
                .Where(s => !s.Eliminado)
                .Include(s => s.Region)
                .OrderBy(s => s.Nombre)
                .ToListAsync();
            return View(sucursales);
        }

        // GET: Sucursales/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Regiones = new SelectList(await _context.Regiones.Where(r => !r.Eliminado).OrderBy(r => r.Nombre).ToListAsync(), "Id", "Nombre");
            return View();
        }

        // POST: Sucursales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Direccion,Telefono,RegionId")] Sucursal sucursal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sucursal);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sucursal creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Regiones = new SelectList(await _context.Regiones.Where(r => !r.Eliminado).OrderBy(r => r.Nombre).ToListAsync(), "Id", "Nombre", sucursal.RegionId);
            return View(sucursal);
        }

        // GET: Sucursales/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sucursal = await _context.Sucursales
                .Where(s => !s.Eliminado)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sucursal == null)
            {
                return NotFound();
            }
            ViewBag.Regiones = new SelectList(await _context.Regiones.Where(r => !r.Eliminado).OrderBy(r => r.Nombre).ToListAsync(), "Id", "Nombre", sucursal.RegionId);
            return View(sucursal);
        }

        // POST: Sucursales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Nombre,Direccion,Telefono,RegionId,Activo")] Sucursal sucursal)
        {
            if (id != sucursal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sucursal);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Sucursal actualizada exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SucursalExists(sucursal.Id))
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
            ViewBag.Regiones = new SelectList(await _context.Regiones.Where(r => !r.Eliminado).OrderBy(r => r.Nombre).ToListAsync(), "Id", "Nombre", sucursal.RegionId);
            return View(sucursal);
        }

        // POST: Sucursales/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var sucursal = await _context.Sucursales
                .Where(s => !s.Eliminado)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sucursal == null)
            {
                return Json(new { success = false, message = "Sucursal no encontrada" });
            }

            // Verificar si tiene items asignados activos
            var tieneItems = await _context.Items.AnyAsync(i => i.SucursalId == id && !i.Eliminado);
            if (tieneItems)
            {
                return Json(new { success = false, message = "No se puede eliminar la sucursal porque tiene items asignados" });
            }

            // Soft delete
            sucursal.Eliminado = true;
            sucursal.FechaEliminacion = DateTime.Now;
            sucursal.UsuarioEliminacion = User.Identity?.Name ?? "Sistema";

            _context.Update(sucursal);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Sucursal eliminada exitosamente" });
        }

        private bool SucursalExists(string id)
        {
            return _context.Sucursales.Any(e => e.Id == id && !e.Eliminado);
        }
    }
}
