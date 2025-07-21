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

public partial class EquipmentGroupViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private readonly IEquipmentGroupService _equipmentGroupService;
    [ObservableProperty] private RequestManagement.Common.Models.EquipmentGroup? _selectedEquipmentGroup;
    [ObservableProperty] private string _newEquipmentGroupName;
    [ObservableProperty] private string _filterText;
    [ObservableProperty] private ObservableCollection<RequestManagement.Common.Models.EquipmentGroup> _equipmentGroupList = [];
    [ObservableProperty] private CollectionViewSource _equipmentGroupViewSource;
    public bool DialogResult { get; private set; }
    public bool EditMode { get; set; }
    private readonly Dispatcher _dispatcher;
    private readonly Timer _filterTimer;
    public event EventHandler CloseWindowRequested;

    public EquipmentGroupViewModel(IEquipmentGroupService equipmentGroupService, IMessageBus messageBus)
    {
        _equipmentGroupService = equipmentGroupService;
        _messageBus = messageBus;
        _dispatcher = Dispatcher.CurrentDispatcher;
        EquipmentGroupViewSource = new CollectionViewSource { Source = EquipmentGroupList };
        _filterTimer = new Timer(Vars.SearchDelay) { AutoReset = false };
        _filterTimer.Elapsed += async (_, _) => await LoadEquipmentGroupAsync();
    }

    partial void OnSelectedEquipmentGroupChanged(RequestManagement.Common.Models.EquipmentGroup? value) => AddToEdit();

    partial void OnFilterTextChanged(string value)
    {
        _filterTimer.Stop();
        _filterTimer.Start();
    }

    public async Task Load()
    {
        await LoadEquipmentGroupAsync();
    }

    [RelayCommand]
    private async Task DeleteEquipmentGroupAsync()
    {
        if (SelectedEquipmentGroup != null)
        {
            await _equipmentGroupService.DeleteEquipmentGroupAsync(SelectedEquipmentGroup.Id);
            await LoadEquipmentGroupAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.EquipmentGroupUpdated));
        }
    }

    [RelayCommand]
    private async Task LoadEquipmentGroupAsync()
    {
        var filter = string.IsNullOrWhiteSpace(FilterText) ? "" : FilterText.Trim();
        var equipmentGroupList = await _equipmentGroupService.GetAllEquipmentGroupsAsync(filter.ToLower());
        await _dispatcher.InvokeAsync(() =>
        {
            var currentSortDescriptions = (EquipmentGroupViewSource.View?.SortDescriptions).ToList() ?? [];
            EquipmentGroupList = new ObservableCollection<RequestManagement.Common.Models.EquipmentGroup>(equipmentGroupList);
            EquipmentGroupViewSource.Source = EquipmentGroupList;
            SelectedEquipmentGroup = null;
            NewEquipmentGroupName = string.Empty;
            if (!currentSortDescriptions.Any()) return Task.CompletedTask;
            foreach (var sortDescription in currentSortDescriptions)
            {
                EquipmentGroupViewSource.View?.SortDescriptions.Add(sortDescription);
            }
            return Task.CompletedTask;
        });
    }

    [RelayCommand]
    private async Task UpdateEquipmentGroupAsync()
    {
        if (SelectedEquipmentGroup != null && !string.IsNullOrEmpty(NewEquipmentGroupName.Trim()))
        {
            await _equipmentGroupService.UpdateEquipmentGroupAsync(new RequestManagement.Common.Models.EquipmentGroup
            {
                Id = SelectedEquipmentGroup.Id,
                Name = NewEquipmentGroupName.Trim()
            });
            await LoadEquipmentGroupAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.EquipmentGroupUpdated));
        }
    }

    [RelayCommand]
    private async Task AddEquipmentGroupAsync()
    {
        if (!string.IsNullOrWhiteSpace(NewEquipmentGroupName.Trim()))
        {
            await _equipmentGroupService.CreateEquipmentGroupAsync(new RequestManagement.Common.Models.EquipmentGroup
            {
                Name = NewEquipmentGroupName.Trim()
            });
            await LoadEquipmentGroupAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.EquipmentGroupUpdated));
        }
    }

    private void AddToEdit()
    {
        if (SelectedEquipmentGroup != null)
        {
            NewEquipmentGroupName = SelectedEquipmentGroup.Name;
        }
    }

    [RelayCommand]
    private void SelectAndClose()
    {
        if (EditMode || SelectedEquipmentGroup == null) return;
        DialogResult = true;
        CloseWindowRequested.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void ClearEquipmentGroupName()
    {
        NewEquipmentGroupName = string.Empty;
    }
}