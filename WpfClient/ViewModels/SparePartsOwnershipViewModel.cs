using CommunityToolkit.Mvvm.ComponentModel;
using RequestManagement.WpfClient.Services.Interfaces;
using RequestManagement.Common.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Threading;
using RequestManagement.Common.Models;
using CommunityToolkit.Mvvm.Input;
using Timer = System.Timers.Timer;

namespace RequestManagement.WpfClient.ViewModels;

public partial class SparePartsOwnershipViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private readonly IEquipmentGroupService _equipmentGroupService;
    private readonly Timer _filterTimer;
    private readonly Dispatcher _dispatcher;
    [ObservableProperty] private string _filterEquipmentText;
    [ObservableProperty] private ObservableCollection<EquipmentGroup> _equipmentGroups = [];
    [ObservableProperty] private CollectionViewSource _equipmentGroupsViewSource;
    [ObservableProperty] private EquipmentGroup? _selectedEquipmentGroup;
    [ObservableProperty] private ObservableCollection<Equipment> _equipments = [];
    public SparePartsOwnershipViewModel()
    {

    }

    public SparePartsOwnershipViewModel(IMessageBus messageBus, IEquipmentGroupService equipmentGroupService)
    {
        _messageBus = messageBus;
        _equipmentGroupService = equipmentGroupService;
        EquipmentGroupsViewSource = new CollectionViewSource { Source = EquipmentGroups };
        _dispatcher = Dispatcher.CurrentDispatcher;
        _filterTimer = new Timer(Vars.SearchDelay) { AutoReset = false };
        _filterTimer.Elapsed += async (_, _) =>
        {
            await _dispatcher.InvokeAsync(async () => { await LoadEquipmentGroupsAsync(); });
        };
    }

    partial void OnFilterEquipmentTextChanged(string value)
    {
        _filterTimer.Stop();
        _filterTimer.Start();
    }

    partial void OnSelectedEquipmentGroupChanged(EquipmentGroup? value) => EquipmentListUpdate();
    public async Task InitializeAsync()
    {
        await LoadEquipmentGroupsAsync();
    }
    private async Task LoadEquipmentGroupsAsync()
    {
        var currentSortDescriptions = (EquipmentGroupsViewSource.View?.SortDescriptions!).ToList() ?? [];
        var equipmentGroupList = await _equipmentGroupService.GetAllEquipmentGroupsAsync(FilterEquipmentText);
        EquipmentGroups = new ObservableCollection<EquipmentGroup>(equipmentGroupList);
        EquipmentGroupsViewSource.Source = EquipmentGroups;
        if (currentSortDescriptions.Any())
        {
            foreach (var sortDescription in currentSortDescriptions)
            {
                EquipmentGroupsViewSource.View?.SortDescriptions.Add(sortDescription);
            }
        }
        EquipmentListUpdate();
    }

    private void EquipmentListUpdate()
    {
        Equipments.Clear();
        if (SelectedEquipmentGroup is { Equipments.Count: > 0 })
        {
            Equipments = new ObservableCollection<Equipment>(SelectedEquipmentGroup.Equipments);
        }
    }

    [RelayCommand]
    private void SelectEquipmentGroup()
    {
        if (SelectedEquipmentGroup != null)
        {
            EquipmentListUpdate();
        }
    }
}