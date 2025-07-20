using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QRCoder;
using RequestManagement.Common.Models;
using RequestManagement.WpfClient.Messages;
using RequestManagement.WpfClient.Models;
using RequestManagement.WpfClient.Services.Interfaces;
using RequestManagement.WpfClient.Views;
using Application = System.Windows.Application;
using Color = System.Drawing.Color;
using FontFamily = System.Windows.Media.FontFamily;
using FontStyle = System.Windows.FontStyle;
using Size = System.Windows.Size;

namespace RequestManagement.WpfClient.ViewModels;

public partial class LabelPrintListViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    [ObservableProperty] private CollectionViewSource _incomingsViewSource;
    [ObservableProperty] private ObservableCollection<IncomingForPrint> _labelList = [];
    [ObservableProperty] private ObservableCollection<IncomingForPrint> _labelListForShow = [];
    [ObservableProperty] private Incoming? _selectedIncoming;
    [ObservableProperty] private int _columns = 2;
    [ObservableProperty] private int _rows = 5;
    [ObservableProperty] private int _printScale = 84;
    [ObservableProperty] private string _fastSearchText = "";
    public event EventHandler CloseWindowRequested;

    public LabelPrintListViewModel(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public LabelPrintListViewModel()
    {

    }

    [RelayCommand]
    private void Print()
    {
        var printDialog = new PrintDialog();
        if (printDialog.ShowDialog() == true)
        {
            const double pageWidth = 794;  // A4 @ 96 DPI
            const double pageHeight = 1123;
            const double margin = 20;
            const int rows = 5;
            const int columns = 2;

            const double labelWidth = (pageWidth - 2 * margin) / columns;
            const double labelHeight = (pageHeight - 2 * margin) / rows;

            var labelWidthScaled = labelWidth * PrintScale / 100.0;
            var labelHeightScaled = labelHeight * PrintScale / 100.0;
            var rowOverride = (int)Math.Round((pageHeight - 2 * margin) / labelHeightScaled, MidpointRounding.ToZero);
            var columnOverride = (int)Math.Round((pageWidth - 2 * margin) / labelWidthScaled, MidpointRounding.ToZero);
            var labelsPerPage = rowOverride * columnOverride;

            var doc = new FixedDocument
            {
                DocumentPaginator = { PageSize = new Size(pageWidth, pageHeight) }
            };

            var labels = ExpandByQuantity(LabelListForShow).ToList();

            for (var i = 0; i < labels.Count; i += labelsPerPage)
            {
                var pageLabels = labels.Skip(i).Take(labelsPerPage).ToList();

                var pageContent = new PageContent();
                var fixedPage = new FixedPage
                {
                    Width = pageWidth,
                    Height = pageHeight
                };

                var grid = new UniformGrid
                {
                    Columns = columnOverride,
                    Rows = rowOverride,
                    Margin = new Thickness(margin)
                };

                foreach (var viewBox in pageLabels.Select(label => new LabelTemplateView
                         {
                             DataContext = label
                         }).Select(view => new Viewbox
                         {
                             Width = labelWidthScaled,
                             Height = labelHeightScaled,
                             Stretch = Stretch.Fill,
                             Child = view
                         }))
                {
                    grid.Children.Add(viewBox);
                }

                FixedPage.SetLeft(grid, 0);
                FixedPage.SetTop(grid, 0);
                fixedPage.Children.Add(grid);
                ((IAddChild)pageContent).AddChild(fixedPage);
                doc.Pages.Add(pageContent);
            }

            printDialog.PrintDocument(doc.DocumentPaginator, "Печать этикеток");
        }
    }

    private IEnumerable<IncomingForPrint> ExpandByQuantity(IEnumerable<IncomingForPrint> source)
    {
        foreach (var incoming in source)
        {
            for (var i = 0; i < incoming.Incoming.Quantity; i++)
                yield return incoming;
        }
    }

    private static ImageSource GenerateQr(string s)
    {
        const int desiredSize = 170;
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(s, QRCodeGenerator.ECCLevel.H);
        var moduleCount = qrCodeData.ModuleMatrix.Count;
        var pixelsPerModule = desiredSize / moduleCount;
        var qrCode = new PngByteQRCode(qrCodeData);
        var qrBytes = qrCode.GetGraphic(pixelsPerModule, Color.Black, Color.White, drawQuietZones: false);
        using var ms = new MemoryStream(qrBytes);
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.StreamSource = ms;
        bitmap.EndInit();
        bitmap.Freeze();
        return bitmap;
    }

    public async Task Init(List<Incoming> labelList)
    {
        LabelList = new ObservableCollection<IncomingForPrint>(labelList.Select(x => IncomingForPrint.FromIncoming(x, GenerateQr)).ToList());
        await UpdateLabelList();
    }

    partial void OnFastSearchTextChanged(string value)
    {
        _ = UpdateLabelList();
    }
    public async Task UpdateLabelList()
    {
        await _messageBus.Publish(
            new ShowResultMessage(
                MessagesEnum.UpdateLabelPrintList, typeof(IncomingListViewModel), LabelList.Select(x => x.Incoming).ToList()));
        var filtered = !string.IsNullOrWhiteSpace(FastSearchText)
            ? LabelList.Where(x =>
                x.Incoming.Stock.Nomenclature.Name.Contains(FastSearchText, StringComparison.OrdinalIgnoreCase) ||
                (x.Incoming.Stock.Nomenclature.Article?.Contains(FastSearchText, StringComparison.OrdinalIgnoreCase) ??
                 false)).ToList()
            : LabelList.ToList();
        LabelListForShow = new ObservableCollection<IncomingForPrint>(filtered);

        IncomingsViewSource = new CollectionViewSource { Source = new ObservableCollection<IncomingForPrint>(filtered) };
    }

    [RelayCommand]
    private async Task DeleteItem(IncomingForPrint item)
    {
        LabelList.Remove(item);
        await UpdateLabelList();
    }
}