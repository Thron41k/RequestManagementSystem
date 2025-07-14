using CommunityToolkit.Mvvm.DependencyInjection;
using RequestManagement.WarehouseScan.Views;

namespace RequestManagement.WarehouseScan
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var page = Ioc.Default.GetRequiredService<MainPage>(); // или любой DI
            return new Window(page);
        }
    }
}