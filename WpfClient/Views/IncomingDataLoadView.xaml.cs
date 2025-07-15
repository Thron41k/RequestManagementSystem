using System.Windows.Controls;
using RequestManagement.WpfClient.ViewModels;

namespace RequestManagement.WpfClient.Views;

/// <summary>
/// Логика взаимодействия для IncomingDataLoadView.xaml
/// </summary>
public partial class IncomingDataLoadView : UserControl
{
    public IncomingDataLoadView(IncomingDataLoadViewModel incomingDataLoadViewModel)
    {
        InitializeComponent();
        DataContext = incomingDataLoadViewModel;
    }
}