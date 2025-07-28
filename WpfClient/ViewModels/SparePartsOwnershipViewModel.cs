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
    private readonly ISparePartsOwnershipService _sparePartsOwnershipService;
    private readonly Timer _filterTimer;
    private readonly Dispatcher _dispatcher;
    [ObservableProperty] private string _filterEquipmentText;
    [ObservableProperty] private Nomenclature? _selectedNomenclature;
    [ObservableProperty] private Warehouse? _selectedWarehouse;
    [ObservableProperty] private ObservableCollection<EquipmentGroup> _equipmentGroups = [];
    [ObservableProperty] private ObservableCollection<Nomenclature> _nomenclatures = [];
    [ObservableProperty] private CollectionViewSource _equipmentGroupsViewSource;
    [ObservableProperty] private CollectionViewSource _nomenclaturesViewSource;
    [ObservableProperty] private EquipmentGroup? _selectedEquipmentGroup;
    [ObservableProperty] private ObservableCollection<Equipment> _equipments = [];
    public SparePartsOwnershipViewModel()
    {

    }

    public SparePartsOwnershipViewModel(IMessageBus messageBus, IEquipmentGroupService equipmentGroupService, ISparePartsOwnershipService sparePartsOwnershipService)
    {
        _messageBus = messageBus;
        _equipmentGroupService = equipmentGroupService;
        _sparePartsOwnershipService = sparePartsOwnershipService;
        EquipmentGroupsViewSource = new CollectionViewSource { Source = EquipmentGroups };
        NomenclaturesViewSource = new CollectionViewSource { Source = Nomenclatures };
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

    private async Task NomenclatureListUpdate()
    {
        Nomenclatures.Clear();
        var currentSortDescriptions = (NomenclaturesViewSource.View?.SortDescriptions!).ToList() ?? [];
        var nomenclatureList = await _sparePartsOwnershipService.GetAllSparePartsOwnershipsAsync(FilterEquipmentText);
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