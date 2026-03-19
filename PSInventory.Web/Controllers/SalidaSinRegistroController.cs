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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearDepartamentoRapido([FromBody] QuickCreateLocationInput input)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearRegionRapida([FromBody] QuickCreateLocationInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Nombre))
            {
                return Json(new { success = false, message = "Debe ingresar el nombre de la región." });
            }

            var nombre = input.Nombre.Trim();
            var existe = await _context.Regiones
                .AnyAsync(r => !r.Eliminado && r.Nombre.ToLower() == nombre.ToLower());
            if (existe)
            {
                return Json(new { success = false, message = "Ya existe una región con ese nombre." });
            }

            var region = new Region
            {
                Nombre = nombre,
                Activo = true
            };
            _context.Regiones.Add(region);
            await _context.SaveChangesAsync();

            return Json(new { success = true, option = new { value = region.RegionId.ToString(), text = region.Nombre } });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearSucursalRapida([FromBody] QuickCreateLocationInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Nombre) || !input.RegionId.HasValue || input.RegionId.Value <= 0)
            {
                return Json(new { success = false, message = "Debe ingresar nombre y región para la sucursal." });
            }

            var region = await _context.Regiones
                .Where(r => !r.Eliminado && r.RegionId == input.RegionId.Value)
                .FirstOrDefaultAsync();
            if (region == null)
            {
                return Json(new { success = false, message = "La región seleccionada no es válida." });
            }

            var nombre = input.Nombre.Trim();
            var existe = await _context.Sucursales
                .AnyAsync(s => !s.Eliminado && s.RegionId == input.RegionId.Value && s.Nombre.ToLower() == nombre.ToLower());
            if (existe)
            {
                return Json(new { success = false, message = "Ya existe una sucursal con ese nombre en esta región." });
            }

            var ultimoId = await _context.Sucursales
                .Where(s => s.Id.StartsWith("SUC-"))
                .OrderByDescending(s => s.Id)
                .Select(s => s.Id)
                .FirstOrDefaultAsync();
            var siguienteNumero = 1;
            if (!string.IsNullOrEmpty(ultimoId))
            {
                var numeroStr = ultimoId.Substring(4);
                if (int.TryParse(numeroStr, out int numero))
                {
                    siguienteNumero = numero + 1;
                }
            }

            var sucursal = new Sucursal
            {
                Id = $"SUC-{siguienteNumero:D3}",
                Nombre = nombre,
                RegionId = input.RegionId.Value,
                Activo = true
            };
            _context.Sucursales.Add(sucursal);
            await _context.SaveChangesAsync();

            return Json(new { success = true, option = new { value = sucursal.Id, text = sucursal.Nombre } });
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
            var usaSucursalTecnica = false;

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

            // MovimientosItem requiere SucursalDestinoId NOT NULL por esquema actual.
            var sucursalMovimientoId = sucursalDestinoId;
            if (string.IsNullOrWhiteSpace(sucursalMovimientoId))
            {
                sucursalMovimientoId = await _context.Sucursales
                    .Where(s => !s.Eliminado && s.Activo)
                    .OrderBy(s => s.Nombre)
                    .Select(s => s.Id)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrWhiteSpace(sucursalMovimientoId))
                {
                    return Json(new
                    {
                        success = false,
                        message = "No hay sucursales activas para registrar el movimiento. Configure al menos una sucursal activa."
                    });
                }

                usaSucursalTecnica = true;
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
                        SucursalDestinoId = sucursalMovimientoId,
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
                var mensaje = entregaDepartamento
                    ? $"{procesados} item(s) procesado(s) correctamente para departamento {(string.IsNullOrWhiteSpace(departamentoNombre) ? "no especificado" : departamentoNombre)}."
                    : $"{procesados} item(s) procesado(s) correctamente.";
                if (usaSucursalTecnica)
                {
                    mensaje += " (Se registró sucursal técnica para trazabilidad de movimiento.)";
                }

                return Json(new { success = true, message = mensaje });
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
