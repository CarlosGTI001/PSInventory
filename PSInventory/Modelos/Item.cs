using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSInventory.Modelos
{
    public class Item
    {
        [Key]
        [Required]
        public string Serial { get; set; }
        [Required]
        public int ArticuloId { get; set; }

        [NotMapped]
        public Articulo? Articulo { get; set; }
        public string? SucursalId { get; set; }

        [NotMapped]
        public Sucursal? Sucursal { get; set; }
        public string? Estado { get; set; } 
        public decimal Costo { get; set; }

        [Required]
        public int CompraId   { get; set; }

        [NotMapped]
        public Compra? Compra { get; set; }
        public string? Departamento { get; set; }
    }
}
