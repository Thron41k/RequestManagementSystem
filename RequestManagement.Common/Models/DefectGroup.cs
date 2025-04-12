using RequestManagement.Common.Models.Interfaces;

namespace RequestManagement.Common.Models;

public class DefectGroup : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Defect> Defects { get; set; } = [];
}