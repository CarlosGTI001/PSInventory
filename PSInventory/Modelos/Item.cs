using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public Articulo? Articulo { get; set; }
        public string? SucursalId { get; set; }
        public Sucursal? Sucursal { get; set; }
        public string? Estado { get; set; } 
        public string? Departamento { get; set; }
    }
}
