using System.Windows;
using System.Windows.Controls;
using WpfClient.ViewModels;

namespace WpfClient.Views;

/// <summary>
/// Логика взаимодействия для LabelPrintListView.xaml
/// </summary>
public partial class LabelPrintListView : UserControl
{
    public LabelPrintListView(LabelPrintListViewModel viewModel)
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