using System.Windows;
using RequestsViewModel = WpfClient.ViewModels.RequestsViewModel;

namespace WpfClient.Views;

public partial class RequestsWindow : Window
{
    public RequestsWindow(RequestsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (s, e) => await viewModel.LoadRequestsAsync();
    }
}