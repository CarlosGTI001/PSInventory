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
        public async Task<IActionResult> Index(string q = "", int page = 1, int pageSize = 30)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 10 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var query = _context.Articulos
                .Where(a => !a.Eliminado)
                .Include(a => a.Categoria)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(a =>
                    a.Marca.ToLower().Contains(term) ||
                    a.Modelo.ToLower().Contains(term) ||
                    (a.Descripcion != null && a.Descripcion.ToLower().Contains(term)) ||
                    (a.Categoria != null && a.Categoria.Nombre.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var articulos = await query
                .OrderBy(a => a.Marca)
                .ThenBy(a => a.Modelo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Query = q;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;
            return View(articulos);
        }

        // GET: Articulos/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Categorias = new SelectList(await _context.Categorias.Where(c => !c.Eliminado).OrderBy(c => c.Nombre).ToListAsync(), "Id", "Nombre");
            return View(new Articulo { RequiereSerial = true });
        }

        // POST: Articulos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Marca,Modelo,Descripcion,CategoriaId,StockMinimo,Especificaciones,RequiereSerial")] Articulo articulo)
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

        // GET: Articulos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articulo = await _context.Articulos
                .Include(a => a.Categoria)
                .Include(a => a.Lotes.OrderByDescending(l => l.Compra.FechaCompra))
                    .ThenInclude(l => l.Compra)
                .Include(a => a.Lotes)
                    .ThenInclude(l => l.Items)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (articulo == null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Marca,Modelo,Descripcion,CategoriaId,StockMinimo,Especificaciones,RequiereSerial")] Articulo articulo)
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
                return Json(new { success = false, message = "Artículo no encontrado o ya deshabilitado" });
            }

            var usuario = User.Identity?.Name ?? "Sistema";

            var itemsAsociados = await _context.Items
                .Where(i => i.ArticuloId == id && !i.Eliminado)
                .ToListAsync();

            foreach (var item in itemsAsociados)
            {
                item.Eliminado = true;
                item.FechaEliminacion = DateTime.Now;
                item.UsuarioEliminacion = usuario;
                item.Estado = "Baja";
            }

            // Soft delete
            articulo.Eliminado = true;
            articulo.FechaEliminacion = DateTime.Now;
            articulo.UsuarioEliminacion = usuario;

            _context.UpdateRange(itemsAsociados);
            _context.Update(articulo);
            await _context.SaveChangesAsync();
            return Json(new
            {
                success = true,
                message = itemsAsociados.Count > 0
                    ? $"Artículo deshabilitado exitosamente. Se deshabilitaron {itemsAsociados.Count} item(s) asociados."
                    : "Artículo deshabilitado exitosamente"
            });
        }

        private bool ArticuloExists(int id)
        {
            return _context.Articulos.Any(e => e.Id == id && !e.Eliminado);
        }
    }
}
