namespace RequestManagement.Common.Models;

public class Nomenclature
{
    public int Id { get; set; }
    public string Code { get; set; }          // Код
    public string Name { get; set; }          // Название
    public string Article { get; set; }       // Артикул
    public string UnitOfMeasure { get; set; } // Единица измерения
    public int InitialQuantity { get; set; }  // Начальное количество
    public int Receipt { get; set; }          // Приход
    public int Consumption { get; set; }      // Расход
    public int FinalQuantity { get; set; }    // Конечное количество
    public int WarehouseId { get; set; }      // Склад (внешний ключ)
    public Warehouse Warehouse { get; set; }  // Навигационное свойство
}