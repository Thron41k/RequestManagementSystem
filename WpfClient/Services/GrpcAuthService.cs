using Grpc.Core;
using RequestManagement.Server.Controllers;

namespace WpfClient.Services
{
    public class GrpcAuthService
    {
        private readonly AuthService.AuthServiceClient _authClient;
        private readonly AuthTokenStore _tokenStore;

        public GrpcAuthService(AuthService.AuthServiceClient authClient, AuthTokenStore tokenStore)
        {
            _authClient = authClient ?? throw new ArgumentNullException(nameof(authClient));
            _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
        }

        public async Task<AuthenticateResponse> AuthenticateAsync(string login, string password)
        {
            try
            {
                var request = new AuthenticateRequest { Login = login, Password = password };
                var response = await _authClient.AuthenticateAsync(request);
                var token = response.Token;

                // Сохраняем токен в AuthTokenStore
                if (!string.IsNullOrEmpty(token))
                {
                    _tokenStore.SetToken(token);
                }
                return response.UserId == 0 ? null : response;
            }
            catch (RpcException ex)
            {
                throw new Exception($"Ошибка аутентификации: {ex.Status.Detail}", ex);
            }
        }
    }
}