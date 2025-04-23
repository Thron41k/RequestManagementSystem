using System.Windows;
using WpfClient.ViewModels;

namespace WpfClient.Views;

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