using OneCOverlayClient.Services.Interfaces;
using Grpc.Core;
using RequestManagement.Server.Controllers;

namespace OneCOverlayClient.Services;

public class GrpcAuthService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore)
{
    private readonly AuthTokenStore _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));

    public async Task<AuthenticateResponse> AuthenticateAsync(string login, string password)
    {
        try
        {
            var request = new AuthenticateRequest { Login = login, Password = password };
            var authClient = clientFactory.CreateAuthClient();
            var response = await authClient.AuthenticateAsync(request);
            var token = response.Token;

            // Сохраняем токен в AuthTokenStore
            if (!string.IsNullOrEmpty(token))
            {
                _tokenStore.SetToken(token);
            }
            return (response.UserId == 0 ? null : response)!;
        }
        catch (RpcException ex)
        {
            throw new Exception($"Ошибка аутентификации: {ex.Status.Detail}", ex);
        }
    }
}