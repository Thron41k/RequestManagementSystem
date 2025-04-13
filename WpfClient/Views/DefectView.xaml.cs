using System.Windows;
using WpfClient.ViewModels;

namespace WpfClient.Views;

/// <summary>
/// Логика взаимодействия для DefectsView.xaml
/// </summary>
public partial class DefectView
{
    public DefectView(DefectViewModel viewModel)
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