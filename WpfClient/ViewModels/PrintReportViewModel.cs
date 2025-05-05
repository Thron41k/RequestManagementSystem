using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Models;
using WpfClient.Messages;
using WpfClient.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using WpfClient.Models.ExcelWriterModels;
using WpfClient.Services.ExcelTemplate;

namespace WpfClient.ViewModels
{
    public partial class PrintReportViewModel : ObservableObject
    {
        public bool EditMode { get; set; }
        private readonly IMessageBus _messageBus;
        private readonly IPrinterService _printerService;
        private readonly IExcelWriterService _excelWriterService;
        private List<Expense> _listExpense = new();
        [ObservableProperty] private Commissions? _selectedCommissions = new();
        [ObservableProperty] private Driver? _selectedFrp = new();
        [ObservableProperty] private ObservableCollection<string> _printerList = [];
        [ObservableProperty] private string _selectedPrinter = "";
        [ObservableProperty] private bool _actPrinted = false;
        [ObservableProperty] private bool _limitPrinted = false;
        [ObservableProperty] private bool _defectPrinted = false;
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

        public void Init(List<Expense> list)
        {
            PrinterList = new ObservableCollection<string>(PrinterSettings.InstalledPrinters);
            SelectedPrinter = _printerService.GetDefaultPrinterName();
            _listExpense = list;
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
            if(SelectedCommissions == null && SelectedFrp == null) return;
            var actParts = new ActPartsModel
            {
                Commissions = SelectedCommissions,
                Frp = SelectedFrp,
                Expenses = _listExpense
            };
            _excelWriterService.ExportAndSave(ExcelTemplateType.ActParts, actParts,"Акты");
        }
    }
}
