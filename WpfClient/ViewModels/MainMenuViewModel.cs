using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Models;
using RequestManagement.Common.Models.Interfaces;
using RequestManagement.WpfClient.Messages;
using RequestManagement.WpfClient.Services.Interfaces;
using RequestManagement.WpfClient.Views;

namespace RequestManagement.WpfClient.ViewModels;

public class MainMenuViewModel
{
    private readonly SparePartsOwnershipViewModel _sparePartsOwnershipViewModel;
    private readonly EquipmentViewModel _equipmentViewModel;
    private readonly DriverViewModel _driverViewModel;
    private readonly EquipmentGroupViewModel _equipmentGroupViewModel;
    private readonly MaterialsInUseLoadViewModel _materialsInUseLoadViewModel;
    private readonly MaterialInUseListViewModel _materialInUseListViewModel;
    private readonly AddMaterialsInUseToOffViewModel _addMaterialsInUseToOffViewModel;
    private readonly ReasonsForWritingOffMaterialsFromOperationViewModel _reasonsForWritingOffMaterialsFromOperationViewModel;
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
    public ICommand ShowEquipmentGroupCommand { get; }
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
    public ICommand ShowSparePartsOwnershipCommand { get; }
    public ICommand ShowMaterialsInUseLoadingCommand { get; }
    public ICommand ShowMaterialInUseListCommand { get; }
    public ICommand ShowReasonsForWritingOffMaterialsFromOperationCommand { get; }

    public MainMenuViewModel(SparePartsOwnershipViewModel sparePartsOwnershipViewModel,
        EquipmentViewModel equipmentViewModel,
        DriverViewModel driverViewModel,
        DefectGroupViewModel defectGroupViewModel,
        DefectViewModel defectViewModel,
        WarehouseViewModel warehouseViewModel,
        NomenclatureViewModel nomenclatureViewModel,
        IMessageBus messageBus,
        StockViewModel stockViewModel,
        ExpenseViewModel expenseViewModel,
        ExpenseListViewModel expenseListViewModel,
        IncomingListViewModel incomingListViewModel,
        StartDataLoadViewModel startDataLoadViewModel,
        CommissionsViewModel commissionsViewModel,
        PrintReportViewModel printReportViewModel,
        ExpenseDataLoadViewModel expenseDataLoadViewModel,
        SparePartsAnalogsViewModel sparePartsAnalogsViewModel,
        IncomingDataLoadViewModel incomingDataLoadViewModel,
        LabelCountSelectorViewModel labelCountSelectorViewModel,
        LabelPrintListViewModel labelPrintListViewModel,
        EquipmentGroupViewModel equipmentGroupViewModel,
        MaterialsInUseLoadViewModel materialsInUseLoadViewModel,
        MaterialInUseListViewModel materialInUseListViewModel,
        AddMaterialsInUseToOffViewModel addMaterialsInUseToOffViewModel,
        ReasonsForWritingOffMaterialsFromOperationViewModel reasonsForWritingOffMaterialsFromOperationViewModel)
    {
        _sparePartsOwnershipViewModel = sparePartsOwnershipViewModel;
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
        _equipmentGroupViewModel = equipmentGroupViewModel;
        _materialsInUseLoadViewModel = materialsInUseLoadViewModel;
        _materialInUseListViewModel = materialInUseListViewModel;
        _addMaterialsInUseToOffViewModel = addMaterialsInUseToOffViewModel;
        _reasonsForWritingOffMaterialsFromOperationViewModel = reasonsForWritingOffMaterialsFromOperationViewModel;
        StockControlProperty = new StockView(_stockViewModel, true);
        _messageBus.Subscribe<SelectTaskMessage>(OnSelect);
        _messageBus.Subscribe<ShowTaskMessage>(OnShowDialog);
        _messageBus.Subscribe<ShowTaskPrintDialogMessageExpense>(OnShowPrintDialogExpense);
        _messageBus.Subscribe<ShowTaskPrintDialogMessageIncoming>(OnShowPrintDialogIncoming);
        _messageBus.Subscribe<ShowResultMessage>(OnShowResultMessageDialog);
        _messageBus.Subscribe<ShowResultMessageForMaterialsInUse>(OnShowResultMessageForMaterialsInUseDialog);
        ShowEquipmentCommand = new RelayCommand(ShowEquipment);
        ShowDriverCommand = new RelayCommand(ShowDriver);
        ShowDefectGroupCommand = new RelayCommand(ShowDefectGroup);
        ShowEquipmentGroupCommand = new RelayCommand(ShowEquipmentGroup);
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
        ShowSparePartsOwnershipCommand = new RelayCommand(ShowSparePartsOwnership);
        ShowMaterialsInUseLoadingCommand = new RelayCommand(ShowMaterialsInUseLoading);
        ShowMaterialInUseListCommand = new RelayCommand(ShowMaterialInUseList);
        ShowReasonsForWritingOffMaterialsFromOperationCommand = new RelayCommand(ShowReasonsForWritingOffMaterialsFromOperation);
    }

