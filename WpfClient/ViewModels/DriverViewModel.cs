using System.Windows.Threading;
using System.Collections.ObjectModel;
using Timer = System.Timers.Timer;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using WpfClient.Services.Interfaces;
using WpfClient.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Data;
using RequestManagement.Common.Models;

namespace WpfClient.ViewModels;

public partial class DriverViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private readonly IDriverService _requestService;
    [ObservableProperty] private Driver? _selectedDriver;
    [ObservableProperty] private string _newDriverFullName;
    [ObservableProperty] private string _newDriverPosition;
    [ObservableProperty] private string _newDriverCode;
    [ObservableProperty] private string _filterText;
    [ObservableProperty] private ObservableCollection<Driver> _driverList = [];
    [ObservableProperty] private CollectionViewSource _driverViewSource;
    private readonly Timer _filterTimer;
    private readonly Dispatcher _dispatcher;
    public bool DialogResult { get; private set; }
    public event EventHandler CloseWindowRequested;
    public bool EditMode { get; set; }

    public DriverViewModel(IDriverService requestService, IMessageBus messageBus)
    {
        _requestService = requestService;
        _messageBus = messageBus;
        DriverViewSource = new CollectionViewSource { Source = DriverList };
        _dispatcher = Dispatcher.CurrentDispatcher;
        _filterTimer = new Timer(Vars.SearchDelay) { AutoReset = false };
        _filterTimer.Elapsed += async (_, _) => await LoadDriverAsync();
    }

    partial void OnFilterTextChanged(string value)
    {
        _filterTimer.Stop();
        _filterTimer.Start();
    }

    public async Task Load()
    {
        await LoadDriverAsync();
    }
    [RelayCommand]
    private async Task DeleteDriver()
    {
        if (SelectedDriver != null)
        {
            await _requestService.DeleteDriverAsync(SelectedDriver.Id);
            await LoadDriverAsync();
            NewDriverFullName = string.Empty;
            NewDriverPosition = string.Empty;
            NewDriverCode = string.Empty;
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DriverUpdated));
        }
    }
    private async Task LoadDriverAsync()
    {
        var filter = string.IsNullOrWhiteSpace(FilterText) ? "" : FilterText.Trim();
        var driverList = await _requestService.GetAllDriversAsync(filter.ToLower());
        await _dispatcher.InvokeAsync(() =>
        {
            DriverList = new ObservableCollection<Driver>(driverList);
            DriverViewSource.Source = DriverList;
            NewDriverFullName = string.Empty;
            NewDriverPosition = string.Empty;
            NewDriverCode = string.Empty;
            return Task.CompletedTask;
        });
    }

    [RelayCommand]
    private async Task UpdateDriver()
    {
        if (SelectedDriver != null && !string.IsNullOrEmpty(NewDriverFullName.Trim()) && !string.IsNullOrEmpty(NewDriverCode.Trim()))
        {
            await _requestService.UpdateDriverAsync(new Driver { Id = SelectedDriver.Id, FullName = NewDriverFullName, ShortName = RequestManagement.Common.Utilities.NameFormatter.FormatToShortName(NewDriverFullName), Position = NewDriverPosition, Code = NewDriverCode });
            await LoadDriverAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DriverUpdated));
        }
    }

    [RelayCommand]
    private async Task AddDriver()
    {
        if (!string.IsNullOrEmpty(NewDriverFullName.Trim()) && !string.IsNullOrEmpty(NewDriverCode.Trim()))
        {
            await _requestService.CreateDriverAsync(new Driver { FullName = NewDriverFullName.Trim(), ShortName = RequestManagement.Common.Utilities.NameFormatter.FormatToShortName(NewDriverFullName), Position = !string.IsNullOrEmpty(NewDriverPosition) ? NewDriverPosition.Trim() : "", Code = NewDriverCode.Trim() });
            await LoadDriverAsync();
            NewDriverFullName = string.Empty;
            NewDriverCode = string.Empty;
            NewDriverPosition = string.Empty;
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DriverUpdated));
        }
    }

    partial void OnSelectedDriverChanged(Driver? value)
    {
        if (value != null)
        {
            NewDriverFullName = value.FullName;
            NewDriverCode = value.Code;
            NewDriverPosition = value.Position;
        }
        else
        {
            NewDriverFullName = string.Empty;
            NewDriverCode = string.Empty;
            NewDriverPosition = string.Empty;
        }
    }

    [RelayCommand]
    private void SelectAndClose()
    {
        if (EditMode || SelectedDriver == null) return;
        DialogResult = true;
        CloseWindowRequested.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void ClearFilterText()
    {
        FilterText = string.Empty;
    }

    [RelayCommand]
    private void ClearDriverFullName()
    {
        NewDriverFullName = string.Empty;
    }

    [RelayCommand]
    private void ClearDriverCode()
    {
        NewDriverCode = string.Empty;
    }

    [RelayCommand]
    private void ClearDriverPosition()
    {
        NewDriverPosition = string.Empty;
    }
}