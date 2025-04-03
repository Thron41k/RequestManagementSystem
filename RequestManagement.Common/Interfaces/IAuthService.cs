using RequestManagement.Common.Models;
using RequestManagement.Common.Models.Enums;
using System.Threading.Tasks;

namespace RequestManagement.Common.Interfaces;

/// <summary>
/// Интерфейс сервиса авторизации
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Аутентифицирует пользователя по логину и паролю
    /// </summary>
    /// <param name="login">Логин пользователя</param>
    /// <param name="password">Пароль пользователя</param>
    /// <returns>Модель пользователя при успешной аутентификации или null, если аутентификация не удалась</returns>
    Task<User> AuthenticateAsync(string login, string password);

    /// <summary>
    /// Проверяет, имеет ли пользователь права на выполнение действия
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="requiredRole">Требуемая роль для действия</param>
    /// <returns>Признак наличия прав</returns>
    Task<bool> AuthorizeAsync(int userId, UserRole requiredRole);
}