    private Task OnShowResultMessageForMaterialsInUseDialog(ShowResultMessageForMaterialsInUse arg)
    {
        if (arg.Message == MessagesEnum.ShowAddMaterialsInUseToOffView && arg.Caller == typeof(MaterialInUseListViewModel))
        {
            ShowAddMaterialsInUseToOffViewDialog(arg.DocumentNumber, arg.Reason, arg.DocumentDate, arg.Caller);
        }
        return Task.CompletedTask;
    }

    private void ShowMaterialInUseList()
    {
        var materialInUseListView = new MaterialInUseListView(_materialInUseListViewModel);
        var window = new Window
        {
            Content = materialInUseListView,
            Title = "Материалы в эксплуатации",
            Width = 820,
            Height = 710
        };
        window.ShowDialog();
    }

    private void ShowMaterialsInUseLoading()
    {
        var materialsInUseLoadView = new MaterialsInUseLoadView(_materialsInUseLoadViewModel);
        var window = new Window
        {
            Content = materialsInUseLoadView,
            Title = "Загрузка начальной эксплуатации",
            Width = 520,
            Height = 310,
            ResizeMode = ResizeMode.NoResize
        };
        _materialsInUseLoadViewModel.Init();
        window.ShowDialog();
    }

    private void ShowSparePartsOwnership()
    {
        var sparePartsOwnershipView = new SparePartsOwnershipView(_sparePartsOwnershipViewModel);
        var window = new Window
        {
            Content = sparePartsOwnershipView,
            Title = "Принадлежность номенклатуры",
            Width = 1000,
            Height = 1200
        };
        _ = _sparePartsOwnershipViewModel.InitializeAsync();
        window.ShowDialog();
    }

    private Task OnShowResultMessageDialog(ShowResultMessage arg)
    {
        if (arg.Message == MessagesEnum.ShowLabelCountSelector && arg.Caller == typeof(RequestManagement.WpfClient.ViewModels.IncomingListViewModel))
        {
            ShowLabelCountAccept(arg.Items, arg.Caller);
        }

        if (arg.Message == MessagesEnum.ShowLabelPrintListViewDialog && arg.Caller == typeof(RequestManagement.WpfClient.ViewModels.IncomingListViewModel))
        {
            ShowLabelPrintListViewDialog(arg.Items, arg.Caller);
        }
        return Task.CompletedTask;
    }


    private void ShowAddMaterialsInUseToOffViewDialog(string documentNumber, ReasonsForWritingOffMaterialsFromOperation reason, DateTime documentDate, Type argCaller)
    {
        var addMaterialsInUseToOffView = new AddMaterialsInUseToOffView(_addMaterialsInUseToOffViewModel);
        var window = new Window
        {
            Content = addMaterialsInUseToOffView,
            Title = "Списание материалов в эксплуатацию",
            Width = 420,
            Height = 280,
            ResizeMode = ResizeMode.NoResize
        };
        _addMaterialsInUseToOffViewModel.Init(reason, documentNumber, documentDate);
        window.ShowDialog();
        if (_addMaterialsInUseToOffViewModel.DialogResult)
        {
            _messageBus.Publish(
                new ShowResultMessageForMaterialsInUse(
                    MessagesEnum.ShowAddMaterialsInUseToOffViewResult,
                    argCaller,
                    _addMaterialsInUseToOffViewModel.DocumentNumber,
                    _addMaterialsInUseToOffViewModel.Reason,
                    _addMaterialsInUseToOffViewModel.DocumentDate));
        }
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
        _ = _labelPrintListViewModel.Init(labelList);
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
    private void ShowPrintReportExpense(List<Expense> list, DateTime startDate, DateTime endDate)
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
        _printReportViewModel.Init(list, startDate, endDate);
        window.ShowDialog();
    }

