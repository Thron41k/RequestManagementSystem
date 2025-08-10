using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using OneCOverlayClient.ViewModels;
using OneCOverlayClient.Views;

namespace OneCOverlayClient;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        ServiceProvider = ServiceConfigurator.ConfigureServices();
        ServiceProvider.ConfigureMessageHandlers();
        var loginViewModel = ServiceProvider.GetRequiredService<LoginViewModel>();
        var loginWindow = new LoginView(loginViewModel);
        loginWindow.Show();
    }
}