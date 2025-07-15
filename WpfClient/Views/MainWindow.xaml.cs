using System.Windows;
using RequestManagement.WpfClient.ViewModels;

namespace RequestManagement.WpfClient.Views;

public partial class MainWindow : Window
{
    public MainWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        // Проверка DataContext
        if (DataContext == null)
        {
            MessageBox.Show("DataContext не установлен!");
        }

        // Привязка пароля из PasswordBox
        passwordBox.PasswordChanged += (s, e) => viewModel.Password = passwordBox.Password;
    }
}