    private void ShowPrintReportIncoming(List<Incoming> list, DateTime startDate, DateTime endDate)
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
        _printReportViewModel.Init(list, startDate, endDate);
        window.ShowDialog();
    }

    private void ShowLabelCountAccept(List<Incoming> labelList, Type argCaller)
    {
        var labelCountSelectorView = new LabelCountSelectorView(_labelCountSelectorViewModel);
        var window = new Window
        {
            Content = labelCountSelectorView,
            Title = "Выбор количества этикеток для печати",
            Width = 1200,
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
            Width = 830,
            Height = 800,
            ResizeMode = ResizeMode.NoResize
        };
        _commissionsViewModel.EditMode = editMode;
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

    private void ShowEquipmentGroup()
    {
        ShowEquipmentGroup(true);
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
            case MessagesEnum.SelectEquipmentGroup:
                ShowEquipmentGroup(false, arg.Caller);
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
            case MessagesEnum.SelectReasonsForWritingOffMaterialsFromOperation:
                ShowReasonsForWritingOffMaterialsFromOperation(false, arg.Caller);
                break;
        }

        return Task.CompletedTask;
    }
    private async Task OnShowDialog(ShowTaskMessage arg)
    {
        switch (arg.Message)
        {
            case MessagesEnum.ShowExpenseDialog:
                await ShowExpenseDialog(arg.EditMode, arg.Caller, arg.Id, arg.Date, arg.Quantity, arg.Term, arg.Item);
                break;
        }
    }
    private Task OnShowPrintDialogExpense(ShowTaskPrintDialogMessageExpense arg)
    {
        switch (arg.Message)
        {
            case MessagesEnum.ShowPrintReportDialog:
                ShowPrintReportExpense(arg.Item, arg.FromDate, arg.ToDate);
                break;
        }

        return Task.CompletedTask;
    }

    private Task OnShowPrintDialogIncoming(ShowTaskPrintDialogMessageIncoming arg)
    {
        switch (arg.Message)
        {
            case MessagesEnum.ShowPrintReportDialog:
                ShowPrintReportIncoming(arg.Item, arg.FromDate, arg.ToDate);
                break;
        }

        return Task.CompletedTask;
    }

    private async Task ShowExpenseDialog(bool editMode, Type argCaller, int id, DateTime? date, decimal quantity, int? term, params IEntity?[] entity)
    {
        var expenseView = new ExpenseView(_expenseViewModel);
        var window = new Window
        {
            Content = expenseView,
            Title = editMode ? "Редактирование расхода" : "Добавить расход",
            Width = 675,
            Height = 550,
            ResizeMode = ResizeMode.NoResize
        };
        _expenseViewModel.SetExpenseId(id);
        _expenseViewModel.DialogResult = false;
        _expenseViewModel.EditMode = editMode;
        _expenseViewModel.SelectedEquipmentText = "";
        _expenseViewModel.TermForOperations = term != null && term != 0 ? term.Value.ToString() : "";
        _expenseViewModel.QuantityForExpense = "";
        if (date != null) _expenseViewModel.SelectedDate = date.Value;
        _expenseViewModel.SetCurrentQuantity(quantity);
        if (entity[0] != null) _expenseViewModel.ExpenseStock = entity[0] as Stock;
        _expenseViewModel.SetEquipment(entity[1] != null ? entity[1] as Equipment : new Equipment());
        _expenseViewModel.SelectedDriver = entity[2] != null ? entity[2] as Driver : new Driver();
        _expenseViewModel.SetDefect(entity[3] != null ? entity[3] as Defect : _expenseViewModel.SelectedDefect);
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
            Width = 1200,
            Height = 700
        };
        _equipmentViewModel.EditMode = editMode;
        _ = _equipmentViewModel.Load();
        window.ShowDialog();
        if (_equipmentViewModel.SelectedEquipment != null && argCaller != null && _equipmentViewModel.DialogResult)
            _messageBus.Publish(
                new SelectResultMessage(
                    MessagesEnum.SelectEquipment, argCaller, new Equipment
                    {
                        Id = _equipmentViewModel.SelectedEquipment.Id,
                        Name = _equipmentViewModel.SelectedEquipment.Name,
                        StateNumber = _equipmentViewModel.SelectedEquipment.StateNumber,
                        Code = _equipmentViewModel.SelectedEquipment.Code,
                        ShortName = _equipmentViewModel.SelectedEquipment.ShortName,
                        EquipmentGroup = _equipmentViewModel.SelectedEquipment.EquipmentGroup,
                        EquipmentGroupId = _equipmentViewModel.SelectedEquipment.EquipmentGroupId
                    }));
    }
    private void ShowDriver(bool editMode, Type? argCaller = null)
    {
        var driverView = new DriverView(_driverViewModel);
        var window = new Window
        {
            Content = driverView,
            Title = "Сотрудники",
            Width = 1070,
            Height = 700
        };
        _ = _driverViewModel.Load();
        _driverViewModel.EditMode = editMode;
        window.ShowDialog();
        if (_driverViewModel.SelectedDriver != null && argCaller != null && _driverViewModel.DialogResult)
            _messageBus.Publish(
                new SelectResultMessage(
                    MessagesEnum.SelectDriver, argCaller, new Driver
                    {
                        Id = _driverViewModel.SelectedDriver.Id,
                        FullName = _driverViewModel.SelectedDriver.FullName,
                        ShortName = _driverViewModel.SelectedDriver.ShortName,
                        Position = _driverViewModel.SelectedDriver.Position,
                        Code = _driverViewModel.SelectedDriver.Code
                    }));
    }
    private void ShowDefectGroup(bool editMode, Type? argCaller = null)
    {
        var defectGroupView = new DefectGroupView(_defectGroupViewModel, editMode);
        var window = new Window
        {
            Content = defectGroupView,
            Title = "Группы дефектов",
            Width = 770,
            Height = 600
        };
        _ = _defectGroupViewModel.Load();
        _defectGroupViewModel.EditMode = editMode;
        window.ShowDialog();
        if (_defectGroupViewModel.SelectedDefectGroup != null && argCaller != null && _defectGroupViewModel.DialogResult)
            _messageBus.Publish(
                new SelectResultMessage(
                    MessagesEnum.SelectDefectGroup, argCaller, new DefectGroup
                    {
                        Id = _defectGroupViewModel.SelectedDefectGroup.Id,
                        Name = _defectGroupViewModel.SelectedDefectGroup.Name
                    }));
    }

    private void ShowReasonsForWritingOffMaterialsFromOperation()
    {
        ShowReasonsForWritingOffMaterialsFromOperation(true);
    }

    private void ShowReasonsForWritingOffMaterialsFromOperation(bool editMode, Type? argCaller = null)
    {
        var reasonsForWritingOffMaterialsFromOperationView = new ReasonsForWritingOffMaterialsFromOperationView(_reasonsForWritingOffMaterialsFromOperationViewModel, editMode);
        var window = new Window
        {
            Content = reasonsForWritingOffMaterialsFromOperationView,
            Title = "Причины списания материалов из эксплуатации",
            Width = 770,
            Height = 600
        };
        _ = _reasonsForWritingOffMaterialsFromOperationViewModel.Load();
        _reasonsForWritingOffMaterialsFromOperationViewModel.EditMode = editMode;
        window.ShowDialog();
        if (_reasonsForWritingOffMaterialsFromOperationViewModel.SelectedReasonsForWritingOffMaterialsFromOperation != null && 
            argCaller != null && _reasonsForWritingOffMaterialsFromOperationViewModel.DialogResult)
            _messageBus.Publish(
                new SelectResultMessage(
                    MessagesEnum.ResultReasonsForWritingOffMaterialsFromOperation, argCaller, new ReasonsForWritingOffMaterialsFromOperation
                    {
                        Id = _reasonsForWritingOffMaterialsFromOperationViewModel.SelectedReasonsForWritingOffMaterialsFromOperation.Id,
                        Reason = _reasonsForWritingOffMaterialsFromOperationViewModel.SelectedReasonsForWritingOffMaterialsFromOperation.Reason
                    }));
    }
    private void ShowEquipmentGroup(bool editMode, Type? argCaller = null)
    {
        var equipmentGroupView = new EquipmentGroupView(_equipmentGroupViewModel, editMode);
        var window = new Window
        {
            Content = equipmentGroupView,
            Title = "Группы техники",
            Width = 770,
            Height = 600
        };
        _ = _equipmentGroupViewModel.Load();
        _equipmentGroupViewModel.EditMode = editMode;
        window.ShowDialog();
        if (_equipmentGroupViewModel.SelectedEquipmentGroup != null && argCaller != null && _equipmentGroupViewModel.DialogResult)
            _messageBus.Publish(
                new SelectResultMessage(
                    MessagesEnum.SelectEquipmentGroup, argCaller, new EquipmentGroup
                    {
                        Id = _equipmentGroupViewModel.SelectedEquipmentGroup.Id,
                        Name = _equipmentGroupViewModel.SelectedEquipmentGroup.Name
                    }));
    }
    private void ShowDefect(bool editMode, Type? argCaller = null)
    {
        var defectView = new DefectView(_defectViewModel);
        var window = new Window
        {
            Content = defectView,
            Title = "Дефекты",
            Width = 815,
            Height = 600
        };
        _ = _defectViewModel.Load();
        _defectViewModel.EditMode = editMode;
        window.ShowDialog();
        if (_defectViewModel.SelectedDefect != null && argCaller != null && _defectViewModel.DialogResult)
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
        _warehouseViewModel.EditMode = editMode;
        window.ShowDialog();
        if (_warehouseViewModel.SelectedWarehouse != null && argCaller != null && _warehouseViewModel.DialogResult)
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
        if (_nomenclatureViewModel.SelectedNomenclature != null && argCaller != null && _nomenclatureViewModel.DialogResult)
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