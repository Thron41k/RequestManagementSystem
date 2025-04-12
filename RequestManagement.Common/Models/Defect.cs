using RequestManagement.Common.Models.Interfaces;

namespace RequestManagement.Common.Models;

public class Defect : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DefectGroupId { get; set; }      // Группа дефекта (внешний ключ)
    public DefectGroup DefectGroup { get; set; }
}