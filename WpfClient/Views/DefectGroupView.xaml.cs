using System.Windows;
using WpfClient.ViewModels;

namespace WpfClient.Views;

/// <summary>
/// Логика взаимодействия для DefectGroupsView.xaml
/// </summary>
public partial class DefectGroupView
{
    private readonly bool _editMode;
    public DefectGroupView(DefectGroupViewModel viewModel, bool editMode)
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