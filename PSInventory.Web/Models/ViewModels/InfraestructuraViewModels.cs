using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PSInventory.Web.Models.ViewModels;

public class InfraestructuraIndexViewModel
{
    public string Query { get; set; } = string.Empty;
    public int? RegionFiltro { get; set; }
    public string? SucursalFiltro { get; set; }
    public int? DepartamentoFiltro { get; set; }
    public string ViewLayout { get; set; } = "vertical";

    public int TotalEquipos { get; set; }
    public int EquiposActivos { get; set; }
    public int TotalServicios { get; set; }
    public int TotalAccesorios { get; set; }

    public IEnumerable<SelectListItem> Regiones { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Sucursales { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Departamentos { get; set; } = new List<SelectListItem>();

    public List<InfraEquipoListItemViewModel> Equipos { get; set; } = new();
    public List<InfraServicioListItemViewModel> Servicios { get; set; } = new();
    public List<InfraAccesorioListItemViewModel> Accesorios { get; set; } = new();
}

public class InfraSucursalResumenViewModel
{
    public string CodigoSucursal { get; set; } = string.Empty;
    public string? Mensaje { get; set; }
    public string ViewLayout { get; set; } = "vertical";

    public IEnumerable<SelectListItem> Sucursales { get; set; } = new List<SelectListItem>();
    public InfraSucursalInfoViewModel? Sucursal { get; set; }

    public int TotalEquipos { get; set; }
    public int EquiposActivos { get; set; }
    public int TotalServicios { get; set; }
    public int TotalAccesorios { get; set; }

    public List<string> DepartamentosRelacionados { get; set; } = new();
    public List<InfraEquipoListItemViewModel> Equipos { get; set; } = new();
    public List<InfraServicioListItemViewModel> Servicios { get; set; } = new();
    public List<InfraAccesorioListItemViewModel> Accesorios { get; set; } = new();
}

public class InfraSucursalInfoViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
}

public class InfraEquipoListItemViewModel
{
    public int Id { get; set; }
    public string? CodigoActivo { get; set; }
    public string Region { get; set; } = string.Empty;
    public string Sucursal { get; set; } = string.Empty;
    public string NombreEquipo { get; set; } = string.Empty;
    public string Serial { get; set; } = string.Empty;
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public string? SistemaOperativo { get; set; }
    public string? Procesador { get; set; }
    public string? Ram { get; set; }
    public string? Almacenamiento { get; set; }
    public string? DireccionIp { get; set; }
    public string? Observaciones { get; set; }
    public string Departamentos { get; set; } = string.Empty;
    public bool Activo { get; set; }
}

public class InfraServicioListItemViewModel
{
    public int Id { get; set; }
    public string Region { get; set; } = string.Empty;
    public string Sucursal { get; set; } = string.Empty;
    public string TipoServicio { get; set; } = string.Empty;
    public string Operador { get; set; } = string.Empty;
    public string? NumeroServicio { get; set; }
    public decimal? VelocidadBajadaMbps { get; set; }
    public decimal? VelocidadSubidaMbps { get; set; }
    public bool Activo { get; set; }
}

public class InfraAccesorioListItemViewModel
{
    public int Id { get; set; }
    public string Region { get; set; } = string.Empty;
    public string Sucursal { get; set; } = string.Empty;
    public string TipoAccesorio { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public string? Especificaciones { get; set; }
    public bool Activo { get; set; }
}

public class InfraEquipoFormViewModel
{
    public int? Id { get; set; }

    public int? RegionId { get; set; }

