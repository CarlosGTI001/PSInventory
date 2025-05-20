using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSInventory.Modelos
{
    public class Usuario
    {
        [Key]
        [Required]
        public string UsuarioId { get; set; }

        [Required]
        public string UsuarioNombre { get; set; }

        [Required]
        public string UsuarioPassword { get; set; }

        [Required]
        public string UsuarioEmail { get; set; }

        [Required]
        public string Rol { get; set; }
    }
}
