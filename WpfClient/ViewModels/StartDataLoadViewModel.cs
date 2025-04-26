using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using WpfClient.Messages;
using WpfClient.Services.Interfaces;
using System.Collections.ObjectModel;
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
        [ObservableProperty] private string _documentPath = "";
        [ObservableProperty] private List<MaterialStock> _materialStocks = [];
        [ObservableProperty] private Warehouse _selectedWarehouse = new();
        [ObservableProperty] private DateTime _toDate = DateTime.Now;
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
                    break;
            }

            return Task.CompletedTask;
        }

        [RelayCommand]
        private void ClearDocumentPath()
        {
            DocumentPath = "";
        }

        [RelayCommand]
        private async Task UploadMaterials()
        {
            try
            {
                IsBusy = true;
                var result =
                    await _stockService.UploadMaterialsStockAsync(MaterialStocks, SelectedWarehouse.Id, ToDate);
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
                    var result = _excelReaderService.ReadMaterialStock(DocumentPath);
                    if (result.materialStocks is { Count: > 0 })
                    {
                        MaterialStocks = result.materialStocks;
                        if (result.date != null) ToDate = DateTime.Parse(result.date);
                        if (result.warehouse != null)
                        {
                            SelectedWarehouse = await _requestService.GetOrCreateWarehousesAsync(result.warehouse);
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
        private async Task SelectWarehouse()
        {
            await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectWarehouse, typeof(StartDataLoadViewModel)));
        }
        [RelayCommand]
        private void ClearSelectedWarehouse()
        {
            SelectedWarehouse = new Warehouse();
        }
    }
}
