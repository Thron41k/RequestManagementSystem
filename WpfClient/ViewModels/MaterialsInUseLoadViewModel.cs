using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.WpfClient.Messages;
using RequestManagement.WpfClient.Services.Interfaces;

namespace RequestManagement.WpfClient.ViewModels;

public partial class MaterialsInUseLoadViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private readonly IExcelReaderService _excelReaderService;
    private readonly IDriverService _driverService;
    private readonly IMaterialsInUseService _materialsInUseService;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isShowResultDialog;
    [ObservableProperty] private string _resultDialogText = "";
    [ObservableProperty] private string _documentPath = "";
    [ObservableProperty] private string _selectedDriverName = "";
    [ObservableProperty] private List<MaterialsInUseForUpload>? _materialsInUseForUploadList = new();
    [ObservableProperty] private Driver? _selectedDriver = new();
    [ObservableProperty] private int _materialsInUseCount;
    public MaterialsInUseLoadViewModel()
    {
    }
    public MaterialsInUseLoadViewModel(IMessageBus messageBus,
        IExcelReaderService excelReaderService,
        IDriverService driverService,
        IMaterialsInUseService materialsInUseService)
    {
        _messageBus = messageBus;
        _excelReaderService = excelReaderService;
        _driverService = driverService;
        _materialsInUseService = materialsInUseService;
        _messageBus.Subscribe<SelectResultMessage>(OnSelect);
    }
    private Task OnSelect(SelectResultMessage arg)
    {
        if (arg.Caller != typeof(MaterialsInUseLoadViewModel) || arg.Item == null) return Task.CompletedTask;
        switch (arg.Message)
        {
            case MessagesEnum.SelectDriver:
                SelectedDriver = (Driver)arg.Item;
                SelectedDriverName = SelectedDriver.FullName;
                break;
        }

        return Task.CompletedTask;
    }
    public void Init()
    {
        DocumentPath = "";
        MaterialsInUseForUploadList = [];
        SelectedDriver = new Driver();
        SelectedDriverName = "";
        MaterialsInUseCount = 0;
    }
    [RelayCommand]
    private async Task UploadMaterialsInUse()
    {
        try
        {
            if (string.IsNullOrEmpty(DocumentPath)) return;
            if (MaterialsInUseForUploadList == null || MaterialsInUseForUploadList.Count == 0) return;
            if (string.IsNullOrEmpty(SelectedDriverName)) return;
            IsBusy = true;
            var result =
                await _materialsInUseService.UploadMaterialsInUseAsync(MaterialsInUseForUploadList);
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
                var result = _excelReaderService.ReadMaterialsInUseForUpload(DocumentPath);
                if (result is { Count: > 0 })
                {
                    MaterialsInUseForUploadList = result;
                    MaterialsInUseCount = result.Count;
                    SelectedDriver = await _driverService.GetOrCreateDriverAsync(result[0].FinanciallyResponsiblePersonFullName, result[0].FinanciallyResponsiblePersonCode);
                    SelectedDriverName = SelectedDriver.FullName;
                }
                else
                {
                    MaterialsInUseCount = 0;
                    SelectedDriver = null;
                    SelectedDriverName = "";
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
    private async Task SelectDriver()
    {
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectDriver, typeof(MaterialsInUseLoadViewModel)));
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
        MaterialsInUseCount = 0;
    }

    [RelayCommand]
    private void ClearDriverName()
    {
        SelectedDriverName = "";
        SelectedDriver = null;
    }
}