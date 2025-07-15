using IncomingListViewModel = RequestManagement.WpfClient.ViewModels.IncomingListViewModel;

namespace RequestManagement.WpfClient.Views;

/// <summary>
/// Логика взаимодействия для IncomingListView.xaml
/// </summary>
public partial class IncomingListView
{
    public IncomingListView(IncomingListViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}