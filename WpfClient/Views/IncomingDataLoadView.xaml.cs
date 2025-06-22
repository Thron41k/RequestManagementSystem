using System.Windows.Controls;
using WpfClient.ViewModels;

namespace WpfClient.Views;

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