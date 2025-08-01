using RequestManagement.Common.Models.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RequestManagement.Common.Models;
public class SparePartsOwnership : IEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int RequiredQuantity { get; set; }
    public int CurrentQuantity { get; set; }
    public string? Comment { get; set; }
    [NotMapped]
    public int? AnalogId { get; set; }
    public int EquipmentGroupId { get; set; }
    public EquipmentGroup EquipmentGroup { get; set; } = null!;

    public int NomenclatureId { get; set; }
    public Nomenclature Nomenclature { get; set; } = null!;
}
