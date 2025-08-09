using System.Windows;
using OneCOverlayClient.Services;
using OneCOverlayClient.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Models.Enums;

namespace OneCOverlayClient.ViewModels;

public partial class LoginViewModel(
    GrpcAuthService authService,
    MainWindowViewModel mainWindowViewModel,
    AuthTokenStore authTokenStore)
    : ObservableObject
{
    [ObservableProperty]
    private string _login = "admin";
    [ObservableProperty] 
    private string _password = "12345";

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Login))
        {
            MessageBox.Show("Введите логин и пароль.");
            return;
        }
        var token = await authService.AuthenticateAsync(Login, Password);
        if (!string.IsNullOrEmpty(token.Token))
        {
            authTokenStore.SetToken(token.Token);
            authTokenStore.SetRole((UserRole)token.Role);
            var mainWindow = new MainWindowView(mainWindowViewModel);
            mainWindow.Show();
            if (Application.Current.MainWindow != null) Application.Current.MainWindow.Close();
        }
        else
        {
            MessageBox.Show("Ошибка входа. Проверьте логин и пароль.");
        }
    }
}