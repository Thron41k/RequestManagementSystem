using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using System;
using System.Threading.Tasks;
using BCrypt.Net;
using RequestManagement.Common.Models.Enums;

namespace RequestManagement.Server.Services
{
    /// <summary>
    /// Сервис авторизации
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;

        public AuthService(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Аутентифицирует пользователя по логину и паролю
        /// </summary>
        public async Task<User> AuthenticateAsync(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Login cannot be null or empty", nameof(login));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            var user = await _userService.GetUserByLoginAsync(login);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null;

            return user;
        }

        /// <summary>
        /// Проверяет, имеет ли пользователь права на выполнение действия
        /// </summary>
        public async Task<bool> AuthorizeAsync(int userId, UserRole requiredRole)
        {
            return await _userService.HasRoleAsync(userId, requiredRole);
        }
    }
}