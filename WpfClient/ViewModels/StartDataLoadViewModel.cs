using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfClient.Messages;
using WpfClient.Services.Interfaces;
using WpfClient.Models;
using Microsoft.Win32;
using System.Windows;
using RequestManagement.Common.Models;
using RequestManagement.Common.Interfaces;

namespace WpfClient.ViewModels
{
    public partial class StartDataLoadViewModel : ObservableObject
    {
        private readonly IMessageBus _messageBus;
        private readonly IExcelReaderService _excelReaderService;
        private readonly IWarehouseService _requestService;
        private readonly IStockService _stockService;
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private bool _isShowResultDialog;
        [ObservableProperty] private string _resultDialogText = "";
        [ObservableProperty] private string _documentPath = "";
        [ObservableProperty] private List<MaterialStock> _materialStocks = [];
        [ObservableProperty] private Warehouse? _selectedWarehouse = new();
        [ObservableProperty] private string _selectedWarehouseName = "";
        [ObservableProperty] private DateTime _toDate = DateTime.Now;
        [ObservableProperty] private int _materialIncomingCount;
        public StartDataLoadViewModel()
        {
           
        }

        public StartDataLoadViewModel(IMessageBus messageBus, IExcelReaderService excelReaderService, IWarehouseService requestService, IStockService stockService)
        {
            _messageBus = messageBus;
            _excelReaderService = excelReaderService;
            _requestService = requestService;
            _stockService = stockService;
            _messageBus.Subscribe<SelectResultMessage>(OnSelect);
        }

        public void Init()
        {
            DocumentPath = "";
            MaterialStocks = [];
            SelectedWarehouse = new Warehouse();
            ToDate = DateTime.Now;
        }

        private Task OnSelect(SelectResultMessage arg)
        {
            if (arg.Caller != typeof(StartDataLoadViewModel) || arg.Item == null) return Task.CompletedTask;
            switch (arg.Message)
            {
                case MessagesEnum.SelectWarehouse:
                    SelectedWarehouse = (Warehouse)arg.Item;
                    SelectedWarehouseName = SelectedWarehouse.Name;
                    break;
            }

            return Task.CompletedTask;
        }
        [RelayCommand]
        private void HideResultDialog()
        {
            IsShowResultDialog = false;
        }
        [RelayCommand]
        private void ClearDocumentPath()
        {
            DocumentPath = "";
            MaterialIncomingCount = 0;
        }

        [RelayCommand]
        private async Task UploadMaterials()
        {
            try
            {
                if (string.IsNullOrEmpty(DocumentPath)) return;
                if (MaterialStocks.Count == 0) return;
                if (string.IsNullOrEmpty(SelectedWarehouseName)) return;
                IsBusy = true;
                var result =
                    await _stockService.UploadMaterialsStockAsync(MaterialStocks, SelectedWarehouse.Id, ToDate);
                ResultDialogText = result ? "Data uploaded successfully" : "Error uploading data";
                IsShowResultDialog = true;
            }
            catch (Exception ex)
            {
                ResultDialogText = "Error loading data";
                IsShowResultDialog = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SelectDocumentPath()
        {
            try
            {
                var openFile = new OpenFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx"
                };
                var dialogResult = openFile.ShowDialog();
                if (dialogResult == true && openFile.FileName.Length > 0)
                {
                    DocumentPath = openFile.FileName;
                    var result = _excelReaderService.ReadMaterialStock(DocumentPath);
                    if (result.materialStocks is { Count: > 0 })
                    {
                        MaterialStocks = result.materialStocks;
                        MaterialIncomingCount = result.materialStocks.Count;
                        if (result.date != null) ToDate = DateTime.Parse(result.date);
                        if (result.warehouse != null)
                        {
                            SelectedWarehouse = await _requestService.GetOrCreateWarehousesAsync(result.warehouse);
                            SelectedWarehouseName = SelectedWarehouse.Name;
                        }
                    }
                    else
                    {
                        MaterialIncomingCount = 0;
                        SelectedWarehouse = new Warehouse();
                        SelectedWarehouseName = "";
                        DocumentPath = "";
                    }
                }
            }
            catch (Exception ex)
            {
                ResultDialogText = "Error loading Excel file";
                IsShowResultDialog = true;
            }
        }
        [RelayCommand]
        private async Task SelectWarehouse()
        {
            await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectWarehouse, typeof(StartDataLoadViewModel)));
        }
        [RelayCommand]
        private void ClearSelectedWarehouse()
        {
            SelectedWarehouseName = "";
            SelectedWarehouse = null;
        }
    }
}
