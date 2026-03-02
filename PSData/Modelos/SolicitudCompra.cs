using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSData.Modelos
{
    public class SolicitudCompra
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Solicitante { get; set; } = string.Empty;

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        /// <summary>Pendiente | En Revisión | Aprobada | Rechazada</summary>
        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "Pendiente";

        [StringLength(500)]
        public string? Observaciones { get; set; }

        [StringLength(100)]
        public string? UsuarioRevisor { get; set; }

        public DateTime? FechaRevision { get; set; }

        [StringLength(500)]
        public string? NotasRevision { get; set; }

        public bool Eliminado { get; set; } = false;

        // Navegación
        public ICollection<DetalleSolicitudCompra> Detalles { get; set; } = new List<DetalleSolicitudCompra>();
    }

    public class DetalleSolicitudCompra
    {
        public int Id { get; set; }

        public int SolicitudId { get; set; }
        public SolicitudCompra? Solicitud { get; set; }

        // Artículo registrado (opcional)
        public int? ArticuloId { get; set; }
        public Articulo? Articulo { get; set; }

        // Descripción libre para artículos no registrados (opcional)
        [StringLength(300)]
        public string? DescripcionLibre { get; set; }

        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; } = 1;

        [StringLength(300)]
        public string? Observaciones { get; set; }
    }
}
