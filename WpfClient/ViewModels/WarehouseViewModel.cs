using System.Windows.Threading;
using System.Collections.ObjectModel;
using Timer = System.Timers.Timer;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using WpfClient.Services.Interfaces;
using WpfClient.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Data;

namespace WpfClient.ViewModels;

public partial class WarehouseViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private readonly IWarehouseService _warehouseService;
    [ObservableProperty] private RequestManagement.Common.Models.Warehouse? _selectedWarehouse;
    [ObservableProperty] private string _newWarehouseName;
    [ObservableProperty] private string _filterText;
    [ObservableProperty] private ObservableCollection<RequestManagement.Common.Models.Warehouse> _warehouseList = [];
    [ObservableProperty] private CollectionViewSource _warehouseViewSource;
    public bool DialogResult { get; private set; }
    public bool EditMode { get; set; }
    private readonly Dispatcher _dispatcher;
    private readonly Timer _filterTimer;
    public event EventHandler CloseWindowRequested;

    public WarehouseViewModel(IWarehouseService warehouseService, IMessageBus messageBus)
    {
        _warehouseService = warehouseService;
        _messageBus = messageBus;
        _dispatcher = Dispatcher.CurrentDispatcher;
        WarehouseViewSource = new CollectionViewSource { Source = WarehouseList };
        _filterTimer = new Timer(Vars.SearchDelay) { AutoReset = false };
        _filterTimer.Elapsed += async (_, _) => await LoadWarehouseAsync();
    }

    partial void OnSelectedWarehouseChanged(RequestManagement.Common.Models.Warehouse? value) => AddToEdit();

    partial void OnFilterTextChanged(string value)
    {
        _filterTimer.Stop(); 
        _filterTimer.Start();
    }

    public async Task Load()
    {
        await LoadWarehouseAsync();
    }

    private void AddToEdit()
    {
        if (SelectedWarehouse != null)
        {
            NewWarehouseName = SelectedWarehouse.Name;
        }
    }

    private async Task LoadWarehouseAsync()
    {
        var filter = string.IsNullOrWhiteSpace(FilterText) ? "" : FilterText.Trim();
        var warehouseList = await _warehouseService.GetAllWarehousesAsync(filter.ToLower());
        await _dispatcher.InvokeAsync(() =>
        {
            var currentSortDescriptions = WarehouseViewSource.View?.SortDescriptions.ToList() ?? [];
            WarehouseList = new ObservableCollection<RequestManagement.Common.Models.Warehouse>(warehouseList);
            WarehouseViewSource.Source = WarehouseList;
            SelectedWarehouse = null;
            NewWarehouseName = string.Empty;
            if (!currentSortDescriptions.Any()) return Task.CompletedTask;
            foreach (var sortDescription in currentSortDescriptions)
            {
                WarehouseViewSource.View?.SortDescriptions.Add(sortDescription);
            }
            return Task.CompletedTask;
        });
    }

    [RelayCommand]
    private async Task DeleteWarehouseAsync()
    {
        if (SelectedWarehouse != null)
        {
            await _warehouseService.DeleteWarehouseAsync(SelectedWarehouse.Id);
            await LoadWarehouseAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.WarehouseUpdated));
        }
    }

    [RelayCommand]
    private async Task UpdateWarehouseAsync()
    {
        if (SelectedWarehouse != null && !string.IsNullOrEmpty(NewWarehouseName.Trim()))
        {
            await _warehouseService.UpdateWarehouseAsync(new RequestManagement.Common.Models.Warehouse { Id = SelectedWarehouse.Id, Name = NewWarehouseName.Trim() });
            await LoadWarehouseAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.WarehouseUpdated));
        }
    }

    [RelayCommand]
    private async Task AddWarehouseAsync()
    {
        if (!string.IsNullOrWhiteSpace(NewWarehouseName.Trim()))
        {
            await _warehouseService.CreateWarehouseAsync(new RequestManagement.Common.Models.Warehouse { Name = NewWarehouseName.Trim() });
            await LoadWarehouseAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.WarehouseUpdated));
        }
    }

    [RelayCommand]
    private void SelectAndClose()
    {
        if (EditMode || SelectedWarehouse == null) return;
        DialogResult = true;
        CloseWindowRequested.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void ClearWarehouseName()
    {
        NewWarehouseName = string.Empty;
    }
}