using System.ComponentModel.DataAnnotations;

namespace PSData.Modelos;

public class Departamento
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Descripcion { get; set; }

    [StringLength(100, ErrorMessage = "El nombre del responsable no puede exceder 100 caracteres")]
    public string? Responsable { get; set; }

    public bool Activo { get; set; } = true;

    // Soft Delete
    public bool Eliminado { get; set; } = false;
    public DateTime? FechaEliminacion { get; set; }
    [StringLength(100)]
    public string? UsuarioEliminacion { get; set; }

    // Relaciones
    public ICollection<Compra> Compras { get; set; } = new List<Compra>();
}
