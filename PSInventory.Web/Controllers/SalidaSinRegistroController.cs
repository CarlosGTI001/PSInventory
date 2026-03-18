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

        // GET: SalidaSinRegistro/SucursalesPorRegion?regionId=3
        [HttpGet]
        public async Task<IActionResult> SucursalesPorRegion(int regionId)
        {
            var sucursales = await _context.Sucursales
                .Where(s => !s.Eliminado && s.Activo && s.RegionId == regionId)
                .OrderBy(s => s.Nombre)
                .Select(s => new { value = s.Id, text = s.Nombre })
                .ToListAsync();

            return Json(sucursales);
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

            var entregaDepartamento = input.EntregaDepartamento;
            string departamentoNombre = string.Empty;
            var sucursalDestinoId = !entregaDepartamento && !string.IsNullOrWhiteSpace(input.SucursalDestinoId)
                ? input.SucursalDestinoId
                : null;

            if (entregaDepartamento)
            {
                if (input.DepartamentoDestinoId.HasValue && input.DepartamentoDestinoId.Value > 0)
                {
                    var departamento = await _context.Departamentos
                        .Where(d => !d.Eliminado && d.Id == input.DepartamentoDestinoId.Value)
                        .FirstOrDefaultAsync();
                    if (departamento == null)
                    {
                        return Json(new { success = false, message = "El departamento destino no es válido." });
                    }

                    departamentoNombre = departamento.Nombre;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(sucursalDestinoId))
                {
                    var sucursalExiste = await _context.Sucursales
                        .AnyAsync(s => s.Id == sucursalDestinoId && !s.Eliminado && s.Activo);
                    if (!sucursalExiste)
                    {
                        return Json(new { success = false, message = "La sucursal destino no es válida." });
                    }
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            var procesados = 0;

            try
            {
                var usuario = HttpContext.Session.GetString("UserName") ?? "Sistema";
                var responsableDestino = entregaDepartamento
                    ? (!string.IsNullOrWhiteSpace(input.PersonaEntregaDepartamento)
                        ? input.PersonaEntregaDepartamento.Trim()
                        : (!string.IsNullOrWhiteSpace(input.ResponsableEmpleado)
                            ? input.ResponsableEmpleado.Trim()
                            : "Sin responsable especificado"))
                    : (!string.IsNullOrWhiteSpace(input.ResponsableEmpleado)
                        ? input.ResponsableEmpleado.Trim()
                        : "Sin responsable especificado");
                var observacionMovimiento = input.Observaciones;
                if (entregaDepartamento)
                {
                    var destinoDepto = string.IsNullOrWhiteSpace(departamentoNombre) ? "No especificado" : departamentoNombre;
                    var prefijoDestino = $"Departamento destino: {destinoDepto}. Entregado a: {responsableDestino}.";
                    observacionMovimiento = string.IsNullOrWhiteSpace(observacionMovimiento)
                        ? prefijoDestino
                        : $"{prefijoDestino} {observacionMovimiento}";
                }
                var compraSalida = new Compra
                {
                    Proveedor = "Salida sin registro",
                    CostoTotal = 0m,
                    FechaCompra = DateTime.Now,
                    NumeroFactura = null,
                    Estado = "Completada",
                    Observaciones = string.IsNullOrWhiteSpace(input.Observaciones)
                        ? "Registro automático por salida sin registro de inventario."
                        : input.Observaciones,
                    UsuarioSolicitante = usuario,
                    FechaSolicitud = DateTime.Now
                };
                _context.Compras.Add(compraSalida);
                await _context.SaveChangesAsync();

                foreach (var itemInput in input.Items)
                {
                    if (string.IsNullOrWhiteSpace(itemInput.Marca) ||
                        string.IsNullOrWhiteSpace(itemInput.Modelo) ||
                        itemInput.CategoriaId <= 0 ||
                        itemInput.Cantidad <= 0)
                    {
                        await transaction.RollbackAsync();
                        return Json(new { success = false, message = "Cada item debe incluir marca, modelo, categoría y cantidad válida." });
                    }

                    var categoriaExiste = await _context.Categorias
                        .AnyAsync(c => c.Id == itemInput.CategoriaId && !c.Eliminado);
                    if (!categoriaExiste)
                    {
                        await transaction.RollbackAsync();
                        return Json(new { success = false, message = $"La categoría seleccionada no es válida para el item {itemInput.Marca} {itemInput.Modelo}." });
                    }

                    // 1. Crear el nuevo artículo
                    var articulo = new Articulo
                    {
                        Marca = itemInput.Marca.Trim(),
                        Modelo = itemInput.Modelo.Trim(),
                        Descripcion = string.IsNullOrWhiteSpace(itemInput.Descripcion) ? null : itemInput.Descripcion.Trim(),
                        CategoriaId = itemInput.CategoriaId,
                        RequiereSerial = !string.IsNullOrEmpty(itemInput.Serial),
                        Eliminado = false
                    };
                    _context.Articulos.Add(articulo);
                    await _context.SaveChangesAsync(); // Guardar para obtener el ID del artículo

                    // 2. Crear el lote asociado para mantener consistencia de inventario
                    var lote = new Lote
                    {
                        ArticuloId = articulo.Id,
                        CompraId = compraSalida.Id,
                        Cantidad = itemInput.Cantidad,
                        CostoUnitario = 0m
                    };
                    _context.Lotes.Add(lote);
                    await _context.SaveChangesAsync();

                    // 3. Crear el nuevo item
                    var item = new Item
                    {
                        ArticuloId = articulo.Id,
                        LoteId = lote.Id,
                        Serial = string.IsNullOrWhiteSpace(itemInput.Serial) ? null : itemInput.Serial.Trim(),
                        Cantidad = itemInput.Cantidad,
                        Estado = "Asignado",
                        SucursalId = sucursalDestinoId,
                        ResponsableEmpleado = responsableDestino,
                        FechaAsignacion = DateTime.Now,
                        Eliminado = false
                    };
                    _context.Items.Add(item);
                    await _context.SaveChangesAsync(); // Guardar para obtener el ID del item

                    // 4. Registrar el movimiento de salida
                    _context.MovimientosItem.Add(new MovimientoItem
                    {
                        ItemId = item.Id,
                        Cantidad = item.Cantidad,
                        SucursalOrigenId = null, // Origen es "nuevo", no hay sucursal
                        SucursalDestinoId = sucursalDestinoId,
                        FechaMovimiento = DateTime.Now,
                        UsuarioResponsable = usuario,
                        Motivo = entregaDepartamento ? "Salida Sin Registro - Departamento" : "Salida Sin Registro",
                        Observaciones = observacionMovimiento,
                        ResponsableRecepcion = responsableDestino,
                        FechaRecepcion = DateTime.Now
                    });
                    await _context.SaveChangesAsync();
                    procesados++;
                }

                await transaction.CommitAsync();
                return Json(new
                {
                    success = true,
                    message = entregaDepartamento
                        ? $"{procesados} item(s) procesado(s) correctamente para departamento {(string.IsNullOrWhiteSpace(departamentoNombre) ? "no especificado" : departamentoNombre)}."
                        : $"{procesados} item(s) procesado(s) correctamente."
                });
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
