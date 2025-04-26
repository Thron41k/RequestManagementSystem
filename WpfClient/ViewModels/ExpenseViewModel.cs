using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using RequestManagement.Common.Models;
using WpfClient.Messages;
using WpfClient.Services.Interfaces;

namespace WpfClient.ViewModels;

public partial class ExpenseViewModel : ObservableObject
{
    public bool EditMode { get; set; }
    public bool DialogResult { get; set; }
    private int _expenseId;
    private decimal _currentQuantity;
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
            }
        }
    }

    public void SetEquipment(Equipment? equipment)
    {
        SelectedEquipment = equipment;
        SelectedEquipmentText = $"{SelectedEquipment.Name} ({SelectedEquipment.StateNumber})";
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
            var convertResul = decimal.TryParse(QuantityForExpense.Replace(".", ","), out var quantityForExpense);
            if (convertResul)
            {
                var result = false;
                if (EditMode)
                {
                    if (quantityForExpense <= _currentQuantity + ExpenseStock?.FinalQuantity)
                    {
                        result = await _expenseService.UpdateExpenseAsync(
                            new RequestManagement.Common.Models.Expense
                            {
                                Id = _expenseId,
                                Quantity = quantityForExpense,
                                DefectId = SelectedDefect.Id,
                                DriverId = SelectedDriver.Id,
                                EquipmentId = SelectedEquipment.Id,
                                StockId = (int)ExpenseStock?.Id!,
                                Date = SelectedDate
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
                        result = (await _expenseService.CreateExpenseAsync(new RequestManagement.Common.Models.Expense
                        {
                            Quantity = quantityForExpense,
                            DefectId = SelectedDefect.Id,
                            DriverId = SelectedDriver.Id,
                            EquipmentId = SelectedEquipment.Id,
                            StockId = (int)ExpenseStock?.Id!,
                            Date = SelectedDate
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
                };
            }
        }
    }
}