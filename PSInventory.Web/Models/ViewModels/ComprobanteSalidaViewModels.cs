using System;
using System.Collections.Generic;

namespace PSInventory.Web.Models.ViewModels
{
    public class ComprobanteSalidaContext
    {
        public List<int> MovimientoIds { get; set; } = new();
        public string TipoSalida { get; set; } = string.Empty;
        public string DestinoNombre { get; set; } = string.Empty;
        public string ResponsableRecepcion { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public string UsuarioResponsable { get; set; } = string.Empty;
        public bool EntregaDepartamento { get; set; }
        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
    }
}
