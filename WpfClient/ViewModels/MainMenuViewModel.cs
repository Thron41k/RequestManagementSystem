using System.Windows;
using System.Windows.Input;
using RequestManagement.Common.Models;
using WpfClient.Helpers;
using WpfClient.Messages;
using WpfClient.Services.Interfaces;
using WpfClient.Views;
using System.Windows.Controls;

namespace WpfClient.ViewModels;

public class MainMenuViewModel
{
    private readonly EquipmentViewModel _equipmentViewModel;
    private readonly DriverViewModel _driverViewModel;
    private readonly DefectGroupViewModel _defectGroupViewModel;
    private readonly DefectViewModel _defectViewModel;
    private readonly WarehouseViewModel _warehouseViewModel;
    private readonly NomenclatureViewModel _nomenclatureViewModel;
    private readonly StockViewModel _stockViewModel;
    private readonly IMessageBus _messageBus;
    public UserControl StockControlProperty { get; }
    public ICommand ShowEquipmentCommand { get; }
    public ICommand ShowDriverCommand { get; }
    public ICommand ShowDefectGroupCommand { get; }
    public ICommand ShowDefectCommand { get; }
    public ICommand ShowWarehouseCommand { get; }
    public ICommand ShowNomenclatureCommand { get; }
    public ICommand ShowStockCommand { get; }
    public MainMenuViewModel(EquipmentViewModel equipmentViewModel, DriverViewModel driverViewModel, DefectGroupViewModel defectGroupViewModel, DefectViewModel defectViewModel, WarehouseViewModel warehouseViewModel, NomenclatureViewModel nomenclatureViewModel, IMessageBus messageBus, StockViewModel stockViewModel)
    {
        _equipmentViewModel = equipmentViewModel;
        _driverViewModel = driverViewModel;
        _defectGroupViewModel = defectGroupViewModel;
        _defectViewModel = defectViewModel;
        _warehouseViewModel = warehouseViewModel;
        _nomenclatureViewModel = nomenclatureViewModel;
        _messageBus = messageBus;
        _stockViewModel = stockViewModel;
        StockControlProperty = new StockView(_stockViewModel, true);
        _messageBus.Subscribe<SelectTaskMessage>(OnSelect);
        ShowEquipmentCommand = new RelayCommand(ShowEquipment);
        ShowDriverCommand = new RelayCommand(ShowDriver);
        ShowDefectGroupCommand = new RelayCommand(ShowDefectGroup);
        ShowDefectCommand = new RelayCommand(ShowDefect);
        ShowWarehouseCommand = new RelayCommand(ShowWarehouse);
        ShowNomenclatureCommand = new RelayCommand(ShowNomenclature);
        ShowStockCommand = new RelayCommand(ShowStock);
    }
    private void ShowStock()
    {
        ShowStock(true);
    }
    private void ShowNomenclature()
    {
        ShowNomenclature(true);
    }
    private void ShowWarehouse()
    {
        ShowWarehouse(true);
    }
    private void ShowDefect()
    {
        ShowDefect(true);
    }
    private void ShowDefectGroup()
    {
        ShowDefectGroup(true);
    }
    private void ShowDriver()
    {
        ShowDriver(true);
    }
    private void ShowEquipment()
    {
        ShowEquipment(true);
    }
    private Task OnSelect(SelectTaskMessage arg)
    {
        switch (arg.Message)
        {
            case MessagesEnum.SelectNomenclature:
                ShowNomenclature(false, arg.Caller);
                break;
            case MessagesEnum.SelectWarehouse:
                ShowWarehouse(false, arg.Caller);
                break;
        }

        return Task.CompletedTask;
    }
    private void ShowStock(bool editMode, Type? argCaller = null)
    {
        var stockView = new StockView(_stockViewModel, editMode);
        var window = new Window
        {
            Content = stockView,
            Title = "Транспорт и ДСТ",
            Width = 1000,
            Height = 600
        };
        window.ShowDialog();
    }
    private void ShowEquipment(bool editMode, Type? argCaller = null)
    {
        var equipmentView = new EquipmentView(_equipmentViewModel, editMode);
        var window = new Window
        {
            Content = equipmentView,
            Title = "Транспорт и ДСТ",
            Width = 800,
            Height = 600
        };
        _ = _equipmentViewModel.Load();
        window.ShowDialog();
    }
    private void ShowDriver(bool editMode, Type? argCaller = null)
    {
        var driverView = new DriverView(_driverViewModel, editMode);
        var window = new Window
        {
            Content = driverView,
            Title = "Сотрудники",
            Width = 800,
            Height = 600
        };
        _ = _driverViewModel.Load();
        window.ShowDialog();
    }
    private void ShowDefectGroup(bool editMode, Type? argCaller = null)
    {
        var defectGroupView = new DefectGroupView(_defectGroupViewModel, editMode);
        var window = new Window
        {
            Content = defectGroupView,
            Title = "Группы дефектов",
            Width = 800,
            Height = 600
        };
        _ = _defectGroupViewModel.Load();
        window.ShowDialog();
    }
    private void ShowDefect(bool editMode, Type? argCaller = null)
    {
        var defectView = new DefectView(_defectViewModel, editMode);
        var window = new Window
        {
            Content = defectView,
            Title = "Дефекты",
            Width = 800,
            Height = 600
        };
        _ = _defectViewModel.Load();
        window.ShowDialog();
    }
    private void ShowWarehouse(bool editMode, Type? argCaller = null)
    {
        var warehouseView = new WarehouseView(_warehouseViewModel, editMode);
        var window = new Window
        {
            Content = warehouseView,
            Title = "Склады",
            Width = 800,
            Height = 600
        };
        _ = _warehouseViewModel.Load();
        window.ShowDialog();
        if (_warehouseViewModel.SelectedWarehouse != null && argCaller != null)
        {
            _messageBus.Publish(
                new SelectResultMessage(
                    MessagesEnum.SelectWarehouse, argCaller, new Warehouse
                    {
                        Id = _warehouseViewModel.SelectedWarehouse.Id,
                        Name = _warehouseViewModel.SelectedWarehouse.Name
                    }));
        }
    }
    private void ShowNomenclature(bool editMode, Type? argCaller = null)
    {
        var nomenclatureView = new NomenclatureView(_nomenclatureViewModel, editMode);
        var window = new Window
        {
            Content = nomenclatureView,
            Title = "Номенклатура",
            Width = 800,
            Height = 600
        };
        _ = _nomenclatureViewModel.Load();
        window.ShowDialog();
        if (_nomenclatureViewModel.SelectedNomenclature != null && argCaller != null)
            _messageBus.Publish(
                new SelectResultMessage(
                    MessagesEnum.SelectNomenclature, argCaller, new Nomenclature
                    {
                        Id = _nomenclatureViewModel.SelectedNomenclature.Id,
                        Code = _nomenclatureViewModel.SelectedNomenclature.Code,
                        Article = _nomenclatureViewModel.SelectedNomenclature.Article,
                        Name = _nomenclatureViewModel.SelectedNomenclature.Name,
                        UnitOfMeasure = _nomenclatureViewModel.SelectedNomenclature.UnitOfMeasure
                    }));

    }
}