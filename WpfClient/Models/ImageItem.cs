using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media.Imaging;

namespace WpfClient.Models;

public partial class ImageItem : ObservableObject
{
    [ObservableProperty]
    private BitmapImage _image;
}