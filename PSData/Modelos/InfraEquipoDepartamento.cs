using System.ComponentModel.DataAnnotations.Schema;

namespace PSData.Modelos;

public class InfraEquipoDepartamento
{
    public int InfraEquipoComputoId { get; set; }

    [ForeignKey(nameof(InfraEquipoComputoId))]
    public virtual InfraEquipoComputo? InfraEquipoComputo { get; set; }

    public int DepartamentoId { get; set; }

    [ForeignKey(nameof(DepartamentoId))]
    public virtual Departamento? Departamento { get; set; }
}

