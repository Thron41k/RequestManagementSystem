using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
        [ObservableProperty] private DateTime _fromDate = DateTime.Parse("01.04.2025");
        [ObservableProperty] private DateTime _toDate = DateTime.Parse("30.04.2025");
        [ObservableProperty] private CollectionViewSource _expensesViewSource;


        public ExpenseListViewModel() { }
        public ExpenseListViewModel(IMessageBus messageBus, IExpenseService expenseService)
        {
            _messageBus = messageBus;
            _expenseService = expenseService;
            _messageBus.Subscribe<SelectResultMessage>(OnSelect);
            _expensesViewSource = new CollectionViewSource { Source = Expenses };
        }

        private Task OnSelect(SelectResultMessage arg)
        {
            if (arg.Caller != typeof(ExpenseListViewModel) || arg.Item == null) return Task.CompletedTask;
            switch (arg.Message)
            {
                case MessagesEnum.SelectWarehouse:
                    SelectedWarehouse = (RequestManagement.Common.Models.Warehouse)arg.Item;
                    break;
                case MessagesEnum.SelectDefect:
                    SelectedDefect = (RequestManagement.Common.Models.Defect)arg.Item;
                    break;
                case MessagesEnum.SelectEquipment:
                    SelectedEquipment = (RequestManagement.Common.Models.Equipment)arg.Item;
                    break;
                case MessagesEnum.SelectDriver:
                    SelectedDriver = (RequestManagement.Common.Models.Driver)arg.Item;
                    break;
            }
            return Task.CompletedTask;
        }

        [RelayCommand]
        private async Task LoadExpensesAsync()
        {
            if(FromDate == DateTime.MinValue || ToDate == DateTime.MaxValue && SelectedWarehouse.Id == 0) return;
            var currentSortDescriptions = ExpensesViewSource.View?.SortDescriptions.ToList() ?? [];
            var expenseList = await _expenseService.GetAllExpensesAsync(SearchText,SelectedWarehouse.Id,SelectedEquipment.Id,SelectedDriver.Id,SelectedDefect.Id,FromDate.ToString(CultureInfo.CurrentCulture),ToDate.ToString(CultureInfo.CurrentCulture));
            Expenses = new ObservableCollection<RequestManagement.Common.Models.Expense>(expenseList);
            ExpensesViewSource.Source = Expenses;
            if (currentSortDescriptions.Any())
            {
                foreach (var sortDescription in currentSortDescriptions)
                {
                    ExpensesViewSource.View?.SortDescriptions.Add(sortDescription);
                }
            }
            ExpenseListCheckedUpdate();
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
            foreach (var expense in Expenses)
            {
                expense.IsSelected = true;
            }

            _expensesViewSource.View.Refresh(); // Принудительно обновляем DataGrid
            ExpenseListCheckedUpdate();
        }

        [RelayCommand]
        private void DeselectAll()
        {
            foreach (var expense in Expenses)
            {
                expense.IsSelected = false;
            }
            _expensesViewSource.View.Refresh(); // Принудительно обновляем DataGrid
            ExpenseListCheckedUpdate();
        }

        [RelayCommand]
        private void InvertSelected()
        {
            foreach (var expense in Expenses)
            {
                expense.IsSelected = !expense.IsSelected;
            }
            _expensesViewSource.View.Refresh(); // Принудительно обновляем DataGrid
            ExpenseListCheckedUpdate();
        }

        [RelayCommand]
        private async Task SelectWarehouse()
        {
            await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectWarehouse, typeof(ExpenseListViewModel)));
        }

        [RelayCommand]
        private async Task SelectDefect()
        {
            await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectDefect, typeof(ExpenseListViewModel)));
        }

        [RelayCommand]
        private async Task SelectEquipment()
        {
            await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectEquipment, typeof(ExpenseListViewModel)));
        }

        [RelayCommand]
        private async Task SelectDriver()
        {
            await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectDriver, typeof(ExpenseListViewModel)));
        }

        [RelayCommand]
        private void ClearSelectedWarehouse()
        {
            SelectedWarehouse = new();
            ExpenseListCheckedUpdate();
        }
        [RelayCommand]
        private void ClearSelectedDefect()
        {
            SelectedDefect = new();
            ExpenseListCheckedUpdate();
        }
        [RelayCommand]
        private void ClearSelectedEquipment()
        {
            SelectedEquipment = new();
            ExpenseListCheckedUpdate();
        }
        [RelayCommand]
        private void ClearSelectedDriver()
        {
            SelectedDriver = new();
            ExpenseListCheckedUpdate();
        }
        [RelayCommand]
        private void ClearSearchText()
        {
            SearchText = "";
            ExpenseListCheckedUpdate();
        }
    }
}
