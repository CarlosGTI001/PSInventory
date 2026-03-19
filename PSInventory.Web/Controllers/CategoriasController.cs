using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;
using PSInventory.Web.Models.ViewModels;

namespace PSInventory.Web.Controllers
{
    [AuthorizeRole("Administrador", "Jefe")]
    public class CategoriasController : Controller
    {
        private readonly PSDatos _context;

        public CategoriasController(PSDatos context)
        {
            _context = context;
        }

        // GET: Categorias
        public async Task<IActionResult> Index(string q = "", int page = 1, int pageSize = 30)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 10 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var query = _context.Categorias
                .Where(c => !c.Eliminado)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(c =>
                    c.Nombre.ToLower().Contains(term) ||
                    (c.Descripcion != null && c.Descripcion.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var categorias = await query
                .OrderBy(c => c.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Query = q;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;
            return View(categorias);
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion")] Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Categoría creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoria = await _context.Categorias
                .Where(c => !c.Eliminado)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Categoría actualizada exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.Id))
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
            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _context.Categorias
                .Where(c => !c.Eliminado)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (categoria == null)
            {
                return Json(new { success = false, message = "Categoría no encontrada" });
            }

            // Verificar si tiene artículos asociados activos
            var tieneArticulos = await _context.Articulos
                .AnyAsync(a => a.CategoriaId == id && !a.Eliminado);
            if (tieneArticulos)
            {
                return Json(new { success = false, message = "No se puede eliminar la categoría porque tiene artículos asociados" });
            }

            // Soft delete
            categoria.Eliminado = true;
            categoria.FechaEliminacion = DateTime.Now;
            categoria.UsuarioEliminacion = User.Identity?.Name ?? "Sistema";

            _context.Update(categoria);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Categoría eliminada exitosamente" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearRapido([FromBody] QuickCreateLocationInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Nombre))
            {
                return Json(new { success = false, message = "Debe ingresar el nombre de la categoría." });
            }

            var nombre = input.Nombre.Trim();
            var existe = await _context.Categorias
                .AnyAsync(c => !c.Eliminado && c.Nombre.ToLower() == nombre.ToLower());
            if (existe)
            {
                return Json(new { success = false, message = "Ya existe una categoría con ese nombre." });
            }

            var categoria = new Categoria
            {
                Nombre = nombre
            };
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return Json(new { success = true, option = new { value = categoria.Id.ToString(), text = categoria.Nombre } });
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.Id == id && !e.Eliminado);
        }
    }
}
