using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;

namespace PSInventory.Web.Controllers
{
    [AuthorizeRole("Administrador", "Jefe")]
    public class ArticulosController : Controller
    {
        private readonly PSDatos _context;

        public ArticulosController(PSDatos context)
        {
            _context = context;
        }

        // GET: Articulos
        public async Task<IActionResult> Index()
        {
            var articulos = await _context.Articulos
                .Where(a => !a.Eliminado)
                .Include(a => a.Categoria)
                .OrderBy(a => a.Marca)
                .ThenBy(a => a.Modelo)
                .ToListAsync();
            return View(articulos);
        }

        // GET: Articulos/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categorias = new SelectList(await _context.Categorias.Where(c => !c.Eliminado).OrderBy(c => c.Nombre).ToListAsync(), "Id", "Nombre");
            return View();
        }

        // POST: Articulos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Marca,Modelo,Descripcion,CategoriaId,StockMinimo,Especificaciones")] Articulo articulo)
        {
            if (articulo.CategoriaId <= 0 || !await _context.Categorias.AnyAsync(c => c.Id == articulo.CategoriaId && !c.Eliminado))
            {
                ModelState.AddModelError("CategoriaId", "Debe seleccionar una categoría válida");
            }

            if (ModelState.IsValid)
            {
                _context.Add(articulo);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Artículo creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categorias = new SelectList(await _context.Categorias.Where(c => !c.Eliminado).OrderBy(c => c.Nombre).ToListAsync(), "Id", "Nombre", articulo.CategoriaId);
            return View(articulo);
        }

        // GET: Articulos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articulo = await _context.Articulos
                .Where(a => !a.Eliminado)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (articulo == null)
            {
                return NotFound();
            }
            ViewBag.Categorias = new SelectList(await _context.Categorias.Where(c => !c.Eliminado).OrderBy(c => c.Nombre).ToListAsync(), "Id", "Nombre", articulo.CategoriaId);
            return View(articulo);
        }

        // POST: Articulos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Marca,Modelo,Descripcion,CategoriaId,StockMinimo,Especificaciones")] Articulo articulo)
        {
            if (id != articulo.Id)
            {
                return NotFound();
            }

            if (articulo.CategoriaId <= 0 || !await _context.Categorias.AnyAsync(c => c.Id == articulo.CategoriaId && !c.Eliminado))
            {
                ModelState.AddModelError("CategoriaId", "Debe seleccionar una categoría válida");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(articulo);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Artículo actualizado exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticuloExists(articulo.Id))
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
            ViewBag.Categorias = new SelectList(await _context.Categorias.Where(c => !c.Eliminado).OrderBy(c => c.Nombre).ToListAsync(), "Id", "Nombre", articulo.CategoriaId);
            return View(articulo);
        }

        // POST: Articulos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var articulo = await _context.Articulos
                .Where(a => !a.Eliminado)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (articulo == null)
            {
                return Json(new { success = false, message = "Artículo no encontrado" });
            }

            // Verificar si tiene items asociados activos
            var tieneItems = await _context.Items.AnyAsync(i => i.ArticuloId == id && !i.Eliminado);
            if (tieneItems)
            {
                return Json(new { success = false, message = "No se puede eliminar el artículo porque tiene items asociados" });
            }

            // Soft delete
            articulo.Eliminado = true;
            articulo.FechaEliminacion = DateTime.Now;
            articulo.UsuarioEliminacion = User.Identity?.Name ?? "Sistema";

            _context.Update(articulo);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Artículo eliminado exitosamente" });
        }

        private bool ArticuloExists(int id)
        {
            return _context.Articulos.Any(e => e.Id == id && !e.Eliminado);
        }
    }
}
