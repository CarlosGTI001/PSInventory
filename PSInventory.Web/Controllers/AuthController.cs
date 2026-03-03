using Microsoft.AspNetCore.Mvc;
using PSData.Datos;
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
            var user = _context.Usuarios
                .FirstOrDefault(u => u.Nombre == username && !u.Eliminado);

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
