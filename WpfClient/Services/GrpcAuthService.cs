using RequestManagement.Server.Controllers;

namespace WpfClient.Services
{
    public class GrpcAuthService
    {
        private readonly AuthService.AuthServiceClient _client;

        public GrpcAuthService(AuthService.AuthServiceClient client)
        {
            _client = client;
        }

        public async Task<AuthenticateResponse> AuthenticateAsync(string login, string password)
        {
            var request = new AuthenticateRequest { Login = login, Password = password };
            var response = await _client.AuthenticateAsync(request);
            return response.UserId == 0 ? null : response;
        }
    }
}