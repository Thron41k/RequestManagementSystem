using System;
using System.Collections.Generic;
using System.ComponentModel;
using RequestManagement.Server.Controllers;
using System.Windows.Threading;
using WpfClient.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Timer = System.Timers.Timer;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using System.CodeDom.Compiler;

namespace WpfClient.ViewModels;

public class DriverViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly GrpcRequestService _requestService;
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

    public DriverViewModel(GrpcRequestService requestService)
    {
        _requestService = requestService;
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
            var request = new DeleteDriverRequest { Id = _selectedDriver.Id };
            await _requestService.DeleteDriverAsync(request);
            await LoadDriverAsync(); // Обновляем список после удаления
            NewDriverFullName = string.Empty;
            NewDriverShortName = string.Empty;
            NewDriverPosition = string.Empty;
        }
    }
    private async Task LoadDriverAsync()
    {
        var filter = string.IsNullOrWhiteSpace(_filterText) ? "" : _filterText.Trim();
        var driverList = await _requestService.GetAllDriversAsync(filter);
        await _dispatcher.InvokeAsync(() =>
        {
            DriverList.Clear();
            foreach (var item in driverList)
            {
                DriverList.Add(item);
            }
            return Task.CompletedTask;
        });
    }
    private async Task UpdateDriverAsync()
    {
        if (_selectedDriver != null && !string.IsNullOrEmpty(NewDriverFullName.Trim()) && !string.IsNullOrEmpty(NewDriverShortName.Trim()) && !string.IsNullOrEmpty(NewDriverPosition.Trim()))
        {
            var request = new UpdateDriverRequest
            {
                Driver = new Driver
                {
                    Id = _selectedDriver.Id,
                    FullName = NewDriverFullName,
                    ShortName = NewDriverShortName,
                    Position = NewDriverPosition
                }
            };
            await _requestService.UpdateDriverAsync(request);
            await LoadDriverAsync();
        }
    }
    private async Task AddDriverAsync()
    {
        if (!string.IsNullOrWhiteSpace(NewDriverFullName.Trim()) && !string.IsNullOrWhiteSpace(NewDriverShortName.Trim()) && !string.IsNullOrWhiteSpace(NewDriverPosition.Trim()))
        {
            var request = new CreateDriverRequest
            {
                Driver = new Driver
                {
                    FullName = NewDriverFullName,
                    ShortName = NewDriverShortName,
                    Position = NewDriverPosition
                }
            };
            await _requestService.CreateDriverAsync(request);
            await LoadDriverAsync();
            NewDriverFullName = string.Empty;
            NewDriverShortName = string.Empty;
            NewDriverPosition = string.Empty;
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
        if (_selectedDriver != null)
        {
            CloseWindowRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}