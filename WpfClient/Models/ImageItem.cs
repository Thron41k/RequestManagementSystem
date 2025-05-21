using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfClient.Models
{
    public partial class ImageItem : ObservableObject
    {
        [ObservableProperty]
        private BitmapImage _image;
    }
}
