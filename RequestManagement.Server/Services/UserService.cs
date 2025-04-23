using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;
using RequestManagement.Common.Models.Enums;

namespace RequestManagement.Server.Services
{
    /// <summary>
    /// Сервис для работы с пользователями
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Создает нового пользователя
        /// </summary>
        public async Task<int> CreateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Проверка на уникальность логина
            if (await _dbContext.Users.AnyAsync(u => u.Login == user.Login))
                throw new InvalidOperationException($"User with login '{user.Login}' already exists");

            // Хэшируем пароль перед сохранением
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user.Id;
        }

        /// <summary>
        /// Обновляет данные пользователя
        /// </summary>
        public async Task<bool> UpdateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (existingUser == null)
                return false;

            // Проверка на уникальность логина (исключая текущего пользователя)
            if (await _dbContext.Users.AnyAsync(u => u.Login == user.Login && u.Id != user.Id))
                throw new InvalidOperationException($"Login '{user.Login}' is already taken by another user");

            existingUser.Login = user.Login;
            // Хэшируем новый пароль только если он изменился
            if (existingUser.Password != user.Password)
            {
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }
            existingUser.Role = user.Role;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Удаляет пользователя по идентификатору
        /// </summary>
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return false;

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Получает пользователя по идентификатору
        /// </summary>
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// Получает пользователя по логину
        /// </summary>
        public async Task<User> GetUserByLoginAsync(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Login cannot be null or empty", nameof(login));

            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Login == login);
        }

        /// <summary>
        /// Проверяет, имеет ли пользователь указанную роль
        /// </summary>
        public async Task<bool> HasRoleAsync(int userId, UserRole role)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user != null && user.Role == role;
        }
    }
}