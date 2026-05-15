using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSInventory.Web.Filters;
using PSInventory.Web.Models.ViewModels;
using System.Linq;

namespace PSInventory.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly PSDatos _context;

        public AuthController(PSDatos context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Si ya está autenticado, redirigir al dashboard
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserName")))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var normalizedUser = (username ?? string.Empty).Trim();
            var user = _context.Usuarios
                .FirstOrDefault(u => u.Id == normalizedUser && !u.Eliminado)
                ?? _context.Usuarios.FirstOrDefault(u => u.Nombre == normalizedUser && !u.Eliminado);

            if (user != null && VerifyPassword(password, user.Password))
            {
                // Guardar en sesión
                HttpContext.Session.SetString("UserName", user.Nombre);
                HttpContext.Session.SetString("UserId", user.Id);
                HttpContext.Session.SetString("UserRole", user.Rol);
                HttpContext.Session.SetString("UserEmail", user.Email);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        [RequireAuth]
        public async Task<IActionResult> CambiarContrasena()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrWhiteSpace(userId))
            {
                HttpContext.Session.Clear();
                return RedirectToAction(nameof(Login));
            }

            var user = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId && !u.Eliminado);

            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction(nameof(Login));
            }

            ViewBag.UserName = user.Nombre;
            return View(new CambiarContrasenaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequireAuth]
        public async Task<IActionResult> CambiarContrasena(CambiarContrasenaViewModel model)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrWhiteSpace(userId))
            {
                HttpContext.Session.Clear();
                return RedirectToAction(nameof(Login));
            }

            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == userId && !u.Eliminado);

            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction(nameof(Login));
            }

            ViewBag.UserName = user.Nombre;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!VerifyPassword(model.ContrasenaActual, user.Password))
            {
                ModelState.AddModelError(nameof(CambiarContrasenaViewModel.ContrasenaActual), "La contraseña actual es incorrecta.");
                return View(model);
            }

            if (VerifyPassword(model.NuevaContrasena, user.Password))
            {
                ModelState.AddModelError(nameof(CambiarContrasenaViewModel.NuevaContrasena), "La nueva contraseña debe ser diferente a la actual.");
                return View(model);
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NuevaContrasena);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tu contraseña fue actualizada correctamente.";
            return RedirectToAction(nameof(CambiarContrasena));
        }

        private static bool VerifyPassword(string password, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}
