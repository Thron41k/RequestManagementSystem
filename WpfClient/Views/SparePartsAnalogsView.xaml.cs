using System.Windows.Controls;
using WpfClient.ViewModels;

namespace WpfClient.Views;

/// <summary>
/// Логика взаимодействия для SparePartsAnalogsView.xaml
/// </summary>
public partial class SparePartsAnalogsView : UserControl
{
    public SparePartsAnalogsView(SparePartsAnalogsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}