using System.Windows;
using WpfClient.ViewModels;

namespace WpfClient.Views;

/// <summary>
/// Логика взаимодействия для WarehouseView.xaml
/// </summary>
public partial class WarehouseView
{
    private readonly bool _editMode;
    public WarehouseView(WarehouseViewModel viewModel,bool editMode)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.CloseWindowRequested += ViewModel_CloseWindowRequested;
        _editMode = editMode;
    }
    private void ViewModel_CloseWindowRequested(object? sender, EventArgs e)
    {
        if(!_editMode) Window.GetWindow(this)?.Close(); // Закрываем окно, содержащее UserControl
    }
}