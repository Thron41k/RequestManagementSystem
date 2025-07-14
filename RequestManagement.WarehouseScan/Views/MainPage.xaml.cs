using RequestManagement.WarehouseScan.ViewModels;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace RequestManagement.WarehouseScan.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _ = viewModel.Login();
        barcodeView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.All,
            AutoRotate = true,
            Multiple = false
        };
    }

    private void OnDetected(object sender, BarcodeDetectionEventArgs e)
    {
        foreach (var barcode in e.Results)
            Console.WriteLine($"Barcodes: {barcode.Format} -> {barcode.Value}");

        var first = e.Results?.FirstOrDefault();
        if (first is not null)
        {
            barcodeView.IsDetecting = false;
            Dispatcher.Dispatch(() =>
            {
                ResultLabel.Text = $"Barcodes: {first.Format} -> {first.Value}";
            });
        }
    }

    private void OnToggleTorchClicked(object sender, EventArgs e)
    {
        barcodeView.IsTorchOn = !barcodeView.IsTorchOn;
    }
}