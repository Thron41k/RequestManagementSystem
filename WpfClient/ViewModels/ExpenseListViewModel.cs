using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Threading;
using RequestManagement.Common.Models;
using WpfClient.Messages;
using WpfClient.Services.Interfaces;
using System.Windows;

namespace WpfClient.ViewModels;

public partial class ExpenseListViewModel : ObservableObject
{
    public bool EditMode { get; set; }
    private readonly IMessageBus _messageBus;
    private readonly IExpenseService _expenseService;
    [ObservableProperty] private string _menuDeleteItemText = "Удалить отмеченные";
    [ObservableProperty] private Expense? _selectedExpense = new();
    [ObservableProperty] private ObservableCollection<Expense> _expenses = [];
    [ObservableProperty] private Warehouse _selectedWarehouse = new();
    [ObservableProperty] private Defect _selectedDefect = new();
    [ObservableProperty] private Equipment _selectedEquipment = new();
    [ObservableProperty] private Driver _selectedDriver = new();
    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private DateTime _fromDate = DateTime.Parse("01.04.2025");
    [ObservableProperty] private DateTime _toDate = DateTime.Parse("30.04.2025");
    [ObservableProperty] private CollectionViewSource _expensesViewSource;
    private readonly System.Timers.Timer _filterTimer;

    public ExpenseListViewModel()
    {
     
    }
    public ExpenseListViewModel(IMessageBus messageBus, IExpenseService expenseService, IExcelWriterService excelWriterService)
    {
        _messageBus = messageBus;
        _expenseService = expenseService;
        _messageBus.Subscribe<SelectResultMessage>(OnSelect);
        _expensesViewSource = new CollectionViewSource { Source = Expenses };
        var dispatcher = Dispatcher.CurrentDispatcher;
        _filterTimer = new System.Timers.Timer(Vars.SearchDelay) { AutoReset = false };
        _filterTimer.Elapsed += async (_, _) =>
        {
            await dispatcher.InvokeAsync(async () => { await LoadExpensesAsync(); });
        };
    }

    private async Task OnSelect(SelectResultMessage arg)
    {
        if (arg.Caller != typeof(ExpenseListViewModel) || arg.Item == null) return;
        switch (arg.Message)
        {
            case MessagesEnum.SelectWarehouse:
                SelectedWarehouse = (Warehouse)arg.Item;
                await LoadExpensesAsync();
                break;
            case MessagesEnum.SelectDefect:
                SelectedDefect = (Defect)arg.Item;
                await LoadExpensesAsync();
                break;
            case MessagesEnum.SelectEquipment:
                SelectedEquipment = (Equipment)arg.Item;
                await LoadExpensesAsync();
                break;
            case MessagesEnum.SelectDriver:
                SelectedDriver = (Driver)arg.Item;
                await LoadExpensesAsync();
                break;
        }
    }
    [RelayCommand]
    private async Task LoadExpensesAsync()
    {
        if (!ValidateDates()) return;
        if (SelectedWarehouse.Id == 0) return;
        var currentSortDescriptions = ExpensesViewSource.View?.SortDescriptions.ToList() ?? [];
        var expenseList = await _expenseService.GetAllExpensesAsync(SearchText, SelectedWarehouse.Id, SelectedEquipment.Id, SelectedDriver.Id, SelectedDefect.Id, FromDate.ToString(CultureInfo.CurrentCulture), ToDate.ToString(CultureInfo.CurrentCulture));
        Expenses = new ObservableCollection<Expense>(expenseList);
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
            await _messageBus.Publish(new ShowTaskMessage(MessagesEnum.ShowExpenseDialog, typeof(ExpenseListViewModel), true, SelectedExpense.Id, SelectedExpense.Date, SelectedExpense.Quantity, SelectedExpense.Term, SelectedExpense.Stock, SelectedExpense.Equipment, SelectedExpense.Driver, SelectedExpense.Defect));
        }
    }
    [RelayCommand]
    private void ExpenseListCheckedUpdate()
    {
        MenuDeleteItemText = $"Удалить отмеченные({Expenses.Count(x => x.IsSelected)})";
    }

    [RelayCommand]
    private async Task Print()
    {
        //var data = new List<ActPartsModel>
        //{
        //    new(),
        //    new()
        //};
        //_excelWriterService.ExportAndPrint(ExcelTemplateType.ActParts, data);
        await _messageBus.Publish(new ShowTaskPrintDialogMessageExpense(MessagesEnum.ShowPrintReportDialog, typeof(ExpenseListViewModel), false, Expenses.Where(x => x is { IsSelected: true}).ToList(),FromDate, ToDate));
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

        ExpensesViewSource.View.Refresh(); // Принудительно обновляем DataGrid
        ExpenseListCheckedUpdate();
    }

    [RelayCommand]
    private void DeselectAll()
    {
        foreach (var expense in Expenses)
        {
            expense.IsSelected = false;
        }
        ExpensesViewSource.View.Refresh(); // Принудительно обновляем DataGrid
        ExpenseListCheckedUpdate();
    }

    [RelayCommand]
    private void InvertSelected()
    {
        foreach (var expense in Expenses)
        {
            expense.IsSelected = !expense.IsSelected;
        }
        ExpensesViewSource.View.Refresh(); // Принудительно обновляем DataGrid
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
    private async Task ClearSelectedWarehouse()
    {
        SelectedWarehouse = new Warehouse();
        await LoadExpensesAsync();
    }
    [RelayCommand]
    private async Task ClearSelectedDefect()
    {
        SelectedDefect = new Defect();
        await LoadExpensesAsync();
    }
    [RelayCommand]
    private async Task ClearSelectedEquipment()
    {
        SelectedEquipment = new Equipment();
        await LoadExpensesAsync();
    }
    [RelayCommand]
    private async Task ClearSelectedDriver()
    {
        SelectedDriver = new Driver();
        await LoadExpensesAsync();
    }
    [RelayCommand]
    private async Task ClearSearchText()
    {
        SearchText = "";
    }
    partial void OnSearchTextChanged(string value)
    {
        _filterTimer.Stop();
        _filterTimer.Start();
    }
    private bool ValidateDates()
    {
        // Проверка на нулевые или экстремальные значения
        if (FromDate == DateTime.MinValue || ToDate == DateTime.MinValue ||
            FromDate == DateTime.MaxValue || ToDate == DateTime.MaxValue)
        {
            MessageBox.Show("Пожалуйста, выберите корректные даты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        // Проверка, что FromDate не позже ToDate
        if (FromDate > ToDate)
        {
            MessageBox.Show("Дата начала не может быть позже даты окончания.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        // Проверка на разумный диапазон (например, не раньше 2000 года и не в будущем)
        if (FromDate.Year < 2000 || ToDate.Year < 2000)
        {
            MessageBox.Show("Даты не могут быть раньше 2000 года.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        // Проверка на слишком большой диапазон (например, не больше года)
        if ((ToDate - FromDate).TotalDays > 365)
        {
            MessageBox.Show("Диапазон дат не должен превышать одного года.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }
}