using System.Windows;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RequestManagement.WpfClient.Models;

public partial class ImageItem : ObservableObject
{
    [ObservableProperty]
    private FrameworkElement _image;
}