using RequestManagement.Common.Models.Interfaces;

namespace RequestManagement.Common.Models;

/// <summary>
/// Модель единицы техники (назначения) для заявки
/// </summary>
public class Equipment : IEntity
{
    /// <summary>
    /// Уникальный идентификатор единицы техники
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название единицы техники
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Государственный номер (может отсутствовать)
    /// </summary>
    public string? StateNumber { get; set; } = string.Empty;
}