using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequestManagement.Server.Controllers;

namespace WpfClient.Services.Interfaces
{
    public interface IGrpcClientFactory
    {
        AuthService.AuthServiceClient CreateAuthClient();
        RequestService.RequestServiceClient CreateRequestClient();
    }
}
