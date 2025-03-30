using RequestManagement.Common.Models.Enums;

namespace RequestManagement.Common.Models;

/// <summary>
/// Модель наименования в заявке
/// </summary>
public class Item
{
    /// <summary>
    /// Уникальный идентификатор наименования
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название наименования
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Артикул наименования
    /// </summary>
    public string Article { get; set; }

    /// <summary>
    /// Количество наименования
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Примечание к наименованию
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Статус наименования
    /// </summary>
    public ItemStatus Status { get; set; }

    /// <summary>
    /// Идентификатор заявки, к которой относится наименование
    /// </summary>
    public int RequestId { get; set; }

    /// <summary>
    /// Ссылка на заявку (навигационное свойство)
    /// </summary>
    public Request Request { get; set; }
}