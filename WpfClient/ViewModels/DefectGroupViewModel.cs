using System.Collections.ObjectModel;
using System.ComponentModel;
using Timer = System.Timers.Timer;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Server.Controllers;
using System.Runtime.CompilerServices;
using RequestManagement.Common.Interfaces;
using WpfClient.Services.Interfaces;
using WpfClient.Messages;

namespace WpfClient.ViewModels;

public class DefectGroupViewModel : INotifyPropertyChanged
{
    private readonly IMessageBus _messageBus;
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly IDefectGroupService _requestService;
    private DefectGroup ? _selectedDefectGroup;
    private string _newDefectGroupName;
    private readonly Timer _filterTimer;
    private string _filterText;
    private readonly Dispatcher _dispatcher;
    public event EventHandler CloseWindowRequested;
    public ObservableCollection<DefectGroup> DefectGroupList { get; } = [];
    public ICommand LoadDefectGroupCommand { get; }
    public ICommand AddDefectGroupCommand { get; }
    public ICommand UpdateDefectGroupCommand { get; }
    public ICommand DeleteDefectGroupCommand { get; }
    public ICommand SelectRowCommand { get; }
    public DefectGroupViewModel(IDefectGroupService requestService, IMessageBus messageBus)
    {
        _requestService = requestService;
        _messageBus = messageBus;
        LoadDefectGroupCommand = new RelayCommand(Execute1);
        AddDefectGroupCommand = new RelayCommand(Execute2);
        UpdateDefectGroupCommand = new RelayCommand(Execute3);
        DeleteDefectGroupCommand = new RelayCommand(Execute4);
        SelectRowCommand = new RelayCommand(Execute5);
        _dispatcher = Dispatcher.CurrentDispatcher;
        _filterTimer = new Timer(1000) { AutoReset = false }; // Задержка 1 секунда
        _filterTimer.Elapsed += async (s, e) => await LoadDefectGroupAsync();
        return;
        async void Execute4() => await DeleteDefectGroupAsync();
        async void Execute3() => await UpdateDefectGroupAsync();
        async void Execute2() => await AddDefectGroupAsync();
        async void Execute1() => await LoadDefectGroupAsync();
        void Execute5() => SelectAndClose();
    }

    public string NewDefectGroupName
    {
        get => _newDefectGroupName;
        set
        {
            if (_newDefectGroupName == value) return;
            _newDefectGroupName = value;
            OnPropertyChanged(); // Уведомляем UI об изменении
        }
    }

    public async Task Load()
    {
        await LoadDefectGroupAsync();
    }
    private async Task DeleteDefectGroupAsync()
    {
        if (_selectedDefectGroup != null)
        {
            await _requestService.DeleteDefectGroupAsync(_selectedDefectGroup.Id);
            await LoadDefectGroupAsync(); // Обновляем список после удаления
            NewDefectGroupName = string.Empty;
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DefectGroupUpdated));
        }
    }
    private async Task LoadDefectGroupAsync()
    {
        var filter = string.IsNullOrWhiteSpace(_filterText) ? "" : _filterText.Trim();
        var defectGroupList = await _requestService.GetAllDefectGroupsAsync(filter.ToLower());
        await _dispatcher.InvokeAsync(() =>
        {
            DefectGroupList.Clear();
            foreach (var item in defectGroupList)
            {
                DefectGroupList.Add(new DefectGroup { Id = item.Id, Name = item.Name });
            }
            return Task.CompletedTask;
        });
    }
    private async Task UpdateDefectGroupAsync()
    {
        if (_selectedDefectGroup != null && !string.IsNullOrEmpty(NewDefectGroupName.Trim()))
        {
            await _requestService.UpdateDefectGroupAsync(new RequestManagement.Common.Models.DefectGroup
            {
                Id = _selectedDefectGroup.Id,
                Name = _selectedDefectGroup.Name,
            });
            await LoadDefectGroupAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DefectGroupUpdated));
        }
    }
    private async Task AddDefectGroupAsync()
    {
        if (!string.IsNullOrWhiteSpace(NewDefectGroupName.Trim()))
        {
            await _requestService.CreateDefectGroupAsync(new RequestManagement.Common.Models.DefectGroup
            {
                Name = NewDefectGroupName
            });
            await LoadDefectGroupAsync();
            NewDefectGroupName = string.Empty;
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DefectGroupUpdated));
        }
    }
    public DefectGroup? SelectedDefectGroup
    {
        get => _selectedDefectGroup ?? null;
        set
        {
            _selectedDefectGroup = value;
            AddToEdit();
        }
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
    private void AddToEdit()
    {
        if (_selectedDefectGroup != null)
        {
            NewDefectGroupName = _selectedDefectGroup.Name;
        }
    }
    private void SelectAndClose()
    {
        if (_selectedDefectGroup != null)
        {
            CloseWindowRequested.Invoke(this, EventArgs.Empty);
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}