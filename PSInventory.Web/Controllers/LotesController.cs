using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering; // Added for SelectListItem
using PSInventory.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering; // Added for SelectListItem // Added for CompraViewModel

namespace PSInventory.Web.Controllers
{
    [AuthorizeRole("Administrador", "Jefe")]
    public class LotesController : Controller
    {
        private readonly PSDatos _context;

        public LotesController(PSDatos context)
        {
            _context = context;
        }

        // GET: Lotes/AddLote (Initiated from Articulos/Details)
        public async Task<IActionResult> AddLote(int? articuloId)
        {
            if (articuloId == null)
            {
                TempData["Error"] = "Debe seleccionar un artículo para agregar un lote.";
                return RedirectToAction("Index", "Articulos");
            }

            var articulo = await _context.Articulos.FindAsync(articuloId);
            if (articulo == null)
            {
                TempData["Error"] = "Artículo no encontrado.";
                return RedirectToAction("Index", "Articulos");
            }
            
            // Populate ViewBags needed for the form
            await PopulateViewBagsForLoteCreation();
            ViewBag.SelectedArticulo = articulo; // Pass selected article to view

            var model = new CompraViewModel
            {
                FechaCompra = DateTime.Now,
                Proveedor = "",
                Estado = "Completada", // Default to completada when adding Lotes
                Lotes = new List<LoteViewModel>
                {
                    new LoteViewModel { ArticuloId = articulo.Id, Cantidad = 1, CostoUnitario = 0m } // Pre-fill with one lote for the selected article
                }
            };

            return View(model);
        }

        // POST: Lotes/AddLote
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLote(CompraViewModel model)
        {
            // Remove CompraId validation from model.Lotes as it's not set yet
            for (int i = 0; i < model.Lotes.Count; i++)
            {
                ModelState.Remove($"Lotes[{i}].ArticuloNombre");
                ModelState.Remove($"Lotes[{i}].Id");
            }

            if (ModelState.IsValid)
            {
                var compra = new Compra
                {
                    Proveedor = model.Proveedor,
                    FechaCompra = model.FechaCompra,
                    NumeroFactura = model.NumeroFactura,
                    Estado = model.Estado,
                    Observaciones = model.Observaciones,
                    FechaSolicitud = DateTime.Now, // Assuming it's immediate
                    UsuarioSolicitante = User.Identity?.Name,
                    CostoTotal = model.Lotes.Sum(l => l.Cantidad * l.CostoUnitario) // Calculate total cost from lotes
                };

                _context.Add(compra);
                await _context.SaveChangesAsync(); // Save Compra to get its ID

                // Add Lotes
                foreach (var loteVm in model.Lotes)
                {
                    var lote = new Lote
                    {
                        ArticuloId = loteVm.ArticuloId,
                        CompraId = compra.Id,
                        Cantidad = loteVm.Cantidad,
                        CostoUnitario = loteVm.CostoUnitario
                    };
                    _context.Lotes.Add(lote);
                    await _context.SaveChangesAsync(); // Save Lote to get its ID before creating Items

                    // For serialized items, create individual Item records.
                    // For non-serialized, the single lote record is enough for now.
                    var articulo = await _context.Articulos.FindAsync(loteVm.ArticuloId);
                    if (articulo != null && !articulo.RequiereSerial)
                    {
                        // Sin serial: un único registro con la cantidad total
                        _context.Items.Add(new Item
                        {
                            ArticuloId      = lote.ArticuloId,
                            LoteId          = lote.Id,
                            Serial          = null,
                            Cantidad        = lote.Cantidad,
                            Estado          = "Disponible",
                            FechaAsignacion = DateTime.Now
                        });
                    }
                    else if (articulo != null && articulo.RequiereSerial && loteVm.Seriales?.Count > 0)
                    {
                        // Con serial: crear un item por cada serial recibido del formulario
                        foreach (var serial in loteVm.Seriales.Where(s => !string.IsNullOrWhiteSpace(s)))
                        {
                            var serialNorm = serial.Trim().ToUpper();
                            if (await _context.Items.AnyAsync(i => i.Serial == serialNorm && !i.Eliminado))
                                continue; // omitir duplicados

                            _context.Items.Add(new Item
                            {
                                ArticuloId      = lote.ArticuloId,
                                LoteId          = lote.Id,
                                Serial          = serialNorm,
                                Cantidad        = 1,
                                Estado          = "Disponible",
                                FechaAsignacion = DateTime.Now
                            });
                        }
                    }
                    // Con serial sin seriales enviados: quedan pendientes para escanear en Lotes/Details
                }
                await _context.SaveChangesAsync();

                TempData["Success"] = "Compra y Lotes registrados exitosamente.";
                return RedirectToAction("Details", "Articulos", new { id = model.Lotes.First().ArticuloId }); // Redirect to the Article's Details page
            }
            
            await PopulateViewBagsForLoteCreation();
            ViewBag.SelectedArticulo = await _context.Articulos.FindAsync(model.Lotes.First().ArticuloId);
            return View(model);
        }

