using RequestManagement.Common.Models.Enums;
using RequestManagement.Common.Models.Interfaces;

namespace RequestManagement.Common.Models;

/// <summary>
/// Модель пользователя системы
/// </summary>
public class User : IEntity
{
    /// <summary>
    /// Уникальный идентификатор пользователя
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Логин пользователя
    /// </summary>
    public string Login { get; set; }

    /// <summary>
    /// Пароль пользователя (хранится в зашифрованном виде)
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Роль пользователя в системе
    /// </summary>
    public UserRole Role { get; set; }

    public List<RefreshToken> RefreshTokens { get; set; } = [];
}