using System.Collections.ObjectModel;
using System.Drawing.Printing;
using Azure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Models;
using RequestManagement.WpfClient.Messages;
using RequestManagement.WpfClient.Models;
using RequestManagement.WpfClient.Models.ExcelWriterModels;
using RequestManagement.WpfClient.Services.ExcelTemplate;
using RequestManagement.WpfClient.Services.Interfaces;

namespace RequestManagement.WpfClient.ViewModels;

public partial class PrintReportViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private readonly IPrinterService _printerService;
    private readonly IExcelWriterService _excelWriterService;
    private DateTime _startDate = DateTime.Now;
    private DateTime _endDate = DateTime.Now;
    private List<Expense> _listExpense = new();
    private List<Incoming> _listIncoming = new();
    private List<MaterialsInUse> _listMaterialsInUse = new();
    private PrintDialogType _printDialogType = PrintDialogType.None;
    [ObservableProperty] private Commissions? _selectedCommissions = new();
    [ObservableProperty] private Driver? _selectedFrp = new();
    [ObservableProperty] private ObservableCollection<string> _printerList = [];
    [ObservableProperty] private string _selectedPrinter = "";
    [ObservableProperty] private int _selectedTypeDocumentForPrint = -1;
    [ObservableProperty] private ObservableCollection<string> _docTypeList = [];
    [ObservableProperty] private bool _incomingMode;
    public PrintReportViewModel()
    {

    }
    public PrintReportViewModel(IMessageBus messageBus, IPrinterService printerService, IExcelWriterService excelWriterService)
    {
        _messageBus = messageBus;
        _printerService = printerService;
        _excelWriterService = excelWriterService;
        _messageBus.Subscribe<SelectResultMessage>(OnSelect);
    }

    public void Init(List<Incoming> list, DateTime startDate, DateTime endDate)
    {
        _printDialogType = PrintDialogType.Incoming;
        IncomingMode = true;
        DocTypeList =
        [
            "Требования-накладные",
            "Перемещения между складами"
        ];
        PrinterList = new ObservableCollection<string>(PrinterSettings.InstalledPrinters);
        SelectedPrinter = _printerService.GetDefaultPrinterName();
        _listIncoming = list;
        _startDate = startDate;
        _endDate = endDate;
    }

    public void Init(List<Expense> list, DateTime startDate, DateTime endDate)
    {
        _printDialogType = PrintDialogType.Expense;
        IncomingMode = false;
        DocTypeList =
        [
            "Акты", "Лимитные карты", "Дефектные ведомости", "Ведомости выдачи АКБ и Автошин",
                "Расходные материалы", "Эксплуатация"
        ];
        PrinterList = new ObservableCollection<string>(PrinterSettings.InstalledPrinters);
        SelectedPrinter = _printerService.GetDefaultPrinterName();
        _listExpense = list;
        _startDate = startDate;
        _endDate = endDate;
    }
    private Task OnSelect(SelectResultMessage arg)
    {
        if (arg.Caller == typeof(PrintReportViewModel) && arg.Item != null)
            switch (arg.Message)
            {
                case MessagesEnum.SelectDriver:
                    SelectedFrp = (Driver)arg.Item;
                    break;
                case MessagesEnum.SelectCommissions:
                    SelectedCommissions = (Commissions)arg.Item;
                    break;
            }
        return null!;
    }

    [RelayCommand]
    private void ClearSelectedFrp()
    {
        SelectedFrp = new();
    }
    [RelayCommand]
    private void SelectFrp()
    {
        _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectDriver, typeof(PrintReportViewModel)));
    }
    [RelayCommand]
    private void ClearSelectedCommissions()
    {
        SelectedCommissions = new();
    }
    [RelayCommand]
    private void SelectCommissions()
    {
        _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectCommissions, typeof(PrintReportViewModel)));
    }

    [RelayCommand]
    private void SaveToFile()
    {
        switch (_printDialogType)
        {
            case PrintDialogType.None:
                break;
            case PrintDialogType.Incoming:
                if (SelectedCommissions == null) return;
                switch (SelectedTypeDocumentForPrint)
                {
                    case 0:
                        var requisitionInvoices = new IncomingPrintModel
                        {
                            Commissions = SelectedCommissions,
                            StartDate = _startDate,
                            EndDate = _endDate,
                            Incomings = _listIncoming
                        };
                        _excelWriterService.ExportAndSave(ExcelTemplateType.RequisitionInvoice, requisitionInvoices,
                            "Перемещения");
                        break;
                    case 1:
                        var movingBetweenWarehouses = new IncomingPrintModel
                        {
                            Commissions = SelectedCommissions,
                            StartDate = _startDate,
                            EndDate = _endDate,
                            Incomings = _listIncoming.Where(x => x.DocType == "Перемещение").ToList()
                        };
                        _excelWriterService.ExportAndSave(ExcelTemplateType.MovingBetweenWarehouses,
                            movingBetweenWarehouses, "Перемещения между складами");
                        break;
                }

                break;
            case PrintDialogType.Expense:
                if (SelectedCommissions == null && SelectedFrp == null) return;
                switch (SelectedTypeDocumentForPrint)
                {
                    case 0:
                        var actParts = new ActPartsModel
                        {
                            Commissions = SelectedCommissions,
                            Frp = SelectedFrp,
                            Expenses = _listExpense
                        };
                        _excelWriterService.ExportAndSave(ExcelTemplateType.ActParts, actParts, "Акты");
                        break;
                    case 1:
                        var limitParts = new ActPartsModel
                        {
                            Commissions = SelectedCommissions,
                            Frp = SelectedFrp,
                            Expenses = _listExpense,
                            StartDate = _startDate,
                            EndDate = _endDate
                        };
                        _excelWriterService.ExportAndSave(ExcelTemplateType.LimitParts, limitParts, "Лимитные карты");
                        break;
                    case 2:
                        var defectParts = new ActPartsModel
                        {
                            Commissions = SelectedCommissions,
                            Frp = SelectedFrp,
                            Expenses = _listExpense,
                            StartDate = _startDate,
                            EndDate = _endDate
                        };
                        _excelWriterService.ExportAndSave(ExcelTemplateType.DefectParts, defectParts,
                            "Дефектные ведомости");
                        break;
                    case 3:
                        var mb7 = new ActPartsModel
                        {
                            Commissions = SelectedCommissions,
                            Frp = SelectedFrp,
                            Expenses = _listExpense,
                            StartDate = _startDate,
                            EndDate = _endDate
                        };
                        _excelWriterService.ExportAndSave(ExcelTemplateType.Mb7Parts, mb7,
                            "Ведомость учета выдачи автошин и АКБ");
                        break;
                    case 4:
                        var consumables = new ActPartsModel
                        {
                            Commissions = SelectedCommissions,
                            Frp = SelectedFrp,
                            Expenses = _listExpense,
                            StartDate = _startDate,
                            EndDate = _endDate
                        };
                        _excelWriterService.ExportAndSave(ExcelTemplateType.Consumables, consumables,
                            "Расходные материалы");
                        break;
                    case 5:
                        var operations = new ActPartsModel
                        {
                            Commissions = SelectedCommissions,
                            Frp = SelectedFrp,
                            Expenses = _listExpense,
                            StartDate = _startDate,
                            EndDate = _endDate
                        };
                        _excelWriterService.ExportAndSave(ExcelTemplateType.Operations, operations,
                            "Передача в эксплуатацию");
                        break;
                }
                break;
            case PrintDialogType.MaterialsInUse:
                if (SelectedCommissions == null) return;
                switch (SelectedTypeDocumentForPrint)
                {
                    case 0:
                    {
                        var materialsInUseOffPrintModel = new MaterialsInUseOffPrintModel
                        {
                            Commissions = SelectedCommissions,
                            MaterialsInUse = _listMaterialsInUse.Where(x=>x.ReasonForWriteOffId is 2 or 12).ToList()
                        };
                        _excelWriterService.ExportAndSave(ExcelTemplateType.MaterialsInUseOffTemplateV1,
                            materialsInUseOffPrintModel,
                            "Списание АКБ и Автошин");
                        break;
                    }
                    case 1:
                    {
                        var materialsInUseOffPrintModel = new MaterialsInUseOffPrintModel
                        {
                            Commissions = SelectedCommissions,
                            MaterialsInUse = _listMaterialsInUse.Where(x => x.ReasonForWriteOffId is not 2 and 12).ToList()
                        };
                        _excelWriterService.ExportAndSave(ExcelTemplateType.MaterialsInUseOffTemplateV2,
                            materialsInUseOffPrintModel,
                            "Списание АКБ и Автошин");
                        break;
                    }
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Init(List<MaterialsInUse> messageMaterialsInUse)
    {
        _listMaterialsInUse = messageMaterialsInUse;
        _printDialogType = PrintDialogType.MaterialsInUse;
        IncomingMode = true;
        DocTypeList =
        [
            "Списание АКБ и Автошин",
            "Списание малоценки"
        ];
        PrinterList = new ObservableCollection<string>(PrinterSettings.InstalledPrinters);
        SelectedPrinter = _printerService.GetDefaultPrinterName();
    }
}