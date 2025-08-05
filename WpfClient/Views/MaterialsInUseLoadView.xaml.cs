using System.Windows.Controls;
using RequestManagement.WpfClient.ViewModels;

namespace RequestManagement.WpfClient.Views;

/// <summary>
/// Логика взаимодействия для MaterialsInUseLoadView.xaml
/// </summary>
public partial class MaterialsInUseLoadView : UserControl
{
    public MaterialsInUseLoadView(MaterialsInUseLoadViewModel materialsInUseLoadViewModel)
    {
        InitializeComponent();
        DataContext = materialsInUseLoadViewModel;
    }
}