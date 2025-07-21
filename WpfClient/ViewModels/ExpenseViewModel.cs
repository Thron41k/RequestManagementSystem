using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.WpfClient.Messages;
using RequestManagement.WpfClient.Services.Interfaces;
using static System.Int32;

namespace RequestManagement.WpfClient.ViewModels;

public partial class ExpenseViewModel : ObservableObject
{
    public bool EditMode { get; set; }
    public bool DialogResult { get; set; }
    private int _expenseId;
    private decimal _currentQuantity;
    private readonly IMessageBus _messageBus;
    private readonly IExpenseService _expenseService;
    [ObservableProperty] private Stock? _expenseStock = new();
    [ObservableProperty] private Equipment? _selectedEquipment = new();
    [ObservableProperty] private string _selectedEquipmentText = string.Empty;
    [ObservableProperty] private string _termForOperations = string.Empty;
    [ObservableProperty] private Driver? _selectedDriver = new();
    [ObservableProperty] private Defect? _selectedDefect = new();
    [ObservableProperty] private string _quantityForExpense = string.Empty;
    [ObservableProperty] private DateTime _selectedDate = DateTime.Now;
    [ObservableProperty] private bool _isTermVisible;
    public ExpenseViewModel(IMessageBus messageBus, IExpenseService expenseService)
    {
        _messageBus = messageBus;
        _expenseService = expenseService;
        _messageBus.Subscribe<SelectResultMessage>(OnShowDialog);
    }

    private Task OnShowDialog(SelectResultMessage arg)
    {
        if (arg.Caller == typeof(ExpenseViewModel) && arg.Item != null)
        {
            switch (arg.Message)
            {
                case MessagesEnum.SelectEquipment:
                    SelectedEquipment = (Equipment)arg.Item;
                    SelectedEquipmentText = $"{SelectedEquipment.Name} ({SelectedEquipment.StateNumber})";
                    break;
                case MessagesEnum.SelectDriver:
                    SelectedDriver = (Driver)arg.Item;
                    break;
                case MessagesEnum.SelectDefect:
                    SelectedDefect = (Defect)arg.Item;
                    IsTermVisible = SelectedDefect.Id == 4;
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
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectEquipment, typeof(ExpenseViewModel)));
    }
    [RelayCommand]
    private async Task SelectDriver()
    {
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectDriver, typeof(ExpenseViewModel)));
    }
    [RelayCommand]
    private async Task SelectDefect()
    {
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectDefect, typeof(ExpenseViewModel)));
    }

    [RelayCommand]
    private void SetMaxQuantity()
    {
        QuantityForExpense = ExpenseStock?.FinalQuantity.ToString(CultureInfo.InvariantCulture)!;
    }

    public void SetCurrentQuantity(decimal quantity)
    {
        _currentQuantity = quantity;
        QuantityForExpense = _currentQuantity.ToString(CultureInfo.InvariantCulture);
    }
    public void SetExpenseId(int id)
    {
        _expenseId = id;
    }

    public async Task LoadNomenclatureMapingAsync()
    {
        if (ExpenseStock != null && !EditMode)
        {
            var result = await _expenseService.GetLastNomenclatureDefectMappingAsync(-1, ExpenseStock.NomenclatureId);
            if (result is { Defect: not null } && SelectedDefect != null && SelectedDefect.Id != result.Defect.Id)
            {
                SelectedDefect = result.Defect;
                IsTermVisible = SelectedDefect.Id == 4;
            }
        }
    }

    public void SetEquipment(Equipment? equipment)
    {
        SelectedEquipment = equipment;
        if (SelectedEquipment == null)
        {
            SelectedEquipmentText = "";
            return;
        }
        if (!string.IsNullOrEmpty(SelectedEquipment.Name))
        {
            SelectedEquipmentText = !string.IsNullOrEmpty(SelectedEquipment.StateNumber) ? $"{SelectedEquipment.Name} ({SelectedEquipment.StateNumber})" : SelectedEquipment.Name;
        }
        else
        {
            SelectedEquipmentText = "";
        }
    }

    [RelayCommand]
    private void ClearSelectedEquipment()
    {
        SetEquipment(new Equipment());
    }

    [RelayCommand]
    private async Task SaveExpenseToDatabase(UserControl window)
    {
        if (SelectedEquipment != null && SelectedDriver != null && SelectedDefect != null && !string.IsNullOrEmpty(QuantityForExpense.Trim()))
        {
            var convertResul = decimal.TryParse((string?)QuantityForExpense.Replace(".", ","), out var quantityForExpense);
            if (convertResul)
            {
                var result = false;
                TryParse((string?)TermForOperations, out var term);
                if (EditMode)
                {
                    if (quantityForExpense <= _currentQuantity + ExpenseStock?.FinalQuantity)
                    {
                        result = await _expenseService.UpdateExpenseAsync(
                            new Expense
                            {
                                Id = _expenseId,
                                Quantity = quantityForExpense,
                                DefectId = SelectedDefect.Id,
                                DriverId = SelectedDriver.Id,
                                EquipmentId = SelectedEquipment.Id,
                                StockId = (int)ExpenseStock?.Id!,
                                Date = SelectedDate,
                                Term = term

                            });
                    }
                    else
                    {
                        MessageBox.Show("Введенное количество больше остатка на складе!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    if (quantityForExpense <= ExpenseStock?.FinalQuantity)
                        result = (await _expenseService.CreateExpenseAsync(new Expense
                        {
                            Quantity = quantityForExpense,
                            DefectId = SelectedDefect.Id,
                            DriverId = SelectedDriver.Id,
                            EquipmentId = SelectedEquipment.Id,
                            StockId = (int)ExpenseStock?.Id!,
                            Date = SelectedDate,
                            Term = term
                        })).Id != 0;
                    else
                    {
                        MessageBox.Show("Введенное количество больше остатка на складе!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                if (result)
                {
                    DialogResult = true;
                    ((Window)window.Parent).Close();
                }
            }
        }
    }

    [RelayCommand]
    private void ClearSelectedDriver()
    {
        SelectedDriver = new Driver();
    }

    public void SetDefect(Defect? selectedDefect)
    {
        SelectedDefect = selectedDefect;
        IsTermVisible = SelectedDefect?.Id == 4;
    }

    [RelayCommand]
    private void ClearSelectedDefect()
    {
        SetDefect(null);
    }
}