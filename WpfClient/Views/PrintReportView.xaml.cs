using System.Windows.Controls;
using WpfClient.ViewModels;

namespace WpfClient.Views;

/// <summary>
/// Логика взаимодействия для PrintReportView.xaml
/// </summary>
public partial class PrintReportView : UserControl
{
    public PrintReportView(PrintReportViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}