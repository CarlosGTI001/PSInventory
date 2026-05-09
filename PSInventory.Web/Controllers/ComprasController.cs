using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;
using PSInventory.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using PSInventory.Web.Services;

namespace PSInventory.Web.Controllers
{
    [AuthorizeRole("Administrador", "Jefe")]
    public class ComprasController : Controller
    {
        private readonly PSDatos _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ComprasController(PSDatos context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Compras
        public async Task<IActionResult> Index(string q = "", int page = 1, int pageSize = 20)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 10 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var query = _context.Compras
                .Where(c => !c.Eliminado)
                .Include(c => c.Departamento)
                .Include(c => c.Lotes)
                    .ThenInclude(l => l.Items)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(c =>
                    (c.Proveedor != null && c.Proveedor.ToLower().Contains(term)) ||
                    (c.NumeroFactura != null && c.NumeroFactura.ToLower().Contains(term)) ||
                    (c.Estado != null && c.Estado.ToLower().Contains(term)) ||
                    (c.Departamento != null && c.Departamento.Nombre.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages > 0 && page > totalPages) page = totalPages;

            var compras = await query
                .OrderByDescending(c => c.FechaCompra)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Query = q;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;
            return View(compras);
        }

        // GET: Compras/ExportarCsv
        public async Task<IActionResult> ExportarCsv(string q = "")
        {
            var query = _context.Compras
                .Where(c => !c.Eliminado)
                .Include(c => c.Departamento)
                .Include(c => c.Lotes)
                    .ThenInclude(l => l.Items)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(c =>
                    (c.Proveedor != null && c.Proveedor.ToLower().Contains(term)) ||
                    (c.NumeroFactura != null && c.NumeroFactura.ToLower().Contains(term)) ||
                    (c.Estado != null && c.Estado.ToLower().Contains(term)) ||
                    (c.Departamento != null && c.Departamento.Nombre.ToLower().Contains(term)));
            }

            var compras = await query
                .OrderByDescending(c => c.FechaCompra)
                .ToListAsync();

            var headers = new[]
            {
                "FechaCompra", "Proveedor", "NumeroFactura", "Estado", "Departamento", "Lotes", "ItemsRegistrados", "CostoTotal", "Observaciones"
            };

            var rows = compras.Select(c => new[]
            {
                c.FechaCompra.ToString("dd/MM/yyyy"),
                c.Proveedor,
                c.NumeroFactura ?? string.Empty,
                c.Estado,
                c.Departamento?.Nombre ?? string.Empty,
                (c.Lotes?.Count ?? 0).ToString(),
                (c.Lotes?.Sum(l => l.Items?.Count ?? 0) ?? 0).ToString(),
                c.CostoTotal.ToString("F2"),
                c.Observaciones ?? string.Empty
            });

            var bytes = CsvExportService.BuildCsv(headers, rows);
            return File(bytes, "text/csv; charset=utf-8", $"compras_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }

        // GET: Compras/Create
        public async Task<IActionResult> Create()
        {
            await PopulateArticulosViewBag();
            return View(new CompraViewModel
            {
                FechaCompra = DateTime.Now,
                Estado = "Solicitud"
            });
        }

        // POST: Compras/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompraViewModel model, IFormFile? facturaFile)
        {
            var lotesValidos = (model.Lotes ?? new List<LoteViewModel>())
                .Where(l => l.ArticuloId > 0 && l.Cantidad > 0 && l.CostoUnitario > 0)
                .ToList();

            if (!lotesValidos.Any())
            {
                ModelState.AddModelError("Lotes", "Debe agregar al menos un lote válido.");
            }

            var articulosPorId = new Dictionary<int, Articulo>();
            if (!ModelState.IsValid)
            {
                await PopulateArticulosViewBag();
                return View(model);
            }

            var articuloIds = lotesValidos.Select(l => l.ArticuloId).Distinct().ToList();
            articulosPorId = await _context.Articulos
                .Where(a => !a.Eliminado && articuloIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id);

            if (articulosPorId.Count != articuloIds.Count)
            {
                ModelState.AddModelError("Lotes", "Uno o más artículos seleccionados no son válidos.");
                await PopulateArticulosViewBag();
                return View(model);
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
                    CostoTotal = lotesValidos.Sum(l => l.Cantidad * l.CostoUnitario),
                    UsuarioSolicitante = User.Identity?.Name,
                    FechaSolicitud = DateTime.Now
                };

                // Handle file upload
                if (facturaFile != null && facturaFile.Length > 0)
                {
                    var extension = Path.GetExtension(facturaFile.FileName).ToLowerInvariant();
                    var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("facturaFile", "Solo se permiten archivos PDF, JPG, JPEG o PNG.");
                        await PopulateArticulosViewBag();
                        return View(model);
                    }
                    if (facturaFile.Length > 10 * 1024 * 1024) // Max 10MB
                    {
                        ModelState.AddModelError("facturaFile", "El archivo no debe superar los 10 MB.");
                        await PopulateArticulosViewBag();
                        return View(model);
                    }

                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "facturas");
                    Directory.CreateDirectory(uploadsFolder);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await facturaFile.CopyToAsync(stream);
                    }
                    compra.RutaFactura = $"/uploads/facturas/{fileName}";
                }

                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.Add(compra);
                    await _context.SaveChangesAsync();

