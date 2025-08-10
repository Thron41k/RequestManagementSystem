using System.Windows;
using System.Windows.Controls;
using OneCOverlayClient.ViewModels;

namespace OneCOverlayClient.Views
{
    /// <summary>
    /// Логика взаимодействия для MaterialsInUseView.xaml
    /// </summary>
    public partial class MaterialsInUseView : UserControl
    {
        public MaterialsInUseView(MaterialsInUseViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.CloseWindowRequested += ViewModel_CloseWindowRequested;
            if (DataContext == null)
            {
                MessageBox.Show("DataContext не установлен!");
            }
        }

        private void ViewModel_CloseWindowRequested(object? sender, EventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }
    }
}
