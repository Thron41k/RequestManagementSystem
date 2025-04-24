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

namespace WpfClient.ViewModels
{
    public partial class StartDataLoadViewModel : ObservableObject
    {
        private readonly IMessageBus _messageBus;
        [ObservableProperty] private string _documentPath = "";
        private List<MaterialStock> _materialStocks = [];
        private readonly IExcelReaderService _excelReaderService;
        public StartDataLoadViewModel() { }

        public StartDataLoadViewModel(IMessageBus messageBus, IExcelReaderService excelReaderService)
        {
            _messageBus = messageBus;
            _excelReaderService = excelReaderService;
            _messageBus.Subscribe<SelectResultMessage>(OnSelect);
        }

        private async Task OnSelect(SelectResultMessage arg)
        {
            if (arg.Caller != typeof(ExpenseListViewModel) || arg.Item == null) return;
            switch (arg.Message)
            {

            }
        }

        [RelayCommand]
        private void ClearDocumentPath()
        {
            DocumentPath = "";
        }

        [RelayCommand]
        private void SelectDocumentPath()
        {
            try
            {
                var openFile = new OpenFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx"
                };
                var result = openFile.ShowDialog();
                if (result == true && openFile.FileName.Length > 0)
                {
                    DocumentPath = openFile.FileName;
                    _materialStocks = _excelReaderService.ReadMaterialStock(DocumentPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Excel file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
