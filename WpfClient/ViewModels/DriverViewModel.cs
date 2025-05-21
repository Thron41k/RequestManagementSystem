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

public class DriverViewModel : INotifyPropertyChanged
{
    private readonly IMessageBus _messageBus;
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly IDriverService _requestService;
    private Driver? _selectedDriver;
    private string _newDriverFullName;
    private string _newDriverShortName;
    private string _newDriverPosition;
    private readonly Timer _filterTimer;
    private string _filterText;
    private readonly Dispatcher _dispatcher;
    public event EventHandler CloseWindowRequested;
    public ObservableCollection<Driver> DriverList { get; } = [];
    public ICommand LoadDriverCommand { get; }
    public ICommand AddDriverCommand { get; }
    public ICommand UpdateDriverCommand { get; }
    public ICommand DeleteDriverCommand { get; }
    public ICommand SelectRowCommand { get; }

    public DriverViewModel(IDriverService requestService, IMessageBus messageBus)
    {
        _requestService = requestService;
        _messageBus = messageBus;
        LoadDriverCommand = new RelayCommand(Execute1);
        AddDriverCommand = new RelayCommand(Execute2);
        UpdateDriverCommand = new RelayCommand(Execute3);
        DeleteDriverCommand = new RelayCommand(Execute4);
        SelectRowCommand = new RelayCommand(Execute5);
        _dispatcher = Dispatcher.CurrentDispatcher;
        _filterTimer = new Timer(1000) { AutoReset = false }; // Задержка 1 секунда
        _filterTimer.Elapsed += async (s, e) => await LoadDriverAsync();
        return;
        async void Execute4() => await DeleteDriverAsync();
        async void Execute3() => await UpdateDriverAsync();
        async void Execute2() => await AddDriverAsync();
        async void Execute1() => await LoadDriverAsync();
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
        await LoadDriverAsync();
    }
    private async Task DeleteDriverAsync()
    {
        if (_selectedDriver != null)
        {
            await _requestService.DeleteDriverAsync(_selectedDriver.Id);
            await LoadDriverAsync(); // Обновляем список после удаления
            NewDriverFullName = string.Empty;
            NewDriverShortName = string.Empty;
            NewDriverPosition = string.Empty;
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DriverUpdated));
        }
    }
    private async Task LoadDriverAsync()
    {
        var filter = string.IsNullOrWhiteSpace(_filterText) ? "" : _filterText.Trim();
        var driverList = await _requestService.GetAllDriversAsync(filter.ToLower());
        await _dispatcher.InvokeAsync(() =>
        {
            DriverList.Clear();
            foreach (var item in driverList)
            {
                DriverList.Add(new Driver { Id = item.Id, FullName = item.FullName, ShortName = item.ShortName, Position = item.Position });
            }
            return Task.CompletedTask;
        });
    }
    private async Task UpdateDriverAsync()
    {
        if (_selectedDriver != null && !string.IsNullOrEmpty(NewDriverFullName.Trim()) && !string.IsNullOrEmpty(NewDriverShortName.Trim()) && !string.IsNullOrEmpty(NewDriverPosition.Trim()))
        {
            await _requestService.UpdateDriverAsync(new RequestManagement.Common.Models.Driver { Id = _selectedDriver.Id, FullName = NewDriverFullName, ShortName = NewDriverShortName, Position = NewDriverPosition });
            await LoadDriverAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DriverUpdated));
        }
    }
    private async Task AddDriverAsync()
    {
        if (!string.IsNullOrWhiteSpace(NewDriverFullName) && !string.IsNullOrWhiteSpace(NewDriverShortName))
        {
            await _requestService.CreateDriverAsync(new RequestManagement.Common.Models.Driver { FullName = NewDriverFullName.Trim(), ShortName = NewDriverShortName.Trim(), Position = !string.IsNullOrEmpty(NewDriverPosition) ? NewDriverPosition.Trim() : "" });
            await LoadDriverAsync();
            NewDriverFullName = string.Empty;
            NewDriverShortName = string.Empty;
            NewDriverPosition = string.Empty;
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DriverUpdated));
        }
    }

    public string NewDriverFullName
    {
        get => _newDriverFullName;
        set
        {
            if (_newDriverFullName == value) return;
            _newDriverFullName = value;
            OnPropertyChanged(); // Уведомляем UI об изменении
        }
    }

    public string NewDriverShortName
    {
        get => _newDriverShortName;
        set
        {
            if (_newDriverShortName == value) return;
            _newDriverShortName = value;
            OnPropertyChanged(); // Уведомляем UI об изменении
        }
    }

    public string NewDriverPosition
    {
        get => _newDriverPosition;
        set
        {
            if (_newDriverPosition == value) return;
            _newDriverPosition = value;
            OnPropertyChanged(); // Уведомляем UI об изменении
        }
    }

    public Driver? SelectedDriver
    {
        get => _selectedDriver ?? null;
        set
        {
            _selectedDriver = value;
            AddToEdit();
        }
    }

    public bool EditMode { get; set; }

    private void AddToEdit()
    {
        if (_selectedDriver != null)
        {
            NewDriverFullName = _selectedDriver.FullName;
            NewDriverShortName = _selectedDriver.ShortName;
            NewDriverPosition = _selectedDriver.Position;
        }
    }

    private void SelectAndClose()
    {
        if (!EditMode && _selectedDriver != null)
        {
            CloseWindowRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}