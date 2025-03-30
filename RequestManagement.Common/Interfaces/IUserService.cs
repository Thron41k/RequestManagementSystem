using RequestManagement.Common.Models;
using RequestManagement.Common.Models.Enums;
using System.Threading.Tasks;

namespace RequestManagement.Common.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для работы с пользователями
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Создает нового пользователя
        /// </summary>
        /// <param name="user">Модель пользователя для создания</param>
        /// <returns>Идентификатор созданного пользователя</returns>
        Task<int> CreateUserAsync(User user);

        /// <summary>
        /// Обновляет данные пользователя
        /// </summary>
        /// <param name="user">Обновленная модель пользователя</param>
        /// <returns>Признак успешного обновления</returns>
        Task<bool> UpdateUserAsync(User user);

        /// <summary>
        /// Удаляет пользователя по идентификатору
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Признак успешного удаления</returns>
        Task<bool> DeleteUserAsync(int userId);

        /// <summary>
        /// Получает пользователя по идентификатору
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <returns>Модель пользователя или null, если пользователь не найден</returns>
        Task<User> GetUserByIdAsync(int userId);

        /// <summary>
        /// Получает пользователя по логину
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <returns>Модель пользователя или null, если пользователь не найден</returns>
        Task<User> GetUserByLoginAsync(string login);

        /// <summary>
        /// Проверяет, имеет ли пользователь указанную роль
        /// </summary>
        /// <param name="userId">Идентификатор пользователя</param>
        /// <param name="role">Роль для проверки</param>
        /// <returns>Признак соответствия роли</returns>
        Task<bool> HasRoleAsync(int userId, UserRole role);
    }
}
