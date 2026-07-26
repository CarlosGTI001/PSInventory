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
        private static readonly string[] InfraChartPalette = new[]
        {
            "#047394", "#0D9488", "#2563EB", "#7C3AED", "#DB2777",
            "#EA580C", "#D97706", "#65A30D", "#059669", "#4F46E5"
        };

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

        // GET: Infraestructura/Sucursal
        public async Task<IActionResult> Sucursal(string? codigoSucursal, string layout = "vertical")
        {
            var sucursalesSelect = await ObtenerSucursalesSelect();
            var vm = new InfraSucursalResumenViewModel
            {
                CodigoSucursal = codigoSucursal ?? string.Empty,
                Sucursales = sucursalesSelect,
                ViewLayout = layout
            };

            if (string.IsNullOrWhiteSpace(codigoSucursal))
            {
                return View(vm);
            }

            var sucursal = await BuscarSucursalPorCodigoONombre(codigoSucursal);
            if (sucursal == null)
            {
                vm.Mensaje = $"No se encontró ninguna sucursal con el código o nombre '{codigoSucursal}'.";
                return View(vm);
            }

            vm.CodigoSucursal = sucursal.Id;
            vm.Sucursal = new InfraSucursalInfoViewModel
            {
                Id = sucursal.Id,
                Nombre = sucursal.Nombre,
                Region = sucursal.Region?.Nombre ?? "Sin Asignar",
                Direccion = sucursal.Direccion,
                Telefono = sucursal.Telefono
            };

            // Cargar Equipos
            var equipos = await _context.InfraEquiposComputo
                .Where(e => !e.Eliminado && e.SucursalId == sucursal.Id)
                .Include(e => e.Sucursal).ThenInclude(s => s.Region)
                .Include(e => e.SistemaOperativo)
                .Include(e => e.TipoProcesador)
                .Include(e => e.TipoRam)
                .Include(e => e.EquiposDepartamentos).ThenInclude(ed => ed.Departamento)
                .OrderBy(e => e.NombreEquipo)
                .ToListAsync();

            // Cargar Servicios
            var servicios = await _context.InfraServiciosSucursal
                .Where(s => !s.Eliminado && s.SucursalId == sucursal.Id)
                .Include(s => s.Sucursal).ThenInclude(s => s.Region)
                .Include(s => s.TipoServicio)
                .Include(s => s.OperadorServicio)
                .OrderBy(s => s.TipoServicio.Nombre)
                .ToListAsync();

            // Cargar Accesorios
            var accesorios = await _context.InfraSucursalesAccesorio
                .Where(a => !a.Eliminado && a.SucursalId == sucursal.Id)
                .Include(a => a.Sucursal).ThenInclude(s => s.Region)
                .Include(a => a.TipoAccesorio)
                .OrderBy(a => a.TipoAccesorio.Nombre)
                .ToListAsync();

            // Cargar Artículos de Inventario
            var items = await _context.Items
                .Where(i => !i.Eliminado && i.SucursalId == sucursal.Id)
                .Include(i => i.Articulo).ThenInclude(a => a.Categoria)
                .OrderByDescending(i => i.FechaAsignacion)
                .ToListAsync();

            var articulosVm = items.Select(i => new InfraArticuloListItemViewModel
            {
                ItemId = i.Id,
                ArticuloId = i.ArticuloId,
                Marca = i.Articulo?.Marca ?? "N/D",
                Modelo = i.Articulo?.Modelo ?? "N/D",
                Categoria = i.Articulo?.Categoria?.Nombre ?? "Sin Categoría",
                Serial = i.Serial,
                Cantidad = i.Cantidad,
                Estado = i.Estado,
                Responsable = i.ResponsableEmpleado,
                FechaAsignacion = i.FechaAsignacion,
                Observaciones = i.Observaciones
            }).ToList();

            var depts = equipos
                .SelectMany(e => e.EquiposDepartamentos.Select(ed => ed.Departamento?.Nombre))
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .Select(n => n!)
                .ToList();

            vm.TotalEquipos = equipos.Count;
            vm.EquiposActivos = equipos.Count(e => e.Activo);
            vm.TotalServicios = servicios.Count;
            vm.TotalAccesorios = accesorios.Sum(a => a.Cantidad);
            vm.TotalArticulos = articulosVm.Sum(a => a.Cantidad);
            vm.DepartamentosRelacionados = depts;

            vm.Equipos = equipos.Select(MapEquipoListItem).ToList();
            vm.Servicios = servicios.Select(MapServicioListItem).ToList();
            vm.Accesorios = accesorios.Select(MapAccesorioListItem).ToList();
            vm.Articulos = articulosVm;

            return View(vm);
        }

        // GET: Infraestructura/ExportarSucursalPdf
        public async Task<IActionResult> ExportarSucursalPdf(string codigoSucursal, string layout = "vertical")
        {
            if (string.IsNullOrWhiteSpace(codigoSucursal))
            {
                return RedirectToAction(nameof(Sucursal));
            }

            var sucursal = await BuscarSucursalPorCodigoONombre(codigoSucursal);
            if (sucursal == null)
            {
                TempData["Error"] = $"No se encontró la sucursal '{codigoSucursal}'.";
                return RedirectToAction(nameof(Sucursal));
            }

            var vm = new InfraSucursalResumenViewModel
            {
                CodigoSucursal = sucursal.Id,
                ViewLayout = layout,
                Sucursal = new InfraSucursalInfoViewModel
                {
                    Id = sucursal.Id,
                    Nombre = sucursal.Nombre,
                    Region = sucursal.Region?.Nombre ?? "Sin Asignar",
                    Direccion = sucursal.Direccion,
                    Telefono = sucursal.Telefono
                }
            };

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
                .Include(s => s.Sucursal).ThenInclude(s => s.Region)
                .Include(s => s.TipoServicio)
                .Include(s => s.OperadorServicio)
                .OrderBy(s => s.TipoServicio.Nombre)
                .ToListAsync();

            var accesorios = await _context.InfraSucursalesAccesorio
                .Where(a => !a.Eliminado && a.SucursalId == sucursal.Id)
                .Include(a => a.Sucursal).ThenInclude(s => s.Region)
                .Include(a => a.TipoAccesorio)
                .OrderBy(a => a.TipoAccesorio.Nombre)
                .ToListAsync();

            var items = await _context.Items
                .Where(i => !i.Eliminado && i.SucursalId == sucursal.Id)
                .Include(i => i.Articulo).ThenInclude(a => a.Categoria)
                .OrderByDescending(i => i.FechaAsignacion)
                .ToListAsync();

            var articulosVm = items.Select(i => new InfraArticuloListItemViewModel
            {
                ItemId = i.Id,
                ArticuloId = i.ArticuloId,
                Marca = i.Articulo?.Marca ?? "N/D",
                Modelo = i.Articulo?.Modelo ?? "N/D",
                Categoria = i.Articulo?.Categoria?.Nombre ?? "Sin Categoría",
                Serial = i.Serial,
                Cantidad = i.Cantidad,
                Estado = i.Estado,
                Responsable = i.ResponsableEmpleado,
                FechaAsignacion = i.FechaAsignacion,
                Observaciones = i.Observaciones
            }).ToList();

            var depts = equipos
                .SelectMany(e => e.EquiposDepartamentos.Select(ed => ed.Departamento?.Nombre))
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .Select(n => n!)
                .ToList();

            vm.TotalEquipos = equipos.Count;
            vm.EquiposActivos = equipos.Count(e => e.Activo);
            vm.TotalServicios = servicios.Count;
            vm.TotalAccesorios = accesorios.Sum(a => a.Cantidad);
            vm.TotalArticulos = articulosVm.Sum(a => a.Cantidad);
            vm.DepartamentosRelacionados = depts;

            vm.Equipos = equipos.Select(MapEquipoListItem).ToList();
            vm.Servicios = servicios.Select(MapServicioListItem).ToList();
            vm.Accesorios = accesorios.Select(MapAccesorioListItem).ToList();
            vm.Articulos = articulosVm;

            var usuario = User.Identity?.Name ?? "Sistema";
            var pdfBytes = PdfReportService.GenerarPdfSucursalInfraestructura(usuario, vm);
            return File(pdfBytes, "application/pdf", $"Infraestructura_{sucursal.Id}.pdf");
        }

        private async Task<Sucursal?> BuscarSucursalPorCodigoONombre(string codigoSucursal)
        {
            var term = codigoSucursal.Trim().ToLower();
            var posiblesIds = new List<string> { term };

            if (int.TryParse(term, out int num))
            {
                posiblesIds.Add($"suc-{num:D3}");
                posiblesIds.Add($"suc-{num:D2}");
                posiblesIds.Add($"suc-{num}");
            }
            if (term.StartsWith("suc-"))
            {
                var rawNumStr = term.Replace("suc-", "");
                if (int.TryParse(rawNumStr, out int parsedNum))
                {
                    posiblesIds.Add($"suc-{parsedNum:D3}");
                    posiblesIds.Add($"suc-{parsedNum:D2}");
                    posiblesIds.Add($"suc-{parsedNum}");
                }
            }

            return await _context.Sucursales
                .Include(s => s.Region)
                .Where(s => !s.Eliminado)
                .FirstOrDefaultAsync(s => posiblesIds.Contains(s.Id.ToLower()) || s.Nombre.ToLower().Contains(term));
        }

        // GET: Infraestructura/Normalizacion
        public async Task<IActionResult> Normalizacion()
        {
            var equipos = await _context.InfraEquiposComputo
                .Where(e => !e.Eliminado)
                .Include(e => e.TipoRam)
                .AsNoTracking()
                .ToListAsync();

            var procesadores = await _context.InfraTiposProcesador.Where(p => !p.Eliminado).AsNoTracking().ToListAsync();
            var sistemasOp = await _context.InfraSistemasOperativos.Where(s => !s.Eliminado).AsNoTracking().ToListAsync();

            var vm = new InfraNormalizacionViewModel
            {
                GruposAlmacenamiento = ConstruirGruposNormalizacion(equipos.Select(e => e.Almacenamiento)),
                GruposCpu = ConstruirGruposNormalizacion(equipos.Select(e => e.CpuDetalle)),
                GruposTipoRam = ConstruirGruposNormalizacion(equipos.Select(e => e.TipoRam?.Nombre)),
                GruposMarca = ConstruirGruposNormalizacion(equipos.Select(e => e.Marca)),
                GruposProcesadorMaestro = ConstruirGruposNormalizacionEntidades(procesadores.Select(p => (p.Id, p.Nombre))),
                GruposOsMaestro = ConstruirGruposNormalizacionEntidades(sistemasOp.Select(s => (s.Id, s.Nombre)))
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
            if (string.IsNullOrWhiteSpace(valorNormalizado) && campo != "cpu")
            {
                TempData["Error"] = "Debe indicar el nuevo valor normalizado.";
                return RedirectToAction(nameof(Normalizacion));
            }

            var campoKey = campo.Trim().ToLowerInvariant();

            // CASO 1: Normalización de registros Maestros (Selects)
            if (campoKey == "maestro-procesador" || campoKey == "maestro-os")
            {
                return await NormalizarRegistrosMaestros(campoKey, clave, valorNormalizado!);
            }

            // CASO 2: Normalización de campos de texto en Equipos
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

        private async Task<IActionResult> NormalizarRegistrosMaestros(string campoKey, string clave, string nuevoValor)
        {
            if (campoKey == "maestro-procesador")
            {
                var todos = await _context.InfraTiposProcesador.Where(p => !p.Eliminado).ToListAsync();
                var variantes = todos.Where(p => NormalizarClave(p.Nombre) == clave).ToList();
                
                // 1. Asegurar que existe el registro "maestro" correcto
                var maestro = variantes.FirstOrDefault(v => v.Nombre.Trim().ToLower() == nuevoValor.Trim().ToLower());
                if (maestro == null)
                {
                    maestro = new InfraTipoProcesador { Nombre = nuevoValor.Trim(), Activo = true };
                    _context.InfraTiposProcesador.Add(maestro);
                    await _context.SaveChangesAsync();
                }

                // 2. Reasignar todos los equipos de las variantes al maestro
                var idsVariantes = variantes.Select(v => v.Id).ToList();
                var equiposAfectados = await _context.InfraEquiposComputo
                    .Where(e => e.TipoProcesadorId.HasValue && idsVariantes.Contains(e.TipoProcesadorId.Value))
                    .ToListAsync();

                foreach (var e in equiposAfectados) e.TipoProcesadorId = maestro.Id;

                // 3. Eliminar las variantes antiguas (excepto el maestro)
                foreach (var v in variantes.Where(v => v.Id != maestro.Id)) v.Eliminado = true;

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Se han unificado {variantes.Count} procesadores bajo '{nuevoValor}'.";
            }
            else if (campoKey == "maestro-os")
            {
                var todos = await _context.InfraSistemasOperativos.Where(s => !s.Eliminado).ToListAsync();
                var variantes = todos.Where(s => NormalizarClave(s.Nombre) == clave).ToList();
                
                var maestro = variantes.FirstOrDefault(v => v.Nombre.Trim().ToLower() == nuevoValor.Trim().ToLower());
                if (maestro == null)
                {
                    maestro = new InfraSistemaOperativo { Nombre = nuevoValor.Trim(), Activo = true };
                    _context.InfraSistemasOperativos.Add(maestro);
                    await _context.SaveChangesAsync();
                }

                var idsVariantes = variantes.Select(v => v.Id).ToList();
                var equiposAfectados = await _context.InfraEquiposComputo
                    .Where(e => e.SistemaOperativoId.HasValue && idsVariantes.Contains(e.SistemaOperativoId.Value))
                    .ToListAsync();

                foreach (var e in equiposAfectados) e.SistemaOperativoId = maestro.Id;
                foreach (var v in variantes.Where(v => v.Id != maestro.Id)) v.Eliminado = true;

                await _context.SaveChangesAsync();
                TempData["Success"] = $"Se han unificado {variantes.Count} sistemas operativos bajo '{nuevoValor}'.";
            }

            return RedirectToAction(nameof(Normalizacion));
        }

        private static List<InfraNormalizacionGrupoViewModel> ConstruirGruposNormalizacion(IEnumerable<string?> valores)
        {
            var limpiados = valores
                .Select(v => string.IsNullOrWhiteSpace(v) ? null : v.Trim())
                .Where(v => v != null)
                .ToList();

            return limpiados
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
                        .ToList()
                })
                .Where(g => g.Variantes.Count > 1)
                .Select(g => new InfraNormalizacionGrupoViewModel
                {
                    Clave = g.Clave,
                    Variantes = g.Variantes,
                    Sugerencia = g.Variantes.First().Valor,
                    Total = g.Variantes.Sum(v => v.Cantidad)
                })
                .OrderByDescending(g => g.Total)
                .ToList();
        }

        private static List<InfraNormalizacionGrupoViewModel> ConstruirGruposNormalizacionEntidades(IEnumerable<(int Id, string Nombre)> items)
        {
            return items
                .GroupBy(i => NormalizarClave(i.Nombre))
                .Select(g => new
                {
                    Clave = g.Key,
                    Variantes = g.GroupBy(v => v.Nombre.Trim())
                        .Select(vg => new InfraNormalizacionVarianteViewModel
                        {
                            Valor = vg.Key,
                            Cantidad = vg.Count(),
                            OriginalIds = vg.Select(x => x.Id).ToList()
                        })
                        .OrderByDescending(v => v.Valor.Length)
                        .ToList()
                })
                .Where(g => g.Variantes.Count > 1)
                .Select(g => new InfraNormalizacionGrupoViewModel
                {
                    Clave = g.Clave,
                    Variantes = g.Variantes,
                    Sugerencia = g.Variantes.First().Valor,
                    Total = g.Variantes.Count
                })
                .OrderByDescending(g => g.Total)
                .ToList();
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

        // --- MÉTODOS DE SOPORTE PARA REPORTES ---

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

        private async Task<string> GenerarPromptInfraestructura()
        {
            var totalEquipos = await _context.InfraEquiposComputo.CountAsync(e => !e.Eliminado);
            var totalServicios = await _context.InfraServiciosSucursal.CountAsync(s => !s.Eliminado);
            var totalAccesorios = await _context.InfraSucursalesAccesorio.CountAsync(a => !a.Eliminado);

            var equipos = await _context.InfraEquiposComputo
                .Where(e => !e.Eliminado)
                .Include(e => e.Sucursal).ThenInclude(s => s.Region)
                .Include(e => e.SistemaOperativo)
                .AsNoTracking()
                .ToListAsync();

            var distZonas = equipos
                .GroupBy(e => e.Sucursal?.Region?.Nombre ?? "N/D")
                .Select(g => $"{g.Key}: {g.Count()}")
                .ToList();

            var distOS = equipos
                .GroupBy(e => e.SistemaOperativo?.Nombre ?? "N/D")
                .Select(g => $"{g.Key}: {g.Count()}")
                .ToList();

            var distRam = equipos
                .GroupBy(e => e.RamCantidadGb.HasValue ? $"{e.RamCantidadGb} GB" : "N/D")
                .Select(g => $"{g.Key}: {g.Count()}")
                .ToList();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Resumen General: {totalEquipos} equipos, {totalServicios} servicios de red, {totalAccesorios} accesorios.");
            sb.AppendLine($"Distribución por Zonas: {string.Join(", ", distZonas)}");
            sb.AppendLine($"Sistemas Operativos: {string.Join(", ", distOS)}");
            sb.AppendLine($"Capacidad de RAM: {string.Join(", ", distRam)}");

            return sb.ToString();
        }

        private static string[] ConstruirPalette(int count)
        {
            return Enumerable.Range(0, count)
                .Select(i => InfraChartPalette[i % InfraChartPalette.Length])
                .ToArray();
        }

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

        private async Task<List<SelectListItem>> ObtenerRegionesSelect()
        {
            return await _context.Regiones
                .Where(r => !r.Eliminado)
                .OrderBy(r => r.Nombre)
                .Select(r => new SelectListItem { Value = r.RegionId.ToString(), Text = r.Nombre })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> ObtenerSucursalesSelect(int? regionId = null)
        {
            var q = _context.Sucursales.Where(s => !s.Eliminado);
            if (regionId.HasValue && regionId.Value > 0)
                q = q.Where(s => s.RegionId == regionId.Value);
            return await q.OrderBy(s => s.Nombre)
                .Select(s => new SelectListItem { Value = s.Id, Text = s.Nombre })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> ObtenerDepartamentosSelect()
        {
            return await _context.Departamentos
                .Where(d => !d.Eliminado)
                .OrderBy(d => d.Nombre)
                .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Nombre })
                .ToListAsync();
        }

        private static InfraEquipoListItemViewModel MapEquipoListItem(InfraEquipoComputo e)
        {
            return new InfraEquipoListItemViewModel
            {
                Id = e.Id,
                NombreEquipo = e.NombreEquipo,
                CodigoActivo = e.CodigoActivo,
                Serial = e.Serial,
                Marca = e.Marca,
                Modelo = e.Modelo,
                Sucursal = e.Sucursal?.Nombre ?? "Sin Asignar",
                Region = e.Sucursal?.Region?.Nombre ?? "Sin Asignar",
                SistemaOperativo = e.SistemaOperativo?.Nombre ?? "N/D",
                Ram = e.RamCantidadGb.HasValue ? $"{e.RamCantidadGb} GB" : "N/D",
                Procesador = e.TipoProcesador?.Nombre ?? "N/D",
                DireccionIp = e.DireccionIp ?? "N/D",
                Activo = e.Activo
            };
        }

        private static InfraServicioListItemViewModel MapServicioListItem(InfraServicioSucursal s)
        {
            return new InfraServicioListItemViewModel
            {
                Id = s.Id,
                TipoServicio = s.TipoServicio?.Nombre ?? "N/D",
                Operador = s.OperadorServicio?.Nombre ?? "N/D",
                Sucursal = s.Sucursal?.Nombre ?? "Sin Asignar",
                Region = s.Sucursal?.Region?.Nombre ?? "Sin Asignar",
                NumeroServicio = s.NumeroServicio ?? "N/D",
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
                TipoAccesorio = a.TipoAccesorio?.Nombre ?? "N/D",
                Cantidad = a.Cantidad,
                Sucursal = a.Sucursal?.Nombre ?? "Sin Asignar",
                Region = a.Sucursal?.Region?.Nombre ?? "Sin Asignar",
                Especificaciones = a.Especificaciones,
                Activo = a.Activo
            };
        }
    }
}