                    foreach (var loteVm in lotesValidos)
                    {
                        var lote = new Lote
                        {
                            ArticuloId = loteVm.ArticuloId,
                            CompraId = compra.Id,
                            Cantidad = loteVm.Cantidad,
                            CostoUnitario = loteVm.CostoUnitario
                        };
                        _context.Lotes.Add(lote);
                        await _context.SaveChangesAsync();

                        var articulo = articulosPorId[loteVm.ArticuloId];
                        if (!articulo.RequiereSerial)
                        {
                            _context.Items.Add(new Item
                            {
                                ArticuloId = lote.ArticuloId,
                                LoteId = lote.Id,
                                Serial = null,
                                Cantidad = lote.Cantidad,
                                Estado = "Disponible",
                                FechaAsignacion = DateTime.Now
                            });
                        }
                        else if (loteVm.Seriales != null && loteVm.Seriales.Any())
                        {
                            var seriales = loteVm.Seriales
                                .Where(s => !string.IsNullOrWhiteSpace(s))
                                .Select(s => s.Trim().ToUpper())
                                .Distinct()
                                .Take(lote.Cantidad)
                                .ToList();

                            foreach (var serial in seriales)
                            {
                                var existeSerial = await _context.Items.AnyAsync(i => !i.Eliminado && i.Serial == serial);
                                if (existeSerial)
                                {
                                    continue;
                                }

                                _context.Items.Add(new Item
                                {
                                    ArticuloId = lote.ArticuloId,
                                    LoteId = lote.Id,
                                    Serial = serial,
                                    Cantidad = 1,
                                    Estado = "Disponible",
                                    FechaAsignacion = DateTime.Now
                                });
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                TempData["Success"] = "Compra creada exitosamente con sus lotes.";
                return RedirectToAction("Details", new { id = compra.Id });
            }
            await PopulateArticulosViewBag();
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Where(c => !c.Eliminado && c.Id == id.Value)
                .Include(c => c.Lotes)
                .FirstOrDefaultAsync();
            if (compra == null)
            {
                return NotFound();
            }

            await PopulateArticulosViewBag();
            return View(ToViewModel(compra));
        }

        // POST: Compras/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CompraViewModel model, IFormFile? facturaFile)
        {
            if (model.Id == null || id != model.Id.Value)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Where(c => !c.Eliminado && c.Id == id)
                .FirstOrDefaultAsync();
            if (compra == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                model.RutaFactura = compra.RutaFactura;
                await PopulateArticulosViewBag();
                return View(model);
            }

            compra.Proveedor = model.Proveedor;
            compra.FechaCompra = model.FechaCompra;
            compra.NumeroFactura = model.NumeroFactura;
            compra.Estado = model.Estado;
            compra.Observaciones = model.Observaciones;

            try
            {
                // Handle file upload
                if (facturaFile != null && facturaFile.Length > 0)
                {
                    var extension = Path.GetExtension(facturaFile.FileName).ToLowerInvariant();
                    var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("facturaFile", "Solo se permiten archivos PDF, JPG, JPEG o PNG.");
                        model.RutaFactura = compra.RutaFactura;
                        await PopulateArticulosViewBag();
                        return View(model);
                    }
                    if (facturaFile.Length > 10 * 1024 * 1024) // Max 10MB
                    {
                        ModelState.AddModelError("facturaFile", "El archivo no debe superar los 10 MB.");
                        model.RutaFactura = compra.RutaFactura;
                        await PopulateArticulosViewBag();
                        return View(model);
                    }

                    // Delete old file if exists
                    if (!string.IsNullOrEmpty(compra.RutaFactura))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, compra.RutaFactura.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "facturas");
                    Directory.CreateDirectory(uploadsFolder);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await facturaFile.CopyToAsync(stream);
                    }
                    compra.RutaFactura = $"/uploads/facturas/{fileName}";
                }

                _context.Update(compra);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Compra actualizada exitosamente.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompraExists(id))
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

        // GET: Compras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compra = await _context.Compras
                .Include(c => c.Lotes)
                .ThenInclude(l => l.Items)
                .ThenInclude(i => i.Articulo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        // POST: Compras/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var compra = await _context.Compras
                .Where(c => !c.Eliminado)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (compra == null)
            {
                return Json(new { success = false, message = "Compra no encontrada" });
            }

            // Verificar si tiene lotes asociados activos
            var tieneLotes = await _context.Lotes.AnyAsync(l => l.CompraId == id && l.Items.Any(i => !i.Eliminado));
            if (tieneLotes)
            {
                return Json(new { success = false, message = "No se puede eliminar la compra porque tiene lotes con items asociados" });
            }

            // Soft delete
            compra.Eliminado = true;
            compra.FechaEliminacion = DateTime.Now;
            compra.UsuarioEliminacion = User.Identity?.Name ?? "Sistema";

            _context.Update(compra);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Compra eliminada exitosamente" });
        }

        private bool CompraExists(int id)
        {
            return _context.Compras.Any(e => e.Id == id && !e.Eliminado);
        }

        private CompraViewModel ToViewModel(Compra compra)
        {
            return new CompraViewModel
            {
                Id = compra.Id,
                Proveedor = compra.Proveedor,
                FechaCompra = compra.FechaCompra,
                NumeroFactura = compra.NumeroFactura,
                Estado = compra.Estado,
                Observaciones = compra.Observaciones,
                RutaFactura = compra.RutaFactura,
                Lotes = compra.Lotes?.Select(l => new LoteViewModel
                {
                    Id = l.Id,
                    ArticuloId = l.ArticuloId,
                    Cantidad = l.Cantidad,
                    CostoUnitario = l.CostoUnitario
                }).ToList() ?? new List<LoteViewModel>()
            };
        }

        private async Task PopulateArticulosViewBag()
        {
            ViewBag.Articulos = await _context.Articulos
                .Where(a => !a.Eliminado)
                .OrderBy(a => a.Marca)
                .ThenBy(a => a.Modelo)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = $"{a.Marca} {a.Modelo}"
                })
                .ToListAsync();
        }
    }
}
