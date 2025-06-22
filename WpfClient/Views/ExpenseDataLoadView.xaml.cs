using System.Windows.Controls;
using WpfClient.ViewModels;

namespace WpfClient.Views;

/// <summary>
/// Логика взаимодействия для ExpenseDataLoadView.xaml
/// </summary>
public partial class ExpenseDataLoadView : UserControl
{
    public ExpenseDataLoadView(ExpenseDataLoadViewModel expenseDataLoadViewModel)
    {
        InitializeComponent();
        DataContext = expenseDataLoadViewModel;
    }
}