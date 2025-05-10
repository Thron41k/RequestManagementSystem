using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfClient.ViewModels;

namespace WpfClient.Views
{
    /// <summary>
    /// Логика взаимодействия для SparePartsAnalogsView.xaml
    /// </summary>
    public partial class SparePartsAnalogsView : UserControl
    {
        public SparePartsAnalogsView(SparePartsAnalogsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
