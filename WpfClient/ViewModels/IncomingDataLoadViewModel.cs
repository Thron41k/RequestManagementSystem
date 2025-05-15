using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequestManagement.Common.Interfaces;
using WpfClient.Services.Interfaces;
using WpfClient.Models;
using RequestManagement.Common.Models;
using WpfClient.Messages;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Windows;
using WpfClient.Views;

namespace WpfClient.ViewModels
{
    public partial class IncomingDataLoadViewModel : ObservableObject
    {
        private readonly IMessageBus _messageBus;
        private readonly IIncomingService _incomingService;
        private readonly IExcelReaderService _excelReaderService;
        private readonly IWarehouseService _requestService;
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private string _documentPath = "";
        [ObservableProperty] private MaterialIncoming? _materialIncoming = new();
        [ObservableProperty] private Warehouse _selectedWarehouse = new();
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
                    break;
            }

            return Task.CompletedTask;
        }
        public void Init()
        {
            DocumentPath = "";
            MaterialIncoming = new();
            SelectedWarehouse = new ();
        }
        [RelayCommand]
        private async Task UploadMaterials()
        {
            try
            {
                if(MaterialIncoming == null) return;
                IsBusy = true;
                var result =
                    await _incomingService.UploadIncomingsAsync(MaterialIncoming);
                if (result)
                {
                    MessageBox.Show("Data uploaded successfully", "Success", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Error uploading data", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Excel file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        [RelayCommand]
        private void ClearDocumentPath()
        {
            DocumentPath = "";
        }
        [RelayCommand]
        private async Task SelectWarehouse()
        {
            await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectWarehouse, typeof(IncomingDataLoadViewModel)));
        }
        [RelayCommand]
        private void ClearSelectedWarehouse()
        {
            SelectedWarehouse = new Warehouse();
        }
    }
}
