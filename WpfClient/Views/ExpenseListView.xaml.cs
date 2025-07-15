using RequestManagement.WpfClient.ViewModels;

namespace RequestManagement.WpfClient.Views;

/// <summary>
/// Логика взаимодействия для ExpenseListView.xaml
/// </summary>
public partial class ExpenseListView
{
    public ExpenseListView(ExpenseListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}