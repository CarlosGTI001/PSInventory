using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;
using PSInventory.Web.Models.ViewModels;

namespace PSInventory.Web.Controllers
{
    [AuthorizeRole("Administrador", "Jefe")]
    public class DepartamentosController : Controller
    {
        private readonly PSDatos _context;

        public DepartamentosController(PSDatos context)
        {
            _context = context;
        }

        // GET: Departamentos
        public async Task<IActionResult> Index(string q = "", int page = 1, int pageSize = 30)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 10 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var query = _context.Departamentos
                .Where(d => !d.Eliminado)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(d =>
                    d.Nombre.ToLower().Contains(term) ||
                    (d.Descripcion != null && d.Descripcion.ToLower().Contains(term)) ||
                    (d.Responsable != null && d.Responsable.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var departamentos = await query
                .OrderBy(d => d.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Query = q;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;
            return View(departamentos);
        }

        // GET: Departamentos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departamentos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion,Responsable,Activo")] Departamento departamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(departamento);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Departamento creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            return View(departamento);
        }

        // GET: Departamentos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departamento = await _context.Departamentos
                .Where(d => !d.Eliminado)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (departamento == null)
            {
                return NotFound();
            }
            return View(departamento);
        }

        // POST: Departamentos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Responsable,Activo")] Departamento departamento)
        {
            if (id != departamento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(departamento);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Departamento actualizado exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartamentoExists(departamento.Id))
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
            return View(departamento);
        }

        // POST: Departamentos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var departamento = await _context.Departamentos
                .Where(d => !d.Eliminado)
                .Include(d => d.Compras)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (departamento == null)
            {
                return Json(new { success = false, message = "Departamento no encontrado" });
            }

            // Verificar si tiene compras asociadas activas
            var tieneCompras = departamento.Compras.Where(c => !c.Eliminado).Any();
            if (tieneCompras)
            {
                return Json(new { success = false, message = "No se puede eliminar el departamento porque tiene compras asociadas" });
            }

            // Soft delete
            departamento.Eliminado = true;
            departamento.FechaEliminacion = DateTime.Now;
            departamento.UsuarioEliminacion = User.Identity?.Name ?? "Sistema";

            _context.Update(departamento);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Departamento eliminado exitosamente" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearRapido([FromBody] QuickCreateLocationInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Nombre))
            {
                return Json(new { success = false, message = "Debe ingresar el nombre del departamento." });
            }

            var nombre = input.Nombre.Trim();
            var existe = await _context.Departamentos
                .AnyAsync(d => !d.Eliminado && d.Nombre.ToLower() == nombre.ToLower());
            if (existe)
            {
                return Json(new { success = false, message = "Ya existe un departamento con ese nombre." });
            }

            var departamento = new Departamento
            {
                Nombre = nombre,
                Activo = true
            };
            _context.Departamentos.Add(departamento);
            await _context.SaveChangesAsync();

            return Json(new { success = true, option = new { value = departamento.Id.ToString(), text = departamento.Nombre } });
        }

        private bool DepartamentoExists(int id)
        {
            return _context.Departamentos.Any(e => e.Id == id && !e.Eliminado);
        }
    }
}
