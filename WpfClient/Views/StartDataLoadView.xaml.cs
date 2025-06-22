using WpfClient.ViewModels;

namespace WpfClient.Views;

/// <summary>
/// Логика взаимодействия для StartDataLoadView.xaml
/// </summary>
public partial class StartDataLoadView
{
    public StartDataLoadView(StartDataLoadViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}