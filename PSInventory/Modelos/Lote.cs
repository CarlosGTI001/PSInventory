using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSInventory.Modelos
{
    public class Lote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ArticuloId { get; set; }
        [NotMapped]
        public Articulo? Articulo { get; set; }

        [Required]
        public int CompraId { get; set; }
        [NotMapped]
        public Compra? Compra { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal CostoUnitario { get; set; }

        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}