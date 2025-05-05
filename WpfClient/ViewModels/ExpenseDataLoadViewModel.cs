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

namespace WpfClient.ViewModels
{
    public partial class ExpenseDataLoadViewModel : ObservableObject
    {
        private readonly IMessageBus _messageBus;
        private readonly IExpenseService _expenseService;
        private readonly IExcelReaderService _excelReaderService;
        private readonly IWarehouseService _requestService;
        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private string _documentPath = "";
        [ObservableProperty] private List<MaterialExpense> _materialExpense = [];
        [ObservableProperty] private Warehouse _selectedWarehouse = new();
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
                    break;
            }

            return Task.CompletedTask;
        }
        public void Init()
        {
            DocumentPath = "";
            MaterialExpense = [];
            SelectedWarehouse = new Warehouse();
        }
        [RelayCommand]
        private async Task UploadMaterials()
        {
            try
            {
                IsBusy = true;
                var result =
                    await _expenseService.UploadMaterialsExpenseAsync(MaterialExpense, SelectedWarehouse.Id);
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
                    var result = _excelReaderService.ReadExpenses(DocumentPath);
                    if (result.materialStocks is { Count: > 0 })
                    {
                        MaterialExpense = result.materialStocks;
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
        private void ClearDocumentPath()
        {
            DocumentPath = "";
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
