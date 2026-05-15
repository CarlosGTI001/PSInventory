using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSData.Modelos;

public class InfraEquipoComputo
{
    public int Id { get; set; }

    [StringLength(50, ErrorMessage = "El código no puede exceder 50 caracteres")]
    public string? CodigoActivo { get; set; }

    [Required(ErrorMessage = "La sucursal es obligatoria")]
    [StringLength(50)]
    public string SucursalId { get; set; } = string.Empty;

    [ForeignKey(nameof(SucursalId))]
    public virtual Sucursal? Sucursal { get; set; }

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

    [ForeignKey(nameof(SistemaOperativoId))]
    public virtual InfraSistemaOperativo? SistemaOperativo { get; set; }

    public int? TipoProcesadorId { get; set; }

    [ForeignKey(nameof(TipoProcesadorId))]
    public virtual InfraTipoProcesador? TipoProcesador { get; set; }

    [StringLength(120, ErrorMessage = "El detalle de CPU no puede exceder 120 caracteres")]
    public string? CpuDetalle { get; set; }

    public int? TipoRamId { get; set; }

    [ForeignKey(nameof(TipoRamId))]
    public virtual InfraTipoRam? TipoRam { get; set; }

    [Range(1, 4096, ErrorMessage = "La RAM debe estar entre 1 y 4096 GB")]
    public int? RamCantidadGb { get; set; }

    [StringLength(120, ErrorMessage = "El almacenamiento no puede exceder 120 caracteres")]
    public string? Almacenamiento { get; set; }

    [StringLength(100, ErrorMessage = "La IP no puede exceder 100 caracteres")]
    public string? DireccionIp { get; set; }

    public bool Activo { get; set; } = true;

    [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
    public string? Observaciones { get; set; }

    // Soft Delete
    public bool Eliminado { get; set; } = false;
    public DateTime? FechaEliminacion { get; set; }

    [StringLength(100)]
    public string? UsuarioEliminacion { get; set; }

    public ICollection<InfraEquipoDepartamento> EquiposDepartamentos { get; set; } = new List<InfraEquipoDepartamento>();
}

