using CommunityToolkit.Mvvm.ComponentModel;
using RequestManagement.Common.Interfaces;
using WpfClient.Services.Interfaces;
using RequestManagement.Common.Models;
using WpfClient.Messages;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace WpfClient.ViewModels;

public partial class IncomingDataLoadViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private readonly IIncomingService _incomingService;
    private readonly IExcelReaderService _excelReaderService;
    private readonly IWarehouseService _requestService;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isShowResultDialog;
    [ObservableProperty] private string _resultDialogText = "";
    [ObservableProperty] private string _documentPath = "";
    [ObservableProperty] private string _selectedWarehouseName = "";
    [ObservableProperty] private MaterialIncoming? _materialIncoming = new();
    [ObservableProperty] private Warehouse? _selectedWarehouse = new();
    [ObservableProperty] private int _materialIncomingCount;
    public IncomingDataLoadViewModel()
    {
    }
    public IncomingDataLoadViewModel(IMessageBus messageBus, IExcelReaderService excelReaderService, IIncomingService incomingService, IWarehouseService requestService)
    {
        _messageBus = messageBus;
        _excelReaderService = excelReaderService;
        _incomingService = incomingService;
        _requestService = requestService;
        _messageBus.Subscribe<SelectResultMessage>(OnSelect);
    }
    private Task OnSelect(SelectResultMessage arg)
    {
        if (arg.Caller != typeof(IncomingDataLoadViewModel) || arg.Item == null) return Task.CompletedTask;
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
        MaterialIncoming = new MaterialIncoming();
        SelectedWarehouse = new Warehouse();
        SelectedWarehouseName = "";
    }
    [RelayCommand]
    private async Task UploadMaterials()
    {
        try
        {
            if(string.IsNullOrEmpty(DocumentPath)) return;
            if(MaterialIncoming == null || MaterialIncoming.Items.Count == 0)return;
            if(string.IsNullOrEmpty(SelectedWarehouseName)) return;
            IsBusy = true;
            var result =
                await _incomingService.UploadIncomingsAsync(MaterialIncoming);
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
                var result = _excelReaderService.ReadMaterialIncoming(DocumentPath);
                if (result.Items is { Count: > 0 })
                {
                    MaterialIncoming = result;
                    MaterialIncomingCount = result.Items.SelectMany(x=>x.Items).Count();
                    if (result.WarehouseName != null)
                    {
                        SelectedWarehouse = await _requestService.GetOrCreateWarehousesAsync(result.WarehouseName);
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
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectWarehouse, typeof(IncomingDataLoadViewModel)));
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
    private void ClearWarehouseName()
    {
        SelectedWarehouseName = "";
        SelectedWarehouse = null;
    }
}