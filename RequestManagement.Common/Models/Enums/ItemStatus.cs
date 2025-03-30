namespace RequestManagement.Common.Models.Enums;

/// <summary>
/// Перечисление статусов наименования в заявке
/// </summary>
public enum ItemStatus
{
    /// <summary>
    /// Наименование получено
    /// </summary>
    Received = 0,

    /// <summary>
    /// Наименование отменено
    /// </summary>
    Canceled = 1,

    /// <summary>
    /// Наименование в ожидании
    /// </summary>
    Pending = 2
}