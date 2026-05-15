using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSData.Modelos;

public class InfraSucursalAccesorio
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La sucursal es obligatoria")]
    [StringLength(50)]
    public string SucursalId { get; set; } = string.Empty;

    [ForeignKey(nameof(SucursalId))]
    public virtual Sucursal? Sucursal { get; set; }

    [Required(ErrorMessage = "El tipo de accesorio es obligatorio")]
    public int TipoAccesorioId { get; set; }

    [ForeignKey(nameof(TipoAccesorioId))]
    public virtual InfraTipoAccesorio? TipoAccesorio { get; set; }

    [Range(1, 10000, ErrorMessage = "La cantidad debe ser mayor que cero")]
    public int Cantidad { get; set; } = 1;

    [StringLength(500, ErrorMessage = "Las especificaciones no pueden exceder 500 caracteres")]
    public string? Especificaciones { get; set; }

    public bool Activo { get; set; } = true;

    // Soft Delete
    public bool Eliminado { get; set; } = false;
    public DateTime? FechaEliminacion { get; set; }

    [StringLength(100)]
    public string? UsuarioEliminacion { get; set; }
}
