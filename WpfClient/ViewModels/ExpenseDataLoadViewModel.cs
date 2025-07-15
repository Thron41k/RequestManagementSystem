using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.WpfClient.Messages;
using RequestManagement.WpfClient.Services.Interfaces;

namespace RequestManagement.WpfClient.ViewModels;

public partial class ExpenseDataLoadViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private readonly IExpenseService _expenseService;
    private readonly IExcelReaderService _excelReaderService;
    private readonly IWarehouseService _requestService;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isShowResultDialog;
    [ObservableProperty] private string _resultDialogText = "";
    [ObservableProperty] private string _documentPath = "";
    [ObservableProperty] private string _selectedWarehouseName = "";
    [ObservableProperty] private List<MaterialExpense> _materialExpense = [];
    [ObservableProperty] private Warehouse? _selectedWarehouse;
    [ObservableProperty] private int _materialIncomingCount;
    public ExpenseDataLoadViewModel()
    {
    }
    public ExpenseDataLoadViewModel(IMessageBus messageBus, IExcelReaderService excelReaderService, IExpenseService expenseService, IWarehouseService requestService)
    {
        _messageBus = messageBus;
        _excelReaderService = excelReaderService;
        _expenseService = expenseService;
        _requestService = requestService;
        _messageBus.Subscribe<SelectResultMessage>(OnSelect);
    }
    private Task OnSelect(SelectResultMessage arg)
    {
        if (arg.Caller != typeof(ExpenseDataLoadViewModel) || arg.Item == null) return Task.CompletedTask;
        switch (arg.Message)
        {
            case MessagesEnum.SelectWarehouse:
                SelectedWarehouse = (Warehouse)arg.Item;
                SelectedWarehouseName = SelectedWarehouse.Name;
                break;
        }

        return Task.CompletedTask;
    }
    public void Init()
    {
        DocumentPath = "";
        MaterialExpense = [];
        SelectedWarehouse = new Warehouse();
        SelectedWarehouseName = "";
    }
    [RelayCommand]
    private void HideResultDialog()
    {
        IsShowResultDialog = false;
    }
    [RelayCommand]
    private async Task UploadMaterials()
    {
        try
        {
            if (string.IsNullOrEmpty(DocumentPath)) return;
            if (MaterialExpense.Count == 0) return;
            if (string.IsNullOrEmpty(SelectedWarehouseName)) return;
            IsBusy = true;
            var result =
                await _expenseService.UploadMaterialsExpenseAsync(MaterialExpense, SelectedWarehouse!.Id);
            ResultDialogText = result.Item1 ? "Data uploaded successfully" : "Error uploading data";
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
                var result = _excelReaderService.ReadExpenses(DocumentPath);
                if (result.materialStocks is { Count: > 0 })
                {
                    MaterialExpense = result.materialStocks;
                    MaterialIncomingCount = result.materialStocks.Count;
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
    private void ClearDocumentPath()
    {
        DocumentPath = "";
        MaterialIncomingCount = 0;
    }
    [RelayCommand]
    private async Task SelectWarehouse()
    {
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectWarehouse, typeof(ExpenseDataLoadViewModel)));
    }
    [RelayCommand]
    private void ClearSelectedWarehouse()
    {
        SelectedWarehouseName = "";
        SelectedWarehouse = null;
    }
}