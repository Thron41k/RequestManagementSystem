using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Common.Utilities;
using RequestManagement.WpfClient.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Timer = System.Timers.Timer;

namespace RequestManagement.WpfClient.ViewModels;

public partial class AddMaterialInUseFromExpenseViewModel : BaseViewModel
{
    private readonly IWarehouseService _warehouseService;
    private readonly IExpenseService _expenseService;
    private readonly Timer _filterTimer;
    public event EventHandler CloseWindowRequested;
    private Driver? _selectedDriver;
    [ObservableProperty] private ObservableCollection<Warehouse> _warehouseList = [];
    [ObservableProperty] private CollectionViewSource _warehouseViewSource;
    [ObservableProperty] private ObservableCollection<Expense> _expenseList = [];
    [ObservableProperty] private CollectionViewSource _expenseViewSource;
    [ObservableProperty] private Warehouse? _selectedWarehouse;
    [ObservableProperty] private DateTime _fromDate;
    [ObservableProperty] private DateTime _toDate;

    public AddMaterialInUseFromExpenseViewModel(IWarehouseService warehouseService, IExpenseService expenseService)
    {
        _warehouseService = warehouseService;
        _expenseService = expenseService;
        var dateRange = DateRangeHelper.GetCurrentHalfMonthRange();
        _fromDate = dateRange.FromDate;
        _toDate = dateRange.ToDate;
        WarehouseViewSource = new CollectionViewSource{Source = WarehouseList };
    }

    public async Task Init(Driver? mol)
    {
        _selectedDriver = mol;
        await LoadWarehouseAsync();
    }

    private async Task LoadWarehouseAsync()
    {
        if(_selectedDriver == null)return;
        var warehouseList = await _warehouseService.GetAllWarehousesByMolIdAsync(_selectedDriver.Id);
        WarehouseList = new ObservableCollection<Warehouse>(warehouseList);
        WarehouseViewSource = new CollectionViewSource { Source = WarehouseList };
    }

    private async Task LoadExpensesByWarehouse()
    {
        if(SelectedWarehouse == null)return;
        var expensesList = await _expenseService.GetAllExpensesAsync("",SelectedWarehouse.Id,0,0,0,FromDate.ToString("dd.MM.yyyy"), ToDate.ToString("dd.MM.yyyy"));
        ExpenseList = new ObservableCollection<Expense>(expensesList.Where(x=>x.Term > 0));
        ExpenseViewSource = new CollectionViewSource{ Source = ExpenseList };
    }

    partial void OnSelectedWarehouseChanged(Warehouse? value)
    {
        _ = LoadExpensesByWarehouse();
    }

    [RelayCommand]
    private void SelectAll()
    {
        foreach (var expense in ExpenseList)
        {
            expense.IsSelected = true;
        }

        ExpenseViewSource.View.Refresh();
    }
    [RelayCommand]
    private void DeselectAll()
    {
        foreach (var expense in ExpenseList)
        {
            expense.IsSelected = false;
        }

        ExpenseViewSource.View.Refresh();
    }
    [RelayCommand]
    private void InvertSelected()
    {
        foreach (var expense in ExpenseList)
        {
            expense.IsSelected = !expense.IsSelected;
        }

        ExpenseViewSource.View.Refresh();
    }

    [RelayCommand]
    private async Task AddMaterialInUse()
    {

    }
}