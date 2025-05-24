using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSData.Modelos
{
    public class Sucursal
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
    }
}
