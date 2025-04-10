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
    private readonly DefectViewModel _defectViewModel;
    private readonly WarehouseViewModel _warehouseViewModel;
    private readonly NomenclatureViewModel _nomenclatureViewModel;

    public ICommand ShowEquipmentCommand { get; }
    public ICommand ShowDriverCommand { get; }
    public ICommand ShowDefectGroupCommand { get; }
    public ICommand ShowDefectCommand { get; }
    public ICommand ShowWarehouseCommand { get; }
    public ICommand ShowNomenclatureCommand { get; }

    public MainMenuViewModel(EquipmentViewModel equipmentViewModel,DriverViewModel driverViewModel,DefectGroupViewModel defectGroupViewModel,DefectViewModel defectViewModel, WarehouseViewModel warehouseViewModel, NomenclatureViewModel nomenclatureViewModel)
    {
        _equipmentViewModel = equipmentViewModel;
        _driverViewModel = driverViewModel;
        _defectGroupViewModel = defectGroupViewModel;
        _defectViewModel = defectViewModel;
        _warehouseViewModel = warehouseViewModel;
        _nomenclatureViewModel = nomenclatureViewModel;
        ShowEquipmentCommand = new RelayCommand(ShowEquipment);
        ShowDriverCommand = new RelayCommand(ShowDriver);
        ShowDefectGroupCommand = new RelayCommand(ShowDefectGroup);
        ShowDefectCommand = new RelayCommand(ShowDefect);
        ShowWarehouseCommand = new RelayCommand(ShowWarehouse);
        ShowNomenclatureCommand = new RelayCommand(ShowNomenclature);
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

    private void ShowDefect()
    {
        var defectView = new DefectView(_defectViewModel, true);
        var window = new Window
        {
            Content = defectView,
            Title = "Дефекты",
            Width = 800,
            Height = 600
        };
        window.Show();
        _ = _defectViewModel.Load();
    }
    private void ShowWarehouse()
    {
        var warehouseView = new WarehouseView(_warehouseViewModel, true);
        var window = new Window
        {
            Content = warehouseView,
            Title = "Склады",
            Width = 800,
            Height = 600
        };
        window.Show();
        _ = _warehouseViewModel.Load();
    }

    private void ShowNomenclature()
    {
        var nomenclatureView = new NomenclatureView(_nomenclatureViewModel, true);
        var window = new Window
        {
            Content = nomenclatureView,
            Title = "Номенклатура",
            Width = 800,
            Height = 600
        };
        window.Show();
        _ = _nomenclatureViewModel.Load();
    }
}