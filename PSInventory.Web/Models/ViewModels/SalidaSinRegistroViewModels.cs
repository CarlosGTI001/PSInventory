using System.Collections.Generic;

namespace PSInventory.Web.Models.ViewModels
{
    public class SalidaSinRegistroInput
    {
        public string SucursalDestinoId { get; set; } = string.Empty;
        public string ResponsableEmpleado { get; set; } = string.Empty;
        public bool EntregaDepartamento { get; set; }
        public int? DepartamentoDestinoId { get; set; }
        public string PersonaEntregaDepartamento { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public List<ItemSinRegistroInput> Items { get; set; } = new();
    }

    public class ItemSinRegistroInput
    {
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string Serial { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}
