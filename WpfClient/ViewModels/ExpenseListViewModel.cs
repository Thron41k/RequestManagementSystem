using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfClient.Messages;
using WpfClient.Services.Interfaces;

namespace WpfClient.ViewModels
{
    public partial class ExpenseListViewModel : ObservableObject
    {
        public bool EditMode { get; set; }
        private readonly IMessageBus _messageBus;
        private readonly IExpenseService _expenseService;
        [ObservableProperty] private string _menuDeleteItemText = "Удалить отмеченные";
        [ObservableProperty] private RequestManagement.Common.Models.Expense? _selectedExpense = new();
        [ObservableProperty] private ObservableCollection<RequestManagement.Common.Models.Expense> _expenses = [];
        [ObservableProperty] private RequestManagement.Common.Models.Warehouse _selectedWarehouse = new();
        [ObservableProperty] private RequestManagement.Common.Models.Defect _selectedDefect = new();
        [ObservableProperty] private RequestManagement.Common.Models.Equipment _selectedEquipment = new();
        [ObservableProperty] private RequestManagement.Common.Models.Driver _selectedDriver = new();
        [ObservableProperty] private string _searchText = "";
        [ObservableProperty] private DateTime _fromDate = DateTime.MinValue;
        [ObservableProperty] private DateTime _toDate = DateTime.MaxValue;

        public ExpenseListViewModel() { }
        public ExpenseListViewModel(IMessageBus messageBus, IExpenseService expenseService)
        {
            _messageBus = messageBus;
            _expenseService = expenseService;
            _messageBus.Subscribe<SelectResultMessage>(OnSelect);
        }

        private Task OnSelect(SelectResultMessage arg)
        {
            if (arg.Caller != typeof(ExpenseListViewModel) || arg.Item == null) return Task.CompletedTask;
            switch (arg.Message)
            {
                case MessagesEnum.SelectWarehouse:
                    SelectedWarehouse = (RequestManagement.Common.Models.Warehouse)arg.Item;
                    break;
            }
            return Task.CompletedTask;
        }

        [RelayCommand]
        private async Task LoadExpensesAsync()
        {
            if(FromDate == DateTime.MinValue || ToDate == DateTime.MaxValue && SelectedWarehouse.Id == 0) return;
            var expenseList = await _expenseService.GetAllExpensesAsync(SearchText,SelectedWarehouse.Id,SelectedEquipment.Id,SelectedDriver.Id,SelectedDefect.Id,FromDate.ToString(CultureInfo.CurrentCulture),ToDate.ToString(CultureInfo.CurrentCulture));
            Expenses = new ObservableCollection<RequestManagement.Common.Models.Expense>(expenseList);
        }


        public async Task Load()
        {
            await LoadExpensesAsync();
        }

        [RelayCommand]
        private async Task DoubleClick()
        {
            if (SelectedExpense != null)
            {
                await _messageBus.Publish(new ShowTaskMessage(MessagesEnum.ShowExpenseDialog, typeof(ExpenseListViewModel), true, SelectedExpense.Id, SelectedExpense.Date, SelectedExpense.Quantity, SelectedExpense.Stock, SelectedExpense.Equipment, SelectedExpense.Driver, SelectedExpense.Defect));
            }
        }

        [RelayCommand]
        private void ExpenseListCheckedUpdate()
        {
            MenuDeleteItemText = $"Удалить отмеченные({Expenses.Count(x=>x.IsSelected)})";
        }

        [RelayCommand]
        private async Task ExpenseDeleteAsync()
        {
            if (SelectedExpense != null)
            {
                var result = await _expenseService.DeleteExpenseAsync(SelectedExpense.Id);
                if (result)
                {
                    await LoadExpensesAsync();
                }
            }
        }
        [RelayCommand]
        private async Task ExpenseDeleteRangeAsync()
        {
            var list = Expenses.Where(x => x.IsSelected).Select(x => x.Id).ToList();
            if (list.Count > 0)
            {
                var result = await _expenseService.DeleteExpensesAsync(list);
                if (result)
                {
                    await LoadExpensesAsync();
                }
            }
        }

        [RelayCommand]
        private void SelectAll()
        {
            for (var index = 0; index < Expenses.Count; index++)
            {
                Expenses[index].IsSelected = true;
                Expenses.
            }
        }

        [RelayCommand]
        private async Task SelectWarehouse()
        {
            await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectWarehouse, typeof(ExpenseListViewModel)));
        }

        [RelayCommand]
        private async Task SelectDefect()
        {

        }

        [RelayCommand]
        private async Task SelectEquipment()
        {

        }

        [RelayCommand]
        private async Task SelectDriver()
        {

        }
    }
}
