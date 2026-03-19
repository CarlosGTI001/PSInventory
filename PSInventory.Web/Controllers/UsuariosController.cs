using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;

namespace PSInventory.Web.Controllers
{
    [AuthorizeRole("Administrador")]
    public class UsuariosController : Controller
    {
        private readonly PSDatos _context;

        public UsuariosController(PSDatos context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index(string q = "", int page = 1, int pageSize = 30)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 10 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var query = _context.Usuarios
                .Where(u => !u.Eliminado)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(u =>
                    u.Nombre.ToLower().Contains(term) ||
                    (u.Email != null && u.Email.ToLower().Contains(term)) ||
                    (u.Rol != null && u.Rol.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var usuarios = await query
                .OrderBy(u => u.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Query = q;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;
            return View(usuarios);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Password,Email,Rol")] Usuario usuario)
        {
            ModelState.Remove("Id"); // Id es generado server-side
            if (ModelState.IsValid)
            {
                usuario.Id = Guid.NewGuid().ToString();
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Usuario creado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Where(u => !u.Eliminado)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Nombre,Password,Email,Rol")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.Usuarios.FindAsync(id);
                    if (existing == null) return NotFound();

                    existing.Nombre = usuario.Nombre;
                    existing.Email = usuario.Email;
                    existing.Rol = usuario.Rol;

                    // Solo actualizar contraseña si se proporcionó una nueva
                    if (!string.IsNullOrWhiteSpace(usuario.Password))
                    {
                        existing.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Usuario actualizado exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
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
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var usuario = await _context.Usuarios
                .Where(u => !u.Eliminado)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no encontrado" });
            }

            // No permitir eliminar al último administrador activo
            if (usuario.Rol == "Administrador")
            {
                var adminCount = await _context.Usuarios
                    .CountAsync(u => u.Rol == "Administrador" && !u.Eliminado);
                if (adminCount <= 1)
                {
                    return Json(new { success = false, message = "No se puede eliminar el último administrador del sistema" });
                }
            }

            // Soft delete
            usuario.Eliminado = true;
            usuario.FechaEliminacion = DateTime.Now;
            usuario.UsuarioEliminacion = User.Identity?.Name ?? "Sistema";

            _context.Update(usuario);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Usuario eliminado exitosamente" });
        }

        private bool UsuarioExists(string id)
        {
            return _context.Usuarios.Any(e => e.Id == id && !e.Eliminado);
        }
    }
}
