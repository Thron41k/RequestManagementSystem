namespace RequestManagement.Common.Models.Enums;

/// <summary>
/// Перечисление ролей пользователей
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Администратор с полными правами
    /// </summary>
    Administrator = 0,

    /// <summary>
    /// Пользователь с ограниченными правами
    /// </summary>
    User = 1,

    /// <summary>
    /// Наблюдатель с правами только на чтение
    /// </summary>
    Observer = 2
}