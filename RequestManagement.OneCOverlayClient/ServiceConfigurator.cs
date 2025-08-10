using System.Net;
using OneCOverlayClient.Services;
using OneCOverlayClient.Services.Interfaces;
using OneCOverlayClient.ViewModels;
using OneCOverlayClient.Views;
using Microsoft.Extensions.DependencyInjection;
using RequestManagement.Server.Controllers;
using System.Net.Http;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using RequestManagement.Common.Interfaces;

namespace OneCOverlayClient;

public class ServiceConfigurator
{
    public static IServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMessageHandlers();
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
            return new MaterialsInUseService.MaterialsInUseServiceClient(channel);
        });

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
            return new DriverService.DriverServiceClient(channel);
        });

        // Сервисы и ViewModel'ы
        serviceCollection.AddScoped<LoginViewModel>();
        serviceCollection.AddScoped<MainWindowViewModel>();
        serviceCollection.AddScoped<MaterialsInUseViewModel>();
        serviceCollection.AddScoped<DriverViewModel>();
        serviceCollection.AddSingleton<AuthTokenStore>();
        serviceCollection.AddSingleton<IWindowService, WindowService>();
        serviceCollection.AddSingleton<IGrpcClientFactory, GrpcClientFactory>();
        serviceCollection.AddScoped<IMaterialsInUseService, GrpcMaterialsInUseService>();
        serviceCollection.AddScoped<GrpcAuthService>();
        serviceCollection.AddScoped<IDriverService, GrpcDriverService>();

        // Представления
        serviceCollection.AddTransient<LoginView>();
        serviceCollection.AddTransient<MainWindowView>();
        serviceCollection.AddTransient<MaterialsInUseView>();
        serviceCollection.AddTransient<DriverView>();

        return serviceCollection.BuildServiceProvider();
    }
}
