using System.Text.RegularExpressions;
using System.Windows.Input;
using WpfClient.ViewModels;

namespace WpfClient.Views;

/// <summary>
/// Логика взаимодействия для StockView.xaml
/// </summary>
public partial class ExpenseListView
{
    public ExpenseListView(ExpenseListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}