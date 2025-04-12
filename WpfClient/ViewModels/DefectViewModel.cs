using System.Collections.ObjectModel;
using System.ComponentModel;
using Timer = System.Timers.Timer;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using System.Runtime.CompilerServices;
using RequestManagement.Common.Interfaces;
using WpfClient.Services.Interfaces;
using WpfClient.Messages;

namespace WpfClient.ViewModels;

public class DefectViewModel : INotifyPropertyChanged
{
    private readonly IMessageBus _messageBus;
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly IDefectService _requestService;
    private DefectViewItem? _selectedDefect;
    private int _selectedDefectGroupIndex;
    private List<RequestManagement.Common.Models.DefectGroup> _defectGroupList = [];
    private List<RequestManagement.Common.Models.Defect> _defectList = [];
    private string _newDefectName;
    private readonly Timer _filterTimer;
    private string _filterText;
    private readonly Dispatcher _dispatcher;
    public event EventHandler CloseWindowRequested;
    public ObservableCollection<DefectViewItem> DefectList { get; } = [];
    public ObservableCollection<string> DefectGroupList { get; } = [];
    public ICommand LoadDefectCommand { get; }
    public ICommand AddDefectCommand { get; }
    public ICommand UpdateDefectCommand { get; }
    public ICommand DeleteDefectCommand { get; }
    public ICommand SelectRowCommand { get; }
    public ICommand UpdateDefectGroupListCommand { get; }

    public DefectViewModel(IDefectService requestService, IMessageBus messageBus)
    {
        _requestService = requestService;
        _messageBus = messageBus;
        _messageBus.Subscribe<UpdatedMessage>(OnUpdated);
        LoadDefectCommand = new RelayCommand(Execute1);
        AddDefectCommand = new RelayCommand(Execute2);
        UpdateDefectCommand = new RelayCommand(Execute3);
        DeleteDefectCommand = new RelayCommand(Execute4);
        SelectRowCommand = new RelayCommand(Execute5);
        UpdateDefectGroupListCommand = new RelayCommand(Execute6);
        _dispatcher = Dispatcher.CurrentDispatcher;
        _filterTimer = new Timer(1000) { AutoReset = false }; // Задержка 1 секунда
        _filterTimer.Elapsed += async (s, e) => await LoadDefectAsync();
        return;
        async void Execute4() => await DeleteDefectAsync();
        async void Execute3() => await UpdateDefectAsync();
        async void Execute2() => await AddDefectAsync();
        async void Execute1() => await LoadDefectAsync();
        void Execute5() => SelectAndClose();
        async void Execute6() => await LoadDefectGroupAsync();
    }

    private async Task OnUpdated(UpdatedMessage obj)
    {
        if (obj.Message == MessagesEnum.DefectGroupUpdated)
        {
            await LoadDefectGroupAsync();
        }
    }

    public string NewDefectName
    {
        get => _newDefectName;
        set
        {
            if (_newDefectName == value) return;
            _newDefectName = value;
            OnPropertyChanged(); // Уведомляем UI об изменении
        }
    }

    public async Task Load()
    {
        await LoadDefectGroupAsync();
        await LoadDefectAsync();
    }
    private async Task DeleteDefectAsync()
    {
        if (_selectedDefect != null)
        {
            await _requestService.DeleteDefectAsync(_selectedDefect.Id);
            await LoadDefectAsync(); // Обновляем список после удаления
            NewDefectName = string.Empty;
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DefectUpdated));
        }
    }

    private async Task LoadDefectGroupAsync()
    {
        var filter = string.IsNullOrWhiteSpace(_filterText) ? "" : _filterText.Trim();
        _defectGroupList = await _requestService.GetAllDefectGroupsAsync(filter.ToLower());
        await _dispatcher.InvokeAsync(() =>
        {
            DefectGroupList.Clear();
            foreach (var item in _defectGroupList)
            {
                DefectGroupList.Add(item.Name);
            }
            SelectedDefectGroupIndex = -1;
            return Task.CompletedTask;
        });
    }
    private async Task LoadDefectAsync()
    {
        var filter = string.IsNullOrWhiteSpace(_filterText) ? "" : _filterText.Trim();
        _defectList = await _requestService.GetAllDefectsAsync(filter.ToLower());
        await _dispatcher.InvokeAsync(() =>
        {
            DefectList.Clear();
            foreach (var item in _defectList)
            {
                DefectList.Add(new DefectViewItem(item, _defectGroupList.First(x => x.Id == item.DefectGroupId).Name));
            }
            return Task.CompletedTask;
        });
    }
    private async Task UpdateDefectAsync()
    {
        if (_selectedDefect != null && !string.IsNullOrEmpty(NewDefectName.Trim()) && SelectedDefectGroupIndex != -1)
        {
            await _requestService.UpdateDefectAsync(new RequestManagement.Common.Models.Defect
            {
                Id = _selectedDefect.Id,
                Name = _selectedDefect.Name,
                DefectGroupId = _defectGroupList[_selectedDefectGroupIndex].Id
            });
            await LoadDefectAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DefectUpdated));
        }
    }
    private async Task AddDefectAsync()
    {
        if (!string.IsNullOrWhiteSpace(NewDefectName.Trim()) && SelectedDefectGroupIndex != -1)
        {
            await _requestService.CreateDefectAsync(new RequestManagement.Common.Models.Defect
            {
                Name = NewDefectName,
                DefectGroupId = _defectGroupList[_selectedDefectGroupIndex].Id
            });
            await LoadDefectAsync();
            NewDefectName = string.Empty;
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DefectUpdated));
        }
    }
    public DefectViewItem? SelectedDefect
    {
        get => _selectedDefect ?? null;
        set
        {
            _selectedDefect = value;
            AddToEdit();
        }
    }

    public int SelectedDefectGroupIndex
    {
        get => _selectedDefectGroupIndex;
        set
        {
            _selectedDefectGroupIndex = value;
            OnPropertyChanged();
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
        if (_selectedDefect != null)
        {
            NewDefectName = _selectedDefect.Name;
            SelectedDefectGroupIndex = _defectGroupList.FindIndex(x => x.Id == _selectedDefect.DefectGroupId);
        }
    }
    private void SelectAndClose()
    {
        if (_selectedDefect != null)
        {
            CloseWindowRequested.Invoke(this, EventArgs.Empty);
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }



    public class DefectViewItem(RequestManagement.Common.Models.Defect defect, string defectGroupName)
    {
        public int Id { get; set; } = defect.Id;
        public string Name { get; set; } = defect.Name;
        public int DefectGroupId { get; set; } = defect.DefectGroupId;
        public string DefectGroupName { get; set; } = defectGroupName;
    }
}