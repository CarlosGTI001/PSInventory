using System.ComponentModel.DataAnnotations;

namespace PSData.Modelos;

public class InfraTipoProcesador
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(120, ErrorMessage = "El nombre no puede exceder 120 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    // Soft Delete
    public bool Eliminado { get; set; } = false;
    public DateTime? FechaEliminacion { get; set; }

    [StringLength(100)]
    public string? UsuarioEliminacion { get; set; }

    public ICollection<InfraEquipoComputo> EquiposComputo { get; set; } = new List<InfraEquipoComputo>();
}

