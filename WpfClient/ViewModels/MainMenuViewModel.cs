using System.Windows;
using System.Windows.Input;
using RequestManagement.Common.Models;
using WpfClient.Messages;
using WpfClient.Services.Interfaces;
using WpfClient.Views;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Models.Interfaces;

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
    private readonly ExpenseViewModel _expenseViewModel;
    private readonly ExpenseListViewModel _expenseListViewModel;
    private readonly IncomingListViewModel _incomingListViewModel;
    private readonly StartDataLoadViewModel _startDataLoadViewModel;
    private readonly ExpenseDataLoadViewModel _expenseDataLoadViewModel;
    private readonly CommissionsViewModel _commissionsViewModel;
    private readonly PrintReportViewModel _printReportViewModel;
    private readonly SparePartsAnalogsViewModel _sparePartsAnalogsViewModel;
    private readonly IncomingDataLoadViewModel _incomingDataLoadViewModel;
    private readonly LabelCountSelectorViewModel _labelCountSelectorViewModel;
    private readonly LabelPrintListViewModel _labelPrintListViewModel;
    private readonly IMessageBus _messageBus;
    public UserControl StockControlProperty { get; }
    public ICommand ShowEquipmentCommand { get; }
    public ICommand ShowDriverCommand { get; }
    public ICommand ShowDefectGroupCommand { get; }
    public ICommand ShowDefectCommand { get; }
    public ICommand ShowWarehouseCommand { get; }
    public ICommand ShowNomenclatureCommand { get; }
    public ICommand ShowStockCommand { get; }
    public ICommand ShowExpensesCommand { get; }
    public ICommand ShowIncomingListCommand { get; }
    public ICommand ShowStartDataLoadingCommand { get; }
    public ICommand ShowCommissionsCommand { get; }
    public ICommand ShowExpensesDataLoadingCommand { get; }
    public ICommand ShowNomenclatureAnalogCommand { get; }
    public ICommand ShowIncomingDataLoadingCommand { get; }

    public MainMenuViewModel(EquipmentViewModel equipmentViewModel, DriverViewModel driverViewModel, DefectGroupViewModel defectGroupViewModel, DefectViewModel defectViewModel, WarehouseViewModel warehouseViewModel, NomenclatureViewModel nomenclatureViewModel, IMessageBus messageBus, StockViewModel stockViewModel, ExpenseViewModel expenseViewModel, ExpenseListViewModel expenseListViewModel, IncomingListViewModel incomingListViewModel, StartDataLoadViewModel startDataLoadViewModel, CommissionsViewModel commissionsViewModel, PrintReportViewModel printReportViewModel, ExpenseDataLoadViewModel expenseDataLoadViewModel, SparePartsAnalogsViewModel sparePartsAnalogsViewModel, IncomingDataLoadViewModel incomingDataLoadViewModel, LabelCountSelectorViewModel labelCountSelectorViewModel, LabelPrintListViewModel labelPrintListViewModel)
    {
        _equipmentViewModel = equipmentViewModel;
        _driverViewModel = driverViewModel;
        _defectGroupViewModel = defectGroupViewModel;
        _defectViewModel = defectViewModel;
        _warehouseViewModel = warehouseViewModel;
        _nomenclatureViewModel = nomenclatureViewModel;
        _messageBus = messageBus;
        _stockViewModel = stockViewModel;
        _expenseViewModel = expenseViewModel;
        _expenseListViewModel = expenseListViewModel;
        _incomingListViewModel = incomingListViewModel;
        _startDataLoadViewModel = startDataLoadViewModel;
        _commissionsViewModel = commissionsViewModel;
        _printReportViewModel = printReportViewModel;
        _expenseDataLoadViewModel = expenseDataLoadViewModel;
        _sparePartsAnalogsViewModel = sparePartsAnalogsViewModel;
        _incomingDataLoadViewModel = incomingDataLoadViewModel;
        _labelCountSelectorViewModel = labelCountSelectorViewModel;
        _labelPrintListViewModel = labelPrintListViewModel;
        StockControlProperty = new StockView(_stockViewModel, true);
        _messageBus.Subscribe<SelectTaskMessage>(OnSelect);
        _messageBus.Subscribe<ShowTaskMessage>(OnShowDialog);
        _messageBus.Subscribe<ShowTaskPrintDialogMessage>(OnShowPrintDialog);
        _messageBus.Subscribe<ShowResultMessage>(OnShowResultMessageDialog);
        ShowEquipmentCommand = new RelayCommand(ShowEquipment);
        ShowDriverCommand = new RelayCommand(ShowDriver);
        ShowDefectGroupCommand = new RelayCommand(ShowDefectGroup);
        ShowDefectCommand = new RelayCommand(ShowDefect);
        ShowWarehouseCommand = new RelayCommand(ShowWarehouse);
        ShowNomenclatureCommand = new RelayCommand(ShowNomenclature);
        ShowStockCommand = new RelayCommand(ShowStock);
        ShowExpensesCommand = new RelayCommand(ShowExpenses);
        ShowIncomingListCommand = new RelayCommand(ShowIncomingList);
        ShowStartDataLoadingCommand = new RelayCommand(ShowStartDataLoading);
        ShowCommissionsCommand = new RelayCommand(ShowCommissions);
        ShowExpensesDataLoadingCommand = new RelayCommand(ShowExpensesDataLoading);
        ShowNomenclatureAnalogCommand = new RelayCommand(ShowNomenclatureAnalog);
        ShowIncomingDataLoadingCommand = new RelayCommand(ShowIncomingDataLoading);
    }

    private Task OnShowResultMessageDialog(ShowResultMessage arg)
    {
        if (arg.Message == MessagesEnum.ShowLabelCountSelector && arg.Caller == typeof(IncomingListViewModel))
        {
            ShowLabelCountAccept(arg.Items,arg.Caller);
        }

        if (arg.Message == MessagesEnum.ShowLabelPrintListViewDialog && arg.Caller == typeof(IncomingListViewModel))
        {
            ShowLabelPrintListViewDialog(arg.Items, arg.Caller);
        }
        return Task.CompletedTask;
    }

    private void ShowLabelPrintListViewDialog(List<Incoming> labelList, Type argCaller)
    {
        var labelPrintListView = new LabelPrintListView(_labelPrintListViewModel);
        var window = new Window
        {
            Content = labelPrintListView,
            Title = "Печать этикеток",
            Width = 1500,
            Height = 820
        };
        _labelPrintListViewModel.Init(labelList);
        window.ShowDialog();
    }
    private void ShowIncomingDataLoading()
    {
        var incomingDataLoadView = new IncomingDataLoadView(_incomingDataLoadViewModel);
        var window = new Window
        {
            Content = incomingDataLoadView,
            Title = "Загрузка приходов",
            Width = 520,
            Height = 310,
            ResizeMode = ResizeMode.NoResize
        };
        _incomingDataLoadViewModel.Init();
        window.ShowDialog();
    }

    private void ShowNomenclatureAnalog()
    {
        var sparePartsAnalogsView = new SparePartsAnalogsView(_sparePartsAnalogsViewModel);
        var window = new Window
        {
            Content = sparePartsAnalogsView,
            Title = "Аналоги номенклатуры",
            Width = 850,
            Height = 510,
            ResizeMode = ResizeMode.NoResize
        };
        window.ShowDialog();
    }
    private void ShowCommissions()
    {
        ShowCommissions(true);
    }
    private void ShowPrintReport(List<Expense> list,DateTime startDate,DateTime endDate)
    {
        var printReportView = new PrintReportView(_printReportViewModel);
        var window = new Window
        {
            Content = printReportView,
            Title = "Печать отчётов",
            Width = 850,
            Height = 490,
            ResizeMode = ResizeMode.NoResize
        };
        _printReportViewModel.Init(list,startDate,endDate);
        window.ShowDialog();
    }

    private void ShowLabelCountAccept(List<Incoming> labelList, Type argCaller)
    {
        var labelCountSelectorView = new LabelCountSelectorView(_labelCountSelectorViewModel);
        var window = new Window
        {
            Content = labelCountSelectorView,
            Title = "Выбор количества этикеток для печати",
            Width = 600,
            Height = 400,
            ResizeMode = ResizeMode.NoResize
        };
        _labelCountSelectorViewModel.Init(labelList);
        window.ShowDialog();
        if (_labelCountSelectorViewModel.DialogResult)
        {
            _messageBus.Publish(
                new ShowResultMessage(
                    MessagesEnum.ResultLabelCountSelector, argCaller, _labelCountSelectorViewModel.LabelList.ToList()));
        }
    }
    private void ShowCommissions(bool editMode, Type? argCaller = null)
    {
        var commissionsView = new CommissionsView(_commissionsViewModel);
        var window = new Window
        {
            Content = commissionsView,
            Title = "Комиссия",
            Width = 850,
            Height = 490,
            ResizeMode = ResizeMode.NoResize
        };
        _ = _commissionsViewModel.Load();
        window.ShowDialog();
        if (argCaller != null)
            _messageBus.Publish(
                new SelectResultMessage(
                    MessagesEnum.SelectCommissions, argCaller, _commissionsViewModel.SelectedCommissions));
    }

    private void ShowStartDataLoading()
    {
        var startDataLoadView = new StartDataLoadView(_startDataLoadViewModel);
        var window = new Window
        {
            Content = startDataLoadView,
            Title = "Загрузка начальных остатков",
            Width = 570,
            Height = 310,
            ResizeMode = ResizeMode.NoResize
        };
        _startDataLoadViewModel.Init();
        window.ShowDialog();
    }

    private void ShowExpensesDataLoading()
    {
        var expenseDataLoadView = new ExpenseDataLoadView(_expenseDataLoadViewModel);
        var window = new Window
        {
            Content = expenseDataLoadView,
            Title = "Загрузка расходов",
            Width = 520,
            Height = 310,
            ResizeMode = ResizeMode.NoResize
        };
        _expenseDataLoadViewModel.Init();
        window.ShowDialog();
    }
    private async void ShowIncomingList()
    {
        var incomingView = new IncomingListView(_incomingListViewModel);
        var window = new Window
        {
            Content = incomingView,
            Title = "Приходы",
            Width = 620,
            Height = 330
        };
        await _incomingListViewModel.Load();
        window.ShowDialog();
    }

    private async void ShowExpenses()
    {
        var expenseView = new ExpenseListView(_expenseListViewModel);
        var window = new Window
        {
            Content = expenseView,
            Title = "Расходы",
            Width = 620,
            Height = 330
        };
        await _expenseListViewModel.Load();
        window.ShowDialog();
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
            case MessagesEnum.SelectDefect:
                ShowDefect(false, arg.Caller);
                break;
            case MessagesEnum.SelectDefectGroup:
                ShowDefectGroup(false, arg.Caller);
                break;
            case MessagesEnum.SelectDriver:
                ShowDriver(false, arg.Caller);
                break;
            case MessagesEnum.SelectEquipment:
                ShowEquipment(false, arg.Caller);
                break;
            case MessagesEnum.SelectCommissions:
                ShowCommissions(false, arg.Caller);
                break;
        }

        return Task.CompletedTask;
    }
    private async Task OnShowDialog(ShowTaskMessage arg)
    {
        switch (arg.Message)
        {
            case MessagesEnum.ShowExpenseDialog:
                await ShowExpenseDialog(arg.EditMode, arg.Caller, arg.Id, arg.Date, arg.Quantity, arg.Item);
                break;
        }
    }
    private async Task OnShowPrintDialog(ShowTaskPrintDialogMessage arg)
    {
        switch (arg.Message)
        {
            case MessagesEnum.ShowPrintReportDialog:
                ShowPrintReport(arg.Item,arg.FromDate,arg.ToDate);
                break;
        }
    }
    private async Task ShowExpenseDialog(bool editMode, Type argCaller, int id, DateTime? date, decimal quantity, params IEntity?[] entity)
    {
        var expenseView = new ExpenseView(_expenseViewModel);
        var window = new Window
        {
            Content = expenseView,
            Title = editMode ? "Редактирование расхода" : "Добавить расход",
            Width = 650,
            Height = 380,
            ResizeMode = ResizeMode.NoResize
        };
        _expenseViewModel.SetExpenseId(id);
        _expenseViewModel.DialogResult = false;
        _expenseViewModel.EditMode = editMode;
        _expenseViewModel.SelectedEquipmentText = "";
        _expenseViewModel.QuantityForExpense = "";
        if (date != null) _expenseViewModel.SelectedDate = date.Value;
        _expenseViewModel.SetCurrentQuantity(quantity);
        if (entity[0] != null) _expenseViewModel.ExpenseStock = entity[0] as Stock;
        _expenseViewModel.SetEquipment(entity[1] != null ? entity[1] as Equipment : new Equipment());
        _expenseViewModel.SelectedDriver = entity[2] != null ? entity[2] as Driver : new Driver();
        _expenseViewModel.SelectedDefect = entity[3] != null ? entity[3] as Defect : _expenseViewModel.SelectedDefect;
        await _expenseViewModel.LoadNomenclatureMapingAsync();
        window.ShowDialog();
        if (_expenseViewModel.DialogResult)
        {
            if (editMode)
            {
                await _expenseListViewModel.Load();
            }
            else
            {
                await _stockViewModel.LoadLastSelectionHistoryAsync();
                await _stockViewModel.LoadStocksAsync();
            }
        }
    }
    private void ShowStock(bool editMode, Type? argCaller = null)
    {
        var stockView = new StockView(_stockViewModel, editMode);
        var window = new Window
        {
            Content = stockView,
            Title = "Остатки на складах",
            Width = 1000,
            Height = 600
        };
        _stockViewModel.EditMode = editMode;
        window.ShowDialog();
    }
    private void ShowEquipment(bool editMode, Type? argCaller = null)
    {
        var equipmentView = new EquipmentView(_equipmentViewModel);
        var window = new Window
        {
            Content = equipmentView,
            Title = "Транспорт и ДСТ",
            Width = 800,
            Height = 600
        };
        _equipmentViewModel.EditMode = editMode;
        _ = _equipmentViewModel.Load();
        window.ShowDialog();
        if (_equipmentViewModel.SelectedEquipment != null && argCaller != null)
            _messageBus.Publish(
                new SelectResultMessage(
                    MessagesEnum.SelectEquipment, argCaller, new Equipment
                    {
                        Id = _equipmentViewModel.SelectedEquipment.Id,
                        Name = _equipmentViewModel.SelectedEquipment.Name,
                        StateNumber = _equipmentViewModel.SelectedEquipment.LicensePlate
                    }));
    }
    private void ShowDriver(bool editMode, Type? argCaller = null)
    {
        var driverView = new DriverView(_driverViewModel);
        var window = new Window
        {
            Content = driverView,
            Title = "Сотрудники",
            Width = 850,
            Height = 700
        };
        _ = _driverViewModel.Load();
        _driverViewModel.EditMode = editMode;
        window.ShowDialog();
        if (_driverViewModel.SelectedDriver != null && argCaller != null)
            _messageBus.Publish(
                new SelectResultMessage(
                    MessagesEnum.SelectDriver, argCaller, new Driver
                    {
                        Id = _driverViewModel.SelectedDriver.Id,
                        FullName = _driverViewModel.SelectedDriver.FullName,
                        ShortName = _driverViewModel.SelectedDriver.ShortName,
                        Position = _driverViewModel.SelectedDriver.Position
                    }));
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
        var defectView = new DefectView(_defectViewModel);
        var window = new Window
        {
            Content = defectView,
            Title = "Дефекты",
            Width = 800,
            Height = 600
        };
        _ = _defectViewModel.Load();
        _defectViewModel.EditMode = editMode;
        window.ShowDialog();
        if (_defectViewModel.SelectedDefect != null && argCaller != null)
            _messageBus.Publish(
                new SelectResultMessage(
                    MessagesEnum.SelectDefect, argCaller, new Defect
                    {
                        Id = _defectViewModel.SelectedDefect.Id,
                        Name = _defectViewModel.SelectedDefect.Name
                    }));
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