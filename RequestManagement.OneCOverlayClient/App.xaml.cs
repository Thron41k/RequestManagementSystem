using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
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
        var loginViewModel = ServiceProvider.GetRequiredService<LoginViewModel>();
        var loginWindow = new LoginView(loginViewModel);
        loginWindow.Show();
    }
}