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
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .Where(u => !u.Eliminado)
                .OrderBy(u => u.Nombre)
                .ToListAsync();
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
            if (ModelState.IsValid)
            {
                usuario.Id = Guid.NewGuid().ToString();
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
                    _context.Update(usuario);
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
