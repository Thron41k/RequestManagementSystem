using RequestManagement.Common.Models.Enums;

namespace RequestManagement.Common.Models;

/// <summary>
/// Модель наименования в заявке
/// </summary>
public class Item
{
    public int Id { get; set; }
    public int NomenclatureId { get; set; }    // Ссылка на номенклатуру
    public Nomenclature Nomenclature { get; set; } // Навигационное свойство
    public int Quantity { get; set; }
    public string Note { get; set; }
    public ItemStatus Status { get; set; }
    public int RequestId { get; set; }
    public Request Request { get; set; }
}