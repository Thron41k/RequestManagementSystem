using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using RequestManagement.Server.Controllers;
using WpfClient.Services;
using WpfClient.ViewModels;
using WpfClient.Views;

namespace WpfClient;

public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        ServiceProvider = serviceCollection.BuildServiceProvider();

        var viewModel = ServiceProvider.GetRequiredService<LoginViewModel>();
        var mainWindow = new MainWindow(viewModel);
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpcClient<AuthService.AuthServiceClient>(o =>
        {
            o.Address = new Uri("http://localhost:5001");
        });
        services.AddGrpcClient<RequestService.RequestServiceClient>(o =>
        {
            o.Address = new Uri("http://localhost:5001");
        });
        services.AddSingleton<AuthTokenStore>();
        services.AddScoped<GrpcAuthService>();
        services.AddScoped<GrpcRequestService>();
        services.AddScoped<LoginViewModel>();
        services.AddScoped<EquipmentViewModel>(); // Добавляем ViewModel для оборудования
        services.AddScoped<MainMenuViewModel>();
        services.AddScoped<DriverViewModel>();
        services.AddScoped<DefectGroupViewModel>();
        services.AddScoped<DefectViewModel>();
        services.AddTransient<MainWindow>();
        services.AddTransient<DriverView>();
        services.AddTransient<MainMenu>();
        services.AddTransient<EquipmentView>(); // Добавляем представление для оборудования
        services.AddTransient<DefectGroupView>();
        services.AddTransient<DefectView>();
    }
}