        // GET: Lotes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lote = await _context.Lotes
                .Include(l => l.Articulo)
                .Include(l => l.Compra)
                .Include(l => l.Items)
                    .ThenInclude(i => i.Sucursal)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (lote == null)
            {
                return NotFound();
            }

            ViewBag.Sucursales = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _context.Sucursales.Where(s => !s.Eliminado).OrderBy(s => s.Nombre).ToListAsync(),
                "Id", "Nombre");

            return View(lote);
        }

        // POST: Lotes/AddItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(int loteId, string? serial, int cantidad = 1)
        {
            var lote = await _context.Lotes.Include(l => l.Articulo).FirstOrDefaultAsync(l => l.Id == loteId);
            if (lote == null)
            {
                return Json(new { success = false, message = "Lote no encontrado." });
            }

            // Validar serial si es requerido
            if (lote.Articulo.RequiereSerial)
            {
                if (string.IsNullOrWhiteSpace(serial))
                {
                    return Json(new { success = false, message = "El serial no puede estar vacío para este artículo." });
                }
                serial = serial.Trim().ToUpper();
                // Verificar si el serial ya existe
                if (await _context.Items.AnyAsync(i => i.Serial == serial && !i.Eliminado))
                {
                    return Json(new { success = false, message = $"El serial '{serial}' ya está registrado." });
                }
            }
            else // Articulo no requiere serial
            {
                serial = null; // Asegurarse de que el serial sea nulo
                if (cantidad <= 0)
                {
                    return Json(new { success = false, message = "La cantidad debe ser mayor a cero para este artículo." });
                }
            }

            // Verificar límite de cantidad total del lote
            var itemsActualesCount = await _context.Items.Where(i => i.LoteId == loteId && !i.Eliminado).SumAsync(i => i.Cantidad);
            if (itemsActualesCount + cantidad > lote.Cantidad)
            {
                return Json(new { success = false, message = $"Se excede la cantidad máxima del lote. Solo quedan {lote.Cantidad - itemsActualesCount} unidades por registrar." });
            }

            Item? nuevoItem = null;

            if (!lote.Articulo.RequiereSerial)
            {
                // Buscar un item existente sin serial del mismo lote y sucursal "Almacén" para agrupar
                var existingNonSerializedItem = await _context.Items
                    .Where(i => i.LoteId == loteId && 
                                i.Serial == null && 
                                i.SucursalId == null && 
                                i.Estado == "Disponible" && 
                                !i.Eliminado)
                    .FirstOrDefaultAsync();

                if (existingNonSerializedItem != null)
                {
                    // Agrupar con item existente
                    existingNonSerializedItem.Cantidad += cantidad;
                    _context.Update(existingNonSerializedItem);
                    nuevoItem = existingNonSerializedItem;
                }
            }

            if (nuevoItem == null) // Si no se encontró un item para agrupar o si requiere serial
            {
                nuevoItem = new Item
                {
                    Serial = serial,
                    ArticuloId = lote.ArticuloId,
                    LoteId = lote.Id,
                    Estado = "Disponible",
                    Cantidad = cantidad,
                    FechaAsignacion = DateTime.Now
                };
                _context.Items.Add(nuevoItem);
            }
            
            await _context.SaveChangesAsync();

            // Recalcular itemsActuales para el progreso
            itemsActualesCount = await _context.Items.Where(i => i.LoteId == loteId && !i.Eliminado).SumAsync(i => i.Cantidad);

            return Json(new { 
                success = true, 
                message = "Item(s) agregado(s) correctamente.",
                item = new {
                    id = nuevoItem.Id,
                    serial = nuevoItem.Serial ?? "N/A",
                    estado = nuevoItem.Estado,
                    fecha = nuevoItem.FechaAsignacion?.ToString("dd/MM/yyyy HH:mm"),
                    cantidad = nuevoItem.Cantidad
                },
                progress = $"{itemsActualesCount} / {lote.Cantidad}"
            });
        }

        private async Task PopulateViewBagsForLoteCreation()
        {
            ViewBag.Articulos = await _context.Articulos
                .Where(a => !a.Eliminado)
                .OrderBy(a => a.Marca).ThenBy(a => a.Modelo)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = $"{a.Marca} {a.Modelo}"
                })
                .ToListAsync();

            ViewBag.Departamentos = await _context.Departamentos
                .Where(d => d.Activo && !d.Eliminado)
                .OrderBy(d => d.Nombre)
                .ToListAsync();
        }
    }
}