using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models.Enums;
using RequestManagement.WarehouseScan.Services;
using CommunityToolkit.Maui.Alerts;

namespace RequestManagement.WarehouseScan.ViewModels;

public enum ScanMode { Qr, Label }

public partial class MainViewModel(
    GrpcAuthService authService,
    AuthTokenStore authTokenStore,
    IIncomingService incomingService)
    : ObservableObject
{
    private readonly GrpcAuthService _authService = authService;
    private readonly AuthTokenStore _authTokenStore = authTokenStore;
    private readonly IIncomingService _incomingService = incomingService;
    [ObservableProperty] private string _modeName = "QR код";
    [ObservableProperty] private bool _isDetecting;
    [ObservableProperty] private string _resultText = "";
    [ObservableProperty] private bool _isTorchOn;
    [ObservableProperty] private double _frameHeight = 200;

    public async Task Login()
    {
        var token = await _authService.AuthenticateAsync("admin", "12345");
        if (token != null && !string.IsNullOrEmpty(token.Token))
        {
            _authTokenStore.SetToken(token.Token);
            _authTokenStore.SetRole((UserRole)token.Role);
            var toast = Toast.Make("Вы успешно авторизовались");
            await toast.Show();
        }
    }

    private ScanMode CurrentMode { get; set; } = ScanMode.Qr;

    [RelayCommand]
    private void SwitchMode()
    {
        var mode = CurrentMode == ScanMode.Qr ? ScanMode.Label : ScanMode.Qr;
        CurrentMode = mode;
        ResultText = string.Empty;
        FrameHeight = mode == ScanMode.Qr ? 200 : 120;
        ModeName = mode == ScanMode.Qr ? "QR код" : "Этикетка";
    }


    [RelayCommand]
    private void Rescan()
    {
        ResultText = string.Empty;
        IsDetecting = true;
    }
}