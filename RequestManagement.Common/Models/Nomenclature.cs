using RequestManagement.Common.Models.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RequestManagement.Common.Models;

public class Nomenclature : IEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Article { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public List<Stock> Stocks { get; set; } = []; // Связь один-ко-многим
}