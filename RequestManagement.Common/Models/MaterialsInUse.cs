using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using RequestManagement.Common.Models.Interfaces;

namespace RequestManagement.Common.Models;

public class MaterialsInUse : IEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public bool IsOut { get; set; }
    public string DocumentNumber { get; set; } = "";
    public DateTime Date { get; set; }
    public decimal Quantity { get; set; }
    public int NomenclatureId { get; set; }
    public Nomenclature Nomenclature { get; set; } = null!;
    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; }
    public Driver FinanciallyResponsiblePerson { get; set; } = null!;
    public int FinanciallyResponsiblePersonId { get; set; } = 1;
    public Driver? MolForMove { get; set; } = null!;
    public int? MolForMoveId { get; set; }
    public int ReasonForWriteOffId { get; set; } = 1;
    public ReasonsForWritingOffMaterialsFromOperation ReasonForWriteOff { get; set; } = null!;
    public string DocumentNumberForWriteOff { get; set; } = "";
    public DateTime DateForWriteOff { get; set; }
    public int? ExpenseId { get; set; }
    [NotMapped] public string DateForList => DateForWriteOff == DateTime.MinValue ? "" : DateForWriteOff.ToString("dd.MM.yyyy");
    [NotMapped] public string ServiceLife => ((DateTime.Now.Year - Date.Year) * 12 + DateTime.Now.Month - Date.Month - 1).ToString();
}