using RequestManagement.Common.Models.Interfaces;

namespace RequestManagement.Common.Models;

public class Warehouse : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    // Другие поля склада, если нужно
    public List<Stock> Stocks { get; set; } = []; // Связь один-ко-многим
}