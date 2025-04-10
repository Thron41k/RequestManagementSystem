// App.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WpfClient.ViewModels;
using WpfClient.Views;

namespace WpfClient;

public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        ServiceProvider = ServiceConfigurator.ConfigureServices();

        var viewModel = ServiceProvider.GetRequiredService<LoginViewModel>();
        var mainWindow = new MainWindow(viewModel);
        mainWindow.Show();
    }
}