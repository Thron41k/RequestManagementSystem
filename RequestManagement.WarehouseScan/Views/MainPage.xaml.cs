using RequestManagement.WarehouseScan.ViewModels;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace RequestManagement.WarehouseScan.Views;

public partial class MainPage : ContentPage
{
    private MainViewModel _viewModel;
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _ = _viewModel.Login();
        barcodeView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.All,
            AutoRotate = true,
            Multiple = false
        };
    }

    private async void OnDetected(object sender, BarcodeDetectionEventArgs e)
    {
        foreach (var barcode in e.Results)
            Console.WriteLine($"Barcodes: {barcode.Format} -> {barcode.Value}");

        var first = e.Results?.FirstOrDefault();
        if (first is not null)
        {
            barcodeView.IsDetecting = false;
            async void Action()
            {
                await _viewModel.DetectionResult(first.Value);
            }
            Dispatcher.Dispatch(Action);
        }
    }

    private void OnToggleTorchClicked(object sender, EventArgs e)
    {
        barcodeView.IsTorchOn = !barcodeView.IsTorchOn;
    }
}