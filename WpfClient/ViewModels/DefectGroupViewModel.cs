using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using RequestManagement.WpfClient.Messages;
using RequestManagement.WpfClient.Services.Interfaces;
using Timer = System.Timers.Timer;

namespace RequestManagement.WpfClient.ViewModels;

public partial class DefectGroupViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private readonly IDefectGroupService _defectGroupService;
    [ObservableProperty] private RequestManagement.Common.Models.DefectGroup? _selectedDefectGroup;
    [ObservableProperty] private string _newDefectGroupName;
    [ObservableProperty] private string _filterText;
    [ObservableProperty] private ObservableCollection<RequestManagement.Common.Models.DefectGroup> _defectGroupList = [];
    [ObservableProperty] private CollectionViewSource _defectGroupViewSource;
    public bool DialogResult { get; private set; }
    public bool EditMode { get; set; }
    private readonly Dispatcher _dispatcher;
    private readonly Timer _filterTimer;
    public event EventHandler CloseWindowRequested;

    public DefectGroupViewModel(IDefectGroupService defectGroupService, IMessageBus messageBus)
    {
        _defectGroupService = defectGroupService;
        _messageBus = messageBus;
        _dispatcher = Dispatcher.CurrentDispatcher;
        DefectGroupViewSource = new CollectionViewSource { Source = DefectGroupList };
        _filterTimer = new Timer(Vars.SearchDelay) { AutoReset = false };
        _filterTimer.Elapsed += async (_, _) => await LoadDefectGroupAsync();
    }

    partial void OnSelectedDefectGroupChanged(RequestManagement.Common.Models.DefectGroup? value) => AddToEdit();

    partial void OnFilterTextChanged(string value)
    {
        _filterTimer.Stop();
        _filterTimer.Start();
    }

    public async Task Load()
    {
        await LoadDefectGroupAsync();
    }

    [RelayCommand]
    private async Task DeleteDefectGroupAsync()
    {
        if (SelectedDefectGroup != null)
        {
            await _defectGroupService.DeleteDefectGroupAsync(SelectedDefectGroup.Id);
            await LoadDefectGroupAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DefectGroupUpdated));
        }
    }

    [RelayCommand]
    private async Task LoadDefectGroupAsync()
    {
        var filter = string.IsNullOrWhiteSpace(FilterText) ? "" : FilterText.Trim();
        var defectGroupList = await _defectGroupService.GetAllDefectGroupsAsync(filter.ToLower());
        await _dispatcher.InvokeAsync(() =>
        {
            var currentSortDescriptions = Enumerable.ToList<SortDescription>(DefectGroupViewSource.View?.SortDescriptions) ?? [];
            DefectGroupList = new ObservableCollection<RequestManagement.Common.Models.DefectGroup>(defectGroupList);
            DefectGroupViewSource.Source = DefectGroupList;
            SelectedDefectGroup = null;
            NewDefectGroupName = string.Empty;
            if (!currentSortDescriptions.Any()) return Task.CompletedTask;
            foreach (var sortDescription in currentSortDescriptions)
            {
                DefectGroupViewSource.View?.SortDescriptions.Add(sortDescription);
            }
            return Task.CompletedTask;
        });
    }

    [RelayCommand]
    private async Task UpdateDefectGroupAsync()
    {
        if (SelectedDefectGroup != null && !string.IsNullOrEmpty(NewDefectGroupName.Trim()))
        {
            await _defectGroupService.UpdateDefectGroupAsync(new RequestManagement.Common.Models.DefectGroup
            {
                Id = SelectedDefectGroup.Id,
                Name = NewDefectGroupName.Trim()
            });
            await LoadDefectGroupAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DefectGroupUpdated));
        }
    }

    [RelayCommand]
    private async Task AddDefectGroupAsync()
    {
        if (!string.IsNullOrWhiteSpace(NewDefectGroupName.Trim()))
        {
            await _defectGroupService.CreateDefectGroupAsync(new RequestManagement.Common.Models.DefectGroup
            {
                Name = NewDefectGroupName.Trim()
            });
            await LoadDefectGroupAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.DefectGroupUpdated));
        }
    }

    private void AddToEdit()
    {
        if (SelectedDefectGroup != null)
        {
            NewDefectGroupName = SelectedDefectGroup.Name;
        }
    }

    [RelayCommand]
    private void SelectAndClose()
    {
        if (EditMode || SelectedDefectGroup == null) return;
        DialogResult = true;
        CloseWindowRequested.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void ClearDefectGroupName()
    {
        NewDefectGroupName = string.Empty;
    }
}