using RequestManagement.Common.Models.Interfaces;

namespace RequestManagement.Common.Models;

public class Warehouse : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; } = string.Empty;
    public Driver? FinanciallyResponsiblePerson { get; set; } = null;
    public int? FinanciallyResponsiblePersonId { get; set; } = null;
    public DateTime LastUpdated { get; set; }
    public List<Stock> Stocks { get; set; } = [];
}