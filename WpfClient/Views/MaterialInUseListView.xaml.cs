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
using RequestManagement.WpfClient.ViewModels;

namespace RequestManagement.WpfClient.Views
{
    /// <summary>
    /// Логика взаимодействия для MaterialInUseListView.xaml
    /// </summary>
    public partial class MaterialInUseListView : UserControl
    {
        public MaterialInUseListView(MaterialInUseListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
