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

public class NomenclatureViewModel : INotifyPropertyChanged
{
    private readonly IMessageBus _messageBus;
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly INomenclatureService _requestService;
    private Nomenclature? _selectedNomenclature;
    private string _newNomenclatureCode;
    private string _newNomenclatureName;
    private string _newNomenclatureArticle;
    private string _newNomenclatureUnitOfMeasure;
    private readonly Timer _filterTimer;
    private string _filterText;
    private readonly Dispatcher _dispatcher;
    public event EventHandler CloseWindowRequested;
    public ObservableCollection<Nomenclature> NomenclatureList { get; } = [];
    public ICommand LoadNomenclatureCommand { get; }
    public ICommand AddNomenclatureCommand { get; }
    public ICommand UpdateNomenclatureCommand { get; }
    public ICommand DeleteNomenclatureCommand { get; }
    public ICommand SelectRowCommand { get; }

    public NomenclatureViewModel(INomenclatureService requestService, IMessageBus messageBus)
    {
        _requestService = requestService;
        _messageBus = messageBus;
        LoadNomenclatureCommand = new RelayCommand(Execute1);
        AddNomenclatureCommand = new RelayCommand(Execute2);
        UpdateNomenclatureCommand = new RelayCommand(Execute3);
        DeleteNomenclatureCommand = new RelayCommand(Execute4);
        SelectRowCommand = new RelayCommand(Execute5);
        _dispatcher = Dispatcher.CurrentDispatcher;
        _filterTimer = new Timer(1000) { AutoReset = false }; // Задержка 1 секунда
        _filterTimer.Elapsed += async (s, e) => await LoadNomenclatureAsync();
        return;
        async void Execute4() => await DeleteNomenclatureAsync();
        async void Execute3() => await UpdateNomenclatureAsync();
        async void Execute2() => await AddNomenclatureAsync();
        async void Execute1() => await LoadNomenclatureAsync();
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
        await LoadNomenclatureAsync();
    }
    private async Task DeleteNomenclatureAsync()
    {
        if (_selectedNomenclature != null)
        {
            await _requestService.DeleteNomenclatureAsync(_selectedNomenclature.Id);
            await LoadNomenclatureAsync(); // Обновляем список после удаления
            NewNomenclatureName = string.Empty;
            NewNomenclatureCode = string.Empty;
            NewNomenclatureUnitOfMeasure = string.Empty;
            NewNomenclatureArticle = string.Empty;
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.NomenclatureUpdated));
        }
    }
    private async Task LoadNomenclatureAsync()
    {
        var filter = string.IsNullOrWhiteSpace(_filterText) ? "" : _filterText.Trim();
        var driverList = await _requestService.GetAllNomenclaturesAsync(filter.ToLower());
        await _dispatcher.InvokeAsync(() =>
        {
            NomenclatureList.Clear();
            foreach (var item in driverList)
            {
                NomenclatureList.Add(new Nomenclature { Id = item.Id, Name = item.Name, Code = item.Code, Article = item.Article, UnitOfMeasure = item.UnitOfMeasure });
            }
            return Task.CompletedTask;
        });
    }
    private async Task UpdateNomenclatureAsync()
    {
        if (_selectedNomenclature != null && !string.IsNullOrEmpty(NewNomenclatureName.Trim()) && !string.IsNullOrEmpty(NewNomenclatureCode.Trim()) && !string.IsNullOrEmpty(NewNomenclatureUnitOfMeasure.Trim()))
        {
            await _requestService.UpdateNomenclatureAsync(new RequestManagement.Common.Models.Nomenclature { Id = _selectedNomenclature.Id, Name = NewNomenclatureName, Code = NewNomenclatureCode, Article = NewNomenclatureArticle, UnitOfMeasure = NewNomenclatureUnitOfMeasure });
            await LoadNomenclatureAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.NomenclatureUpdated));
        }
    }
    private async Task AddNomenclatureAsync()
    {
        if (!string.IsNullOrWhiteSpace(NewNomenclatureName.Trim()) && !string.IsNullOrWhiteSpace(NewNomenclatureCode.Trim()) && !string.IsNullOrWhiteSpace(NewNomenclatureUnitOfMeasure.Trim()))
        {
            await _requestService.CreateNomenclatureAsync(new RequestManagement.Common.Models.Nomenclature { Name = NewNomenclatureName, Code = NewNomenclatureCode, Article = NewNomenclatureArticle, UnitOfMeasure = NewNomenclatureUnitOfMeasure });
            await LoadNomenclatureAsync();
            NewNomenclatureName = string.Empty;
            NewNomenclatureCode = string.Empty;
            NewNomenclatureUnitOfMeasure = string.Empty;
            NewNomenclatureArticle = string.Empty;
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.NomenclatureUpdated));
        }
    }

    public string NewNomenclatureCode
    {
        get => _newNomenclatureCode;
        set
        {
            if (_newNomenclatureCode == value) return;
            _newNomenclatureCode = value;
            OnPropertyChanged(); // Уведомляем UI об изменении
        }
    }
    public string NewNomenclatureName
    {
        get => _newNomenclatureName;
        set
        {
            if (_newNomenclatureName == value) return;
            _newNomenclatureName = value;
            OnPropertyChanged(); // Уведомляем UI об изменении
        }
    }
    public string NewNomenclatureArticle
    {
        get => _newNomenclatureArticle;
        set
        {
            if (_newNomenclatureArticle == value) return;
            _newNomenclatureArticle = value;
            OnPropertyChanged(); // Уведомляем UI об изменении
        }
    }
    public string NewNomenclatureUnitOfMeasure
    {
        get => _newNomenclatureUnitOfMeasure;
        set
        {
            if (_newNomenclatureUnitOfMeasure == value) return;
            _newNomenclatureUnitOfMeasure = value;
            OnPropertyChanged(); // Уведомляем UI об изменении
        }
    }
    public Nomenclature? SelectedNomenclature
    {
        get => _selectedNomenclature ?? null;
        set
        {
            _selectedNomenclature = value;
            AddToEdit();
        }
    }

    private void AddToEdit()
    {
        if (_selectedNomenclature != null)
        {
            NewNomenclatureName = _selectedNomenclature.Name;
            NewNomenclatureCode = _selectedNomenclature.Code;
            NewNomenclatureArticle = _selectedNomenclature.Article;
            NewNomenclatureUnitOfMeasure = _selectedNomenclature.UnitOfMeasure;
        }
    }

    private void SelectAndClose()
    {
        if (_selectedNomenclature != null)
        {
            CloseWindowRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}