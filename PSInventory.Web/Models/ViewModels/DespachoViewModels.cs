namespace PSInventory.Web.Models.ViewModels
{
    public class DespachoItemInput
    {
        // Con serial: referencia al item específico
        public int? ItemId { get; set; }
        // Sin serial: referencia al artículo, el backend distribuye el stock
        public int? ArticuloId { get; set; }
        public int Cantidad { get; set; } = 1;
    }

    public class DespachoInput
    {
        public int? DepartamentoId { get; set; }
        public string SucursalDestinoId { get; set; } = string.Empty;
        public string ResponsableEmpleado { get; set; } = string.Empty;
        public bool EntregaDepartamento { get; set; }
        public int? DepartamentoDestinoId { get; set; }
        public string PersonaEntregaDepartamento { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public List<DespachoItemInput> Items { get; set; } = new();
    }
}
