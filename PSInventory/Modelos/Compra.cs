using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSInventory.Modelos
{
    public class Compra
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Tienda { get; set; }
        public decimal costoTotal { get; set; }
    }
}
