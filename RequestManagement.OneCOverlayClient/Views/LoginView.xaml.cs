using System.Windows;
using System.Windows.Controls;
using OneCOverlayClient.ViewModels;

namespace OneCOverlayClient.Views;

/// <summary>
/// Interaction logic for LoginView.xaml
/// </summary>
public partial class LoginView : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginView(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        if (DataContext == null)
        {
            MessageBox.Show("DataContext не установлен!");
        }
    }
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        var passwordBox = (PasswordBox)sender;
        _viewModel.Password = passwordBox.Password;
    }
}