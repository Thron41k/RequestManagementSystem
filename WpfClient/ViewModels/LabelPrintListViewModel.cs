using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using QRCoder;
using RequestManagement.Common.Models;
using System.Drawing.Imaging;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.IO;
using WpfClient.Models;
using Microsoft.Office.Interop.Excel;
using Font = System.Drawing.Font;
using Rectangle = System.Drawing.Rectangle;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using Brushes = System.Drawing.Brushes;
using Color = System.Drawing.Color;
using Size = System.Windows.Size;

namespace WpfClient.ViewModels
{
    public partial class LabelPrintListViewModel : ObservableObject
    {
        private const int LabelPerPage = 8;
        [ObservableProperty] private ObservableCollection<Incoming> _labelList = [];
        [ObservableProperty] private ObservableCollection<ImageItem> _images = [];
        [ObservableProperty] private int _columns = 2;
        [ObservableProperty] private int _rows = 4;
        [ObservableProperty] private int _pageIndex = 0;
        public event EventHandler CloseWindowRequested;

        public LabelPrintListViewModel()
        {
          
        }

        private Bitmap CreateLabel(Incoming i)
        {
            const int pos1 = 20;
            var bmp = new Bitmap(600, 400);
            bmp.SetResolution(96, 96);
            var rectF = new RectangleF(10, 10, 580, 160);
            var g = Graphics.FromImage(bmp);
            g.Clear(Color.Black);
            g.FillRectangle(Brushes.White, 2, 2, 594, 394);
            var q = GetQr(i.Id.ToString());
            //g.FillRectangle(Brushes.Aqua, rectF);
            g.DrawImage(q, 400, 200);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            var stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            g.DrawString(i.Stock.Nomenclature.Name, new Font("Arial", 18, System.Drawing.FontStyle.Bold), Brushes.Black, rectF, stringFormat);
            rectF = new RectangleF(10, 172, 580, 40);
            stringFormat.LineAlignment = StringAlignment.Center;
            //g.FillRectangle(Brushes.Chartreuse, rectF);
            g.DrawString(i.Stock.Nomenclature.Article != "" ? $"{i.Stock.Nomenclature.Article}" : "", new Font("Arial", 16), Brushes.Black, rectF, stringFormat);
            rectF = new RectangleF(10, 214, 390, 135);
            //g.FillRectangle(Brushes.Yellow, rectF);
            g.DrawString(i.Application?.Equipment?.FullName, new Font("Arial", 16, System.Drawing.FontStyle.Regular), Brushes.Black, rectF, stringFormat);
            rectF = new RectangleF(10, 352, 390, 40);
            //g.FillRectangle(Brushes.Crimson, rectF);
            g.DrawString($"Дата поступления: {i.Date.ToShortDateString()}", new Font("Arial", 16, System.Drawing.FontStyle.Italic), Brushes.Black, rectF, stringFormat);
            g.Flush();
            return bmp;
        }

        [RelayCommand]
        public void Print()
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                var doc = new FixedDocument();
                doc.DocumentPaginator.PageSize = new Size(794, 1123); // A4 @ 96 DPI

                var pageContent = new PageContent();
                var fixedPage = new FixedPage
                {
                    Width = 794,
                    Height = 1123
                };

                var grid = new UniformGrid
                {
                    Columns = Columns,
                    Rows = Rows,
                    Margin = new Thickness(20)
                };
                //var tmpImages = Images.
                foreach (var item in Images.ToList().GetRange(0,6))
                {
                    var img = new System.Windows.Controls.Image
                    {
                        Source = item.Image,
                        Width = 280,
                        Height = 93,
                        Margin = new Thickness(5),
                        Stretch = Stretch.None
                    };
                    grid.Children.Add(img);
                }

                FixedPage.SetLeft(grid, 0);
                FixedPage.SetTop(grid, 0);
                fixedPage.Children.Add(grid);
                ((IAddChild)pageContent).AddChild(fixedPage);
                doc.Pages.Add(pageContent);

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

        private BitmapImage BitmapToBitmapImage(Bitmap bitmap)
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

        public void Init(List<Incoming> labelList)
        {
            LabelList = new ObservableCollection<Incoming>(labelList);
            UpdateLabelList();
        }

        private void UpdateLabelList()
        {
            var labelStartIndex = LabelPerPage * PageIndex;
            var labelCount = LabelList.Count >= labelStartIndex + LabelPerPage ? LabelPerPage : LabelList.Count - labelStartIndex;
            foreach (var label in LabelList.ToList().GetRange(LabelPerPage * PageIndex, labelCount))
            {
                for (var i = 0; i < label.Quantity; i++)
                {
                    var image = BitmapToBitmapImage(CreateLabel(label));
                    Images.Add(new ImageItem { Image = image });
                    image.Freeze();
                }
            }
        }
        partial void OnImagesChanged(ObservableCollection<global::WpfClient.Models.ImageItem> value)
        {
            Console.WriteLine(value.Count);
        }
    }
}
