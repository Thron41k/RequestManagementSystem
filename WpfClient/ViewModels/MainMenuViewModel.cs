using System.Windows;
using System.Windows.Input;
using WpfClient.Helpers;
using WpfClient.Views;

namespace WpfClient.ViewModels;

public class MainMenuViewModel
{
    private readonly EquipmentViewModel _equipmentViewModel;
    private readonly DriverViewModel _driverViewModel;
    private readonly DefectGroupViewModel _defectGroupViewModel;

    public ICommand ShowEquipmentCommand { get; }
    public ICommand ShowDriverCommand { get; }
    public ICommand ShowDefectGroupCommand { get; }

    public MainMenuViewModel(EquipmentViewModel equipmentViewModel,DriverViewModel driverViewModel,DefectGroupViewModel defectGroupViewModel)
    {
        _equipmentViewModel = equipmentViewModel;
        _driverViewModel = driverViewModel;
        _defectGroupViewModel = defectGroupViewModel;
        ShowEquipmentCommand = new RelayCommand(ShowEquipment);
        ShowDriverCommand = new RelayCommand(ShowDriver);
        ShowDefectGroupCommand = new RelayCommand(ShowDefectGroup);
    }

    private void ShowEquipment()
    {
        var equipmentView = new EquipmentView(_equipmentViewModel,true);
        var window = new Window
        {
            Content = equipmentView,
            Title = "Транспорт и ДСТ",
            Width = 800,
            Height = 600
        };
        window.Show();
        _ = _equipmentViewModel.Load();
    }
    private void ShowDriver()
    {
        var driverView = new DriverView(_driverViewModel, true);
        var window = new Window
        {
            Content = driverView,
            Title = "Сотрудники",
            Width = 800,
            Height = 600
        };
        window.Show();
        _ = _driverViewModel.Load();
    }
    private void ShowDefectGroup()
    {
        var defectGroupView = new DefectGroupView(_defectGroupViewModel, true);
        var window = new Window
        {
            Content = defectGroupView,
            Title = "Группы дефектов",
            Width = 800,
            Height = 600
        };
        window.Show();
        _ = _defectGroupViewModel.Load();
    }
}