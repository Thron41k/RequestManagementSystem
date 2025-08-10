using System.Windows;
using OneCOverlayClient.ViewModels;

namespace OneCOverlayClient.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            if (DataContext == null)
            {
                MessageBox.Show("DataContext не установлен!");
            }
        }
    }
}
