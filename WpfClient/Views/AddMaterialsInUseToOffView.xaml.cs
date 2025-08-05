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
    /// Логика взаимодействия для AddMaterialsInUseToOffView.xaml
    /// </summary>
    public partial class AddMaterialsInUseToOffView : UserControl
    {
        public AddMaterialsInUseToOffView(AddMaterialsInUseToOffViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.CloseWindowRequested += ViewModel_CloseWindowRequested;
        }

        private void ViewModel_CloseWindowRequested(object? sender, EventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }
    }
}
