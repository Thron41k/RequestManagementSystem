using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
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
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QRCoder;
using QRCoder.Xaml;
using RequestManagement.Common.Models;
using RequestManagement.WpfClient.Messages;
using RequestManagement.WpfClient.Models;
using RequestManagement.WpfClient.Services.Interfaces;
using Application = System.Windows.Application;
using Font = System.Drawing.Font;
using Rectangle = System.Drawing.Rectangle;
using Brushes = System.Drawing.Brushes;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using Size = System.Windows.Size;

namespace RequestManagement.WpfClient.ViewModels;

public partial class LabelPrintListViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private const int LabelPerPage = 10;
    [ObservableProperty] private CollectionViewSource _incomingsViewSource;
    [ObservableProperty] private ObservableCollection<Incoming> _labelList = [];
    [ObservableProperty] private ObservableCollection<Incoming> _labelListForShow = [];
    [ObservableProperty] private ObservableCollection<ImageItem> _images = [];
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


    private FrameworkElement CreateLabelWpf(Incoming incoming)
    {
        var outerBorder = new Border
        {
            BorderThickness = new Thickness(1),
            BorderBrush = System.Windows.Media.Brushes.Black,
            Width = 600,
            Height = 340,
            Background = System.Windows.Media.Brushes.White,
            Padding = new Thickness(5)
        };

        var grid = new Grid();
        outerBorder.Child = grid;

        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(106) });   // Название
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(70) });   // Артикул
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(145) });  // Описание + QR

        // Название
        var name = new TextBlock
        {
            Text = incoming.Stock.Nomenclature.Name,
            FontWeight = FontWeights.Bold,
            FontSize = 24,
            TextWrapping = TextWrapping.Wrap,
            TextAlignment = TextAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        grid.Children.Add(name);
        Grid.SetRow(name, 0);

        var line1 = CreateLine();
        grid.Children.Add(line1);
        Grid.SetRow(line1, 0);
        Grid.SetZIndex(line1, -1); // За текстом

        // Артикул
        var article = new TextBlock
        {
            Text = incoming.Stock.Nomenclature.Article,
            FontWeight = FontWeights.Bold,
            FontStyle = FontStyles.Italic,
            FontSize = 24,
            TextAlignment = TextAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        grid.Children.Add(article);
        Grid.SetRow(article, 1);

        var line2 = CreateLine();
        grid.Children.Add(line2);
        Grid.SetRow(line2, 1);
        Grid.SetZIndex(line2, -1);

        // Описание + QR
        var descQrGrid = new Grid();
        descQrGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        descQrGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) }); // QR ширина
        descQrGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(105) });   // Артикул
        descQrGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });

        var description = new TextBlock
        {
            Text = incoming.Application?.Equipment?.FullNameFromShortName ?? "",
            FontSize = 22,
            TextWrapping = TextWrapping.Wrap,
            TextAlignment = TextAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 10, 0, 0)
        };
        descQrGrid.Children.Add(description);
        Grid.SetColumn(description, 0);
        Grid.SetRow(description, 0);

        var qr = new System.Windows.Controls.Image
        {
            Source = GetQr(incoming.Stock.Nomenclature.Id.ToString(),170),
            Width = 140,
            Height = 140,
            Stretch = Stretch.Fill,
            VerticalAlignment = VerticalAlignment.Bottom,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(0, 0, 5, -5)
        };
        descQrGrid.Children.Add(qr);
        Grid.SetColumn(qr, 1);
        Grid.SetRowSpan(qr,2);
        Grid.SetZIndex(qr, -1);

        grid.Children.Add(descQrGrid);
        Grid.SetRow(descQrGrid, 2);

        // Линия под QR+описанием
        var line3 = CreateLine();
        descQrGrid.Children.Add(line3);
        Grid.SetColumn(line3, 0);
        Grid.SetRow(line3, 0);
        Grid.SetZIndex(line3, -1);

        // Дата поступления
        var date = new TextBlock
        {
            Text = $"Дата поступления: {incoming.Date:dd.MM.yyyy}",
            FontStyle = FontStyles.Italic,
            FontSize = 22,
            TextAlignment = TextAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 5, 0, 0)
        };
        descQrGrid.Children.Add(date);
        Grid.SetColumn(date, 0);
        Grid.SetRow(date, 1);

        return outerBorder;
    }

    // Горизонтальная линия
    private static Border CreateLine()
    {
        return new Border
        {
            BorderBrush = System.Windows.Media.Brushes.Black,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Margin = new Thickness(0, 0, 0, 0)
        };
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

            var labelWidth = (pageWidth - 2 * margin) / columns;
            var labelHeight = (pageHeight - 2 * margin) / rows;

            var labelWidthScaled = labelWidth * PrintScale / 100.0;
            var labelHeightScaled = labelHeight * PrintScale / 100.0;
            var rowOverride = (int)Math.Round(pageHeight / labelHeightScaled, MidpointRounding.ToZero);
            var columnOverride = (int)Math.Round(pageWidth / labelWidthScaled, MidpointRounding.ToZero);
            var labelsPerPage = rowOverride * columnOverride;

            var doc = new FixedDocument
            {
                DocumentPaginator = { PageSize = new Size(pageWidth, pageHeight) }
            };

            var labels = LabelListForShow
                .SelectMany(i => Enumerable.Repeat(i, (int)i.Quantity))
                .ToList();

            for (int i = 0; i < labels.Count; i += labelsPerPage)
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

                foreach (var item in pageLabels)
                {
                    var label = CreateLabelWpf(item);

                    var viewbox = new Viewbox
                    {
                        Width = labelWidthScaled,
                        Height = labelHeightScaled,
                        Stretch = Stretch.Fill,
                        Child = label
                    };

                    grid.Children.Add(viewbox);
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

    private static DrawingImage GetQr(string s, int desiredSize = 170)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(s, QRCodeGenerator.ECCLevel.H);

        int moduleCount = qrCodeData.ModuleMatrix.Count;
        int pixelsPerModule = desiredSize / moduleCount;

        var qrCode = new QRCode(qrCodeData);
        using var bitmap = qrCode.GetGraphic(pixelsPerModule, Color.Black, Color.White, drawQuietZones: false);

        return BitmapToDrawingImage(bitmap);
    }

    private static DrawingImage BitmapToDrawingImage(Bitmap bitmap)
    {
        var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            bitmap.GetHbitmap(),
            IntPtr.Zero,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());

        var drawing = new ImageDrawing(bitmapSource, new Rect(0, 0, bitmap.Width, bitmap.Height));
        return new DrawingImage(drawing);
    }

    private static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
    {
        using var memory = new MemoryStream();
        //bitmap.SetResolution(300, 300); // Важно для качества
        bitmap.Save(memory, ImageFormat.Png);
        memory.Position = 0;

        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = memory;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.DecodePixelWidth = bitmap.Width; // Добавьте это, чтобы WPF не делал сжатие
        bitmapImage.EndInit();
        bitmapImage.Freeze();
        return bitmapImage;
    }


    public async Task Init(List<Incoming> labelList)
    {
        LabelList = new ObservableCollection<Incoming>(labelList);
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
                MessagesEnum.UpdateLabelPrintList, typeof(RequestManagement.WpfClient.ViewModels.IncomingListViewModel), Enumerable.ToList<Incoming>(LabelList)));
        Images.Clear();
        if (!string.IsNullOrEmpty(FastSearchText))
        {
            LabelListForShow = new ObservableCollection<Incoming>(Enumerable.Where<Incoming>(LabelList, x => x.Stock.Nomenclature.Name.ToLower().Contains(FastSearchText.ToLower())
                                                                                                            || x.Stock.Nomenclature.Article!.ToLower().Contains(FastSearchText.ToLower())));
        }
        else
        {
            LabelListForShow = new ObservableCollection<Incoming>(LabelList);
        }
        await ProcessImagesAsync();
        IncomingsViewSource = new CollectionViewSource
        {
            Source = LabelListForShow
        };
    }
    private async Task ProcessImagesAsync()
    {
        foreach (var label in LabelListForShow)
        {
            for (var i = 0; i < label.Quantity; i++)
            {
                // Можно параллельно, если нужно — собрать данные, но сам UI создаём в UI-потоке:
                var image = await Application.Current.Dispatcher.InvokeAsync(() => CreateLabelWpf(label));

                Images.Add(new ImageItem { Image = image });
            }
        }
    }

    [RelayCommand]
    private async Task DeleteItem(Incoming item)
    {
        LabelList.Remove(item);
        await UpdateLabelList();
    }
}