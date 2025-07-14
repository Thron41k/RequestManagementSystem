using CommunityToolkit.Mvvm.DependencyInjection;
using Grpc.Net.ClientFactory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using RequestManagement.WarehouseScan.Services;
using RequestManagement.WarehouseScan.Services.Interfaces;
using RequestManagement.WarehouseScan.ViewModels;
using RequestManagement.WarehouseScan.Views;
using ZXing.Net.Maui.Controls;
using GrpcClientFactory = RequestManagement.WarehouseScan.Services.GrpcClientFactory;

namespace RequestManagement.WarehouseScan
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseBarcodeReader() // ZXing
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddGrpcClient<AuthService.AuthServiceClient>(o => { o.Address = new Uri("http://192.168.20.191:5001"); });
            builder.Services.AddGrpcClient<IncomingService.IncomingServiceClient>(o => { o.Address = new Uri("http://192.168.20.191:5001"); });
            builder.Services.AddSingleton<AuthTokenStore>();
            builder.Services.AddSingleton<IGrpcClientFactory, GrpcClientFactory>();
            builder.Services.AddScoped<GrpcAuthService>();
            builder.Services.AddScoped<IIncomingService, GrpcIncomingService>();

            var mauiApp = builder.Build();

            Ioc.Default.ConfigureServices(mauiApp.Services);

            return mauiApp;
        }
    }
}
