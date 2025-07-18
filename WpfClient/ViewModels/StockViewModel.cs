﻿using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.WpfClient.Messages;
using RequestManagement.WpfClient.Services.Interfaces;
using static System.Decimal;
using Nomenclature = RequestManagement.Common.Models.Nomenclature;
using Warehouse = RequestManagement.Common.Models.Warehouse;

namespace RequestManagement.WpfClient.ViewModels;

public partial class StockViewModel : ObservableObject
{
    public bool EditMode { get; set; }
    private readonly IStockService _stockService;
    private readonly IMessageBus _messageBus;
    private readonly IExpenseService _expenseService;
    [ObservableProperty]
    private ObservableCollection<RequestManagement.Common.Models.Stock> _stocks = [];

    [ObservableProperty]
    private RequestManagement.Common.Models.Stock? _selectedStock;

    [ObservableProperty]
    private string _selectedNomenclatureName = string.Empty;

    [ObservableProperty]
    private string _initialQuantity = string.Empty;

    [ObservableProperty]
    private string _warehouseName = "Не выбран";

    [ObservableProperty]
    private int _warehouseId;

    [ObservableProperty]
    private int _initialQuantityFilterType;

    [ObservableProperty]
    private string _initialQuantityFilter = string.Empty;

    [ObservableProperty]
    private int _receivedQuantityFilterType;

    [ObservableProperty]
    private string _receivedQuantityFilter = string.Empty;

    [ObservableProperty]
    private int _consumedQuantityFilterType;

    [ObservableProperty]
    private string _consumedQuantityFilter = string.Empty;

    [ObservableProperty]
    private int _finalQuantityFilterType;

    [ObservableProperty]
    private string _finalQuantityFilter = string.Empty;

    [ObservableProperty]
    private string _nomenclatureFilter = string.Empty;

    private Driver? _lastSelectedDriver = null;
    private Equipment? _lastSelectedEquipment = null;
    public StockViewModel()
    {
        // Для дизайнера
    }
    public StockViewModel(IStockService stockService, IMessageBus messageBus, IExpenseService expenseService)
    {
        _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
        _messageBus = messageBus;
        _expenseService = expenseService;
        _messageBus.Subscribe<SelectResultMessage>(OnSelect);
        //RefreshCommand.Execute(null);
    }

    private Task OnSelect(SelectResultMessage arg)
    {
        if (arg.Caller != typeof(IStockService) || arg.Item == null)
            return Task.CompletedTask;
        switch (arg.Message)
        {
            case MessagesEnum.SelectNomenclature:
                if(SelectedStock == null) return Task.CompletedTask;
                SelectedStock.Nomenclature = (Nomenclature)arg.Item;
                SelectedStock.NomenclatureId = ((Nomenclature)arg.Item).Id;
                SelectedNomenclatureName =
                    $"{SelectedStock.Nomenclature.Name} ({SelectedStock.Nomenclature.Article})({SelectedStock.Nomenclature.Code})";
                break;
            case MessagesEnum.SelectWarehouse:
                WarehouseId = ((Warehouse)arg.Item).Id;
                WarehouseName = ((Warehouse)arg.Item).Name;
                break;
        }
        return Task.CompletedTask;
    }

    partial void OnSelectedStockChanged(RequestManagement.Common.Models.Stock? value)
    {
        if (value != null)
        {
            SelectedNomenclatureName = $"{value.Nomenclature.Name} ({value.Nomenclature.Article})({value.Nomenclature.Code})";
            InitialQuantity = value.InitialQuantity.ToString("F2");
        }
        else
        {
            SelectedNomenclatureName = string.Empty;
            InitialQuantity = string.Empty;
        }
    }

    partial void OnInitialQuantityChanged(string value)
    {
        if (!Regex.IsMatch(value, @"^\d*([.,]\d{0,2})?$|^$"))
        {
            InitialQuantity = Regex.Replace(value, @"[^\d.,]", "");
            MessageBox.Show("Допустимы только цифры и запятая.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    partial void OnNomenclatureFilterChanged(string value)
    {
        RefreshCommand.Execute(null);
    }

    [RelayCommand]
    private async Task SelectNomenclature()
    {
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectNomenclature,typeof(IStockService)));
    }

    [RelayCommand]
    private async Task SelectWarehouse()
    {
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectWarehouse, typeof(IStockService)));
    }

    [RelayCommand]
    private void DoubleClick()
    {
        _messageBus.Publish(new ShowTaskMessage(MessagesEnum.ShowExpenseDialog, typeof(IStockService), false,-1,null,0,0, SelectedStock, _lastSelectedEquipment, _lastSelectedDriver, null));
    }

    [RelayCommand]
    private async Task Save()
    {
        if (SelectedStock == null)
        {
            MessageBox.Show("Выберите запись для сохранения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!TryParse((string?)InitialQuantity, out var initialQuantity))
        {
            MessageBox.Show("Некорректное значение начального количества.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        SelectedStock.InitialQuantity = initialQuantity;
        //SelectedStock.NomenclatureId = 
        var success = await _stockService.UpdateStockAsync(SelectedStock);
        if (success)
        {
            await RefreshStocks();
        }
        else
        {
            MessageBox.Show("Ошибка при сохранении записи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task Delete()
    {
        if (SelectedStock == null)
        {
            MessageBox.Show("Выберите запись для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show("Вы уверены, что хотите удалить запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            var success = await _stockService.DeleteStockAsync(SelectedStock.Id);
            if (success)
            {
                await RefreshStocks();
            }
            else
            {
                MessageBox.Show("Ошибка при удалении записи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await RefreshStocks();
    }
    public async Task LoadStocksAsync() => await RefreshStocks();
    public async Task LoadLastSelectionHistoryAsync()
    {
        if (SelectedStock != null)
        {
            var result = await _expenseService.GetUserLastSelectionAsync(SelectedStock.NomenclatureId);
            if (result != null)
            {
                _lastSelectedDriver = result.Driver;
                _lastSelectedEquipment = result.Equipment;
            }
        }
    }
    private async Task RefreshStocks()
    {
        try
        {
            TryParse((string?)InitialQuantityFilter, out var initialQty);
            TryParse((string?)ReceivedQuantityFilter, out var receivedQty);
            TryParse((string?)ConsumedQuantityFilter, out var consumedQty);
            TryParse((string?)FinalQuantityFilter, out var finalQty);

            var stocks = await _stockService.GetAllStocksAsync(
                WarehouseId,
                NomenclatureFilter,
                InitialQuantityFilterType,
                (double)initialQty,
                ReceivedQuantityFilterType,
                (double)receivedQty,
                ConsumedQuantityFilterType,
                (double)consumedQty,
                FinalQuantityFilterType,
                (double)finalQty);

            if (!string.IsNullOrWhiteSpace(NomenclatureFilter))
            {
                var words = Enumerable
                    .Select<string, string>(NomenclatureFilter.Split(' ', StringSplitOptions.RemoveEmptyEntries), w => w.ToLower())
                    .ToList();

                stocks = stocks.Where(s =>
                        words.All(w =>
                            s.Nomenclature.Name.ToLower().Contains(w) ||
                            s.Nomenclature.Code.ToLower().Contains(w) ||
                            s.Nomenclature.Article.ToLower().Contains(w) ||
                            s.Nomenclature.UnitOfMeasure.ToLower().Contains(w)))
                    .ToList();
            }

            Stocks = new ObservableCollection<RequestManagement.Common.Models.Stock>(stocks);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}