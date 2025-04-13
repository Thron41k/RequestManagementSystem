using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfClient.Messages;
using WpfClient.Services.Interfaces;

namespace WpfClient.ViewModels
{
    public partial class ExpenseViewModel : ObservableObject
    {
        public bool EditMode { get; set; }
        private readonly IMessageBus _messageBus;
        private readonly IExpenseService _expenseService;
        [ObservableProperty] private RequestManagement.Common.Models.Stock? _expenseStock = new();
        [ObservableProperty] private RequestManagement.Common.Models.Equipment? _selectedEquipment = new();
        [ObservableProperty] private string _selectedEquipmentText = string.Empty;
        [ObservableProperty] private RequestManagement.Common.Models.Driver? _selectedDriver = new();
        [ObservableProperty] private RequestManagement.Common.Models.Defect? _selectedDefect = new();
        [ObservableProperty] private string _quantityForExpense = string.Empty;
        [ObservableProperty] private DateTime _selectedDate = DateTime.Now;
        public ExpenseViewModel(IMessageBus messageBus, IExpenseService expenseService)
        {
            _messageBus = messageBus;
            _expenseService = expenseService;
            _messageBus.Subscribe<SelectResultMessage>(OnShowDialog);
        }

        private Task OnShowDialog(SelectResultMessage arg)
        {
            if (arg.Caller == typeof(IExpenseService) && arg.Item != null)
            {
                switch (arg.Message)
                {
                    case MessagesEnum.SelectEquipment:
                        SelectedEquipment = (RequestManagement.Common.Models.Equipment)arg.Item;
                        SelectedEquipmentText = $"{SelectedEquipment.Name} ({SelectedEquipment.StateNumber})";
                        break;
                    case MessagesEnum.SelectDriver:
                        SelectedDriver = (RequestManagement.Common.Models.Driver)arg.Item;
                        break;
                    case MessagesEnum.SelectDefect:
                        SelectedDefect = (RequestManagement.Common.Models.Defect)arg.Item;
                        break;
                }
            }
            return Task.CompletedTask;
        }

        public ExpenseViewModel()
        {

        }
        [RelayCommand]
        private async Task SelectEquipment()
        {
            await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectEquipment, typeof(IExpenseService)));
        }
        [RelayCommand]
        private async Task SelectDriver()
        {
            await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectDriver, typeof(IExpenseService)));
        }
        [RelayCommand]
        private async Task SelectDefect()
        {
            await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectDefect, typeof(IExpenseService)));
        }

        [RelayCommand]
        private void SetMaxQuantity()
        {
            QuantityForExpense = ExpenseStock?.FinalQuantity.ToString();
        }

        [RelayCommand]
        private async Task SaveExpenseToDatabase(UserControl window)
        {
            if (SelectedEquipment != null &&
                !string.IsNullOrEmpty(SelectedEquipmentText.Trim()) &&
                !string.IsNullOrEmpty(SelectedDriver?.FullName.Trim()) &&
                !string.IsNullOrEmpty(SelectedDefect?.Name.Trim()) &&
                !string.IsNullOrEmpty(QuantityForExpense.Trim()))
            {
                var convertResul = decimal.TryParse(QuantityForExpense, out var quantityForExpense);
                if (convertResul)
                {
                    var result = await _expenseService.CreateExpenseAsync(new RequestManagement.Common.Models.Expense
                    {
                        Quantity = quantityForExpense,
                        DefectId = SelectedDefect.Id,
                        DriverId = SelectedDriver.Id,
                        EquipmentId = SelectedEquipment.Id,
                        StockId = (int)ExpenseStock?.Id!,
                        Date = SelectedDate
                    });
                    if (result != -1)
                    {
                        ((Window)window.Parent).Close();
                    };
                }
            }
        }
    }
}
