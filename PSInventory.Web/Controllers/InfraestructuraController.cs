using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using PSData.Datos;
using PSData.Modelos;
using PSInventory.Web.Filters;
using PSInventory.Web.Models.ViewModels;
using PSInventory.Web.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PSInventory.Web.Controllers
{
    [AuthorizeRole("Administrador", "Jefe")]
    public class InfraestructuraController : Controller
    {
        private readonly PSDatos _context;
        private readonly CohereAiService _cohereAiService;

        public InfraestructuraController(PSDatos context, CohereAiService cohereAiService)
        {
            _context = context;
            _cohereAiService = cohereAiService;
        }

        // GET: Infraestructura
        public async Task<IActionResult> Index(string q = "", int? regionId = null, string? sucursalId = null, int? departamentoId = null, string layout = "vertical")
        {
            var equiposQuery = _context.InfraEquiposComputo
                .Where(e => !e.Eliminado)
                .Include(e => e.Sucursal)
                    .ThenInclude(s => s.Region)
                .Include(e => e.SistemaOperativo)
                .Include(e => e.TipoProcesador)
                .Include(e => e.TipoRam)
                .Include(e => e.EquiposDepartamentos)
                    .ThenInclude(ed => ed.Departamento)
                .AsQueryable();

            if (regionId.HasValue && regionId.Value > 0)
            {
                equiposQuery = equiposQuery.Where(e => e.Sucursal != null && e.Sucursal.RegionId == regionId.Value);
            }

            if (!string.IsNullOrWhiteSpace(sucursalId))
            {
                equiposQuery = equiposQuery.Where(e => e.SucursalId == sucursalId);
            }

            if (departamentoId.HasValue && departamentoId.Value > 0)
            {
                equiposQuery = equiposQuery.Where(e => e.EquiposDepartamentos.Any(ed => ed.DepartamentoId == departamentoId.Value));
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                equiposQuery = equiposQuery.Where(e =>
                    e.NombreEquipo.ToLower().Contains(term) ||
                    e.Serial.ToLower().Contains(term) ||
                    (e.CodigoActivo != null && e.CodigoActivo.ToLower().Contains(term)) ||
                    (e.Marca != null && e.Marca.ToLower().Contains(term)) ||
                    (e.Modelo != null && e.Modelo.ToLower().Contains(term)) ||
                    (e.Sucursal != null && e.Sucursal.Region != null && e.Sucursal.Region.Nombre.ToLower().Contains(term)) ||
                    (e.Sucursal != null && e.Sucursal.Nombre.ToLower().Contains(term)) ||
                    e.EquiposDepartamentos.Any(ed => ed.Departamento != null && ed.Departamento.Nombre.ToLower().Contains(term)));
            }

            var equipos = await equiposQuery
                .OrderBy(e => e.Sucursal!.Nombre)
                .ThenBy(e => e.NombreEquipo)
                .ToListAsync();

            var servicios = await _context.InfraServiciosSucursal
                .Where(s => !s.Eliminado
                            && (string.IsNullOrWhiteSpace(sucursalId) || s.SucursalId == sucursalId)
                            && (!regionId.HasValue || regionId.Value <= 0 || (s.Sucursal != null && s.Sucursal.RegionId == regionId.Value)))
                .Include(s => s.Sucursal)
                    .ThenInclude(su => su.Region)
                .Include(s => s.TipoServicio)
                .Include(s => s.OperadorServicio)
                .OrderBy(s => s.Sucursal!.Nombre)
                .ThenBy(s => s.TipoServicio!.Nombre)
                .Take(200)
                .ToListAsync();

            var accesorios = await _context.InfraSucursalesAccesorio
                .Where(a => !a.Eliminado
                            && (string.IsNullOrWhiteSpace(sucursalId) || a.SucursalId == sucursalId)
                            && (!regionId.HasValue || regionId.Value <= 0 || (a.Sucursal != null && a.Sucursal.RegionId == regionId.Value)))
                .Include(a => a.Sucursal)
                    .ThenInclude(su => su.Region)
                .Include(a => a.TipoAccesorio)
                .OrderBy(a => a.Sucursal!.Nombre)
                .ThenBy(a => a.TipoAccesorio!.Nombre)
                .Take(200)
                .ToListAsync();

            var vm = new InfraestructuraIndexViewModel
            {
                Query = q,
                RegionFiltro = regionId,
                SucursalFiltro = sucursalId,
                DepartamentoFiltro = departamentoId,
                TotalEquipos = equipos.Count,
                EquiposActivos = equipos.Count(e => e.Activo),
                TotalServicios = servicios.Count,
                TotalAccesorios = accesorios.Count,
                Regiones = await ObtenerRegionesSelect(),
                Sucursales = await ObtenerSucursalesSelect(regionId),
                Departamentos = await ObtenerDepartamentosSelect(),
                Equipos = equipos.Select(MapEquipoListItem).ToList(),
                Servicios = servicios.Select(MapServicioListItem).ToList(),
                Accesorios = accesorios.Select(MapAccesorioListItem).ToList(),
                ViewLayout = layout
            };

            return View(vm);
        }

        // GET: Infraestructura/Normalizacion
        public async Task<IActionResult> Normalizacion()
        {
            var equipos = await _context.InfraEquiposComputo
                .Where(e => !e.Eliminado)
                .Include(e => e.TipoRam)
                .AsNoTracking()
                .ToListAsync();

            var vm = new InfraNormalizacionViewModel
            {
                GruposAlmacenamiento = ConstruirGruposNormalizacion(equipos.Select(e => e.Almacenamiento), "almacenamiento"),
                GruposCpu = ConstruirGruposNormalizacion(equipos.Select(e => e.CpuDetalle), "cpu"),
                GruposTipoRam = ConstruirGruposNormalizacion(equipos.Select(e => e.TipoRam?.Nombre), "tipo-ram"),
                GruposMarca = ConstruirGruposNormalizacion(equipos.Select(e => e.Marca), "marca")
            };

            return View(vm);
        }

        // POST: Infraestructura/AplicarNormalizacion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AplicarNormalizacion(string campo, string clave, string nuevoValor)
        {
            if (string.IsNullOrWhiteSpace(campo) || string.IsNullOrWhiteSpace(clave))
            {
                TempData["Error"] = "La solicitud de normalización es inválida.";
                return RedirectToAction(nameof(Normalizacion));
            }

            var valorNormalizado = NormalizarTexto(nuevoValor);
            if (string.IsNullOrWhiteSpace(valorNormalizado) && campo != "cpu") // CPU puede quedar vacío
            {
                TempData["Error"] = "Debe indicar el nuevo valor normalizado.";
                return RedirectToAction(nameof(Normalizacion));
            }

            var campoKey = campo.Trim().ToLowerInvariant();
            
            var equipos = await _context.InfraEquiposComputo
                .Where(e => !e.Eliminado)
                .Include(e => e.TipoRam)
                .ToListAsync();

            var afectados = equipos
                .Where(e => NormalizarClave(ObtenerValorCampoSync(e, campoKey)) == clave)
                .ToList();

            if (!afectados.Any())
            {
                TempData["Error"] = "No se encontraron registros para normalizar.";
                return RedirectToAction(nameof(Normalizacion));
            }

            // Para tipo-ram, resolvemos el ID una sola vez
            int? nuevoTipoRamId = null;
            if (campoKey == "tipo-ram" && !string.IsNullOrWhiteSpace(valorNormalizado))
            {
                var tipoRam = await _context.InfraTiposRam
                    .FirstOrDefaultAsync(tr => !tr.Eliminado && tr.Nombre.ToLower() == valorNormalizado.Trim().ToLower());
                
                if (tipoRam == null)
                {
                    tipoRam = new InfraTipoRam { Nombre = valorNormalizado.Trim(), Activo = true };
                    _context.InfraTiposRam.Add(tipoRam);
                    await _context.SaveChangesAsync();
                }
                nuevoTipoRamId = tipoRam.Id;
            }

            foreach (var equipo in afectados)
            {
                if (campoKey == "tipo-ram")
                {
                    equipo.TipoRamId = nuevoTipoRamId;
                }
                else
                {
                    AsignarValorCampoSync(equipo, campoKey, valorNormalizado);
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Se normalizaron {afectados.Count} equipos en {NombreCampo(campoKey)}.";
            return RedirectToAction(nameof(Normalizacion));
        }

        // GET: Infraestructura/Reportes
        public IActionResult Reportes()
        {
            return View();
        }

        // API para Reportes - Resumen Infraestructura
        [HttpGet]
        public IActionResult GetInfraestructuraResumen()
        {
            var equipos = _context.InfraEquiposComputo.Count(e => !e.Eliminado);
            var servicios = _context.InfraServiciosSucursal.Count(s => !s.Eliminado);
            var accesorios = _context.InfraSucursalesAccesorio.Count(a => !a.Eliminado);

            return Json(new
            {
                labels = new[] { "Equipos", "Servicios", "Accesorios" },
                datasets = new[]
                {
                    new
                    {
                        data = new[] { equipos, servicios, accesorios },
                        backgroundColor = new[] { "#047394", "#ff5c00", "#10b981" },
                        borderWidth = 0
                    }
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetEquiposPorZona()
        {
            var data = await _context.InfraEquiposComputo
                .Where(e => !e.Eliminado)
                .AsNoTracking()
                .GroupBy(e => e.Sucursal != null && e.Sucursal.Region != null ? e.Sucursal.Region.Nombre : "Sin zona")
                .Select(g => new { Nombre = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            return Json(ConstruirBarChartData("Equipos", data.Select(x => (x.Nombre, x.Cantidad)), 8));
        }

        [HttpGet]
        public async Task<IActionResult> GetEquiposPorSistemaOperativo()
        {
            var data = await _context.InfraEquiposComputo
                .Where(e => !e.Eliminado)
                .AsNoTracking()
                .GroupBy(e => e.SistemaOperativo != null ? e.SistemaOperativo.Nombre : "Sin sistema operativo")
                .Select(g => new { Nombre = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            return Json(ConstruirBarChartData("Equipos", data.Select(x => (x.Nombre, x.Cantidad)), 8));
        }

        [HttpGet]
        public async Task<IActionResult> GetEquiposPorRam()
        {
            var data = await _context.InfraEquiposComputo
                .Where(e => !e.Eliminado)
                .Include(e => e.TipoRam)
                .AsNoTracking()
                .ToListAsync();

            var agrupados = data
                .GroupBy(e => {
                    var cant = e.RamCantidadGb.HasValue ? $"{e.RamCantidadGb} GB" : "Sin RAM";
                    var tipo = e.TipoRam?.Nombre ?? "";
                    return $"{cant} {tipo}".Trim();
                })
                .Select(g => new { Nombre = g.Key, Cantidad = g.Count() })
                .OrderByDescending(x => x.Cantidad)
                .ToList();

            return Json(ConstruirBarChartData("Equipos", agrupados.Select(x => (x.Nombre, x.Cantidad)), 10));
        }

        [HttpGet]
        public async Task<IActionResult> GetEquiposPorProcesador()
        {
            var data = await _context.InfraEquiposComputo
                .Where(e => !e.Eliminado)
                .AsNoTracking()
                .GroupBy(e => e.TipoProcesador != null ? e.TipoProcesador.Nombre : "Sin procesador")
                .Select(g => new { Nombre = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            return Json(ConstruirBarChartData("Equipos", data.Select(x => (x.Nombre, x.Cantidad)), 10));
        }

        [HttpGet]
        public async Task<IActionResult> GetEquiposPorTipoDisco()
        {
            var equipos = await _context.InfraEquiposComputo
                .Where(e => !e.Eliminado && e.Almacenamiento != null)
                .Select(e => e.Almacenamiento)
                .ToListAsync();

            var data = equipos
                .Select(a => {
                    var text = a!.ToUpperInvariant();
                    if (text.Contains("SSD") || text.Contains("NVME") || text.Contains("M.2")) return "SSD / NVMe";
                    if (text.Contains("HDD") || text.Contains("MECANICO")) return "HDD (Mecánico)";
                    return "Otros / No especificado";
                })
                .GroupBy(t => t)
                .Select(g => new { Nombre = g.Key, Cantidad = g.Count() })
                .ToList();

            return Json(ConstruirBarChartData("Equipos", data.Select(x => (x.Nombre, x.Cantidad)), 5));
        }

        [HttpGet]
        public async Task<IActionResult> GetEquiposPorCapacidadDisco()
        {
            var equipos = await _context.InfraEquiposComputo
                .Where(e => !e.Eliminado && e.Almacenamiento != null)
                .Select(e => e.Almacenamiento)
                .ToListAsync();

            var data = equipos
                .Select(a => {
                    var match = Regex.Match(a!, @"(\d+)\s*(GB|TB)", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        var valor = match.Groups[1].Value;
                        var unidad = match.Groups[2].Value.ToUpperInvariant();
                        return unidad == "TB" ? $"{valor} TB" : $"{valor} GB";
                    }
                    return "No especificado";
                })
                .GroupBy(t => t)
                .Select(g => new { Nombre = g.Key, Cantidad = g.Count() })
                .OrderBy(g => {
                    var m = Regex.Match(g.Nombre, @"(\d+)");
                    if (!m.Success) return 0;
                    var val = int.Parse(m.Groups[1].Value);
                    return g.Nombre.Contains("TB") ? val * 1024 : val;
                })
                .ToList();

            return Json(ConstruirBarChartData("Equipos", data.Select(x => (x.Nombre, x.Cantidad)), 10));
        }

        [HttpGet]
        public async Task<IActionResult> GetEquiposPorAlmacenamiento()
        {
            var data = await _context.InfraEquiposComputo
                .Where(e => !e.Eliminado)
                .AsNoTracking()
                .GroupBy(e => e.Almacenamiento ?? "Sin disco")
                .Select(g => new { Nombre = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            return Json(ConstruirBarChartData("Equipos", data.Select(x => (x.Nombre, x.Cantidad)), 10));
        }

        [HttpGet]
        public async Task<IActionResult> GetServiciosPorTipo()
        {
            var data = await _context.InfraServiciosSucursal
                .Where(s => !s.Eliminado)
                .AsNoTracking()
                .GroupBy(s => s.TipoServicio != null ? s.TipoServicio.Nombre : "Sin tipo")
                .Select(g => new { Nombre = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            return Json(ConstruirBarChartData("Servicios", data.Select(x => (x.Nombre, x.Cantidad)), 8));
        }

        [HttpGet]
        public async Task<IActionResult> GetAccesoriosPorTipo()
        {
            var data = await _context.InfraSucursalesAccesorio
                .Where(a => !a.Eliminado)
                .AsNoTracking()
                .GroupBy(a => a.TipoAccesorio != null ? a.TipoAccesorio.Nombre : "Sin tipo")
                .Select(g => new { Nombre = g.Key, Cantidad = g.Count() })
                .ToListAsync();

            return Json(ConstruirBarChartData("Accesorios", data.Select(x => (x.Nombre, x.Cantidad)), 8));
        }

        // POST: Infraestructura/ExportarReporteGrafico
        [HttpPost]
        public async Task<IActionResult> ExportarReporteGrafico([FromBody] InfraReporteGraficoExportViewModel data)
        {
            if (data == null || data.Graficos == null || !data.Graficos.Any())
            {
                return BadRequest("No se recibieron gráficos para exportar.");
            }

            var usuario = HttpContext.Session.GetString("UserName") ?? "Sistema";
            
            // Construimos un resumen textual basado en los datos reales de los gráficos enviados
            var resumenTecnico = GenerarResumenDesdeGraficos(data.Graficos);
            data.AnalisisAi = await _cohereAiService.GenerarAnalisisInfraestructura(resumenTecnico);

            var pdfBytes = PdfReportService.GenerarPdfInfraestructuraGrafica(usuario, data);
            
            return Json(new { 
                success = true, 
                fileName = $"reporte_grafico_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
                fileBase64 = Convert.ToBase64String(pdfBytes) 
            });
        }

        private string GenerarResumenDesdeGraficos(List<InfraChartExportItem> graficos)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("RESUMEN TÉCNICO DE INFRAESTRUCTURA PARA ANÁLISIS:");
            
            foreach (var g in graficos)
            {
                sb.AppendLine($"- {g.Titulo}: {g.RawData}");
            }

            return sb.ToString();
        }

        // Eliminar GenerarPromptInfraestructura si ya no se usa o dejarlo como backup
        // ... (resto de métodos) ...

        // GET: Infraestructura/ExportarEquiposExcel
        public async Task<IActionResult> ExportarEquiposExcel(string q = "", int? regionId = null, string? sucursalId = null, int? departamentoId = null)
        {
            var query = _context.InfraEquiposComputo
                .Where(e => !e.Eliminado)
                .Include(e => e.Sucursal).ThenInclude(s => s.Region)
                .Include(e => e.SistemaOperativo)
                .Include(e => e.TipoProcesador)
                .Include(e => e.TipoRam)
                .Include(e => e.EquiposDepartamentos).ThenInclude(ed => ed.Departamento)
                .AsQueryable();

            if (regionId.HasValue && regionId.Value > 0)
                query = query.Where(e => e.Sucursal != null && e.Sucursal.RegionId == regionId.Value);

            if (!string.IsNullOrWhiteSpace(sucursalId))
                query = query.Where(e => e.SucursalId == sucursalId);

            if (departamentoId.HasValue && departamentoId.Value > 0)
                query = query.Where(e => e.EquiposDepartamentos.Any(ed => ed.DepartamentoId == departamentoId.Value));

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(e =>
                    e.NombreEquipo.ToLower().Contains(term) ||
                    e.Serial.ToLower().Contains(term) ||
                    (e.CodigoActivo != null && e.CodigoActivo.ToLower().Contains(term)));
            }

            var items = await query.OrderBy(e => e.Sucursal!.Nombre).ToListAsync();
            var headers = new[] { "Sucursal", "Zona", "Equipo", "Serial", "Código Activo", "Marca", "Modelo", "S.O.", "Procesador", "Detalle CPU", "RAM (GB)", "Tipo RAM", "Almacenamiento", "Departamentos", "Estado" };
            
            var rows = items.Select(e => new string?[] {
                e.Sucursal?.Nombre,
                e.Sucursal?.Region?.Nombre,
                e.NombreEquipo,
                e.Serial,
                e.CodigoActivo,
                e.Marca,
                e.Modelo,
                e.SistemaOperativo?.Nombre,
                e.TipoProcesador?.Nombre,
                e.CpuDetalle,
                e.RamCantidadGb?.ToString(),
                e.TipoRam?.Nombre,
                e.Almacenamiento,
                string.Join(", ", e.EquiposDepartamentos.Select(ed => ed.Departamento?.Nombre ?? "")),
                e.Activo ? "Activo" : "Inactivo"
            });

            var bytes = ExcelExportService.BuildExcel("Equipos", headers, rows);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"equipos_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        // GET: Infraestructura/ExportarServiciosExcel
        public async Task<IActionResult> ExportarServiciosExcel(string q = "", int? regionId = null, string? sucursalId = null)
        {
            var query = _context.InfraServiciosSucursal
                .Where(s => !s.Eliminado)
                .Include(s => s.Sucursal).ThenInclude(su => su.Region)
                .Include(s => s.TipoServicio)
                .Include(s => s.OperadorServicio)
                .AsQueryable();

            if (regionId.HasValue && regionId.Value > 0)
                query = query.Where(s => s.Sucursal != null && s.Sucursal.RegionId == regionId.Value);

            if (!string.IsNullOrWhiteSpace(sucursalId))
                query = query.Where(s => s.SucursalId == sucursalId);

            var items = await query.OrderBy(s => s.Sucursal!.Nombre).ToListAsync();
            var headers = new[] { "Sucursal", "Zona", "Tipo de Servicio", "Operador", "Número de Servicio", "Baja (Mbps)", "Subida (Mbps)", "Estado" };
            
            var rows = items.Select(s => new string?[] {
                s.Sucursal?.Nombre,
                s.Sucursal?.Region?.Nombre,
                s.TipoServicio?.Nombre,
                s.OperadorServicio?.Nombre,
                s.NumeroServicio,
                s.VelocidadBajadaMbps?.ToString(),
                s.VelocidadSubidaMbps?.ToString(),
                s.Activo ? "Activo" : "Inactivo"
            });

            var bytes = ExcelExportService.BuildExcel("Servicios", headers, rows);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"servicios_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        // GET: Infraestructura/ExportarAccesoriosExcel
        public async Task<IActionResult> ExportarAccesoriosExcel(string q = "", int? regionId = null, string? sucursalId = null)
        {
            var query = _context.InfraSucursalesAccesorio
                .Where(a => !a.Eliminado)
                .Include(a => a.Sucursal).ThenInclude(su => su.Region)
                .Include(a => a.TipoAccesorio)
                .AsQueryable();

            if (regionId.HasValue && regionId.Value > 0)
                query = query.Where(a => a.Sucursal != null && a.Sucursal.RegionId == regionId.Value);

            if (!string.IsNullOrWhiteSpace(sucursalId))
                query = query.Where(a => a.SucursalId == sucursalId);

            var items = await query.OrderBy(a => a.Sucursal!.Nombre).ToListAsync();
            var headers = new[] { "Sucursal", "Zona", "Tipo de Accesorio", "Cantidad", "Especificaciones", "Estado" };
            
            var rows = items.Select(a => new string?[] {
                a.Sucursal?.Nombre,
                a.Sucursal?.Region?.Nombre,
                a.TipoAccesorio?.Nombre,
                a.Cantidad.ToString(),
                a.Especificaciones,
                a.Activo ? "Activo" : "Inactivo"
            });

            var bytes = ExcelExportService.BuildExcel("Accesorios", headers, rows);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"accesorios_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        // GET: Infraestructura/ExportarEquiposCsv
        public async Task<IActionResult> ExportarEquiposCsv(string q = "", int? regionId = null, string? sucursalId = null, int? departamentoId = null)
        {
            var query = _context.InfraEquiposComputo
                .Where(e => !e.Eliminado)
                .Include(e => e.Sucursal).ThenInclude(s => s.Region)
                .Include(e => e.SistemaOperativo)
                .Include(e => e.TipoProcesador)
                .Include(e => e.TipoRam)
                .Include(e => e.EquiposDepartamentos).ThenInclude(ed => ed.Departamento)
                .AsQueryable();

            if (regionId.HasValue && regionId.Value > 0)
                query = query.Where(e => e.Sucursal != null && e.Sucursal.RegionId == regionId.Value);

            if (!string.IsNullOrWhiteSpace(sucursalId))
                query = query.Where(e => e.SucursalId == sucursalId);

            if (departamentoId.HasValue && departamentoId.Value > 0)
                query = query.Where(e => e.EquiposDepartamentos.Any(ed => ed.DepartamentoId == departamentoId.Value));

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(e =>
                    e.NombreEquipo.ToLower().Contains(term) ||
                    e.Serial.ToLower().Contains(term) ||
                    (e.CodigoActivo != null && e.CodigoActivo.ToLower().Contains(term)));
            }

            var items = await query.OrderBy(e => e.Sucursal!.Nombre).ToListAsync();
            var headers = new[] { "Sucursal", "Zona", "Equipo", "Serial", "Código Activo", "Marca", "Modelo", "S.O.", "Procesador", "RAM", "Almacenamiento", "Departamentos", "Estado" };
            var rows = items.Select(e => new[] {
                e.Sucursal?.Nombre ?? "N/D",
                e.Sucursal?.Region?.Nombre ?? "N/D",
                e.NombreEquipo,
                e.Serial,
                e.CodigoActivo ?? "",
                e.Marca ?? "",
                e.Modelo ?? "",
                e.SistemaOperativo?.Nombre ?? "",
                e.TipoProcesador?.Nombre ?? "",
                e.RamCantidadGb.HasValue ? $"{e.RamCantidadGb} GB" : "",
                e.Almacenamiento ?? "",
                string.Join("|", e.EquiposDepartamentos.Select(ed => ed.Departamento?.Nombre ?? "")),
                e.Activo ? "Activo" : "Inactivo"
            });

            var bytes = CsvExportService.BuildCsv(headers, rows);
            return File(bytes, "text/csv", $"equipos_{DateTime.Now:yyyyMMdd}.csv");
        }

        // GET: Infraestructura/ExportarEquiposPdf
        public async Task<IActionResult> ExportarEquiposPdf(string q = "", int? regionId = null, string? sucursalId = null, int? departamentoId = null, string layout = "vertical")
        {
            var usuario = HttpContext.Session.GetString("UserName") ?? "Sistema";
            var query = _context.InfraEquiposComputo
                .Where(e => !e.Eliminado)
                .Include(e => e.Sucursal).ThenInclude(s => s.Region)
                .Include(e => e.SistemaOperativo)
                .Include(e => e.TipoProcesador)
                .Include(e => e.TipoRam)
                .Include(e => e.EquiposDepartamentos).ThenInclude(ed => ed.Departamento)
                .AsQueryable();

            if (regionId.HasValue && regionId.Value > 0)
                query = query.Where(e => e.Sucursal != null && e.Sucursal.RegionId == regionId.Value);

            if (!string.IsNullOrWhiteSpace(sucursalId))
                query = query.Where(e => e.SucursalId == sucursalId);

            if (departamentoId.HasValue && departamentoId.Value > 0)
                query = query.Where(e => e.EquiposDepartamentos.Any(ed => ed.DepartamentoId == departamentoId.Value));

            var items = await query.OrderBy(e => e.Sucursal!.Nombre).ToListAsync();
            if (!items.Any()) return File(PdfReportService.GenerarPdfVacio("Reporte de Equipos", "No hay equipos"), "application/pdf", "equipos.pdf");

            var filtros = new Dictionary<string, string>();
            if (regionId.HasValue && regionId.Value > 0) filtros.Add("Zona", (await _context.Regiones.FindAsync(regionId))?.Nombre ?? "N/D");
            if (!string.IsNullOrWhiteSpace(sucursalId)) filtros.Add("Sucursal", (await _context.Sucursales.FindAsync(sucursalId))?.Nombre ?? "N/D");
            if (departamentoId.HasValue && departamentoId.Value > 0) filtros.Add("Departamento", (await _context.Departamentos.FindAsync(departamentoId))?.Nombre ?? "N/D");

            List<string> headers;
            List<List<string>> filas;

            if (layout == "horizontal")
            {
                headers = new List<string> { "Sucursal", "Zona", "Equipo", "Serial", "Marca/Modelo", "S.O.", "CPU", "RAM", "Disco" };
                filas = items.Select(e => new List<string> {
                    e.Sucursal?.Nombre ?? "N/D",
                    e.Sucursal?.Region?.Nombre ?? "N/D",
                    e.NombreEquipo,
                    e.Serial ?? "—",
                    $"{e.Marca ?? "—"} {e.Modelo ?? ""}".Trim(),
                    e.SistemaOperativo?.Nombre ?? "—",
                    (string.IsNullOrWhiteSpace(e.CpuDetalle) ? e.TipoProcesador?.Nombre : $"{e.TipoProcesador?.Nombre} {e.CpuDetalle}") ?? "—",
                    (e.RamCantidadGb?.ToString() ?? "0") + " GB",
                    e.Almacenamiento ?? "—"
                }).ToList();

                // Ajuste de anchos para que quepan en 732 pts (792 - 60 de márgenes)
                // Sucursal(70), Zona(60), Equipo(80), Serial(80), Marca/Modelo(100), S.O(70), CPU(120), RAM(40), Disco(80) = 700 total
                var widths = new List<int> { 70, 60, 80, 80, 100, 70, 120, 40, 80 };
                var pdfBytesH = PdfReportService.GenerarPdfDinamico("Reporte de Equipos", usuario, filtros, headers, filas, true, widths);
                return File(pdfBytesH, "application/pdf", $"equipos_{DateTime.Now:yyyyMMdd}.pdf");
            }
            else
            {
                headers = new List<string> { "Sucursal", "Zona", "Equipo", "Serial", "Especificaciones", "Estado" };
                filas = items.Select(e => new List<string> {
                    e.Sucursal?.Nombre ?? "N/D",
                    e.Sucursal?.Region?.Nombre ?? "N/D",
                    e.NombreEquipo,
                    e.Serial ?? "—",
                    $"• Marca/Modelo: {e.Marca ?? "N/D"} {e.Modelo ?? ""}\n• S.O: {e.SistemaOperativo?.Nombre ?? "N/D"}\n• CPU: {(string.IsNullOrWhiteSpace(e.CpuDetalle) ? e.TipoProcesador?.Nombre : $"{e.TipoProcesador?.Nombre} {e.CpuDetalle}")}\n• RAM: {e.RamCantidadGb?.ToString() ?? "0"} GB {e.TipoRam?.Nombre}\n• Disco: {e.Almacenamiento ?? "N/D"}",
                    e.Activo ? "Activo" : "Inactivo"
                }).ToList();
            }

            var pdfBytes = PdfReportService.GenerarPdfDinamico("Reporte de Equipos", usuario, filtros, headers, filas, layout == "horizontal");
            return File(pdfBytes, "application/pdf", $"equipos_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // GET: Infraestructura/Equipos
        public async Task<IActionResult> Equipos(string q = "", int? regionId = null, string? sucursalId = null, int? departamentoId = null, string layout = "vertical")
        {
            var equiposQuery = _context.InfraEquiposComputo
                .Where(e => !e.Eliminado)
                .Include(e => e.Sucursal)
                    .ThenInclude(s => s.Region)
                .Include(e => e.SistemaOperativo)
                .Include(e => e.TipoProcesador)
                .Include(e => e.TipoRam)
                .Include(e => e.EquiposDepartamentos)
                    .ThenInclude(ed => ed.Departamento)
                .AsQueryable();

            if (regionId.HasValue && regionId.Value > 0)
            {
                equiposQuery = equiposQuery.Where(e => e.Sucursal != null && e.Sucursal.RegionId == regionId.Value);
            }

            if (!string.IsNullOrWhiteSpace(sucursalId))
            {
                equiposQuery = equiposQuery.Where(e => e.SucursalId == sucursalId);
            }

            if (departamentoId.HasValue && departamentoId.Value > 0)
            {
                equiposQuery = equiposQuery.Where(e => e.EquiposDepartamentos.Any(ed => ed.DepartamentoId == departamentoId.Value));
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                equiposQuery = equiposQuery.Where(e =>
                    e.NombreEquipo.ToLower().Contains(term) ||
                    e.Serial.ToLower().Contains(term) ||
                    (e.CodigoActivo != null && e.CodigoActivo.ToLower().Contains(term)) ||
                    (e.Marca != null && e.Marca.ToLower().Contains(term)) ||
                    (e.Modelo != null && e.Modelo.ToLower().Contains(term)) ||
                    (e.Sucursal != null && e.Sucursal.Region != null && e.Sucursal.Region.Nombre.ToLower().Contains(term)) ||
                    (e.Sucursal != null && e.Sucursal.Nombre.ToLower().Contains(term)) ||
                    e.EquiposDepartamentos.Any(ed => ed.Departamento != null && ed.Departamento.Nombre.ToLower().Contains(term)));
            }

            var equipos = await equiposQuery
                .OrderBy(e => e.Sucursal!.Nombre)
                .ThenBy(e => e.NombreEquipo)
                .ToListAsync();

            var vm = new InfraestructuraIndexViewModel
            {
                Query = q,
                RegionFiltro = regionId,
                SucursalFiltro = sucursalId,
                DepartamentoFiltro = departamentoId,
                TotalEquipos = equipos.Count,
                EquiposActivos = equipos.Count(e => e.Activo),
                Regiones = await ObtenerRegionesSelect(),
                Sucursales = await ObtenerSucursalesSelect(regionId),
                Departamentos = await ObtenerDepartamentosSelect(),
                Equipos = equipos.Select(MapEquipoListItem).ToList(),
                ViewLayout = layout
            };

            return View(vm);
        }

        // GET: Infraestructura/ExportarServiciosCsv
        public async Task<IActionResult> ExportarServiciosCsv(string q = "", int? regionId = null, string? sucursalId = null)
        {
            var query = _context.InfraServiciosSucursal
                .Where(s => !s.Eliminado)
                .Include(s => s.Sucursal).ThenInclude(su => su.Region)
                .Include(s => s.TipoServicio)
                .Include(s => s.OperadorServicio)
                .AsQueryable();

            if (regionId.HasValue && regionId.Value > 0)
                query = query.Where(s => s.Sucursal != null && s.Sucursal.RegionId == regionId.Value);

            if (!string.IsNullOrWhiteSpace(sucursalId))
                query = query.Where(s => s.SucursalId == sucursalId);

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                query = query.Where(s =>
                    (s.NumeroServicio != null && s.NumeroServicio.ToLower().Contains(term)) ||
                    (s.TipoServicio != null && s.TipoServicio.Nombre.ToLower().Contains(term)));
            }

            var items = await query.OrderBy(s => s.Sucursal!.Nombre).ToListAsync();
            var headers = new[] { "Sucursal", "Zona", "Tipo de Servicio", "Operador", "Número", "Baja (Mbps)", "Subida (Mbps)", "Estado" };
            var rows = items.Select(s => new[] {
                s.Sucursal?.Nombre ?? "N/D",
                s.Sucursal?.Region?.Nombre ?? "N/D",
                s.TipoServicio?.Nombre ?? "N/D",
                s.OperadorServicio?.Nombre ?? "N/D",
                s.NumeroServicio ?? "",
                s.VelocidadBajadaMbps?.ToString() ?? "0",
                s.VelocidadSubidaMbps?.ToString() ?? "0",
                s.Activo ? "Activo" : "Inactivo"
            });

            var bytes = CsvExportService.BuildCsv(headers, rows);
            return File(bytes, "text/csv", $"servicios_{DateTime.Now:yyyyMMdd}.csv");
        }

        // GET: Infraestructura/ExportarServiciosPdf
        public async Task<IActionResult> ExportarServiciosPdf(string q = "", int? regionId = null, string? sucursalId = null)
        {
            var usuario = HttpContext.Session.GetString("UserName") ?? "Sistema";
            var query = _context.InfraServiciosSucursal
                .Where(s => !s.Eliminado)
                .Include(s => s.Sucursal).ThenInclude(su => su.Region)
                .Include(s => s.TipoServicio)
                .Include(s => s.OperadorServicio)
                .AsQueryable();

            if (regionId.HasValue && regionId.Value > 0)
                query = query.Where(s => s.Sucursal != null && s.Sucursal.RegionId == regionId.Value);

            if (!string.IsNullOrWhiteSpace(sucursalId))
                query = query.Where(s => s.SucursalId == sucursalId);

            var items = await query.OrderBy(s => s.Sucursal!.Nombre).ToListAsync();
            if (!items.Any()) return File(PdfReportService.GenerarPdfVacio("Reporte de Servicios", "No hay servicios"), "application/pdf", "servicios.pdf");

            var headers = new List<string> { "Sucursal", "Zona", "Tipo", "Operador", "Número", "Velocidad", "Estado" };
            var filas = items.Select(s => new List<string> {
                s.Sucursal?.Nombre ?? "N/D",
                s.Sucursal?.Region?.Nombre ?? "N/D",
                s.TipoServicio?.Nombre ?? "N/D",
                s.OperadorServicio?.Nombre ?? "N/D",
                s.NumeroServicio ?? "—",
                $"{s.VelocidadBajadaMbps}/{s.VelocidadSubidaMbps} Mbps",
                s.Activo ? "Activo" : "Inactivo"
            }).ToList();

            var document = Document.Create(container => {
                container.Page(page => {
                    page.Size(PageSizes.Letter);
                    page.Margin(30);
                    page.Header().Element(c => PdfReportService.GenerarHeader(c, "Servicios de Sucursal", usuario));
                    page.Content().PaddingTop(10).Element(c => PdfReportService.GenerarTablaSimple(c, headers, filas));
                    page.Footer().Element(c => PdfReportService.GenerarFooter(c, usuario));
                });
            });

            return File(document.GeneratePdf(), "application/pdf", $"servicios_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // GET: Infraestructura/ExportarAccesoriosCsv
        public async Task<IActionResult> ExportarAccesoriosCsv(string q = "", int? regionId = null, string? sucursalId = null)
        {
            var query = _context.InfraSucursalesAccesorio
                .Where(a => !a.Eliminado)
                .Include(a => a.Sucursal).ThenInclude(su => su.Region)
                .Include(a => a.TipoAccesorio)
                .AsQueryable();

            if (regionId.HasValue && regionId.Value > 0)
                query = query.Where(a => a.Sucursal != null && a.Sucursal.RegionId == regionId.Value);

            if (!string.IsNullOrWhiteSpace(sucursalId))
                query = query.Where(a => a.SucursalId == sucursalId);

            var items = await query.OrderBy(a => a.Sucursal!.Nombre).ToListAsync();
            var headers = new[] { "Sucursal", "Zona", "Tipo de Accesorio", "Cantidad", "Especificaciones", "Estado" };
            var rows = items.Select(a => new[] {
                a.Sucursal?.Nombre ?? "N/D",
                a.Sucursal?.Region?.Nombre ?? "N/D",
                a.TipoAccesorio?.Nombre ?? "N/D",
                a.Cantidad.ToString(),
                a.Especificaciones ?? "",
                a.Activo ? "Activo" : "Inactivo"
            });

            var bytes = CsvExportService.BuildCsv(headers, rows);
            return File(bytes, "text/csv", $"accesorios_{DateTime.Now:yyyyMMdd}.csv");
        }

        // GET: Infraestructura/ExportarAccesoriosPdf
        public async Task<IActionResult> ExportarAccesoriosPdf(string q = "", int? regionId = null, string? sucursalId = null)
        {
            var usuario = HttpContext.Session.GetString("UserName") ?? "Sistema";
            var query = _context.InfraSucursalesAccesorio
                .Where(a => !a.Eliminado)
                .Include(a => a.Sucursal).ThenInclude(su => su.Region)
                .Include(a => a.TipoAccesorio)
                .AsQueryable();

            if (regionId.HasValue && regionId.Value > 0)
                query = query.Where(a => a.Sucursal != null && a.Sucursal.RegionId == regionId.Value);

            if (!string.IsNullOrWhiteSpace(sucursalId))
                query = query.Where(a => a.SucursalId == sucursalId);

            var items = await query.OrderBy(a => a.Sucursal!.Nombre).ToListAsync();
            if (!items.Any()) return File(PdfReportService.GenerarPdfVacio("Reporte de Accesorios", "No hay accesorios"), "application/pdf", "accesorios.pdf");

            var headers = new List<string> { "Sucursal", "Zona", "Tipo", "Cantidad", "Especificaciones", "Estado" };
            var filas = items.Select(a => new List<string> {
                a.Sucursal?.Nombre ?? "N/D",
                a.Sucursal?.Region?.Nombre ?? "N/D",
                a.TipoAccesorio?.Nombre ?? "N/D",
                a.Cantidad.ToString(),
                a.Especificaciones ?? "—",
                a.Activo ? "Activo" : "Inactivo"
            }).ToList();

            var document = Document.Create(container => {
                container.Page(page => {
                    page.Size(PageSizes.Letter);
                    page.Margin(30);
                    page.Header().Element(c => PdfReportService.GenerarHeader(c, "Accesorios de Sucursal", usuario));
                    page.Content().PaddingTop(10).Element(c => PdfReportService.GenerarTablaSimple(c, headers, filas));
                    page.Footer().Element(c => PdfReportService.GenerarFooter(c, usuario));
                });
            });

            return File(document.GeneratePdf(), "application/pdf", $"accesorios_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // GET: Infraestructura/Servicios
        public async Task<IActionResult> Servicios(string q = "", int? regionId = null, string? sucursalId = null)
        {
            var serviciosQuery = _context.InfraServiciosSucursal
                .Where(s => !s.Eliminado)
                .Include(s => s.Sucursal)
                    .ThenInclude(su => su.Region)
                .Include(s => s.TipoServicio)
                .Include(s => s.OperadorServicio)
                .AsQueryable();

            if (regionId.HasValue && regionId.Value > 0)
            {
                serviciosQuery = serviciosQuery.Where(s => s.Sucursal != null && s.Sucursal.RegionId == regionId.Value);
            }

            if (!string.IsNullOrWhiteSpace(sucursalId))
            {
                serviciosQuery = serviciosQuery.Where(s => s.SucursalId == sucursalId);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                serviciosQuery = serviciosQuery.Where(s =>
                    (s.Sucursal != null && s.Sucursal.Region != null && s.Sucursal.Region.Nombre.ToLower().Contains(term)) ||
                    (s.Sucursal != null && s.Sucursal.Nombre.ToLower().Contains(term)) ||
                    (s.TipoServicio != null && s.TipoServicio.Nombre.ToLower().Contains(term)) ||
                    (s.OperadorServicio != null && s.OperadorServicio.Nombre.ToLower().Contains(term)) ||
                    (s.NumeroServicio != null && s.NumeroServicio.ToLower().Contains(term)));
            }

            var servicios = await serviciosQuery
                .OrderBy(s => s.Sucursal!.Nombre)
                .ThenBy(s => s.TipoServicio!.Nombre)
                .ToListAsync();

            var vm = new InfraestructuraIndexViewModel
            {
                Query = q,
                RegionFiltro = regionId,
                SucursalFiltro = sucursalId,
                TotalServicios = servicios.Count,
                Regiones = await ObtenerRegionesSelect(),
                Sucursales = await ObtenerSucursalesSelect(regionId),
                Servicios = servicios.Select(MapServicioListItem).ToList()
            };

            return View(vm);
        }

        // GET: Infraestructura/Accesorios
        public async Task<IActionResult> Accesorios(string q = "", int? regionId = null, string? sucursalId = null)
        {
            var accesoriosQuery = _context.InfraSucursalesAccesorio
                .Where(a => !a.Eliminado)
                .Include(a => a.Sucursal)
                    .ThenInclude(su => su.Region)
                .Include(a => a.TipoAccesorio)
                .AsQueryable();

            if (regionId.HasValue && regionId.Value > 0)
            {
                accesoriosQuery = accesoriosQuery.Where(a => a.Sucursal != null && a.Sucursal.RegionId == regionId.Value);
            }

            if (!string.IsNullOrWhiteSpace(sucursalId))
            {
                accesoriosQuery = accesoriosQuery.Where(a => a.SucursalId == sucursalId);
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLower();
                accesoriosQuery = accesoriosQuery.Where(a =>
                    (a.Sucursal != null && a.Sucursal.Region != null && a.Sucursal.Region.Nombre.ToLower().Contains(term)) ||
                    (a.Sucursal != null && a.Sucursal.Nombre.ToLower().Contains(term)) ||
                    (a.TipoAccesorio != null && a.TipoAccesorio.Nombre.ToLower().Contains(term)) ||
                    (a.Especificaciones != null && a.Especificaciones.ToLower().Contains(term)));
            }

            var accesorios = await accesoriosQuery
                .OrderBy(a => a.Sucursal!.Nombre)
                .ThenBy(a => a.TipoAccesorio!.Nombre)
                .ToListAsync();

            var vm = new InfraestructuraIndexViewModel
            {
                Query = q,
                RegionFiltro = regionId,
                SucursalFiltro = sucursalId,
                TotalAccesorios = accesorios.Count,
                Regiones = await ObtenerRegionesSelect(),
                Sucursales = await ObtenerSucursalesSelect(regionId),
                Accesorios = accesorios.Select(MapAccesorioListItem).ToList()
            };

            return View(vm);
        }

        // GET: Infraestructura/Sucursal
        public async Task<IActionResult> Sucursal(string codigoSucursal = "", string layout = "vertical")
        {
            var vm = new InfraSucursalResumenViewModel
            {
                CodigoSucursal = codigoSucursal ?? string.Empty,
                Sucursales = await ObtenerSucursalesSelect(),
                ViewLayout = layout
            };

            if (string.IsNullOrWhiteSpace(codigoSucursal))
            {
                return View(vm);
            }

            var codigoNormalizado = NormalizarCodigoSucursal(codigoSucursal.Trim());

            var sucursal = await _context.Sucursales
                .Where(s => !s.Eliminado && s.Activo)
                .Include(s => s.Region)
                .FirstOrDefaultAsync(s => 
                    s.Id.ToLower() == codigoNormalizado.ToLower() || 
                    s.Id.ToLower() == codigoSucursal.Trim().ToLower() ||
                    s.Id.ToLower().Contains(codigoSucursal.Trim().ToLower()) ||
                    s.Nombre.ToLower().Contains(codigoSucursal.Trim().ToLower()));

            if (sucursal == null)
            {
                vm.Mensaje = "No se encontró una sucursal con ese código.";
                return View(vm);
            }

            var equipos = await _context.InfraEquiposComputo
                .Where(e => !e.Eliminado && e.SucursalId == sucursal.Id)
                .Include(e => e.Sucursal).ThenInclude(s => s.Region)
                .Include(e => e.SistemaOperativo)
                .Include(e => e.TipoProcesador)
                .Include(e => e.TipoRam)
                .Include(e => e.EquiposDepartamentos).ThenInclude(ed => ed.Departamento)
                .OrderBy(e => e.NombreEquipo)
                .ToListAsync();

            var servicios = await _context.InfraServiciosSucursal
                .Where(s => !s.Eliminado && s.SucursalId == sucursal.Id)
                .Include(s => s.Sucursal).ThenInclude(su => su.Region)
                .Include(s => s.TipoServicio)
                .Include(s => s.OperadorServicio)
                .OrderBy(s => s.TipoServicio!.Nombre)
                .ToListAsync();

            var accesorios = await _context.InfraSucursalesAccesorio
                .Where(a => !a.Eliminado && a.SucursalId == sucursal.Id)
                .Include(a => a.Sucursal).ThenInclude(su => su.Region)
                .Include(a => a.TipoAccesorio)
                .OrderBy(a => a.TipoAccesorio!.Nombre)
                .ToListAsync();

            vm.Sucursal = new InfraSucursalInfoViewModel
            {
                Id = sucursal.Id,
                Nombre = sucursal.Nombre,
                Region = sucursal.Region?.Nombre ?? "N/D",
                Direccion = sucursal.Direccion,
                Telefono = sucursal.Telefono
            };
            vm.TotalEquipos = equipos.Count;
            vm.EquiposActivos = equipos.Count(e => e.Activo);
            vm.TotalServicios = servicios.Count;
            vm.TotalAccesorios = accesorios.Count;
            vm.Equipos = equipos.Select(MapEquipoListItem).ToList();
            vm.Servicios = servicios.Select(MapServicioListItem).ToList();
            vm.Accesorios = accesorios.Select(MapAccesorioListItem).ToList();
            vm.DepartamentosRelacionados = equipos
                .SelectMany(e => e.EquiposDepartamentos)
                .Where(ed => ed.Departamento != null && !ed.Departamento.Eliminado)
                .Select(ed => ed.Departamento!.Nombre)
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            return View(vm);
        }

        // GET: Infraestructura/ExportarSucursalPdf
        public async Task<IActionResult> ExportarSucursalPdf(string codigoSucursal, string layout = "vertical")
        {
            var usuario = HttpContext.Session.GetString("UserName") ?? "Sistema";
            if (string.IsNullOrWhiteSpace(codigoSucursal)) return BadRequest("Código de sucursal requerido.");

            var codigoNormalizado = NormalizarCodigoSucursal(codigoSucursal.Trim());
            var sucursal = await _context.Sucursales
                .Where(s => !s.Eliminado && s.Activo)
                .Include(s => s.Region)
                .FirstOrDefaultAsync(s => s.Id.ToLower() == codigoNormalizado.ToLower() || s.Id.ToLower() == codigoSucursal.Trim().ToLower() || s.Nombre.ToLower().Contains(codigoSucursal.Trim().ToLower()));

            if (sucursal == null) return NotFound("Sucursal no encontrada.");

            var equipos = await _context.InfraEquiposComputo
                .Where(e => !e.Eliminado && e.SucursalId == sucursal.Id)
                .Include(e => e.Sucursal).ThenInclude(s => s.Region)
                .Include(e => e.SistemaOperativo)
                .Include(e => e.TipoProcesador)
                .Include(e => e.TipoRam)
                .OrderBy(e => e.NombreEquipo)
                .ToListAsync();

            var servicios = await _context.InfraServiciosSucursal
                .Where(s => !s.Eliminado && s.SucursalId == sucursal.Id)
                .Include(s => s.Sucursal).ThenInclude(su => su.Region)
                .Include(s => s.TipoServicio)
                .Include(s => s.OperadorServicio)
                .OrderBy(s => s.TipoServicio!.Nombre)
                .ToListAsync();

            var accesorios = await _context.InfraSucursalesAccesorio
                .Where(a => !a.Eliminado && a.SucursalId == sucursal.Id)
                .Include(a => a.Sucursal).ThenInclude(su => su.Region)
                .Include(a => a.TipoAccesorio)
                .OrderBy(a => a.TipoAccesorio!.Nombre)
                .ToListAsync();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(layout == "horizontal" ? PageSizes.Letter.Landscape() : PageSizes.Letter);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Element(c => PdfReportService.GenerarHeader(c, $"Vista Holística: {sucursal.Nombre}", usuario));

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        // Info Sucursal
                        col.Item().PaddingBottom(10).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(sCol => {
                            sCol.Item().Text($"Sucursal: {sucursal.Nombre} ({sucursal.Id})").Bold().FontSize(12);
                            sCol.Item().Text($"Zona: {sucursal.Region?.Nombre ?? "N/D"}");
                            if (!string.IsNullOrEmpty(sucursal.Direccion)) sCol.Item().Text($"Dirección: {sucursal.Direccion}");
                        });

                        // Equipos
                        col.Item().PaddingTop(10).PaddingBottom(5).Text("EQUIPOS DE CÓMPUTO").Bold().FontColor(Colors.Blue.Darken3);
                        
                        List<string> headers;
                        List<List<string>> filas;

                        if (layout == "horizontal")
                        {
                            headers = new List<string> { "Sucursal", "Zona", "Equipo", "Serial", "Marca/Modelo", "S.O.", "CPU", "RAM", "Disco" };
                            filas = equipos.Select(e => new List<string> {
                                sucursal.Nombre, sucursal.Region?.Nombre ?? "N/D", e.NombreEquipo, e.Serial ?? "—", $"{e.Marca ?? "—"} {e.Modelo ?? ""}".Trim(),
                                e.SistemaOperativo?.Nombre ?? "—", (string.IsNullOrWhiteSpace(e.CpuDetalle) ? e.TipoProcesador?.Nombre : $"{e.TipoProcesador?.Nombre} {e.CpuDetalle}") ?? "—",
                                (e.RamCantidadGb?.ToString() ?? "0") + " GB", e.Almacenamiento ?? "—"
                            }).ToList();
                            
                            var hWidths = new List<int> { 80, 70, 90, 100, 120, 80, 120, 40, 80 };
                            col.Item().Element(c => PdfReportService.GenerarTablaSimple(c, headers, filas, hWidths));
                        }
                        else
                        {
                            headers = new List<string> { "Sucursal", "Zona", "Equipo", "Serial", "Especificaciones", "Estado" };
                            filas = equipos.Select(e => new List<string> {
                                sucursal.Nombre, sucursal.Region?.Nombre ?? "N/D", e.NombreEquipo, e.Serial,
                                $"• Marca/Modelo: {e.Marca ?? "N/D"} {e.Modelo ?? ""}\n• S.O: {e.SistemaOperativo?.Nombre ?? "N/D"}\n• CPU: {(string.IsNullOrWhiteSpace(e.CpuDetalle) ? e.TipoProcesador?.Nombre : $"{e.TipoProcesador?.Nombre} {e.CpuDetalle}")}\n• RAM: {e.RamCantidadGb?.ToString() ?? "0"} GB {e.TipoRam?.Nombre}\n• Disco: {e.Almacenamiento ?? "N/D"}",
                                e.Activo ? "Activo" : "Inactivo"
                            }).ToList();
                            col.Item().Element(c => PdfReportService.GenerarTablaSimple(c, headers, filas));
                        }

                        // Servicios
                        col.Item().PaddingTop(20).PaddingBottom(5).Text("SERVICIOS DE RED/INTERNET").Bold().FontColor(Colors.Blue.Darken3);
                        var sHeaders = new List<string> { "Sucursal", "Zona", "Tipo", "Operador", "Número", "Velocidad", "Estado" };
                        var sFilas = servicios.Select(s => new List<string> {
                            sucursal.Nombre, sucursal.Region?.Nombre ?? "N/D", s.TipoServicio?.Nombre ?? "N/D", s.OperadorServicio?.Nombre ?? "N/D",
                            s.NumeroServicio ?? "—", $"{s.VelocidadBajadaMbps}/{s.VelocidadSubidaMbps} Mbps", s.Activo ? "Activo" : "Inactivo"
                        }).ToList();
                        col.Item().Element(c => PdfReportService.GenerarTablaSimple(c, sHeaders, sFilas));

                        // Accesorios
                        col.Item().PaddingTop(20).PaddingBottom(5).Text("ACCESORIOS Y PERIFÉRICOS").Bold().FontColor(Colors.Blue.Darken3);
                        var aHeaders = new List<string> { "Sucursal", "Zona", "Tipo", "Cantidad", "Especificaciones", "Estado" };
                        var aFilas = accesorios.Select(a => new List<string> {
                            sucursal.Nombre, sucursal.Region?.Nombre ?? "N/D", a.TipoAccesorio?.Nombre ?? "N/D", a.Cantidad.ToString(),
                            a.Especificaciones ?? "—", a.Activo ? "Activo" : "Inactivo"
                        }).ToList();
                        col.Item().Element(c => PdfReportService.GenerarTablaSimple(c, aHeaders, aFilas));
                    });

                    page.Footer().Element(c => PdfReportService.GenerarFooter(c, usuario));
                });
            });

            return File(pdf.GeneratePdf(), "application/pdf", $"holistico_{sucursal.Nombre}_{DateTime.Now:yyyyMMdd}.pdf");
        }

        // GET: Infraestructura/SucursalesPorRegion?regionId=3
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

        // GET: Infraestructura/CreateEquipo
        public async Task<IActionResult> CreateEquipo(string? sucursalId = null)
        {
            var vm = new InfraEquipoFormViewModel
            {
                Activo = true
            };

            if (!string.IsNullOrEmpty(sucursalId))
            {
                var sucursal = await _context.Sucursales.FindAsync(sucursalId);
                if (sucursal != null)
                {
                    vm.SucursalId = sucursal.Id;
                    vm.RegionId = sucursal.RegionId;
                }
            }

            await CargarCatalogosEquipo(vm);
            return View(vm);
        }

        // POST: Infraestructura/CreateEquipo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEquipo(InfraEquipoFormViewModel vm)
        {
            await ValidarEquipo(vm);

            if (!ModelState.IsValid)
            {
                await CargarCatalogosEquipo(vm);
                return View(vm);
            }

            var equipo = new InfraEquipoComputo
            {
                CodigoActivo = NormalizarTexto(vm.CodigoActivo),
                SucursalId = vm.SucursalId,
                NombreEquipo = vm.NombreEquipo.Trim(),
                Marca = NormalizarTexto(vm.Marca),
                Modelo = NormalizarTexto(vm.Modelo),
                Serial = vm.Serial.Trim().ToUpperInvariant(),
                SistemaOperativoId = vm.SistemaOperativoId,
                TipoProcesadorId = vm.TipoProcesadorId,
                CpuDetalle = NormalizarTexto(vm.CpuDetalle),
                TipoRamId = vm.TipoRamId,
                RamCantidadGb = vm.RamCantidadGb,
                Almacenamiento = NormalizarTexto(vm.Almacenamiento),
                DireccionIp = NormalizarTexto(vm.DireccionIp),
                Observaciones = NormalizarTexto(vm.Observaciones),
                Activo = vm.Activo
            };

            _context.InfraEquiposComputo.Add(equipo);
            await _context.SaveChangesAsync();
            await ReemplazarDepartamentosEquipo(equipo.Id, vm.DepartamentosSeleccionados);

            TempData["Success"] = "Equipo registrado exitosamente.";
            return RedirectToAction(nameof(Equipos));
        }

        // GET: Infraestructura/EditEquipo/5
        public async Task<IActionResult> EditEquipo(int id)
        {
            var equipo = await _context.InfraEquiposComputo
                .Where(e => !e.Eliminado && e.Id == id)
                .Include(e => e.Sucursal)
                .Include(e => e.EquiposDepartamentos)
                .FirstOrDefaultAsync();

            if (equipo == null)
            {
                return NotFound();
            }

            var vm = new InfraEquipoFormViewModel
            {
                Id = equipo.Id,
                RegionId = equipo.Sucursal?.RegionId,
                CodigoActivo = equipo.CodigoActivo,
                SucursalId = equipo.SucursalId,
                NombreEquipo = equipo.NombreEquipo,
                Marca = equipo.Marca,
                Modelo = equipo.Modelo,
                Serial = equipo.Serial,
                SistemaOperativoId = equipo.SistemaOperativoId,
                TipoProcesadorId = equipo.TipoProcesadorId,
                CpuDetalle = equipo.CpuDetalle,
                TipoRamId = equipo.TipoRamId,
                RamCantidadGb = equipo.RamCantidadGb,
                Almacenamiento = equipo.Almacenamiento,
                DireccionIp = equipo.DireccionIp,
                Observaciones = equipo.Observaciones,
                Activo = equipo.Activo,
                DepartamentosSeleccionados = equipo.EquiposDepartamentos.Select(ed => ed.DepartamentoId).ToList()
            };

            await CargarCatalogosEquipo(vm);
            return View(vm);
        }

        // POST: Infraestructura/EditEquipo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEquipo(int id, InfraEquipoFormViewModel vm)
        {
            if (vm.Id != id)
            {
                return NotFound();
            }

            var equipo = await _context.InfraEquiposComputo
                .Where(e => !e.Eliminado && e.Id == id)
                .FirstOrDefaultAsync();

            if (equipo == null)
            {
                return NotFound();
            }

            await ValidarEquipo(vm, id);

            if (!ModelState.IsValid)
            {
                await CargarCatalogosEquipo(vm);
                return View(vm);
            }

            equipo.CodigoActivo = NormalizarTexto(vm.CodigoActivo);
            equipo.SucursalId = vm.SucursalId;
            equipo.NombreEquipo = vm.NombreEquipo.Trim();
            equipo.Marca = NormalizarTexto(vm.Marca);
            equipo.Modelo = NormalizarTexto(vm.Modelo);
            equipo.Serial = vm.Serial.Trim();
            equipo.SistemaOperativoId = vm.SistemaOperativoId;
            equipo.TipoProcesadorId = vm.TipoProcesadorId;
            equipo.CpuDetalle = NormalizarTexto(vm.CpuDetalle);
            equipo.TipoRamId = vm.TipoRamId;
            equipo.RamCantidadGb = vm.RamCantidadGb;
            equipo.Almacenamiento = NormalizarTexto(vm.Almacenamiento);
            equipo.DireccionIp = NormalizarTexto(vm.DireccionIp);
            equipo.Observaciones = NormalizarTexto(vm.Observaciones);
            equipo.Activo = vm.Activo;

            _context.InfraEquiposComputo.Update(equipo);
            await _context.SaveChangesAsync();
            await ReemplazarDepartamentosEquipo(equipo.Id, vm.DepartamentosSeleccionados);

            TempData["Success"] = "Equipo actualizado exitosamente.";
            return RedirectToAction(nameof(Equipos));
        }

        // POST: Infraestructura/DeleteEquipo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEquipo(int id)
        {
            var equipo = await _context.InfraEquiposComputo
                .Where(e => !e.Eliminado && e.Id == id)
                .FirstOrDefaultAsync();

            if (equipo == null)
            {
                return Json(new { success = false, message = "Equipo no encontrado." });
            }

            equipo.Eliminado = true;
            equipo.FechaEliminacion = DateTime.Now;
            equipo.UsuarioEliminacion = User.Identity?.Name ?? "Sistema";

            _context.InfraEquiposComputo.Update(equipo);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Equipo eliminado exitosamente." });
        }

        // GET: Infraestructura/CreateServicio
        public async Task<IActionResult> CreateServicio()
        {
            var vm = new InfraServicioFormViewModel
            {
                Activo = true
            };
            await CargarCatalogosServicio(vm);
            return View(vm);
        }

        // POST: Infraestructura/CreateServicio
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateServicio(InfraServicioFormViewModel vm)
        {
            await ValidarServicio(vm);

            if (!ModelState.IsValid)
            {
                await CargarCatalogosServicio(vm);
                return View(vm);
            }

            var servicio = new InfraServicioSucursal
            {
                SucursalId = vm.SucursalId,
                TipoServicioId = vm.TipoServicioId,
                OperadorServicioId = vm.OperadorServicioId,
                NumeroServicio = NormalizarTexto(vm.NumeroServicio),
                VelocidadBajadaMbps = vm.VelocidadBajadaMbps,
                VelocidadSubidaMbps = vm.VelocidadSubidaMbps,
                Observaciones = NormalizarTexto(vm.Observaciones),
                Activo = vm.Activo
            };

            _context.InfraServiciosSucursal.Add(servicio);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Servicio registrado exitosamente.";
            return RedirectToAction(nameof(Servicios));
        }

        // GET: Infraestructura/EditServicio/5
        public async Task<IActionResult> EditServicio(int id)
        {
            var servicio = await _context.InfraServiciosSucursal
                .Where(s => !s.Eliminado && s.Id == id)
                .Include(s => s.Sucursal)
                .FirstOrDefaultAsync();

            if (servicio == null)
            {
                return NotFound();
            }

            var vm = new InfraServicioFormViewModel
            {
                Id = servicio.Id,
                RegionId = servicio.Sucursal?.RegionId,
                SucursalId = servicio.SucursalId,
                TipoServicioId = servicio.TipoServicioId,
                OperadorServicioId = servicio.OperadorServicioId,
                NumeroServicio = servicio.NumeroServicio,
                VelocidadBajadaMbps = servicio.VelocidadBajadaMbps,
                VelocidadSubidaMbps = servicio.VelocidadSubidaMbps,
                Observaciones = servicio.Observaciones,
                Activo = servicio.Activo
            };

            await CargarCatalogosServicio(vm);
            return View(vm);
        }

        // POST: Infraestructura/EditServicio/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditServicio(int id, InfraServicioFormViewModel vm)
        {
            if (vm.Id != id)
            {
                return NotFound();
            }

            var servicio = await _context.InfraServiciosSucursal
                .Where(s => !s.Eliminado && s.Id == id)
                .FirstOrDefaultAsync();

            if (servicio == null)
            {
                return NotFound();
            }

            await ValidarServicio(vm);

            if (!ModelState.IsValid)
            {
                await CargarCatalogosServicio(vm);
                return View(vm);
            }

            servicio.SucursalId = vm.SucursalId;
            servicio.TipoServicioId = vm.TipoServicioId;
            servicio.OperadorServicioId = vm.OperadorServicioId;
            servicio.NumeroServicio = NormalizarTexto(vm.NumeroServicio);
            servicio.VelocidadBajadaMbps = vm.VelocidadBajadaMbps;
            servicio.VelocidadSubidaMbps = vm.VelocidadSubidaMbps;
            servicio.Observaciones = NormalizarTexto(vm.Observaciones);
            servicio.Activo = vm.Activo;

            _context.InfraServiciosSucursal.Update(servicio);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Servicio actualizado exitosamente.";
            return RedirectToAction(nameof(Servicios));
        }

        // POST: Infraestructura/DeleteServicio/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteServicio(int id)
        {
            var servicio = await _context.InfraServiciosSucursal
                .Where(s => !s.Eliminado && s.Id == id)
                .FirstOrDefaultAsync();

            if (servicio == null)
            {
                return Json(new { success = false, message = "Servicio no encontrado." });
            }

            servicio.Eliminado = true;
            servicio.FechaEliminacion = DateTime.Now;
            servicio.UsuarioEliminacion = User.Identity?.Name ?? "Sistema";

            _context.InfraServiciosSucursal.Update(servicio);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Servicio eliminado exitosamente." });
        }

        // GET: Infraestructura/CreateAccesorio
        public async Task<IActionResult> CreateAccesorio(string? sucursalId = null)
        {
            var vm = new InfraAccesorioFormViewModel
            {
                Activo = true,
                Cantidad = 1
            };

            if (!string.IsNullOrEmpty(sucursalId))
            {
                var sucursal = await _context.Sucursales.FindAsync(sucursalId);
                if (sucursal != null)
                {
                    vm.SucursalId = sucursal.Id;
                    vm.RegionId = sucursal.RegionId;
                }
            }

            await CargarCatalogosAccesorio(vm);
            return View(vm);
        }

        // POST: Infraestructura/CreateAccesorio
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccesorio(InfraAccesorioFormViewModel vm)
        {
            await ValidarAccesorio(vm);

            if (!ModelState.IsValid)
            {
                await CargarCatalogosAccesorio(vm);
                return View(vm);
            }

            var accesorio = new InfraSucursalAccesorio
            {
                SucursalId = vm.SucursalId,
                TipoAccesorioId = vm.TipoAccesorioId,
                Cantidad = vm.Cantidad,
                Especificaciones = NormalizarTexto(vm.Especificaciones),
                Activo = vm.Activo
            };

            _context.InfraSucursalesAccesorio.Add(accesorio);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Accesorio registrado exitosamente.";
            return RedirectToAction(nameof(Accesorios));
        }

        // GET: Infraestructura/EditAccesorio/5
        public async Task<IActionResult> EditAccesorio(int id)
        {
            var accesorio = await _context.InfraSucursalesAccesorio
                .Where(a => !a.Eliminado && a.Id == id)
                .Include(a => a.Sucursal)
                .FirstOrDefaultAsync();

            if (accesorio == null)
            {
                return NotFound();
            }

            var vm = new InfraAccesorioFormViewModel
            {
                Id = accesorio.Id,
                RegionId = accesorio.Sucursal?.RegionId,
                SucursalId = accesorio.SucursalId,
                TipoAccesorioId = accesorio.TipoAccesorioId,
                Cantidad = accesorio.Cantidad,
                Especificaciones = accesorio.Especificaciones,
                Activo = accesorio.Activo
            };

            await CargarCatalogosAccesorio(vm);
            return View(vm);
        }

        // POST: Infraestructura/EditAccesorio/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAccesorio(int id, InfraAccesorioFormViewModel vm)
        {
            if (vm.Id != id)
            {
                return NotFound();
            }

            var accesorio = await _context.InfraSucursalesAccesorio
                .Where(a => !a.Eliminado && a.Id == id)
                .FirstOrDefaultAsync();

            if (accesorio == null)
            {
                return NotFound();
            }

            await ValidarAccesorio(vm);

            if (!ModelState.IsValid)
            {
                await CargarCatalogosAccesorio(vm);
                return View(vm);
            }

            accesorio.SucursalId = vm.SucursalId;
            accesorio.TipoAccesorioId = vm.TipoAccesorioId;
            accesorio.Cantidad = vm.Cantidad;
            accesorio.Especificaciones = NormalizarTexto(vm.Especificaciones);
            accesorio.Activo = vm.Activo;

            _context.InfraSucursalesAccesorio.Update(accesorio);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Accesorio actualizado exitosamente.";
            return RedirectToAction(nameof(Accesorios));
        }

        // POST: Infraestructura/DeleteAccesorio/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccesorio(int id)
        {
            var accesorio = await _context.InfraSucursalesAccesorio
                .Where(a => !a.Eliminado && a.Id == id)
                .FirstOrDefaultAsync();

            if (accesorio == null)
            {
                return Json(new { success = false, message = "Accesorio no encontrado." });
            }

            accesorio.Eliminado = true;
            accesorio.FechaEliminacion = DateTime.Now;
            accesorio.UsuarioEliminacion = User.Identity?.Name ?? "Sistema";

            _context.InfraSucursalesAccesorio.Update(accesorio);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Accesorio eliminado exitosamente." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearSistemaOperativoRapido([FromBody] QuickCreateLocationInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Nombre))
            {
                return Json(new { success = false, message = "Debe ingresar el nombre del sistema operativo." });
            }

            var nombre = input.Nombre.Trim();
            var existe = await _context.InfraSistemasOperativos
                .AnyAsync(x => !x.Eliminado && x.Nombre.ToLower() == nombre.ToLower());
            if (existe)
            {
                return Json(new { success = false, message = "Ya existe un sistema operativo con ese nombre." });
            }

            var item = new InfraSistemaOperativo { Nombre = nombre, Activo = true };
            _context.InfraSistemasOperativos.Add(item);
            await _context.SaveChangesAsync();

            return Json(new { success = true, option = new { value = item.Id.ToString(), text = item.Nombre } });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearProcesadorRapido([FromBody] QuickCreateLocationInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Nombre))
            {
                return Json(new { success = false, message = "Debe ingresar el nombre del procesador." });
            }

            var nombre = input.Nombre.Trim();
            var existe = await _context.InfraTiposProcesador
                .AnyAsync(x => !x.Eliminado && x.Nombre.ToLower() == nombre.ToLower());
            if (existe)
            {
                return Json(new { success = false, message = "Ya existe un procesador con ese nombre." });
            }

            var item = new InfraTipoProcesador { Nombre = nombre, Activo = true };
            _context.InfraTiposProcesador.Add(item);
            await _context.SaveChangesAsync();

            return Json(new { success = true, option = new { value = item.Id.ToString(), text = item.Nombre } });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearTipoRamRapido([FromBody] QuickCreateLocationInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Nombre))
            {
                return Json(new { success = false, message = "Debe ingresar el tipo de RAM." });
            }

            var nombre = input.Nombre.Trim();
            var existe = await _context.InfraTiposRam
                .AnyAsync(x => !x.Eliminado && x.Nombre.ToLower() == nombre.ToLower());
            if (existe)
            {
                return Json(new { success = false, message = "Ya existe un tipo de RAM con ese nombre." });
            }

            var item = new InfraTipoRam { Nombre = nombre, Activo = true };
            _context.InfraTiposRam.Add(item);
            await _context.SaveChangesAsync();

            return Json(new { success = true, option = new { value = item.Id.ToString(), text = item.Nombre } });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearTipoServicioRapido([FromBody] QuickCreateLocationInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Nombre))
            {
                return Json(new { success = false, message = "Debe ingresar el tipo de servicio." });
            }

            var nombre = input.Nombre.Trim();
            var existe = await _context.InfraTiposServicio
                .AnyAsync(x => !x.Eliminado && x.Nombre.ToLower() == nombre.ToLower());
            if (existe)
            {
                return Json(new { success = false, message = "Ya existe un tipo de servicio con ese nombre." });
            }

            var item = new InfraTipoServicio { Nombre = nombre, Activo = true };
            _context.InfraTiposServicio.Add(item);
            await _context.SaveChangesAsync();

            return Json(new { success = true, option = new { value = item.Id.ToString(), text = item.Nombre } });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearOperadorRapido([FromBody] QuickCreateLocationInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Nombre))
            {
                return Json(new { success = false, message = "Debe ingresar el nombre del operador." });
            }

            var nombre = input.Nombre.Trim();
            var existe = await _context.InfraOperadoresServicio
                .AnyAsync(x => !x.Eliminado && x.Nombre.ToLower() == nombre.ToLower());
            if (existe)
            {
                return Json(new { success = false, message = "Ya existe un operador con ese nombre." });
            }

            var item = new InfraOperadorServicio { Nombre = nombre, Activo = true };
            _context.InfraOperadoresServicio.Add(item);
            await _context.SaveChangesAsync();

            return Json(new { success = true, option = new { value = item.Id.ToString(), text = item.Nombre } });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearTipoAccesorioRapido([FromBody] QuickCreateLocationInput input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Nombre))
            {
                return Json(new { success = false, message = "Debe ingresar el tipo de accesorio." });
            }

            var nombre = input.Nombre.Trim();
            var existe = await _context.InfraTiposAccesorio
                .AnyAsync(x => !x.Eliminado && x.Nombre.ToLower() == nombre.ToLower());
            if (existe)
            {
                return Json(new { success = false, message = "Ya existe un tipo de accesorio con ese nombre." });
            }

            var item = new InfraTipoAccesorio { Nombre = nombre, Activo = true };
            _context.InfraTiposAccesorio.Add(item);
            await _context.SaveChangesAsync();

            return Json(new { success = true, option = new { value = item.Id.ToString(), text = item.Nombre } });
        }

        private async Task ValidarEquipo(InfraEquipoFormViewModel vm, int? equipoId = null)
        {
            if (vm.RegionId.HasValue && vm.RegionId.Value > 0 &&
                !await _context.Regiones.AnyAsync(r => !r.Eliminado && r.RegionId == vm.RegionId.Value))
            {
                ModelState.AddModelError(nameof(vm.RegionId), "La zona seleccionada no es válida.");
            }

            if (string.IsNullOrWhiteSpace(vm.SucursalId) ||
                !await _context.Sucursales.AnyAsync(s => !s.Eliminado && s.Activo && s.Id == vm.SucursalId))
            {
                ModelState.AddModelError(nameof(vm.SucursalId), "La sucursal seleccionada no es válida.");
            }
            else if (vm.RegionId.HasValue && vm.RegionId.Value > 0)
            {
                var sucursalEnRegion = await _context.Sucursales
                    .AnyAsync(s => !s.Eliminado && s.Activo && s.Id == vm.SucursalId && s.RegionId == vm.RegionId.Value);
                if (!sucursalEnRegion)
                {
                    ModelState.AddModelError(nameof(vm.SucursalId), "La sucursal seleccionada no pertenece a la zona indicada.");
                }
            }

            if (!string.IsNullOrWhiteSpace(vm.Serial))
            {
                var serialNormalizado = vm.Serial.Trim().ToLower();
                var serialExiste = await _context.InfraEquiposComputo
                    .AnyAsync(e => !e.Eliminado &&
                                   e.Serial.ToLower() == serialNormalizado &&
                                   (!equipoId.HasValue || e.Id != equipoId.Value));
                if (serialExiste)
                {
                    ModelState.AddModelError(nameof(vm.Serial), "Ya existe un equipo con ese serial.");
                }
            }

            if (vm.SistemaOperativoId.HasValue &&
                !await _context.InfraSistemasOperativos.AnyAsync(s => !s.Eliminado && s.Id == vm.SistemaOperativoId.Value))
            {
                ModelState.AddModelError(nameof(vm.SistemaOperativoId), "El sistema operativo seleccionado no es válido.");
            }

            if (vm.TipoProcesadorId.HasValue &&
                !await _context.InfraTiposProcesador.AnyAsync(p => !p.Eliminado && p.Id == vm.TipoProcesadorId.Value))
            {
                ModelState.AddModelError(nameof(vm.TipoProcesadorId), "El procesador seleccionado no es válido.");
            }

            if (vm.TipoRamId.HasValue &&
                !await _context.InfraTiposRam.AnyAsync(r => !r.Eliminado && r.Id == vm.TipoRamId.Value))
            {
                ModelState.AddModelError(nameof(vm.TipoRamId), "El tipo de RAM seleccionado no es válido.");
            }

            var ids = vm.DepartamentosSeleccionados.Where(d => d > 0).Distinct().ToList();
            if (ids.Any())
            {
                var validos = await _context.Departamentos.CountAsync(d => !d.Eliminado && ids.Contains(d.Id));
                if (validos != ids.Count)
                {
                    ModelState.AddModelError(nameof(vm.DepartamentosSeleccionados), "Uno o más departamentos no son válidos.");
                }
            }
        }

        private async Task ValidarServicio(InfraServicioFormViewModel vm)
        {
            if (vm.RegionId.HasValue && vm.RegionId.Value > 0 &&
                !await _context.Regiones.AnyAsync(r => !r.Eliminado && r.RegionId == vm.RegionId.Value))
            {
                ModelState.AddModelError(nameof(vm.RegionId), "La zona seleccionada no es válida.");
            }

            if (string.IsNullOrWhiteSpace(vm.SucursalId) ||
                !await _context.Sucursales.AnyAsync(s => !s.Eliminado && s.Activo && s.Id == vm.SucursalId))
            {
                ModelState.AddModelError(nameof(vm.SucursalId), "La sucursal seleccionada no es válida.");
            }
            else if (vm.RegionId.HasValue && vm.RegionId.Value > 0)
            {
                var sucursalEnRegion = await _context.Sucursales
                    .AnyAsync(s => !s.Eliminado && s.Activo && s.Id == vm.SucursalId && s.RegionId == vm.RegionId.Value);
                if (!sucursalEnRegion)
                {
                    ModelState.AddModelError(nameof(vm.SucursalId), "La sucursal seleccionada no pertenece a la zona indicada.");
                }
            }

            if (!await _context.InfraTiposServicio.AnyAsync(t => !t.Eliminado && t.Id == vm.TipoServicioId))
            {
                ModelState.AddModelError(nameof(vm.TipoServicioId), "El tipo de servicio seleccionado no es válido.");
            }

            if (!await _context.InfraOperadoresServicio.AnyAsync(o => !o.Eliminado && o.Id == vm.OperadorServicioId))
            {
                ModelState.AddModelError(nameof(vm.OperadorServicioId), "El operador seleccionado no es válido.");
            }
        }

        private async Task ValidarAccesorio(InfraAccesorioFormViewModel vm)
        {
            if (vm.RegionId.HasValue && vm.RegionId.Value > 0 &&
                !await _context.Regiones.AnyAsync(r => !r.Eliminado && r.RegionId == vm.RegionId.Value))
            {
                ModelState.AddModelError(nameof(vm.RegionId), "La zona seleccionada no es válida.");
            }

            if (string.IsNullOrWhiteSpace(vm.SucursalId) ||
                !await _context.Sucursales.AnyAsync(s => !s.Eliminado && s.Activo && s.Id == vm.SucursalId))
            {
                ModelState.AddModelError(nameof(vm.SucursalId), "La sucursal seleccionada no es válida.");
            }
            else if (vm.RegionId.HasValue && vm.RegionId.Value > 0)
            {
                var sucursalEnRegion = await _context.Sucursales
                    .AnyAsync(s => !s.Eliminado && s.Activo && s.Id == vm.SucursalId && s.RegionId == vm.RegionId.Value);
                if (!sucursalEnRegion)
                {
                    ModelState.AddModelError(nameof(vm.SucursalId), "La sucursal seleccionada no pertenece a la zona indicada.");
                }
            }

            if (!await _context.InfraTiposAccesorio.AnyAsync(t => !t.Eliminado && t.Id == vm.TipoAccesorioId))
            {
                ModelState.AddModelError(nameof(vm.TipoAccesorioId), "El tipo de accesorio seleccionado no es válido.");
            }
        }

        private async Task CargarCatalogosEquipo(InfraEquipoFormViewModel vm)
        {
            if (!vm.RegionId.HasValue && !string.IsNullOrWhiteSpace(vm.SucursalId))
            {
                vm.RegionId = await _context.Sucursales
                    .Where(s => !s.Eliminado && s.Activo && s.Id == vm.SucursalId)
                    .Select(s => (int?)s.RegionId)
                    .FirstOrDefaultAsync();
            }

            vm.Regiones = await ObtenerRegionesSelect();
            vm.Sucursales = await ObtenerSucursalesSelect(vm.RegionId);
            vm.SistemasOperativos = await _context.InfraSistemasOperativos
                .Where(s => !s.Eliminado)
                .OrderBy(s => s.Nombre)
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Nombre })
                .ToListAsync();
            vm.TiposProcesador = await _context.InfraTiposProcesador
                .Where(p => !p.Eliminado)
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre })
                .ToListAsync();
            vm.TiposRam = await _context.InfraTiposRam
                .Where(r => !r.Eliminado)
                .OrderBy(r => r.Nombre)
                .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Nombre })
                .ToListAsync();
            vm.Departamentos = await ObtenerDepartamentosSelect();
        }

        private async Task CargarCatalogosServicio(InfraServicioFormViewModel vm)
        {
            if (!vm.RegionId.HasValue && !string.IsNullOrWhiteSpace(vm.SucursalId))
            {
                vm.RegionId = await _context.Sucursales
                    .Where(s => !s.Eliminado && s.Activo && s.Id == vm.SucursalId)
                    .Select(s => (int?)s.RegionId)
                    .FirstOrDefaultAsync();
            }

            vm.Regiones = await ObtenerRegionesSelect();
            vm.Sucursales = await ObtenerSucursalesSelect(vm.RegionId);
            vm.TiposServicio = await _context.InfraTiposServicio
                .Where(t => !t.Eliminado)
                .OrderBy(t => t.Nombre)
                .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Nombre })
                .ToListAsync();
            vm.Operadores = await _context.InfraOperadoresServicio
                .Where(o => !o.Eliminado)
                .OrderBy(o => o.Nombre)
                .Select(o => new SelectListItem { Value = o.Id.ToString(), Text = o.Nombre })
                .ToListAsync();
        }

        private async Task CargarCatalogosAccesorio(InfraAccesorioFormViewModel vm)
        {
            if (!vm.RegionId.HasValue && !string.IsNullOrWhiteSpace(vm.SucursalId))
            {
                vm.RegionId = await _context.Sucursales
                    .Where(s => !s.Eliminado && s.Activo && s.Id == vm.SucursalId)
                    .Select(s => (int?)s.RegionId)
                    .FirstOrDefaultAsync();
            }

            vm.Regiones = await ObtenerRegionesSelect();
            vm.Sucursales = await ObtenerSucursalesSelect(vm.RegionId);
            vm.TiposAccesorio = await _context.InfraTiposAccesorio
                .Where(t => !t.Eliminado)
                .OrderBy(t => t.Nombre)
                .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Nombre })
                .ToListAsync();
        }

        private async Task ReemplazarDepartamentosEquipo(int equipoId, List<int> departamentosSeleccionados)
        {
            var existentes = await _context.InfraEquiposDepartamentos
                .Where(ed => ed.InfraEquipoComputoId == equipoId)
                .ToListAsync();
            _context.InfraEquiposDepartamentos.RemoveRange(existentes);

            var ids = departamentosSeleccionados
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            foreach (var departamentoId in ids)
            {
                _context.InfraEquiposDepartamentos.Add(new InfraEquipoDepartamento
                {
                    InfraEquipoComputoId = equipoId,
                    DepartamentoId = departamentoId
                });
            }

            await _context.SaveChangesAsync();
        }

        private async Task<List<SelectListItem>> ObtenerSucursalesSelect(int? regionId = null)
        {
            var query = _context.Sucursales
                .Where(s => !s.Eliminado && s.Activo)
                .AsQueryable();

            if (regionId.HasValue && regionId.Value > 0)
            {
                query = query.Where(s => s.RegionId == regionId.Value);
            }

            return await query
                .OrderBy(s => s.Nombre)
                .Select(s => new SelectListItem { Value = s.Id, Text = s.Nombre })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> ObtenerRegionesSelect()
        {
            return await _context.Regiones
                .Where(r => !r.Eliminado)
                .OrderBy(r => r.Nombre)
                .Select(r => new SelectListItem { Value = r.RegionId.ToString(), Text = r.Nombre })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> ObtenerDepartamentosSelect()
        {
            return await _context.Departamentos
                .Where(d => !d.Eliminado && d.Activo)
                .OrderBy(d => d.Nombre)
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nombre })
                .ToListAsync();
        }

        private static InfraEquipoListItemViewModel MapEquipoListItem(InfraEquipoComputo e)
        {
            return new InfraEquipoListItemViewModel
            {
                Id = e.Id,
                CodigoActivo = e.CodigoActivo,
                Region = e.Sucursal?.Region?.Nombre ?? "N/D",
                Sucursal = e.Sucursal?.Nombre ?? "N/D",
                NombreEquipo = e.NombreEquipo,
                Serial = e.Serial,
                Marca = e.Marca,
                Modelo = e.Modelo,
                SistemaOperativo = e.SistemaOperativo?.Nombre,
                Procesador = string.IsNullOrWhiteSpace(e.CpuDetalle)
                    ? e.TipoProcesador?.Nombre
                    : $"{e.TipoProcesador?.Nombre} {e.CpuDetalle}".Trim(),
                Ram = e.RamCantidadGb.HasValue
                    ? $"{e.RamCantidadGb} GB {(e.TipoRam?.Nombre ?? string.Empty)}".Trim()
                    : e.TipoRam?.Nombre,
                Almacenamiento = e.Almacenamiento,
                DireccionIp = e.DireccionIp,
                Observaciones = e.Observaciones,
                Departamentos = string.Join(", ", e.EquiposDepartamentos
                    .Where(ed => ed.Departamento != null && !ed.Departamento.Eliminado)
                    .Select(ed => ed.Departamento!.Nombre)
                    .OrderBy(n => n)),
                Activo = e.Activo
            };
        }

        private static InfraServicioListItemViewModel MapServicioListItem(InfraServicioSucursal s)
        {
            return new InfraServicioListItemViewModel
            {
                Id = s.Id,
                Region = s.Sucursal?.Region?.Nombre ?? "N/D",
                Sucursal = s.Sucursal?.Nombre ?? "N/D",
                TipoServicio = s.TipoServicio?.Nombre ?? "N/D",
                Operador = s.OperadorServicio?.Nombre ?? "N/D",
                NumeroServicio = s.NumeroServicio,
                VelocidadBajadaMbps = s.VelocidadBajadaMbps,
                VelocidadSubidaMbps = s.VelocidadSubidaMbps,
                Activo = s.Activo
            };
        }

        private static InfraAccesorioListItemViewModel MapAccesorioListItem(InfraSucursalAccesorio a)
        {
            return new InfraAccesorioListItemViewModel
            {
                Id = a.Id,
                Region = a.Sucursal?.Region?.Nombre ?? "N/D",
                Sucursal = a.Sucursal?.Nombre ?? "N/D",
                TipoAccesorio = a.TipoAccesorio?.Nombre ?? "N/D",
                Cantidad = a.Cantidad,
                Especificaciones = a.Especificaciones,
                Activo = a.Activo
            };
        }

        private static readonly string[] InfraChartPalette =
        {
            "#047394",
            "#ff5c00",
            "#10b981",
            "#6366f1",
            "#f59e0b",
            "#ef4444",
            "#14b8a6",
            "#64748b",
            "#a855f7"
        };

        private static object ConstruirBarChartData(string etiqueta, IEnumerable<(string Label, int Count)> items, int top)
        {
            var ordenados = items
                .OrderByDescending(i => i.Count)
                .ThenBy(i => i.Label)
                .ToList();

            if (!ordenados.Any())
            {
                return new
                {
                    labels = new[] { "Sin datos" },
                    datasets = new[]
                    {
                        new
                        {
                            label = etiqueta,
                            data = new[] { 0 },
                            backgroundColor = new[] { "#CBD5E1" },
                            borderRadius = 6
                        }
                    }
                };
            }

            if (ordenados.Count > top)
            {
                var topItems = ordenados.Take(top).ToList();
                var otros = ordenados.Skip(top).Sum(i => i.Count);
                if (otros > 0)
                {
                    topItems.Add(("Otros", otros));
                }
                ordenados = topItems;
            }

            var labels = ordenados.Select(i => i.Label).ToArray();
            var data = ordenados.Select(i => i.Count).ToArray();
            var colors = ConstruirPalette(labels.Length);

            return new
            {
                labels,
                datasets = new[]
                {
                    new
                    {
                        label = etiqueta,
                        data,
                        backgroundColor = colors,
                        borderRadius = 6
                    }
                }
            };
        }

        private static string[] ConstruirPalette(int count)
        {
            return Enumerable.Range(0, count)
                .Select(i => InfraChartPalette[i % InfraChartPalette.Length])
                .ToArray();
        }

        private static List<InfraNormalizacionGrupoViewModel> ConstruirGruposNormalizacion(IEnumerable<string?> valores, string campo)
        {
            var limpiados = valores
                .Select(NormalizarTexto)
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .ToList();

            var grupos = limpiados
                .GroupBy(v => NormalizarClave(v))
                .Select(g => new
                {
                    Clave = g.Key,
                    Variantes = g.GroupBy(v => v!)
                        .Select(vg => new InfraNormalizacionVarianteViewModel
                        {
                            Valor = vg.Key,
                            Cantidad = vg.Count()
                        })
                        .OrderByDescending(v => v.Cantidad)
                        .ThenBy(v => v.Valor)
                        .ToList()
                })
                .Where(g => g.Variantes.Count > 1)
                .OrderByDescending(g => g.Variantes.Sum(v => v.Cantidad))
                .ThenBy(g => g.Variantes[0].Valor)
                .Select(g => new InfraNormalizacionGrupoViewModel
                {
                    Clave = g.Clave,
                    Variantes = g.Variantes,
                    Sugerencia = g.Variantes.First().Valor,
                    Total = g.Variantes.Sum(v => v.Cantidad)
                })
                .ToList();

            return grupos;
        }

        private static string? ObtenerValorCampoSync(InfraEquipoComputo equipo, string campoKey)
        {
            return campoKey switch
            {
                "almacenamiento" => equipo.Almacenamiento,
                "cpu" => equipo.CpuDetalle,
                "marca" => equipo.Marca,
                "tipo-ram" => equipo.TipoRam?.Nombre,
                _ => null
            };
        }

        private static void AsignarValorCampoSync(InfraEquipoComputo equipo, string campoKey, string? valor)
        {
            switch (campoKey)
            {
                case "almacenamiento":
                    equipo.Almacenamiento = valor;
                    break;
                case "cpu":
                    equipo.CpuDetalle = valor;
                    break;
                case "marca":
                    equipo.Marca = valor;
                    break;
            }
        }

        private static string NombreCampo(string campoKey)
        {
            return campoKey switch
            {
                "almacenamiento" => "Almacenamiento",
                "cpu" => "CPU (Detalle)",
                "marca" => "Marca",
                "tipo-ram" => "Tipo de RAM",
                _ => campoKey
            };
        }

        private static string NormalizarClave(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return string.Empty;
            }

            var upper = valor.Trim().ToUpperInvariant();
            return Regex.Replace(upper, @"[^A-Z0-9]+", "");
        }

        private static string NormalizarCodigoSucursal(string codigo)
        {
            if (int.TryParse(codigo, out var numero) && numero > 0)
            {
                return $"SUC-{numero:D3}";
            }

            var limpio = codigo.ToUpperInvariant().Trim();
            if (limpio.StartsWith("SUC-"))
            {
                return limpio;
            }

            return limpio;
        }

        private static string? NormalizarTexto(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
        }
    }
}
