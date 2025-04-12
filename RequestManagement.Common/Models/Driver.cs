using RequestManagement.Common.Models.Interfaces;

namespace RequestManagement.Common.Models;

public class Driver : IEntity
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
}