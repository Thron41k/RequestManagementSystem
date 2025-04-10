using System.ComponentModel;
using RequestManagement.Server.Controllers;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Timer = System.Timers.Timer;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using WpfClient.Services.Interfaces;
using WpfClient.Messages;

namespace WpfClient.ViewModels;

public class WarehouseViewModel : INotifyPropertyChanged
{
    private readonly IMessageBus _messageBus;
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly IWarehouseService _requestService;
    private Warehouse? _selectedWarehouse;
    private string _newWarehouseName;
    private readonly Timer _filterTimer;
    private string _filterText;
    private readonly Dispatcher _dispatcher;
    public event EventHandler CloseWindowRequested;
    public ObservableCollection<Warehouse> WarehouseList { get; } = [];
    public ICommand LoadWarehouseCommand { get; }
    public ICommand AddWarehouseCommand { get; }
    public ICommand UpdateWarehouseCommand { get; }
    public ICommand DeleteWarehouseCommand { get; }
    public ICommand SelectRowCommand { get; }

    public WarehouseViewModel(IWarehouseService requestService, IMessageBus messageBus)
    {
        _requestService = requestService;
        _messageBus = messageBus;
        LoadWarehouseCommand = new RelayCommand(Execute1);
        AddWarehouseCommand = new RelayCommand(Execute2);
        UpdateWarehouseCommand = new RelayCommand(Execute3);
        DeleteWarehouseCommand = new RelayCommand(Execute4);
        SelectRowCommand = new RelayCommand(Execute5);
        _dispatcher = Dispatcher.CurrentDispatcher;
        _filterTimer = new Timer(1000) { AutoReset = false }; // Задержка 1 секунда
        _filterTimer.Elapsed += async (s, e) => await LoadWarehouseAsync();
        return;
        async void Execute4() => await DeleteWarehouseAsync();
        async void Execute3() => await UpdateWarehouseAsync();
        async void Execute2() => await AddWarehouseAsync();
        async void Execute1() => await LoadWarehouseAsync();
        void Execute5() => SelectAndClose();
    }

    public string FilterText
    {
        get => _filterText;
        set
        {
            if (_filterText == value) return;
            _filterText = value;
            OnPropertyChanged();
            _filterTimer.Stop(); // Сбрасываем таймер при каждом вводе
            _filterTimer.Start(); // Запускаем таймер заново
        }
    }
    public async Task Load()
    {
        await LoadWarehouseAsync();
    }
    private async Task DeleteWarehouseAsync()
    {
        if (_selectedWarehouse != null)
        {
            await _requestService.DeleteWarehouseAsync(_selectedWarehouse.Id);
            await LoadWarehouseAsync(); // Обновляем список после удаления
            NewWarehouseName = string.Empty;
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.WarehouseUpdated));
        }
    }
    private async Task LoadWarehouseAsync()
    {
        var filter = string.IsNullOrWhiteSpace(_filterText) ? "" : _filterText.Trim();
        var driverList = await _requestService.GetAllWarehousesAsync(filter.ToLower());
        await _dispatcher.InvokeAsync(() =>
        {
            WarehouseList.Clear();
            foreach (var item in driverList)
            {
                WarehouseList.Add(new Warehouse { Id = item.Id, Name = item.Name});
            }
            return Task.CompletedTask;
        });
    }
    private async Task UpdateWarehouseAsync()
    {
        if (_selectedWarehouse != null && !string.IsNullOrEmpty(NewWarehouseName.Trim()))
        {
            await _requestService.UpdateWarehouseAsync(new RequestManagement.Common.Models.Warehouse { Id = _selectedWarehouse.Id, Name = NewWarehouseName});
            await LoadWarehouseAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.WarehouseUpdated));
        }
    }
    private async Task AddWarehouseAsync()
    {
        if (!string.IsNullOrWhiteSpace(NewWarehouseName.Trim()))
        {
            await _requestService.CreateWarehouseAsync(new RequestManagement.Common.Models.Warehouse { Name = NewWarehouseName});
            await LoadWarehouseAsync();
            NewWarehouseName = string.Empty;
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.WarehouseUpdated));
        }
    }

    public string NewWarehouseName
    {
        get => _newWarehouseName;
        set
        {
            if (_newWarehouseName == value) return;
            _newWarehouseName = value;
            OnPropertyChanged(); // Уведомляем UI об изменении
        }
    }
    public Warehouse? SelectedWarehouse
    {
        get => _selectedWarehouse ?? null;
        set
        {
            _selectedWarehouse = value;
            AddToEdit();
        }
    }

    private void AddToEdit()
    {
        if (_selectedWarehouse != null)
        {
            NewWarehouseName = _selectedWarehouse.Name;
        }
    }

    private void SelectAndClose()
    {
        if (_selectedWarehouse != null)
        {
            CloseWindowRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}