using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Common.Models.Enums;
using RequestManagement.Server.Data;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace RequestManagement.Server.Services;

/// <summary>
/// Сервис авторизации
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly ApplicationDbContext _dbContext;

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
        //var tmp_pass = BCrypt.Net.BCrypt.HashPassword(password);
        //Console.WriteLine(tmp_pass);
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
    private RefreshToken GenerateRefreshToken(string ipAddress)
    {
        return new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(1),
            Created = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };
    }
    public async Task<User> RefreshTokenAsync(string token, string ipAddress)
    {
        var user = await _dbContext.Users.Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

        var refreshToken = user?.RefreshTokens.SingleOrDefault(rt => rt.Token == token);

        if (user == null || refreshToken is not { IsActive: true })
            throw new SecurityTokenException("Invalid refresh token");

        // Генерируем новые токены
        var newRefreshToken = GenerateRefreshToken(ipAddress);
        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReplacedByToken = newRefreshToken.Token;

        user.RefreshTokens.Add(newRefreshToken);
        await _dbContext.SaveChangesAsync();
        return user;
    }

}