using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Services;
using PSInventory.Web.Filters;
using PSInventory.Web.Models.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text.Json;

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

        private async Task<Dictionary<int, DateTime?>> ObtenerUltimoMovimientoPorItemAsync(IEnumerable<int> itemsIds)
        {
            var ids = itemsIds.Distinct().ToList();
            if (!ids.Any())
            {
                return new Dictionary<int, DateTime?>();
            }

            return await _context.MovimientosItem
                .Where(m => ids.Contains(m.ItemId))
                .Where(m => m.Observaciones == null || !m.Observaciones.Contains(MovimientoLogicoService.MarcaEliminadoLogico))
                .GroupBy(m => m.ItemId)
                .Select(g => new
                {
                    ItemId = g.Key,
                    FechaUltimoMovimiento = g.Max(m => m.FechaMovimiento)
                })
                .ToDictionaryAsync(x => x.ItemId, x => (DateTime?)x.FechaUltimoMovimiento);
        }

        // GET: Reportes
        public async Task<IActionResult> Index()
        {
            // Cargar datos para dropdowns de filtros
            ViewBag.Categorias = await _context.Categorias
                .Where(c => !c.Eliminado)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
            ViewBag.Sucursales = await _context.Sucursales
                .Where(s => !s.Eliminado && s.Activo)
                .OrderBy(s => s.Nombre)
                .ToListAsync();
            return View();
        }

        /// <summary>
        /// 1. Inventario General - Reporte completo de todos los items con filtros
        /// </summary>
        public async Task<IActionResult> InventarioGeneral(string? estado, int? categoriaId, string? sucursalId, bool includeCostos = true)
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
                    .ThenInclude(s => s.Region)
                    .Where(i => i.Articulo != null && !i.Articulo.Eliminado)
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
                    query = query.Where(i => i.SucursalId == sucursalId && i.Sucursal != null && !i.Sucursal.Eliminado);
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

                var ultimoMovimientoPorItem = await ObtenerUltimoMovimientoPorItemAsync(items.Select(i => i.Id));

                // Preparar datos para filtros (fuera de la lambda)
                var filtros = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(estado))
                    filtros.Add("Estado", estado);
                if (categoriaId.HasValue)
                {
                    var categoria = await _context.Categorias
                        .Where(c => !c.Eliminado && c.Id == categoriaId.Value)
                        .FirstOrDefaultAsync();
                    filtros.Add("Categoría", categoria?.Nombre ?? "No especificada");
                }
                if (!string.IsNullOrEmpty(sucursalId))
                {
                    var sucursal = await _context.Sucursales
                        .Where(s => !s.Eliminado && s.Id == sucursalId)
                        .FirstOrDefaultAsync();
                    filtros.Add("Sucursal", sucursal?.Nombre ?? "No especificada");
                }

                // Título dinámico según filtros
                var tituloReporte = !string.IsNullOrEmpty(sucursalId) && filtros.ContainsKey("Sucursal")
                    ? $"Inventario General — {filtros["Sucursal"]}"
                    : "Inventario General";

                // Preparar datos para tabla
                var headers = new List<string>
                {
                    "Sucursal", "Zona", "Serial / ID", "Artículo", "Categoría", "Cantidad", "Estado", "Último Movimiento", "Responsable"
                };
                if (includeCostos)
                {
                    headers.Add("Costo");
                }

                var filas = items.Select(i =>
                {
                    var fila = new List<string>
                    {
                        i.Sucursal?.Nombre ?? "Sin Sucursal",
                        i.Sucursal?.Region?.Nombre ?? "N/D",
                        i.Serial ?? "N/A",
                        $"{i.Articulo.Marca} {i.Articulo.Modelo}",
                        i.Articulo.Categoria.Nombre,
                        i.Cantidad.ToString(),
                        i.Estado,
                        ultimoMovimientoPorItem.TryGetValue(i.Id, out var fechaMovimiento) && fechaMovimiento.HasValue
                            ? fechaMovimiento.Value.ToString("dd/MM/yyyy")
                            : "Sin movimientos",
                        i.ResponsableEmpleado ?? "No asignado"
                    };

                    if (includeCostos)
                    {
                        fila.Add((i.Lote?.CostoUnitario ?? 0).ToString("C"));
                    }

                    return fila;
                }).ToList();

                // Preparar totales
                var totales = new Dictionary<string, string>
                {
                    { "Total Unidades", items.Sum(i => i.Cantidad).ToString() }
                };
                if (includeCostos)
                {
                    totales.Add("Costo Total", items.Sum(i => (i.Lote?.CostoUnitario ?? 0) * i.Cantidad).ToString("C"));
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
        public async Task<IActionResult> InventarioPorSucursal(string sucursalId, bool includeCostos = true)
        {
            try
            {
                var usuario = UsuarioActual;

                // Obtener sucursal
                var sucursal = await _context.Sucursales
                    .Where(s => !s.Eliminado && s.Activo && s.Id == sucursalId)
                    .FirstOrDefaultAsync();
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
                    .Where(i => !i.Eliminado && i.SucursalId == sucursalId)
                    .Include(i => i.Articulo)
                    .ThenInclude(a => a.Categoria)
                    .Include(i => i.Lote)
                    .ThenInclude(l => l.Compra)
                    .Include(i => i.Sucursal)
                    .Where(i => i.Articulo != null && !i.Articulo.Eliminado)
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

                var ultimoMovimientoPorItem = await ObtenerUltimoMovimientoPorItemAsync(items.Select(i => i.Id));

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
                                "Zona", "Serial / ID", "Artículo", "Categoría", "Cantidad", "Estado", "Último Movimiento", "Responsable"
                            };
                            if (includeCostos)
                            {
                                headers.Add("Costo");
                            }

                            var filas = items.Select(i =>
                            {
                                var fila = new List<string>
                                {
                                    sucursal.Region?.Nombre ?? "N/D",
                                    i.Serial ?? "N/A",
                                    $"{i.Articulo.Marca} {i.Articulo.Modelo}",
                                    i.Articulo.Categoria.Nombre,
                                    i.Cantidad.ToString(),
                                    i.Estado,
                                    ultimoMovimientoPorItem.TryGetValue(i.Id, out var fechaMovimiento) && fechaMovimiento.HasValue
                                        ? fechaMovimiento.Value.ToString("dd/MM/yyyy")
                                        : "Sin movimientos",
                                    i.ResponsableEmpleado ?? "No asignado"
                                };

                                if (includeCostos)
                                {
                                    fila.Add((i.Lote?.CostoUnitario ?? 0).ToString("C"));
                                }

                                return fila;
                            }).ToList();

                            column.Item().Element(c => 
                                PdfReportService.GenerarTablaSimple(c, headers, filas));

                            column.Item().PaddingTop(15);

                            // Totales
                            var totales = new Dictionary<string, string>
                            {
                                { "Total Unidades en Sucursal", items.Sum(i => i.Cantidad).ToString() }
                            };
                            if (includeCostos)
                            {
                                totales.Add("Costo Total", items.Sum(i => (i.Lote?.CostoUnitario ?? 0) * i.Cantidad).ToString("C"));
                            }

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
        public async Task<IActionResult> MovimientosItems(DateTime? fechaInicio, DateTime? fechaFin, string? sucursalId, string? motivo, string? usuario)
        {
            try
            {
                var usuarioActual = UsuarioActual;

                // Query base con includes
                var query = _context.MovimientosItem
                    .Include(m => m.Item)
                        .ThenInclude(i => i.Articulo)
                    .Include(m => m.SucursalOrigen)
                        .ThenInclude(s => s.Region)
                    .Include(m => m.SucursalDestino)
                        .ThenInclude(s => s.Region)
                    .AsQueryable();

                query = query.Where(m =>
                    m.Observaciones == null ||
                    !m.Observaciones.Contains(MovimientoLogicoService.MarcaEliminadoLogico));

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

                if (!string.IsNullOrWhiteSpace(motivo))
                {
                    query = query.Where(m => m.Motivo == motivo);
                }

                if (!string.IsNullOrWhiteSpace(usuario))
                {
                    var usuarioTerm = usuario.Trim().ToLower();
                    query = query.Where(m => m.UsuarioResponsable != null && m.UsuarioResponsable.ToLower().Contains(usuarioTerm));
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
                    var sucursal = await _context.Sucursales
                        .Where(s => !s.Eliminado && s.Id == sucursalId)
                        .FirstOrDefaultAsync();
                    filtros.Add("Sucursal", sucursal?.Nombre ?? "No especificada");
                }
                if (!string.IsNullOrWhiteSpace(motivo))
                    filtros.Add("Motivo", motivo);
                if (!string.IsNullOrWhiteSpace(usuario))
                    filtros.Add("Usuario", usuario);

                string resolverDestinoMovimiento(MovimientoItem movimiento)
                {
                    var destinoSucursal = movimiento.SucursalDestino?.Nombre ?? "Almacén Central";
                    if (string.IsNullOrWhiteSpace(movimiento.Motivo) ||
                        !movimiento.Motivo.Contains("Departamento", StringComparison.OrdinalIgnoreCase))
                    {
                        return destinoSucursal;
                    }

                    var observacion = movimiento.Observaciones ?? string.Empty;
                    const string marcadorDestino = "Departamento destino:";
                    var indiceDestino = observacion.IndexOf(marcadorDestino, StringComparison.OrdinalIgnoreCase);
                    if (indiceDestino < 0)
                    {
                        return "Departamento";
                    }

                    var textoDestino = observacion[(indiceDestino + marcadorDestino.Length)..].Trim();
                    var finDestino = textoDestino.IndexOf('.');
                    if (finDestino >= 0)
                    {
                        textoDestino = textoDestino[..finDestino].Trim();
                    }

                    return string.IsNullOrWhiteSpace(textoDestino)
                        ? "Departamento"
                        : $"Departamento: {textoDestino}";
                }

                // Preparar datos para tabla
                var headers = new List<string>
                {
                    "Origen", "Zona Origen", "Destino", "Zona Destino", "Fecha", "Fecha Recepción", "Serial", "Artículo", "Cant.", "Motivo", "Responsable Recepción", "Usuario", "Observaciones"
                };

                var filas = movimientos.Select(m => new List<string>
                {
                    m.SucursalOrigen?.Nombre ?? "Almacén Central",
                    m.SucursalOrigen?.Region?.Nombre ?? "N/D",
                    resolverDestinoMovimiento(m),
                    m.SucursalDestino?.Region?.Nombre ?? "N/D",
                    m.FechaMovimiento.ToString("dd/MM/yyyy HH:mm"),
                    m.FechaRecepcion.HasValue ? m.FechaRecepcion.Value.ToString("dd/MM/yyyy HH:mm") : "N/A",
                    m.Item?.Serial ?? $"ID: {m.ItemId}",
                    m.Item?.Articulo != null ? $"{m.Item.Articulo.Marca} {m.Item.Articulo.Modelo}" : "Artículo no disponible",
                    m.Cantidad.ToString(),
                    m.Motivo ?? "N/A",
                    string.IsNullOrWhiteSpace(m.ResponsableRecepcion) ? "N/A" : m.ResponsableRecepcion,
                    m.UsuarioResponsable ?? "N/A",
                    string.IsNullOrWhiteSpace(m.Observaciones) ? "N/A" : m.Observaciones
                }).ToList();

                // Preparar totales
                var totales = new Dictionary<string, string>
                {
                    { "Total Movimientos", movimientos.Count.ToString() },
                    { "Total Unidades Movidas", movimientos.Sum(m => m.Cantidad).ToString() }
                };

                // Crear documento PDF
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.Letter.Landscape());
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                        // Header
                        page.Header().Element(c => 
                            PdfReportService.GenerarHeader(c, "Movimientos de Items", usuarioActual));

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
                        page.Footer().Element(c => PdfReportService.GenerarFooter(c, usuarioActual));
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
        /// Comprobante de salida generado al finalizar un despacho/salida.
        /// </summary>
        public async Task<IActionResult> ComprobanteSalida(string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key) || !key.StartsWith("comprobante-salida-"))
                {
                    var pdfVacio = PdfReportService.GenerarPdfVacio(
                        "Comprobante de Salida",
                        "No se recibió un identificador de comprobante válido"
                    );
                    return File(pdfVacio, "application/pdf", "ComprobanteSalida.pdf");
                }

                var rawComprobante = HttpContext.Session.GetString(key);
                HttpContext.Session.Remove(key);

                if (string.IsNullOrWhiteSpace(rawComprobante))
                {
                    var pdfVacio = PdfReportService.GenerarPdfVacio(
                        "Comprobante de Salida",
                        "No se encontró información del comprobante o ya fue consumida"
                    );
                    return File(pdfVacio, "application/pdf", "ComprobanteSalida.pdf");
                }

                var contexto = JsonSerializer.Deserialize<ComprobanteSalidaContext>(rawComprobante);
                if (contexto == null || contexto.MovimientoIds == null || !contexto.MovimientoIds.Any())
                {
                    var pdfVacio = PdfReportService.GenerarPdfVacio(
                        "Comprobante de Salida",
                        "No hay movimientos disponibles para generar el comprobante"
                    );
                    return File(pdfVacio, "application/pdf", "ComprobanteSalida.pdf");
                }

                var movimientos = await _context.MovimientosItem
                    .Where(m => contexto.MovimientoIds.Contains(m.Id))
                    .Include(m => m.Item)
                        .ThenInclude(i => i.Articulo)
                    .Include(m => m.SucursalOrigen)
                        .ThenInclude(s => s.Region)
                    .Include(m => m.SucursalDestino)
                        .ThenInclude(s => s.Region)
                    .OrderBy(m => m.FechaMovimiento)
                    .ToListAsync();

                if (!movimientos.Any())
                {
                    var pdfVacio = PdfReportService.GenerarPdfVacio(
                        "Comprobante de Salida",
                        "Los movimientos asociados no están disponibles"
                    );
                    return File(pdfVacio, "application/pdf", "ComprobanteSalida.pdf");
                }

                var usuarioMovimiento = movimientos
                    .Select(m => m.UsuarioResponsable)
                    .FirstOrDefault(u => !string.IsNullOrWhiteSpace(u));
                var usuario = !string.IsNullOrWhiteSpace(contexto.UsuarioResponsable)
                    ? contexto.UsuarioResponsable
                    : (!string.IsNullOrWhiteSpace(usuarioMovimiento) ? usuarioMovimiento : UsuarioActual);

                var destinoMovimiento = movimientos
                    .Select(m => m.SucursalDestino?.Nombre)
                    .FirstOrDefault(d => !string.IsNullOrWhiteSpace(d));
                var destino = !string.IsNullOrWhiteSpace(contexto.DestinoNombre)
                    ? contexto.DestinoNombre
                    : (!string.IsNullOrWhiteSpace(destinoMovimiento) ? destinoMovimiento : "No especificado");

                var responsableMovimiento = movimientos
                    .Select(m => m.ResponsableRecepcion)
                    .FirstOrDefault(r => !string.IsNullOrWhiteSpace(r));
                var responsableRecepcion = !string.IsNullOrWhiteSpace(contexto.ResponsableRecepcion)
                    ? contexto.ResponsableRecepcion
                    : (!string.IsNullOrWhiteSpace(responsableMovimiento) ? responsableMovimiento : "No especificado");

                var observacionesMovimiento = string.Join(" | ", movimientos
                    .Select(m => m.Observaciones?.Trim())
                    .Where(o => !string.IsNullOrWhiteSpace(o))
                    .Distinct());
                var observaciones = !string.IsNullOrWhiteSpace(contexto.Observaciones)
                    ? contexto.Observaciones.Trim()
                    : (!string.IsNullOrWhiteSpace(observacionesMovimiento) ? observacionesMovimiento : "N/A");
                var fechaEmision = contexto.FechaGeneracion == default ? DateTime.Now : contexto.FechaGeneracion;

                var filtros = new Dictionary<string, string>
                {
                    { "Tipo de Salida", string.IsNullOrWhiteSpace(contexto.TipoSalida) ? "Salida de inventario" : contexto.TipoSalida },
                    { "Destino", destino },
                    { "Responsable Recepción", responsableRecepcion },
                    { "Fecha Emisión", fechaEmision.ToString("dd/MM/yyyy HH:mm") },
                    { "Observaciones", observaciones }
                };

                var headers = new List<string>
                {
                    "Origen", "Zona Origen", "Destino", "Zona Destino", "Fecha", "Serial / ID", "Artículo", "Cant.", "Motivo", "Usuario"
                };

                var filas = movimientos.Select(m => new List<string>
                {
                    m.SucursalOrigen?.Nombre ?? "Almacén",
                    m.SucursalOrigen?.Region?.Nombre ?? "N/D",
                    contexto.EntregaDepartamento ? destino : (m.SucursalDestino?.Nombre ?? destino),
                    m.SucursalDestino?.Region?.Nombre ?? (contexto.EntregaDepartamento ? "N/D" : "N/D"),
                    m.FechaMovimiento.ToString("dd/MM/yyyy HH:mm"),
                    m.Item?.Serial ?? $"ID: {m.ItemId}",
                    m.Item?.Articulo != null
                        ? $"{m.Item.Articulo.Marca} {m.Item.Articulo.Modelo}"
                        : "Artículo no disponible",
                    m.Cantidad.ToString(),
                    m.Motivo ?? "N/A",
                    m.UsuarioResponsable ?? "N/A"
                }).ToList();

                var headersDetalle = new List<string>
                {
                    "Fecha", "Responsable Recepción", "Observaciones"
                };

                var filasDetalle = movimientos.Select(m => new List<string>
                {
                    m.FechaMovimiento.ToString("dd/MM/yyyy HH:mm"),
                    string.IsNullOrWhiteSpace(m.ResponsableRecepcion) ? "N/A" : m.ResponsableRecepcion,
                    string.IsNullOrWhiteSpace(m.Observaciones) ? "N/A" : m.Observaciones
                }).ToList();

                var totales = new Dictionary<string, string>
                {
                    { "Total Movimientos", movimientos.Count.ToString() },
                    { "Total Unidades", movimientos.Sum(m => m.Cantidad).ToString() }
                };

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.Letter);
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                        page.Header().Element(c =>
                            PdfReportService.GenerarHeader(c, "Comprobante de Salida", usuario));

                        page.Content().Column(column =>
                        {
                            column.Item().Element(c => PdfReportService.GenerarFiltros(c, filtros));
                            column.Item().PaddingTop(15);
                            column.Item().Element(c => PdfReportService.GenerarTablaSimple(c, headers, filas));
                            column.Item().PaddingTop(15);
                            column.Item().Element(c => PdfReportService.GenerarResumenTotales(c, totales));
                            column.Item().PaddingTop(15);
                            column.Item().Text("Detalle de recepción y observaciones").FontSize(11).Bold().FontColor("#047394");
                            column.Item().PaddingTop(6);
                            column.Item().Element(c => PdfReportService.GenerarTablaSimple(c, headersDetalle, filasDetalle));

                            column.Item().PaddingTop(15)
                                .Background(Colors.Grey.Lighten4)
                                .Padding(12)
                                .Column(sign =>
                                {
                                    sign.Item().Text("Conduce de Entrega").FontSize(11).Bold().FontColor("#047394");
                                    sign.Item().PaddingTop(5).Text($"Observaciones: {observaciones}").FontSize(9);
                                    sign.Item().PaddingTop(12).Row(row =>
                                    {
                                        row.RelativeItem().Column(col =>
                                        {
                                            col.Item().Text("Entregado por").FontSize(9).Bold();
                                            col.Item().PaddingTop(18).BorderBottom(1).BorderColor(Colors.Grey.Medium).Text("");
                                            col.Item().PaddingTop(3).Text(usuario).FontSize(8).FontColor(Colors.Grey.Darken2);
                                        });

                                        row.ConstantItem(20);

                                        row.RelativeItem().Column(col =>
                                        {
                                            col.Item().Text("Recibido por").FontSize(9).Bold();
                                            col.Item().PaddingTop(18).BorderBottom(1).BorderColor(Colors.Grey.Medium).Text("");
                                            col.Item().PaddingTop(3).Text(responsableRecepcion).FontSize(8).FontColor(Colors.Grey.Darken2);
                                        });
                                    });
                                });
                        });

                        page.Footer().Element(c => PdfReportService.GenerarFooter(c, usuario));
                    });
                });

                var pdfBytes = document.GeneratePdf();
                return File(pdfBytes, "application/pdf", $"ComprobanteSalida_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al generar el comprobante: {ex.Message}");
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
                    .Where(i => !i.Eliminado && i.Articulo != null && !i.Articulo.Eliminado)
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
                                "Sucursal", "Zona", "Serial", "Artículo", "Estado", "Responsable", "Fecha Inicio", "Vencimiento", "Días Restantes" 
                            };

                            var filas = items.Select(i => 
                            {
                                var diasRestantes = i.FechaGarantiaVencimiento.HasValue 
                                    ? (int)(i.FechaGarantiaVencimiento.Value - DateTime.Now).TotalDays
                                    : 0;

                                return new List<string>
                                {
                                    i.Sucursal?.Nombre ?? "Sin Sucursal",
                                    i.Sucursal?.Region?.Nombre ?? "N/D",
                                    i.Serial ?? "N/A",
                                    $"{i.Articulo.Marca} {i.Articulo.Modelo}",
                                    i.Estado,
                                    i.ResponsableEmpleado ?? "No asignado",
                                    i.FechaGarantiaInicio.HasValue ? i.FechaGarantiaInicio.Value.ToString("dd/MM/yyyy") : "N/A",
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
        public async Task<IActionResult> ReporteCompras(DateTime? fechaInicio, DateTime? fechaFin, bool includeCostos = true)
        {
            try
            {
                var usuario = UsuarioActual;

                // Query base
                var query = _context.Compras
                    .Where(c => !c.Eliminado)
                    .Include(c => c.Lotes)
                    .ThenInclude(l => l.Articulo)
                    .ThenInclude(a => a.Categoria)
                    .Include(c => c.Lotes)
                    .ThenInclude(l => l.Items)
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
                        page.Size(PageSizes.Letter.Landscape());
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

                            // Resumen por compra
                            column.Item().Text("Resumen por Compra")
                                .FontSize(11).Bold().FontColor("#047394");
                            column.Item().PaddingTop(6);

                            var headersResumen = new List<string>
                            {
                                "Fecha", "Proveedor", "Nº Factura", "Lotes", "Uds Compra", "Uds Registradas", "Artículos", "Estado", "Solicitante"
                            };
                            if (includeCostos)
                            {
                                headersResumen.Insert(7, "Costo Total");
                            }

                            var filasResumen = compras.Select(compra => {
                                var lotes = compra.Lotes?.ToList() ?? new List<Lote>();
                                var lotesCount = lotes.Count;
                                var unidadesCompra = lotes.Sum(l => l.Cantidad);
                                var unidadesRegistradas = lotes.Sum(l => l.Items?.Where(i => !i.Eliminado).Sum(i => i.Cantidad) ?? 0);
                                var articulosDistintos = lotes.Select(l => l.ArticuloId).Distinct().Count();

                                var fila = new List<string>
                                {
                                    compra.FechaCompra.ToString("dd/MM/yyyy"),
                                    compra.Proveedor,
                                    string.IsNullOrWhiteSpace(compra.NumeroFactura) ? "N/A" : compra.NumeroFactura,
                                    lotesCount.ToString(),
                                    unidadesCompra.ToString(),
                                    unidadesRegistradas.ToString(),
                                    articulosDistintos.ToString(),
                                    compra.Estado,
                                    string.IsNullOrWhiteSpace(compra.UsuarioSolicitante) ? "N/A" : compra.UsuarioSolicitante
                                };

                                if (includeCostos)
                                {
                                    fila.Insert(7, compra.CostoTotal.ToString("C"));
                                }

                                return fila;
                            }).ToList();

                            column.Item().Element(c =>
                                PdfReportService.GenerarTablaSimple(c, headersResumen, filasResumen));

                            column.Item().PaddingTop(15);

                            // Detalle por lote
                            var detalleLotes = compras
                                .SelectMany(compra => (compra.Lotes ?? new List<Lote>())
                                    .Select(lote => new
                                    {
                                        Compra = compra,
                                        Lote = lote
                                    }))
                                .OrderByDescending(x => x.Compra.FechaCompra)
                                .ThenByDescending(x => x.Lote.Id)
                                .ToList();

                            if (detalleLotes.Any())
                            {
                                column.Item().Text("Detalle por Lote")
                                    .FontSize(11).Bold().FontColor("#047394");
                                column.Item().PaddingTop(6);

                                var headersDetalle = new List<string>
                                {
                                    "Fecha", "Proveedor", "Nº Factura", "Artículo", "Categoría", "Cant."
                                };
                                if (includeCostos)
                                {
                                    headersDetalle.Add("Costo U.");
                                    headersDetalle.Add("Subtotal");
                                }

                                var filasDetalle = detalleLotes.Select(x =>
                                {
                                    var fila = new List<string>
                                    {
                                        x.Compra.FechaCompra.ToString("dd/MM/yyyy"),
                                        x.Compra.Proveedor,
                                        string.IsNullOrWhiteSpace(x.Compra.NumeroFactura) ? "N/A" : x.Compra.NumeroFactura,
                                        x.Lote.Articulo != null ? $"{x.Lote.Articulo.Marca} {x.Lote.Articulo.Modelo}" : $"Artículo #{x.Lote.ArticuloId}",
                                        x.Lote.Articulo?.Categoria?.Nombre ?? "N/A",
                                        x.Lote.Cantidad.ToString()
                                    };

                                    if (includeCostos)
                                    {
                                        fila.Add(x.Lote.CostoUnitario.ToString("C"));
                                        fila.Add((x.Lote.Cantidad * x.Lote.CostoUnitario).ToString("C"));
                                    }

                                    return fila;
                                }).ToList();

                                column.Item().Element(c =>
                                    PdfReportService.GenerarTablaSimple(c, headersDetalle, filasDetalle));

                                column.Item().PaddingTop(15);
                            }

                            var comprasConObservacion = compras
                                .Where(c => !string.IsNullOrWhiteSpace(c.Observaciones))
                                .ToList();

                            if (comprasConObservacion.Any())
                            {
                                column.Item().Text("Observaciones de Compra")
                                    .FontSize(11).Bold().FontColor("#047394");
                                column.Item().PaddingTop(6);

                                var headersObservaciones = new List<string>
                                {
                                    "Fecha", "Proveedor", "Nº Factura", "Solicitante", "Observaciones"
                                };

                                var filasObservaciones = comprasConObservacion.Select(c => new List<string>
                                {
                                    c.FechaCompra.ToString("dd/MM/yyyy"),
                                    c.Proveedor,
                                    string.IsNullOrWhiteSpace(c.NumeroFactura) ? "N/A" : c.NumeroFactura,
                                    string.IsNullOrWhiteSpace(c.UsuarioSolicitante) ? "N/A" : c.UsuarioSolicitante,
                                    c.Observaciones ?? "N/A"
                                }).ToList();

                                column.Item().Element(c =>
                                    PdfReportService.GenerarTablaSimple(c, headersObservaciones, filasObservaciones));

                                column.Item().PaddingTop(15);
                            }

                            // Totales
                            var totalLotes = compras.Sum(c => c.Lotes?.Count ?? 0);
                            var totalUnidadesCompra = compras.Sum(c => c.Lotes?.Sum(l => l.Cantidad) ?? 0);
                            var totalUnidadesRegistradas = compras.Sum(c => c.Lotes?.Sum(l => l.Items?.Where(i => !i.Eliminado).Sum(i => i.Cantidad) ?? 0) ?? 0);
                            var comprasSinLotes = compras.Count(c => c.Lotes == null || !c.Lotes.Any());
                            var proveedoresDistintos = compras
                                .Select(c => c.Proveedor?.Trim().ToLower())
                                .Where(p => !string.IsNullOrWhiteSpace(p))
                                .Distinct()
                                .Count();
                            var ticketPromedio = compras.Any() ? compras.Average(c => c.CostoTotal) : 0m;

                            var totales = new Dictionary<string, string>
                            {
                                { "Total Compras", compras.Count.ToString() },
                                { "Total Lotes", totalLotes.ToString() },
                                { "Unidades Compra (lotes)", totalUnidadesCompra.ToString() },
                                { "Unidades Registradas (items)", totalUnidadesRegistradas.ToString() },
                                { "Compras Sin Lotes", comprasSinLotes.ToString() },
                                { "Proveedores Distintos", proveedoresDistintos.ToString() }
                            };
                            if (includeCostos)
                            {
                                totales.Add("Monto Total", compras.Sum(c => c.CostoTotal).ToString("C"));
                                totales.Add("Ticket Promedio", ticketPromedio.ToString("C"));
                            }

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
        public async Task<IActionResult> EstadisticasGenerales(bool includeCostos = true)
        {
            try
            {
                var usuario = UsuarioActual;

                // Obtener datos agregados
                var items = await _context.Items
                    .Where(i => !i.Eliminado && i.Articulo != null && !i.Articulo.Eliminado)
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

                            var itemsPorEstado = items.GroupBy(i => i.Estado)
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

                            var itemsPorCategoria = items.GroupBy(i => i.Articulo.Categoria.Nombre)
                                .OrderByDescending(g => g.Sum(i => i.Cantidad))
                                .ToList();

                            var headerCategoria = new List<string> { "Categoría", "Unidades" };
                            if (includeCostos)
                            {
                                headerCategoria.Add("Costo Total");
                            }

                            var filasCategoria = itemsPorCategoria.Select(g =>
                            {
                                var fila = new List<string>
                                {
                                    g.Key,
                                    g.Sum(i => i.Cantidad).ToString()
                                };

                                if (includeCostos)
                                {
                                    fila.Add(g.Sum(i => (i.Lote?.CostoUnitario ?? 0) * i.Cantidad).ToString("C"));
                                }

                                return fila;
                            }).ToList();

                            column.Item().Element(c => 
                                PdfReportService.GenerarTablaSimple(c, headerCategoria, filasCategoria));

                            column.Item().PaddingTop(15);

                            // SECCIÓN 3: Items por Sucursal
                            column.Item().Text("Items por Sucursal").FontSize(12).Bold().FontColor("#047394");
                            column.Item().PaddingTop(5).PaddingBottom(10);

                            var itemsPorSucursal = items.GroupBy(i => i.Sucursal?.Nombre ?? "Sin Sucursal")
                                .OrderByDescending(g => g.Sum(i => i.Cantidad))
                                .ToList();

                            var headerSucursal = new List<string> { "Sucursal", "Unidades" };
                            if (includeCostos)
                            {
                                headerSucursal.Add("Costo Total");
                            }

                            var filasSucursal = itemsPorSucursal.Select(g =>
                            {
                                var fila = new List<string>
                                {
                                    g.Key,
                                    g.Sum(i => i.Cantidad).ToString()
                                };

                                if (includeCostos)
                                {
                                    fila.Add(g.Sum(i => (i.Lote?.CostoUnitario ?? 0) * i.Cantidad).ToString("C"));
                                }

                                return fila;
                            }).ToList();

                            column.Item().Element(c => 
                                PdfReportService.GenerarTablaSimple(c, headerSucursal, filasSucursal));

                            column.Item().PaddingTop(15);

                            // Totales generales
                            var itemsActivos = items.ToList();
                            var totales = new Dictionary<string, string>
                            {
                                { "Total Unidades", itemsActivos.Sum(i => i.Cantidad).ToString() },
                                { "Unidades Disponibles", itemsActivos.Where(i => i.Estado == "Disponible").Sum(i => i.Cantidad).ToString() }
                            };
                            if (includeCostos)
                            {
                                totales.Add("Valor Total Inventario", itemsActivos.Sum(i => (i.Lote?.CostoUnitario ?? 0) * i.Cantidad).ToString("C"));
                            }

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
