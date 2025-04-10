namespace RequestManagement.Common.Models;

public class Nomenclature
{
    public int Id { get; set; }
    public string Code { get; set; }          // Код
    public string Name { get; set; }          // Название
    public string Article { get; set; }       // Артикул
    public string UnitOfMeasure { get; set; } // Единица измерения
}