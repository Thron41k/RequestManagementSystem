using System.Net;
using OneCOverlayClient.Services;
using OneCOverlayClient.Services.Interfaces;
using OneCOverlayClient.ViewModels;
using OneCOverlayClient.Views;
using Microsoft.Extensions.DependencyInjection;
using RequestManagement.Server.Controllers;
using System.Net.Http;
using System.Net.Sockets;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

namespace OneCOverlayClient;

public class ServiceConfigurator
{
    public static IServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(services =>
        {
            var httpClientHandler = new HttpClientHandler();
            var grpcWebHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, httpClientHandler)
            {
                GrpcWebMode = GrpcWebMode.GrpcWeb
            };

            var channel = GrpcChannel.ForAddress($"http://{Vars.Server}:5001", new GrpcChannelOptions
            {
                HttpHandler = grpcWebHandler,
                HttpVersion = HttpVersion.Version11,
                HttpVersionPolicy = HttpVersionPolicy.RequestVersionOrLower
            });
            return new AuthService.AuthServiceClient(channel);
        });

        // Сервисы и ViewModel'ы
        serviceCollection.AddScoped<LoginViewModel>();
        serviceCollection.AddScoped<MainWindowViewModel>();
        serviceCollection.AddScoped<MaterialsInUseViewModel>();
        serviceCollection.AddSingleton<AuthTokenStore>();
        serviceCollection.AddSingleton<IWindowService, WindowService>();
        serviceCollection.AddSingleton<IMessageBus, MessageBusService>();
        serviceCollection.AddSingleton<IGrpcClientFactory, GrpcClientFactory>();
        serviceCollection.AddScoped<GrpcAuthService>();

        // Представления
        serviceCollection.AddTransient<LoginView>();
        serviceCollection.AddTransient<MainWindowView>();
        serviceCollection.AddTransient<MaterialsInUseView>();

        return serviceCollection.BuildServiceProvider();
    }
}
