using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Timer = System.Timers.Timer;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using WpfClient.Services;
using RequestManagement.Server.Controllers;
using System.Runtime.CompilerServices;
using RequestManagement.Common.Models;
using DefectGroup = RequestManagement.Server.Controllers.DefectGroup;

namespace WpfClient.ViewModels;

public class DefectViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly GrpcRequestService _requestService;
    private DefectViewItem? _selectedDefect;
    private int _selectedDefectGroupIndex;
    private List<DefectGroup> _defectGroupList = [];
    private List<RequestManagement.Server.Controllers.Defect> _defectList = [];
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
    public DefectViewModel(GrpcRequestService requestService)
    {
        _requestService = requestService;
        LoadDefectCommand = new RelayCommand(Execute1);
        AddDefectCommand = new RelayCommand(Execute2);
        UpdateDefectCommand = new RelayCommand(Execute3);
        DeleteDefectCommand = new RelayCommand(Execute4);
        SelectRowCommand = new RelayCommand(Execute5);
        _dispatcher = Dispatcher.CurrentDispatcher;
        _filterTimer = new Timer(1000) { AutoReset = false }; // Задержка 1 секунда
        _filterTimer.Elapsed += async (s, e) => await LoadDefectAsync();
        return;
        async void Execute4() => await DeleteDefectAsync();
        async void Execute3() => await UpdateDefectAsync();
        async void Execute2() => await AddDefectAsync();
        async void Execute1() => await LoadDefectAsync();
        void Execute5() => SelectAndClose();
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
            var request = new DeleteDefectRequest { Id = _selectedDefect.Id };
            await _requestService.DeleteDefectAsync(request);
            await LoadDefectAsync(); // Обновляем список после удаления
            NewDefectName = string.Empty;
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
            var request = new UpdateDefectRequest
            {
                Defect = new RequestManagement.Server.Controllers.Defect
                {
                    Id = _selectedDefect.Id,
                    Name = _selectedDefect.Name,
                    DefectGroupId = _defectGroupList[_selectedDefectGroupIndex].Id
                }
            };
            await _requestService.UpdateDefectAsync(request);
            await LoadDefectAsync();
        }
    }
    private async Task AddDefectAsync()
    {
        if (!string.IsNullOrWhiteSpace(NewDefectName.Trim()) && SelectedDefectGroupIndex != -1)
        {
            var request = new CreateDefectRequest
            {
                Defect = new RequestManagement.Server.Controllers.Defect
                {
                    Name = NewDefectName,
                    DefectGroupId = _defectGroupList[_selectedDefectGroupIndex].Id
                }
            };
            await _requestService.CreateDefectAsync(request);
            await LoadDefectAsync();
            NewDefectName = string.Empty;
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



    public class DefectViewItem(RequestManagement.Server.Controllers.Defect defect, string defectGroupName)
    {
        public int Id { get; set; } = defect.Id;
        public string Name { get; set; } = defect.Name;
        public int DefectGroupId { get; set; } = defect.DefectGroupId;
        public string DefectGroupName { get; set; } = defectGroupName; 
    }
}