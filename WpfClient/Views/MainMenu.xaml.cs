using System.Windows;
using RequestManagement.WpfClient.ViewModels;

namespace RequestManagement.WpfClient.Views;

/// <summary>
/// Логика взаимодействия для Window1.xaml
/// </summary>
public partial class MainMenu : Window
{
    public MainMenu(MainMenuViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}