using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Services;
using PSInventory.Web.Filters;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PSInventory.Web.Controllers
{
    [RequireAuth]
    public class ReportesController : Controller
    {
        private readonly PSDatos _context;

        public ReportesController(PSDatos context)
        {
            _context = context;
        }

        // Usuario logueado (sesión)
        private string UsuarioActual => HttpContext.Session.GetString("UserName") ?? "Sistema";

        // GET: Reportes
        public async Task<IActionResult> Index()
        {
            // Cargar datos para dropdowns de filtros
            ViewBag.Categorias = await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync();
            ViewBag.Sucursales = await _context.Sucursales.OrderBy(s => s.Nombre).ToListAsync();
            return View();
        }

        /// <summary>
        /// 1. Inventario General - Reporte completo de todos los items con filtros
        /// </summary>
        public async Task<IActionResult> InventarioGeneral(string? estado, int? categoriaId, string? sucursalId)
        {
            try
            {
                var usuario = UsuarioActual;
                
                // Query base con includes necesarios
                var query = _context.Items
                    .Where(i => !i.Eliminado)
                    .Include(i => i.Articulo)
                    .ThenInclude(a => a.Categoria)
                    .Include(i => i.Lote)
                    .ThenInclude(l => l.Compra)
                    .Include(i => i.Sucursal)
                    .AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrEmpty(estado))
                {
                    query = query.Where(i => i.Estado == estado);
                }

                if (categoriaId.HasValue)
                {
                    query = query.Where(i => i.Articulo.CategoriaId == categoriaId.Value);
                }

                if (!string.IsNullOrEmpty(sucursalId))
                {
                    query = query.Where(i => i.SucursalId == sucursalId);
                }

                var items = await query.OrderBy(i => i.Serial).ToListAsync();

                // Si no hay datos, retornar PDF vacío
                if (!items.Any())
                {
                    var pdfVacio = PdfReportService.GenerarPdfVacio(
                        "Inventario General",
                        "No hay datos disponibles con los filtros aplicados"
                    );
                    return File(pdfVacio, "application/pdf", "InventarioGeneral.pdf");
                }

                // Preparar datos para filtros (fuera de la lambda)
                var filtros = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(estado))
                    filtros.Add("Estado", estado);
                if (categoriaId.HasValue)
                {
                    var categoria = await _context.Categorias.FindAsync(categoriaId.Value);
                    filtros.Add("Categoría", categoria?.Nombre ?? "No especificada");
                }
                if (!string.IsNullOrEmpty(sucursalId))
                {
                    var sucursal = await _context.Sucursales.FindAsync(sucursalId);
                    filtros.Add("Sucursal", sucursal?.Nombre ?? "No especificada");
                }

                // Título dinámico según filtros
                var tituloReporte = !string.IsNullOrEmpty(sucursalId) && filtros.ContainsKey("Sucursal")
                    ? $"Inventario General — {filtros["Sucursal"]}"
                    : "Inventario General";

                // Preparar datos para tabla
                var headers = new List<string> 
                { 
                    "Serial / ID", "Artículo", "Categoría", "Cantidad", "Sucursal", "Estado", "Costo" 
                };

                var filas = items.Select(i => new List<string>
                {
                    i.Serial ?? "N/A",
                    $"{i.Articulo.Marca} {i.Articulo.Modelo}",
                    i.Articulo.Categoria.Nombre,
                    i.Cantidad.ToString(),
                    i.Sucursal?.Nombre ?? "Sin Sucursal",
                    i.Estado,
                    (i.Lote?.CostoUnitario ?? 0).ToString("C")
                }).ToList();

                // Preparar totales
                var totales = new Dictionary<string, string>
                {
                    { "Total Unidades", items.Sum(i => i.Cantidad).ToString() },
                    { "Costo Total", items.Sum(i => (i.Lote?.CostoUnitario ?? 0) * i.Cantidad).ToString("C") }
                };

                // Crear documento PDF
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.Letter);
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                        // Header
                        page.Header().Element(c => PdfReportService.GenerarHeader(c, tituloReporte, usuario));

                        // Contenido
                        page.Content().Column(column =>
                        {
                            if (filtros.Any())
                                column.Item().Element(c => PdfReportService.GenerarFiltros(c, filtros));

                            column.Item().PaddingTop(15);

                            column.Item().Element(c => 
                                PdfReportService.GenerarTablaSimple(c, headers, filas));

                            column.Item().PaddingTop(15);

                            column.Item().Element(c => 
                                PdfReportService.GenerarResumenTotales(c, totales));
                        });

                        // Footer
                        page.Footer().Element(c => PdfReportService.GenerarFooter(c, usuario));
                    });
                });

                var pdfBytes = document.GeneratePdf();
                return File(pdfBytes, "application/pdf", "InventarioGeneral.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al generar el reporte: {ex.Message}");
            }
        }

        /// <summary>
        /// 2. Inventario Por Sucursal - Reporte detallado de items por sucursal específica
        /// </summary>
        public async Task<IActionResult> InventarioPorSucursal(string sucursalId)
        {
            try
            {
                var usuario = UsuarioActual;

                // Obtener sucursal
                var sucursal = await _context.Sucursales.FindAsync(sucursalId);
                if (sucursal == null)
                {
                    var pdfVacio = PdfReportService.GenerarPdfVacio(
                        "Inventario Por Sucursal",
                        "Sucursal no encontrada"
                    );
                    return File(pdfVacio, "application/pdf", "InventarioPorSucursal.pdf");
                }

                // Query con items de la sucursal
                var items = await _context.Items
                    .Where(i => i.SucursalId == sucursalId)
                    .Include(i => i.Articulo)
                    .ThenInclude(a => a.Categoria)
                    .Include(i => i.Lote)
                    .ThenInclude(l => l.Compra)
                    .Include(i => i.Sucursal)
                    .OrderBy(i => i.Serial)
                    .ToListAsync();

                // Si no hay datos, retornar PDF vacío
                if (!items.Any())
                {
                    var pdfVacio = PdfReportService.GenerarPdfVacio(
                        "Inventario Por Sucursal",
                        $"No hay items registrados en la sucursal {sucursal.Nombre}"
                    );
                    return File(pdfVacio, "application/pdf", "InventarioPorSucursal.pdf");
                }

                // Crear documento PDF
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.Letter);
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                        // Header
                        page.Header().Element(c => 
                            PdfReportService.GenerarHeader(c, $"Inventario Por Sucursal: {sucursal.Nombre}", usuario));

                        // Contenido
                        page.Content().Column(column =>
                        {
                            // Filtros
                            var filtros = new Dictionary<string, string>
                            {
                                { "Sucursal", sucursal.Nombre }
                            };

                            column.Item().Element(c => PdfReportService.GenerarFiltros(c, filtros));
                            column.Item().PaddingTop(15);

                            // Tabla
                            var headers = new List<string> 
                            { 
                                "Serial / ID", "Artículo", "Categoría", "Cantidad", "Estado", "Responsable", "Costo" 
                            };

                            var filas = items.Select(i => new List<string>
                            {
                                i.Serial ?? "N/A",
                                $"{i.Articulo.Marca} {i.Articulo.Modelo}",
                                i.Articulo.Categoria.Nombre,
                                i.Cantidad.ToString(),
                                i.Estado,
                                i.ResponsableEmpleado ?? "No asignado",
                                (i.Lote?.CostoUnitario ?? 0).ToString("C")
                            }).ToList();

                            column.Item().Element(c => 
                                PdfReportService.GenerarTablaSimple(c, headers, filas));

                            column.Item().PaddingTop(15);

                            // Totales
                            var totales = new Dictionary<string, string>
                            {
                                { "Total Unidades en Sucursal", items.Sum(i => i.Cantidad).ToString() },
                                { "Costo Total", items.Sum(i => (i.Lote?.CostoUnitario ?? 0) * i.Cantidad).ToString("C") }
                            };

                            column.Item().Element(c => 
                                PdfReportService.GenerarResumenTotales(c, totales));
                        });

                        // Footer
                        page.Footer().Element(c => PdfReportService.GenerarFooter(c, usuario));
                    });
                });

                var pdfBytes = document.GeneratePdf();
                return File(pdfBytes, "application/pdf", $"InventarioPorSucursal_{sucursal.Nombre}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al generar el reporte: {ex.Message}");
            }
        }

        /// <summary>
        /// 3. Movimientos de Items - Historial de transferencias y movimientos
        /// </summary>
        public async Task<IActionResult> MovimientosItems(DateTime? fechaInicio, DateTime? fechaFin, string? sucursalId)
        {
            try
            {
                var usuario = UsuarioActual;

                // Query base con includes
                var query = _context.MovimientosItem
                    .Include(m => m.Item)
                    .ThenInclude(i => i.Articulo)
                    .Include(m => m.SucursalOrigen)
                    .Include(m => m.SucursalDestino)
                    .AsQueryable();

                // Aplicar filtros de fecha
                if (fechaInicio.HasValue)
                {
                    query = query.Where(m => m.FechaMovimiento >= fechaInicio.Value);
                }

                if (fechaFin.HasValue)
                {
                    var fechaFinConHora = fechaFin.Value.AddDays(1).AddSeconds(-1);
                    query = query.Where(m => m.FechaMovimiento <= fechaFinConHora);
                }

                // Aplicar filtro de sucursal (origen o destino)
                if (!string.IsNullOrEmpty(sucursalId))
                {
                    query = query.Where(m => 
                        m.SucursalOrigenId == sucursalId || m.SucursalDestinoId == sucursalId);
                }

                var movimientos = await query.OrderByDescending(m => m.FechaMovimiento).ToListAsync();

                // Si no hay datos, retornar PDF vacío
                if (!movimientos.Any())
                {
                    var pdfVacio = PdfReportService.GenerarPdfVacio(
                        "Movimientos de Items",
                        "No hay movimientos registrados con los filtros aplicados"
                    );
                    return File(pdfVacio, "application/pdf", "MovimientosItems.pdf");
                }

                // Preparar datos para filtros (fuera de la lambda)
                var filtros = new Dictionary<string, string>();
                if (fechaInicio.HasValue)
                    filtros.Add("Fecha Inicio", fechaInicio.Value.ToString("dd/MM/yyyy"));
                if (fechaFin.HasValue)
                    filtros.Add("Fecha Fin", fechaFin.Value.ToString("dd/MM/yyyy"));
                if (!string.IsNullOrEmpty(sucursalId))
                {
                    var sucursal = await _context.Sucursales.FindAsync(sucursalId);
                    filtros.Add("Sucursal", sucursal?.Nombre ?? "No especificada");
                }

                // Preparar datos para tabla
                var headers = new List<string> 
                { 
                    "Fecha", "Serial", "Artículo", "Origen", "Destino", "Motivo", "Usuario" 
                };

                var filas = movimientos.Select(m => new List<string>
                {
                    m.FechaMovimiento.ToString("dd/MM/yyyy HH:mm"),
                    m.Item.Serial ?? "N/A",
                    $"{m.Item.Articulo.Marca} {m.Item.Articulo.Modelo}",
                    m.SucursalOrigen?.Nombre ?? "Almacén Central",
                    m.SucursalDestino?.Nombre ?? "Almacén Central",
                    m.Motivo,
                    m.UsuarioResponsable
                }).ToList();

                // Preparar totales
                var totales = new Dictionary<string, string>
                {
                    { "Total Movimientos", movimientos.Count.ToString() }
                };

                // Crear documento PDF
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.Letter);
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                        // Header
                        page.Header().Element(c => 
                            PdfReportService.GenerarHeader(c, "Movimientos de Items", usuario));

                        // Contenido
                        page.Content().Column(column =>
                        {
                            if (filtros.Any())
                                column.Item().Element(c => PdfReportService.GenerarFiltros(c, filtros));

                            column.Item().PaddingTop(15);

                            column.Item().Element(c => 
                                PdfReportService.GenerarTablaSimple(c, headers, filas));

                            column.Item().PaddingTop(15);

                            column.Item().Element(c => 
                                PdfReportService.GenerarResumenTotales(c, totales));
                        });

                        // Footer
                        page.Footer().Element(c => PdfReportService.GenerarFooter(c, usuario));
                    });
                });

                var pdfBytes = document.GeneratePdf();
                return File(pdfBytes, "application/pdf", "MovimientosItems.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al generar el reporte: {ex.Message}");
            }
        }

        /// <summary>
        /// 4. Items Garantía Por Vencer - Reporte de items próximos a vencer garantía
        /// </summary>
        public async Task<IActionResult> ItemsGarantiaPorVencer(int dias = 30)
        {
            try
            {
                var usuario = UsuarioActual;
                var fechaHoy = DateTime.Now;
                var fechaLimite = fechaHoy.AddDays(dias);

                // Query de items con garantía por vencer
                var items = await _context.Items
                    .Include(i => i.Articulo)
                    .ThenInclude(a => a.Categoria)
                    .Include(i => i.Sucursal)
                    .Where(i => i.FechaGarantiaVencimiento.HasValue)
                    .Where(i => i.FechaGarantiaVencimiento > fechaHoy && i.FechaGarantiaVencimiento <= fechaLimite)
                    .OrderBy(i => i.FechaGarantiaVencimiento)
                    .ToListAsync();

                // Si no hay datos, retornar PDF vacío
                if (!items.Any())
                {
                    var pdfVacio = PdfReportService.GenerarPdfVacio(
                        "Items Garantía Por Vencer",
                        $"No hay items con garantía venciendo en los próximos {dias} días"
                    );
                    return File(pdfVacio, "application/pdf", "ItemsGarantiaPorVencer.pdf");
                }

                // Crear documento PDF
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.Letter);
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                        // Header
                        page.Header().Element(c => 
                            PdfReportService.GenerarHeader(c, "Items Garantía Por Vencer", usuario));

                        // Contenido
                        page.Content().Column(column =>
                        {
                            // Filtros
                            var filtros = new Dictionary<string, string>
                            {
                                { "Días de Vigencia", $"Próximos {dias} días" }
                            };

                            column.Item().Element(c => PdfReportService.GenerarFiltros(c, filtros));
                            column.Item().PaddingTop(15);

                            // Tabla
                            var headers = new List<string> 
                            { 
                                "Serial", "Artículo", "Fecha Inicio", "Meses", "Vencimiento", "Días Restantes" 
                            };

                            var filas = items.Select(i => 
                            {
                                var diasRestantes = i.FechaGarantiaVencimiento.HasValue 
                                    ? (int)(i.FechaGarantiaVencimiento.Value - DateTime.Now).TotalDays
                                    : 0;

                                return new List<string>
                                {
                                    i.Serial ?? "N/A",
                                    $"{i.Articulo.Marca} {i.Articulo.Modelo}",
                                    i.FechaGarantiaInicio.HasValue ? i.FechaGarantiaInicio.Value.ToString("dd/MM/yyyy") : "N/A",
                                    i.MesesGarantia?.ToString() ?? "N/A",
                                    i.FechaGarantiaVencimiento.HasValue ? i.FechaGarantiaVencimiento.Value.ToString("dd/MM/yyyy") : "N/A",
                                    diasRestantes.ToString()
                                };
                            }).ToList();

                            column.Item().Element(c => 
                                PdfReportService.GenerarTablaSimple(c, headers, filas));

                            column.Item().PaddingTop(15);

                            // Totales
                            var totales = new Dictionary<string, string>
                            {
                                { "Total Items por Vencer", items.Count.ToString() }
                            };

                            column.Item().Element(c => 
                                PdfReportService.GenerarResumenTotales(c, totales));
                        });

                        // Footer
                        page.Footer().Element(c => PdfReportService.GenerarFooter(c, usuario));
                    });
                });

                var pdfBytes = document.GeneratePdf();
                return File(pdfBytes, "application/pdf", "ItemsGarantiaPorVencer.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al generar el reporte: {ex.Message}");
            }
        }

        /// <summary>
        /// 5. Reporte de Compras - Reporte detallado de compras realizadas
        /// </summary>
        public async Task<IActionResult> ReporteCompras(DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                var usuario = UsuarioActual;

                // Query base
                var query = _context.Compras
                    .Include(c => c.Lotes)
                    .ThenInclude(l => l.Items)
                    .ThenInclude(i => i.Articulo)
                    .AsQueryable();

                // Aplicar filtros de fecha
                if (fechaInicio.HasValue)
                {
                    query = query.Where(c => c.FechaCompra >= fechaInicio.Value);
                }

                if (fechaFin.HasValue)
                {
                    var fechaFinConHora = fechaFin.Value.AddDays(1).AddSeconds(-1);
                    query = query.Where(c => c.FechaCompra <= fechaFinConHora);
                }

                var compras = await query.OrderByDescending(c => c.FechaCompra).ToListAsync();

                // Si no hay datos, retornar PDF vacío
                if (!compras.Any())
                {
                    var pdfVacio = PdfReportService.GenerarPdfVacio(
                        "Reporte de Compras",
                        "No hay compras registradas con los filtros aplicados"
                    );
                    return File(pdfVacio, "application/pdf", "ReporteCompras.pdf");
                }

                // Crear documento PDF
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.Letter);
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                        // Header
                        page.Header().Element(c => 
                            PdfReportService.GenerarHeader(c, "Reporte de Compras", usuario));

                        // Contenido
                        page.Content().Column(column =>
                        {
                            // Filtros
                            var filtros = new Dictionary<string, string>();
                            if (fechaInicio.HasValue)
                                filtros.Add("Fecha Inicio", fechaInicio.Value.ToString("dd/MM/yyyy"));
                            if (fechaFin.HasValue)
                                filtros.Add("Fecha Fin", fechaFin.Value.ToString("dd/MM/yyyy"));

                            if (filtros.Any())
                                column.Item().Element(c => PdfReportService.GenerarFiltros(c, filtros));

                            column.Item().PaddingTop(15);

                            // Tabla
                            var headers = new List<string> 
                            { 
                                "Fecha", "Proveedor", "Nº Factura", "Items", "Costo Total", "Estado" 
                            };

                            var filas = compras.Select(c => new List<string>
                            {
                                c.FechaCompra.ToString("dd/MM/yyyy"),
                                c.Proveedor,
                                c.NumeroFactura ?? "N/A",
                                c.Lotes?.Sum(l => l.Items?.Sum(i => i.Cantidad) ?? 0).ToString() ?? "0",
                                c.CostoTotal.ToString("C"),
                                c.Estado
                            }).ToList();

                            column.Item().Element(c => 
                                PdfReportService.GenerarTablaSimple(c, headers, filas));

                            column.Item().PaddingTop(15);

                            // Totales
                            var totales = new Dictionary<string, string>
                            {
                                { "Total Compras", compras.Count.ToString() },
                                { "Monto Total", compras.Sum(c => c.CostoTotal).ToString("C") }
                            };

                            column.Item().Element(c => 
                                PdfReportService.GenerarResumenTotales(c, totales));
                        });

                        // Footer
                        page.Footer().Element(c => PdfReportService.GenerarFooter(c, usuario));
                    });
                });

                var pdfBytes = document.GeneratePdf();
                return File(pdfBytes, "application/pdf", "ReporteCompras.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al generar el reporte: {ex.Message}");
            }
        }

        /// <summary>
        /// 6. Estadísticas Generales - Reporte con datos agregados y múltiples tablas
        /// </summary>
        public async Task<IActionResult> EstadisticasGenerales()
        {
            try
            {
                var usuario = UsuarioActual;

                // Obtener datos agregados
                var items = await _context.Items
                    .Include(i => i.Articulo)
                    .ThenInclude(a => a.Categoria)
                    .Include(i => i.Lote)
                    .Include(i => i.Sucursal)
                    .ToListAsync();

                if (!items.Any())
                {
                    var pdfVacio = PdfReportService.GenerarPdfVacio(
                        "Estadísticas Generales",
                        "No hay datos disponibles en el sistema"
                    );
                    return File(pdfVacio, "application/pdf", "EstadisticasGenerales.pdf");
                }

                // Crear documento PDF
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.Letter);
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                        // Header
                        page.Header().Element(c => 
                            PdfReportService.GenerarHeader(c, "Estadísticas Generales", usuario));

                        // Contenido
                        page.Content().Column(column =>
                        {
                            // SECCIÓN 1: Items por Estado
                            column.Item().Text("Items por Estado").FontSize(12).Bold().FontColor("#047394");
                            column.Item().PaddingTop(5).PaddingBottom(10);

                            var itemsPorEstado = items.Where(i => !i.Eliminado).GroupBy(i => i.Estado)
                                .OrderByDescending(g => g.Sum(i => i.Cantidad))
                                .ToList();

                            var totalUnidades = itemsPorEstado.Sum(g => g.Sum(i => i.Cantidad));
                            var headerEstado = new List<string> { "Estado", "Unidades", "Porcentaje" };
                            var filasEstado = itemsPorEstado.Select(g => new List<string>
                            {
                                g.Key,
                                g.Sum(i => i.Cantidad).ToString(),
                                $"{(g.Sum(i => i.Cantidad) * 100.0 / (totalUnidades > 0 ? totalUnidades : 1)):F1}%"
                            }).ToList();

                            column.Item().Element(c => 
                                PdfReportService.GenerarTablaSimple(c, headerEstado, filasEstado));

                            column.Item().PaddingTop(15);

                            // SECCIÓN 2: Items por Categoría
                            column.Item().Text("Items por Categoría").FontSize(12).Bold().FontColor("#047394");
                            column.Item().PaddingTop(5).PaddingBottom(10);

                            var itemsPorCategoria = items.Where(i => !i.Eliminado).GroupBy(i => i.Articulo.Categoria.Nombre)
                                .OrderByDescending(g => g.Sum(i => i.Cantidad))
                                .ToList();

                            var headerCategoria = new List<string> { "Categoría", "Unidades", "Costo Total" };
                            var filasCategoria = itemsPorCategoria.Select(g => new List<string>
                            {
                                g.Key,
                                g.Sum(i => i.Cantidad).ToString(),
                                g.Sum(i => (i.Lote?.CostoUnitario ?? 0) * i.Cantidad).ToString("C")
                            }).ToList();

                            column.Item().Element(c => 
                                PdfReportService.GenerarTablaSimple(c, headerCategoria, filasCategoria));

                            column.Item().PaddingTop(15);

                            // SECCIÓN 3: Items por Sucursal
                            column.Item().Text("Items por Sucursal").FontSize(12).Bold().FontColor("#047394");
                            column.Item().PaddingTop(5).PaddingBottom(10);

                            var itemsPorSucursal = items.Where(i => !i.Eliminado).GroupBy(i => i.Sucursal?.Nombre ?? "Sin Sucursal")
                                .OrderByDescending(g => g.Sum(i => i.Cantidad))
                                .ToList();

                            var headerSucursal = new List<string> { "Sucursal", "Unidades", "Costo Total" };
                            var filasSucursal = itemsPorSucursal.Select(g => new List<string>
                            {
                                g.Key,
                                g.Sum(i => i.Cantidad).ToString(),
                                g.Sum(i => (i.Lote?.CostoUnitario ?? 0) * i.Cantidad).ToString("C")
                            }).ToList();

                            column.Item().Element(c => 
                                PdfReportService.GenerarTablaSimple(c, headerSucursal, filasSucursal));

                            column.Item().PaddingTop(15);

                            // Totales generales
                            var itemsActivos = items.Where(i => !i.Eliminado).ToList();
                            var totales = new Dictionary<string, string>
                            {
                                { "Total Unidades", itemsActivos.Sum(i => i.Cantidad).ToString() },
                                { "Unidades Disponibles", itemsActivos.Where(i => i.Estado == "Disponible").Sum(i => i.Cantidad).ToString() },
                                { "Valor Total Inventario", itemsActivos.Sum(i => (i.Lote?.CostoUnitario ?? 0) * i.Cantidad).ToString("C") }
                            };

                            column.Item().Element(c => 
                                PdfReportService.GenerarResumenTotales(c, totales));
                        });

                        // Footer
                        page.Footer().Element(c => PdfReportService.GenerarFooter(c, usuario));
                    });
                });

                var pdfBytes = document.GeneratePdf();
                return File(pdfBytes, "application/pdf", "EstadisticasGenerales.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al generar el reporte: {ex.Message}");
            }
        }
    }
}
