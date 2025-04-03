using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using WpfClient.Services;
using WpfClient.Views;

namespace WpfClient.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly GrpcAuthService _authService;
        private readonly IServiceProvider _serviceProvider;
        private readonly RelayCommand _loginCommand; // Сохраняем ссылку для вызова NotifyCanExecuteChanged

        [ObservableProperty]
        private string login;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string errorMessage;

        [ObservableProperty]
        private bool isAuthenticated;

        public ICommand LoginCommand { get; }

        public LoginViewModel(GrpcAuthService authService, IServiceProvider serviceProvider)
        {
            _authService = authService;
            _serviceProvider = serviceProvider;

            _loginCommand = new RelayCommand(async () => await LoginAsync(), () => CanLogin());
            LoginCommand = _loginCommand;

            // Подписка на изменения свойств
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName is nameof(Login) or nameof(Password))
                {
                    _loginCommand.NotifyCanExecuteChanged();
                }
            };

            System.Diagnostics.Debug.WriteLine("LoginCommand инициализирован: " + (LoginCommand != null));
        }

        public async Task LoginAsync()
        {
            try
            {
                var response = await _authService.AuthenticateAsync(Login, Password);
                if (response == null)
                {
                    ErrorMessage = "Неверный логин или пароль.";
                    return;
                }

                AuthTokenStore.JwtToken = response.Token;
                AuthTokenStore.UserRole = response.Role;
                IsAuthenticated = true;
                ErrorMessage = null;

                var requestsWindow = _serviceProvider.GetRequiredService<MainMenu>();
                requestsWindow.Show();
                if (App.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.Close();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
        }

        private bool CanLogin()
        {
            return !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password);
        }
    }
}