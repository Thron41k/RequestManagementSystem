// ServiceConfigurator.cs
using Microsoft.Extensions.DependencyInjection;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using WpfClient.Services;
using WpfClient.Services.Interfaces;
using WpfClient.ViewModels;
using WpfClient.Views;

namespace WpfClient;

public class ServiceConfigurator
{
    public static IServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        // gRPC клиенты
        serviceCollection.AddGrpcClient<AuthService.AuthServiceClient>(o => {o.Address = new Uri("http://localhost:5001"); });
        serviceCollection.AddGrpcClient<RequestService.RequestServiceClient>(o => {o.Address = new Uri("http://localhost:5001");});
        serviceCollection.AddGrpcClient<StockService.StockServiceClient>(o => { o.Address = new Uri("http://localhost:5001"); });
        serviceCollection.AddGrpcClient<ExpenseService.ExpenseServiceClient>(o => { o.Address = new Uri("http://localhost:5001"); });

        // Сервисы и ViewModel'ы
        serviceCollection.AddSingleton<AuthTokenStore>();
        serviceCollection.AddSingleton<IMessageBus, MessageBusService>();
        serviceCollection.AddSingleton<IGrpcClientFactory, GrpcClientFactory>();

        serviceCollection.AddScoped<GrpcAuthService>();
        serviceCollection.AddScoped<LoginViewModel>();
        serviceCollection.AddScoped<EquipmentViewModel>();
        serviceCollection.AddScoped<MainMenuViewModel>();
        serviceCollection.AddScoped<DriverViewModel>();
        serviceCollection.AddScoped<DefectGroupViewModel>();
        serviceCollection.AddScoped<DefectViewModel>();
        serviceCollection.AddScoped<WarehouseViewModel>();
        serviceCollection.AddScoped<NomenclatureViewModel>();
        serviceCollection.AddScoped<StockViewModel>();
        serviceCollection.AddScoped<IEquipmentService, GrpcEquipmentService>();
        serviceCollection.AddScoped<IDriverService, GrpcDriverService>();
        serviceCollection.AddScoped<IDefectService, GrpcDefectService>();
        serviceCollection.AddScoped<IWarehouseService, GrpcWarehouseService>();
        serviceCollection.AddScoped<INomenclatureService, GrpcNomenclatureService>();
        serviceCollection.AddScoped<IStockService, GrpcStockService>();
        serviceCollection.AddScoped<IExpenseService, GrpcExpenseService>();

        // Представления
        serviceCollection.AddTransient<MainWindow>();
        serviceCollection.AddTransient<DriverView>();
        serviceCollection.AddTransient<MainMenu>();
        serviceCollection.AddTransient<EquipmentView>();
        serviceCollection.AddTransient<DefectGroupView>();
        serviceCollection.AddTransient<DefectView>();
        serviceCollection.AddTransient<WarehouseView>();
        serviceCollection.AddTransient<NomenclatureView>();
        serviceCollection.AddTransient<StockView>();

        return serviceCollection.BuildServiceProvider();
    }
}