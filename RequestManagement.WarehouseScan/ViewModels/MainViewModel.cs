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
    [ObservableProperty] private string _resultName = "";
    [ObservableProperty] private string _resultArticle = "";
    [ObservableProperty] private string _resultTotalCount = "";
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
        ResultName = ResultArticle = ResultTotalCount = string.Empty;
        FrameHeight = mode == ScanMode.Qr ? 200 : 120;
        ModeName = mode == ScanMode.Qr ? "QR код" : "Этикетка";
    }


    [RelayCommand]
    private void Rescan()
    {
        ResultName = ResultArticle = ResultTotalCount = string.Empty;
        IsDetecting = true;
    }

    internal async Task DetectionResult(string value)
    {
        if (CurrentMode == ScanMode.Qr)
        {
            var parseResult = int.TryParse(value, out var id);
            if (parseResult)
            {
                var result = await _incomingService.FindIncomingByIdAsync(id);
                if (result.Id != 0)
                {
                    ResultName = result.Stock.Nomenclature.Name;
                    ResultArticle = result.Stock.Nomenclature.Article!;
                    ResultTotalCount = result.Stock.FinalQuantity.ToString("0.000");
                }
                else
                    ResultName = "Номенклатура не найдена!";
            }
        }
    }
}