    [StringLength(50, ErrorMessage = "El código no puede exceder 50 caracteres")]
    public string? CodigoActivo { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una sucursal")]
    public string SucursalId { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del equipo es obligatorio")]
    [StringLength(120, ErrorMessage = "El nombre no puede exceder 120 caracteres")]
    public string NombreEquipo { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "La marca no puede exceder 100 caracteres")]
    public string? Marca { get; set; }

    [StringLength(100, ErrorMessage = "El modelo no puede exceder 100 caracteres")]
    public string? Modelo { get; set; }

    [Required(ErrorMessage = "El serial es obligatorio")]
    [StringLength(120, ErrorMessage = "El serial no puede exceder 120 caracteres")]
    public string Serial { get; set; } = string.Empty;

    public int? SistemaOperativoId { get; set; }
    public int? TipoProcesadorId { get; set; }

    [StringLength(120, ErrorMessage = "El detalle de CPU no puede exceder 120 caracteres")]
    public string? CpuDetalle { get; set; }

    public int? TipoRamId { get; set; }

    [Range(1, 4096, ErrorMessage = "La RAM debe estar entre 1 y 4096 GB")]
    public int? RamCantidadGb { get; set; }

    [StringLength(120, ErrorMessage = "El almacenamiento no puede exceder 120 caracteres")]
    public string? Almacenamiento { get; set; }

    [StringLength(100, ErrorMessage = "La dirección IP no puede exceder 100 caracteres")]
    public string? DireccionIp { get; set; }

    [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
    public string? Observaciones { get; set; }

    public bool Activo { get; set; } = true;

    public List<int> DepartamentosSeleccionados { get; set; } = new();

    public IEnumerable<SelectListItem> Regiones { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Sucursales { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> SistemasOperativos { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> TiposProcesador { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> TiposRam { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Departamentos { get; set; } = new List<SelectListItem>();
}

public class InfraServicioFormViewModel
{
    public int? Id { get; set; }

    public int? RegionId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una sucursal")]
    public string SucursalId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar un tipo de servicio")]
    public int TipoServicioId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un operador")]
    public int OperadorServicioId { get; set; }

    [StringLength(80, ErrorMessage = "El número de servicio no puede exceder 80 caracteres")]
    public string? NumeroServicio { get; set; }

    [Range(0, 100000, ErrorMessage = "La velocidad de bajada no es válida")]
    public decimal? VelocidadBajadaMbps { get; set; }

    [Range(0, 100000, ErrorMessage = "La velocidad de subida no es válida")]
    public decimal? VelocidadSubidaMbps { get; set; }

    [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
    public string? Observaciones { get; set; }

    public bool Activo { get; set; } = true;

    public IEnumerable<SelectListItem> Regiones { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Sucursales { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> TiposServicio { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Operadores { get; set; } = new List<SelectListItem>();
}

public class InfraAccesorioFormViewModel
{
    public int? Id { get; set; }

    public int? RegionId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una sucursal")]
    public string SucursalId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar un tipo de accesorio")]
    public int TipoAccesorioId { get; set; }

    [Range(1, 10000, ErrorMessage = "La cantidad debe ser mayor que cero")]
    public int Cantidad { get; set; } = 1;

    [StringLength(500, ErrorMessage = "Las especificaciones no pueden exceder 500 caracteres")]
    public string? Especificaciones { get; set; }

    public bool Activo { get; set; } = true;

    public IEnumerable<SelectListItem> Regiones { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Sucursales { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> TiposAccesorio { get; set; } = new List<SelectListItem>();
}

public class InfraNormalizacionViewModel
{
    public List<InfraNormalizacionGrupoViewModel> GruposAlmacenamiento { get; set; } = new();
    public List<InfraNormalizacionGrupoViewModel> GruposCpu { get; set; } = new();
    public List<InfraNormalizacionGrupoViewModel> GruposTipoRam { get; set; } = new();
    public List<InfraNormalizacionGrupoViewModel> GruposMarca { get; set; } = new();
}

public class InfraNormalizacionGrupoViewModel
{
    public string Clave { get; set; } = string.Empty;
    public string Sugerencia { get; set; } = string.Empty;
    public int Total { get; set; }
    public List<InfraNormalizacionVarianteViewModel> Variantes { get; set; } = new();
}

public class InfraNormalizacionVarianteViewModel
{
    public string Valor { get; set; } = string.Empty;
    public int Cantidad { get; set; }
}

public class InfraReporteGraficoExportViewModel
{
    public string? AnalisisAi { get; set; }
    public List<InfraChartExportItem> Graficos { get; set; } = new();
}

public class InfraChartExportItem
{
    public string Titulo { get; set; } = string.Empty;
    public string Base64Image { get; set; } = string.Empty;
    public string? RawData { get; set; }
}
