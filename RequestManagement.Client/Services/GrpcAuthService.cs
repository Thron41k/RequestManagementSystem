using Grpc.Net.Client;
using RequestManagement.Server.Controllers;
using System;
using System.Threading.Tasks;

namespace RequestManagement.Client.Services
{
    public class GrpcAuthService
    {
        private readonly AuthService.AuthServiceClient _client;

        public GrpcAuthService(AuthService.AuthServiceClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// Аутентифицирует пользователя по логину и паролю
        /// </summary>
        /// <returns>Ответ с данными пользователя или null при неудаче</returns>
        public async Task<AuthenticateResponse> AuthenticateAsync(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Login cannot be null or empty", nameof(login));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            var request = new AuthenticateRequest
            {
                Login = login,
                Password = password
            };

            try
            {
                var response = await _client.AuthenticateAsync(request);
                if (response.UserId == 0) // Сервер возвращает UserId = 0 при неудаче
                {
                    return null;
                }
                return response;
            }
            catch (Exception ex)
            {
                // Можно добавить логирование ошибки
                throw new InvalidOperationException("Authentication failed due to a server error.", ex);
            }
        }
    }
}