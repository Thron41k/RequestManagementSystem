using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequestManagement.Server.Controllers;
using WpfClient.Services.Interfaces;

namespace WpfClient.Services
{
    public class GrpcClientFactory(IServiceProvider serviceProvider) : IGrpcClientFactory
    {
        public AuthService.AuthServiceClient CreateAuthClient()
        {
            return serviceProvider.GetRequiredService<AuthService.AuthServiceClient>();
        }

        public RequestService.RequestServiceClient CreateRequestClient()
        {
            return serviceProvider.GetRequiredService<RequestService.RequestServiceClient>();
        }
    }
}
