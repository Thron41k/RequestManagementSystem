namespace RequestManagement.Common.Models.Enums;

/// <summary>
/// Перечисление статусов заявки
/// </summary>
public enum RequestStatus
{
    /// <summary>
    /// Заявка создана
    /// </summary>
    Created = 0,

    /// <summary>
    /// Заявка принята ДМТО
    /// </summary>
    AcceptedByDMTO = 1,

    /// <summary>
    /// Заявка в статусе черновика
    /// </summary>
    Draft = 2,

    /// <summary>
    /// Заявка согласована департаментом
    /// </summary>
    ApprovedByDepartment = 3,

    /// <summary>
    /// Заявка аннулирована
    /// </summary>
    Canceled = 4
}