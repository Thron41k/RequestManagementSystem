// App.xaml.cs

using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using RequestManagement.WpfClient.ViewModels;
using RequestManagement.WpfClient.Views;

namespace RequestManagement.WpfClient;

public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        ServiceProvider = ServiceConfigurator.ConfigureServices();
        ServiceProvider.ConfigureMessageHandlers();
        var viewModel = ServiceProvider.GetRequiredService<LoginViewModel>();
        var mainWindow = new MainWindow(viewModel);
        mainWindow.Show();
    }
}