using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Models.Enums;
using RequestManagement.WpfClient.Services;
using RequestManagement.WpfClient.Views;

namespace RequestManagement.WpfClient.ViewModels;

public class LoginViewModel
{
    private readonly GrpcAuthService _authService;
    private readonly MainMenuViewModel _mainMenuViewModel;
    private readonly AuthTokenStore _authTokenStore;

    public string Login { get; set; } = "admin";
    public string Password { get; set; } = "12345";
    public ICommand LoginCommand { get; }

    public LoginViewModel(GrpcAuthService authService, MainMenuViewModel mainMenuViewModel,AuthTokenStore authTokenStore)
    {
        _authService = authService;
        _mainMenuViewModel = mainMenuViewModel;
        _authTokenStore = authTokenStore;
        LoginCommand = new RelayCommand(async () => await LoginAsync());
    }

    private async Task LoginAsync()
    {
        var passwordBox = (Application.Current.MainWindow as MainWindow)?.passwordBox;
        if (passwordBox == null || string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(passwordBox.Password))
        {
            MessageBox.Show("Введите логин и пароль.");
            return;
        }

        var token = await _authService.AuthenticateAsync(Login, passwordBox.Password);
        if (token != null && !string.IsNullOrEmpty(token.Token))
        {
            _authTokenStore.SetToken(token.Token);
            _authTokenStore.SetRole((UserRole)token.Role);
            var mainMenu = new MainMenu(_mainMenuViewModel);
            mainMenu.Show();
            Application.Current.MainWindow.Close();
        }
        else
        {
            MessageBox.Show("Ошибка входа. Проверьте логин и пароль.");
        }
    }
}