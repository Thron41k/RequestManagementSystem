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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QRCoder;
using RequestManagement.Common.Models;
using RequestManagement.WpfClient.Messages;
using RequestManagement.WpfClient.Models;
using RequestManagement.WpfClient.Services.Interfaces;
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
    [ObservableProperty] private int _printScale = 100;
    [ObservableProperty] private string _fastSearchText = "";
    public event EventHandler CloseWindowRequested;

    public LabelPrintListViewModel(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public LabelPrintListViewModel()
    {
        
    }

    private Bitmap CreateLabel(Incoming i)
    {
        var bmp = new Bitmap(600, 340);
        var rectF = new RectangleF(10, 10, 580, 106);
        var g = Graphics.FromImage(bmp);
        var pen = new Pen(Brushes.Black, 2);
        g.Clear(Color.White);
        var q = GetQr(i.Stock.Nomenclature.Id.ToString());
        g.DrawImage(q, 410, 147);
        //g.FillRectangle(Brushes.Aqua, rectF);
        g.DrawLine(pen, 15, 116, 575, 116);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        var stringFormat = new StringFormat();
        stringFormat.Alignment = StringAlignment.Center;
        stringFormat.LineAlignment = StringAlignment.Center;
        g.DrawString(i.Stock.Nomenclature.Name, new Font("Arial", 18, System.Drawing.FontStyle.Bold), Brushes.Black, rectF, stringFormat);
        rectF = new RectangleF(10, 117, 580, 40);
        g.DrawLine(pen, 15, 156, 575, 156);
        stringFormat.LineAlignment = StringAlignment.Center;
        // g.FillRectangle(Brushes.Chartreuse, rectF);
        g.DrawString(i.Stock.Nomenclature.Article != "" ? $"{i.Stock.Nomenclature.Article}" : "", new Font("Arial", 18), Brushes.Black, rectF, stringFormat);
        rectF = new RectangleF(10, 158, 410, 135);
        //g.FillRectangle(Brushes.Yellow, rectF);
        g.DrawLine(pen, 15, 292, 405, 292);
        g.DrawString(i.Application?.Equipment?.FullName, new Font("Arial", 16, System.Drawing.FontStyle.Regular), Brushes.Black, rectF, stringFormat);
        rectF = new RectangleF(10, 293, 410, 40);
        //g.FillRectangle(Brushes.Crimson, rectF);
        g.DrawString($"Дата поступления: {i.Date.ToShortDateString()}", new Font("Arial", 18, System.Drawing.FontStyle.Italic), Brushes.Black, rectF, stringFormat);
        g.DrawLine(pen, 0, 1, 600, 1);
        g.DrawLine(pen, 0, 339, 600, 339);
        g.DrawLine(pen, 1, 0, 1, 340);
        g.DrawLine(pen, 599, 0, 599, 340);
        g.Flush();
        return bmp;
    }
    [RelayCommand]
    private void Print()
    {
        var printDialog = new PrintDialog();
        if (printDialog.ShowDialog() == true)
        {
            var pageWidth = 794;// * PrintScale/100.0; // A4 @ 96 DPI
            var pageHeight = 1123;// * PrintScale / 100.0;
            const double margin = 20;
            var labelWidth = (pageWidth - 2 * margin) / Columns;
            var labelHeight = (pageHeight - 2 * margin) / Rows;
            var labelWidthScaled = labelWidth * PrintScale / 100.0;
            var labelHeightScaled = labelHeight * PrintScale / 100.0;
            var rowOverride = (int)Math.Round(pageHeight / labelHeightScaled, MidpointRounding.ToZero);
            var columnOverride = (int)Math.Round(pageWidth / labelWidthScaled, MidpointRounding.ToZero);
            var labelsPerPage = rowOverride * columnOverride;
            var doc = new FixedDocument
            {
                DocumentPaginator = { PageSize = new Size(pageWidth, pageHeight) }
            };

            // Разбиваем изображения на группы по 8 этикеток
            for (var i = 0; i < Images.Count; i += labelsPerPage)
            {
                var pageImages = Enumerable.Skip<ImageItem>(Images, i).Take(labelsPerPage).ToList();

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
                    Margin = new Thickness(margin),
                    VerticalAlignment = VerticalAlignment.Top
                };

                foreach (var item in pageImages)
                {
                    var sourceImage = ResizeImage(item.Image, labelWidthScaled, labelHeightScaled); // Высота этикетки

                    var img = new System.Windows.Controls.Image
                    {
                        Source = sourceImage,
                        Width = sourceImage.Width,
                        Height = sourceImage.Height,
                        Margin = new Thickness(0),
                        Stretch = Stretch.Uniform,
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    grid.Children.Add(img);
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
    private static Bitmap GetQr(string s)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(s, QRCodeGenerator.ECCLevel.H);
        using var qrCode = new QRCode(qrCodeData);
        var r = qrCode.GetGraphic(20);
        return ResizeImage(r, 196, 196);
    }
    private static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
    {
        var destRect = new Rectangle(0, 0, width, height);
        var destImage = new Bitmap(width, height);
        destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
        using var graphics = Graphics.FromImage(destImage);
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        using var wrapMode = new ImageAttributes();
        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
        return destImage;
    }
    private static BitmapSource ResizeImage(BitmapSource source, double maxWidth, double maxHeight)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        var scaleX = maxWidth / source.PixelWidth;
        var scaleY = maxHeight / source.PixelHeight;
        var scale = Math.Min(scaleX, scaleY);
        if (scale >= 1.0)
            return source;
        var transform = new ScaleTransform(scale, scale);
        var scaledBitmap = new TransformedBitmap(source, transform);
        var resized = new WriteableBitmap(scaledBitmap);
        return resized;
    }
    private static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
    {
        var memory = new MemoryStream();
        bitmap.Save(memory, ImageFormat.Png);
        memory.Position = 0;
        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = memory;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
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
                var image = await Task.Run(() =>
                {
                    var bitmap = CreateLabel(label);
                    return BitmapToBitmapImage(bitmap);
                });
                Images.Add(new ImageItem { Image = image });
                image.Freeze();
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