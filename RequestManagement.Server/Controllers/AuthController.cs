using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RequestManagement.Common.Models.Enums;

namespace RequestManagement.Server.Controllers
{
    /// <summary>
    /// gRPC-контроллер для аутентификации
    /// </summary>
    public class AuthController : AuthService.AuthServiceBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, IConfiguration configuration)
        {
            _authService = authService;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Аутентифицирует пользователя и возвращает JWT-токен
        /// </summary>
        public override async Task<AuthenticateResponse> Authenticate(AuthenticateRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Authenticating user with login: {Login}", request.Login);

            var user = await _authService.AuthenticateAsync(request.Login, request.Password);
            if (user == null)
            {
                return new AuthenticateResponse
                {
                    UserId = 0,
                    Login = "",
                    Role = 0,
                    Token = ""
                };
            }

            // Генерация JWT-токена
            var token = GenerateJwtToken(user);

            return new AuthenticateResponse
            {
                UserId = user.Id,
                Login = user.Login,
                Role = (int)user.Role,
                Token = token
            };
        }

        /// <summary>
        /// Проверяет права доступа пользователя
        /// </summary>
        public override async Task<AuthorizeResponse> Authorize(AuthorizeRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Authorizing user {UserId} for role {Role}", request.UserId, request.RequiredRole);

            bool isAuthorized = await _authService.AuthorizeAsync(request.UserId, (UserRole)request.RequiredRole);
            return new AuthorizeResponse { IsAuthorized = isAuthorized };
        }

        /// <summary>
        /// Генерирует JWT-токен для пользователя
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Login),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1), // Токен действителен 1 час
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}