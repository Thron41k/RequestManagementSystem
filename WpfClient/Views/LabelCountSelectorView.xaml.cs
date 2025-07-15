using System.Windows;
using System.Windows.Controls;
using RequestManagement.WpfClient.ViewModels;

namespace RequestManagement.WpfClient.Views;

/// <summary>
/// Логика взаимодействия для LabelCountSelectorView.xaml
/// </summary>
public partial class LabelCountSelectorView : UserControl
{
    public LabelCountSelectorView(LabelCountSelectorViewModel viewModel)
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