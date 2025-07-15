using System.Text.RegularExpressions;
using System.Windows.Input;
using RequestManagement.WpfClient.ViewModels;

namespace RequestManagement.WpfClient.Views;

/// <summary>
/// Логика взаимодействия для StockView.xaml
/// </summary>
public partial class StockView
{
    private readonly bool _editMode;
    public StockView(StockViewModel viewModel,bool editMode)
    {
        InitializeComponent();
        DataContext = viewModel;
        _editMode = editMode;
    }
    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9,]+");
        e.Handled = regex.IsMatch(e.Text);
    }
}