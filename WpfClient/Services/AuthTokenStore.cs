using RequestManagement.Common.Models;

namespace WpfClient.Services
{
    public class AuthTokenStore
    {
        public static string JwtToken { get; set; }
        public static int UserRole { get; set; }
    }
}