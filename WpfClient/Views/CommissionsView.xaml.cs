using System.Windows;
using WpfClient.ViewModels;

namespace WpfClient.Views
{
    /// <summary>
    /// Логика взаимодействия для CommissionsView.xaml
    /// </summary>
    public partial class CommissionsView
    {
        public CommissionsView(CommissionsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.CloseWindowRequested += ViewModel_CloseWindowRequested;
        }
        private void ViewModel_CloseWindowRequested(object? sender, EventArgs e)
        {
            Window.GetWindow(this)?.Close(); // Закрываем окно, содержащее UserControl
        }
    }
}
