using RequestManagement.Common.Models.Interfaces;
using System;

namespace RequestManagement.Common.Models;

public class DefectGroup : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Defect> Defects { get; set; } = [];
    public override bool Equals(object obj) => obj is DefectGroup person && Id == person.Id;
}