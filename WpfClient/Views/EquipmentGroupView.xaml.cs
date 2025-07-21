using System.Windows;
using RequestManagement.WpfClient.ViewModels;

namespace RequestManagement.WpfClient.Views;

/// <summary>
/// Логика взаимодействия для EquipmentGroupView.xaml
/// </summary>
public partial class EquipmentGroupView
{
    private readonly bool _editMode;
    public EquipmentGroupView(EquipmentGroupViewModel viewModel, bool editMode)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.CloseWindowRequested += ViewModel_CloseWindowRequested;
        _editMode = editMode;
    }
    private void ViewModel_CloseWindowRequested(object? sender, EventArgs e)
    {
        if (!_editMode) Window.GetWindow(this)?.Close(); // Закрываем окно, содержащее UserControl
    }
}