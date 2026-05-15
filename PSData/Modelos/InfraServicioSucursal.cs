using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSData.Modelos;

public class InfraServicioSucursal
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La sucursal es obligatoria")]
    [StringLength(50)]
    public string SucursalId { get; set; } = string.Empty;

    [ForeignKey(nameof(SucursalId))]
    public virtual Sucursal? Sucursal { get; set; }

    [Required(ErrorMessage = "El tipo de servicio es obligatorio")]
    public int TipoServicioId { get; set; }

    [ForeignKey(nameof(TipoServicioId))]
    public virtual InfraTipoServicio? TipoServicio { get; set; }

    [Required(ErrorMessage = "El operador es obligatorio")]
    public int OperadorServicioId { get; set; }

    [ForeignKey(nameof(OperadorServicioId))]
    public virtual InfraOperadorServicio? OperadorServicio { get; set; }

    [StringLength(80, ErrorMessage = "El número de servicio no puede exceder 80 caracteres")]
    public string? NumeroServicio { get; set; }

    [Range(0, 100000, ErrorMessage = "La velocidad de bajada debe ser válida")]
    public decimal? VelocidadBajadaMbps { get; set; }

    [Range(0, 100000, ErrorMessage = "La velocidad de subida debe ser válida")]
    public decimal? VelocidadSubidaMbps { get; set; }

    public bool Activo { get; set; } = true;

    [StringLength(1000, ErrorMessage = "Las observaciones no pueden exceder 1000 caracteres")]
    public string? Observaciones { get; set; }

    // Soft Delete
    public bool Eliminado { get; set; } = false;
    public DateTime? FechaEliminacion { get; set; }

    [StringLength(100)]
    public string? UsuarioEliminacion { get; set; }
}

