using System.Text.RegularExpressions;
using System.Windows.Input;
using WpfClient.ViewModels;

namespace WpfClient.Views;

/// <summary>
/// Логика взаимодействия для ExpenseView.xaml
/// </summary>
public partial class ExpenseView
{
    public ExpenseView(ExpenseViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}