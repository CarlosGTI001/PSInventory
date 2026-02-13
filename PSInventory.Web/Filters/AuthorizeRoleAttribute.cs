using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PSInventory.Web.Filters
{
    public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;
            var userName = session.GetString("UserName");
            var userRole = session.GetString("UserRole");

            // Verificar si está autenticado
            if (string.IsNullOrEmpty(userName))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            // Verificar si tiene el rol requerido
            if (_roles.Length > 0 && !_roles.Contains(userRole))
            {
                context.Result = new ForbidResult();
            }
        }
    }

    // Atributo simplificado para requerir solo autenticación
    public class RequireAuthAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;
            var userName = session.GetString("UserName");

            if (string.IsNullOrEmpty(userName))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
        }
    }
}
