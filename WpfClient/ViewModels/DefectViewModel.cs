using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.WpfClient.Messages;
using RequestManagement.WpfClient.Services.Interfaces;
using Timer = System.Timers.Timer;

namespace RequestManagement.WpfClient.ViewModels;

public partial class DefectViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private readonly IDefectService _defectService;
    private readonly IDefectGroupService _defectGroupService;
    [ObservableProperty] private string _filterText;
    [ObservableProperty] private string _newDefectName;
    [ObservableProperty] private string _selectedDefectGroupName;
    [ObservableProperty] private ObservableCollection<DefectViewItem> _defectViewItemList = [];
    [ObservableProperty] private CollectionViewSource _defectViewSource;
    [ObservableProperty] private ObservableCollection<DefectGroup> _defectGroupList = [];
    [ObservableProperty] private DefectViewItem? _selectedDefect;
    [ObservableProperty] private int _selectedDefectGroupIndex;
    private readonly Timer _filterTimer;
    private readonly Dispatcher _dispatcher;
    public bool EditMode { get; set; }
    public bool DialogResult { get; private set; }
    public event EventHandler CloseWindowRequested;

    public DefectViewModel(IDefectService defectService, IDefectGroupService defectGroupService, IMessageBus messageBus)
    {
        _defectService = defectService;
        _defectGroupService = defectGroupService;
        _messageBus = messageBus;
        DefectViewSource = new CollectionViewSource { Source = DefectViewItemList };
        _messageBus.Subscribe<UpdatedMessage>(OnUpdated);
        _dispatcher = Dispatcher.CurrentDispatcher;
        _filterTimer = new Timer(Vars.SearchDelay) { AutoReset = false };
        _filterTimer.Elapsed += async (s, e) => await LoadDefectAsync();
    }

    partial void OnFilterTextChanged(string value)
    {
        _filterTimer.Stop();
        _filterTimer.Start();
    }

    partial void OnSelectedDefectChanged(DefectViewItem? value)
    {
        AddToEdit();
    }

    private async Task OnUpdated(UpdatedMessage obj)
    {
        if (obj.Message == MessagesEnum.DefectGroupUpdated)
        {
            await LoadDefectGroupAsync();
        }
    }

    public async Task Load()
    {
        await LoadDefectGroupAsync();
        await LoadDefectAsync();
    }

    private async Task LoadDefectGroupAsync()
    {
        var defectGroupList = await _defectGroupService.GetAllDefectGroupsAsync();
        await _dispatcher.InvokeAsync(() =>
        {
            DefectGroupList = new ObservableCollection<DefectGroup>(defectGroupList);
            SelectedDefect = null;
            NewDefectName = string.Empty;
            SelectedDefectGroupIndex = -1;
            return Task.CompletedTask;
        });
    }
    private async Task LoadDefectAsync()
    {
        var filter = string.IsNullOrWhiteSpace(FilterText) ? "" : FilterText.Trim();
        var defectList = await _defectService.GetAllDefectsAsync(filter.ToLower());
        await _dispatcher.InvokeAsync(() =>
        {
            var currentSortDescriptions = Enumerable.ToList<SortDescription>(DefectViewSource.View?.SortDescriptions) ?? [];
            var list = defectList.Select(x =>
                new DefectViewItem(x, Enumerable.First<DefectGroup>(DefectGroupList, y => y.Id == x.DefectGroupId).Name));
            DefectViewItemList = new ObservableCollection<DefectViewItem>(list);
            DefectViewSource.Source = DefectViewItemList;
            SelectedDefect = null;
            NewDefectName = string.Empty;
            SelectedDefectGroupIndex = -1;
            if (!currentSortDescriptions.Any()) return Task.CompletedTask;
            foreach (var sortDescription in currentSortDescriptions)
            {
                DefectViewSource.View?.SortDescriptions.Add(sortDescription);
            }
            return Task.CompletedTask;
        });
    }

    private void AddToEdit()
    {
        if (SelectedDefect == null) return;
        NewDefectName = SelectedDefect.Name;
        SelectedDefectGroupIndex = DefectGroupList.IndexOf(new DefectGroup { Id = SelectedDefect.DefectGroupId });
    }

    [RelayCommand]
    private async Task DeleteDefect()
    {
        if (SelectedDefect != null)
        {
            await _defectService.DeleteDefectAsync(SelectedDefect.Id);
            await LoadDefectAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DefectUpdated));
        }
    }

    [RelayCommand]
    private async Task UpdateDefect()
    {
        if (SelectedDefect != null && !string.IsNullOrEmpty(NewDefectName.Trim()) && SelectedDefectGroupIndex != -1)
        {
            await _defectService.UpdateDefectAsync(new Defect
            {
                Id = SelectedDefect.Id,
                Name = NewDefectName.Trim(),
                DefectGroupId = DefectGroupList[SelectedDefectGroupIndex].Id
            });
            await LoadDefectAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DefectUpdated));
        }
    }

    [RelayCommand]
    private async Task AddDefect()
    {
        if (!string.IsNullOrWhiteSpace(NewDefectName.Trim()) && SelectedDefectGroupIndex != -1)
        {
            await _defectService.CreateDefectAsync(new Defect
            {
                Name = NewDefectName,
                DefectGroupId = DefectGroupList[SelectedDefectGroupIndex].Id
            });
            await LoadDefectAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DefectUpdated));
        }
    }

    [RelayCommand]
    private void SelectAndClose()
    {
        if (EditMode || SelectedDefect == null) return;
        DialogResult = true;
        CloseWindowRequested.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void ClearDefectName()
    {
        NewDefectName = string.Empty;
    }

    public class DefectViewItem(Defect defect, string defectGroupName)
    {
        public int Id { get; } = defect.Id;
        public string Name { get; } = defect.Name;
        public int DefectGroupId { get; } = defect.DefectGroupId;
        public string DefectGroupName { get; set; } = defectGroupName;
    }
}