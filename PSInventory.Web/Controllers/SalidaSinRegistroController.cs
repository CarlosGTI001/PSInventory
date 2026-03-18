using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;
using PSInventory.Web.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSInventory.Web.Controllers
{
    [AuthorizeRole("Administrador", "Jefe")]
    public class SalidaSinRegistroController : Controller
    {
        private readonly PSDatos _context;

        public SalidaSinRegistroController(PSDatos context)
        {
            _context = context;
        }

        // GET: SalidaSinRegistro
        public async Task<IActionResult> Index()
        {
            ViewBag.Categorias = await _context.Categorias
                .Where(c => !c.Eliminado)
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nombre })
                .ToListAsync();

            ViewBag.Departamentos = await _context.Departamentos
                .Where(d => !d.Eliminado)
                .OrderBy(d => d.Nombre)
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nombre })
                .ToListAsync();

            ViewBag.Regiones = await _context.Regiones
                .Where(r => !r.Eliminado)
                .OrderBy(r => r.Nombre)
                .Select(r => new SelectListItem { Value = r.RegionId.ToString(), Text = r.Nombre })
                .ToListAsync();

            return View();
        }

        // POST: SalidaSinRegistro/Procesar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Procesar([FromBody] SalidaSinRegistroInput input)
        {
            if (input == null || input.Items == null || !input.Items.Any())
            {
                return Json(new { success = false, message = "No se proporcionaron datos para procesar." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            var errores = new List<string>();
            var procesados = 0;

            try
            {
                foreach (var itemInput in input.Items)
                {
                    // 1. Crear el nuevo artículo
                    var articulo = new Articulo
                    {
                        Marca = itemInput.Marca,
                        Modelo = itemInput.Modelo,
                        Descripcion = itemInput.Descripcion,
                        CategoriaId = itemInput.CategoriaId,
                        RequiereSerial = !string.IsNullOrEmpty(itemInput.Serial),
                        Eliminado = false
                    };
                    _context.Articulos.Add(articulo);
                    await _context.SaveChangesAsync(); // Guardar para obtener el ID del artículo

                    // 2. Crear el nuevo item
                    var item = new Item
                    {
                        ArticuloId = articulo.Id,
                        Serial = itemInput.Serial,
                        Cantidad = itemInput.Cantidad,
                        Estado = "Asignado",
                        SucursalId = input.SucursalDestinoId,
                        ResponsableEmpleado = input.ResponsableEmpleado,
                        FechaAsignacion = DateTime.Now,
                        Eliminado = false
                    };
                    _context.Items.Add(item);
                    await _context.SaveChangesAsync(); // Guardar para obtener el ID del item

                    // 3. Registrar el movimiento de salida
                    _context.MovimientosItem.Add(new MovimientoItem
                    {
                        ItemId = item.Id,
                        Cantidad = item.Cantidad,
                        SucursalOrigenId = null, // Origen es "nuevo", no hay sucursal
                        SucursalDestinoId = input.SucursalDestinoId,
                        FechaMovimiento = DateTime.Now,
                        UsuarioResponsable = HttpContext.Session.GetString("UserName") ?? "Sistema",
                        Motivo = "Salida Sin Registro",
                        Observaciones = input.Observaciones,
                        ResponsableRecepcion = input.ResponsableEmpleado,
                        FechaRecepcion = DateTime.Now
                    });
                    await _context.SaveChangesAsync();
                    procesados++;
                }

                await transaction.CommitAsync();
                return Json(new { success = true, message = $"{procesados} item(s) procesado(s) correctamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Log the exception details (ex.ToString())
                return Json(new { success = false, message = "Ocurrió un error inesperado al procesar la solicitud.", details = ex.Message });
            }
        }
    }
}
