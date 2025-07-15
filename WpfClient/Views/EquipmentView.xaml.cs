using System.Windows;
using RequestManagement.WpfClient.ViewModels;

namespace RequestManagement.WpfClient.Views;

/// <summary>
/// Логика взаимодействия для EquipmentView.xaml
/// </summary>
public partial class EquipmentView
{

    public EquipmentView(EquipmentViewModel viewModel)
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