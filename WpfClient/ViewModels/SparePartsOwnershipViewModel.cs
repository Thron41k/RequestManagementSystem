using CommunityToolkit.Mvvm.ComponentModel;
using RequestManagement.WpfClient.Services.Interfaces;
using RequestManagement.Common.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Threading;
using RequestManagement.Common.Models;
using CommunityToolkit.Mvvm.Input;
using Timer = System.Timers.Timer;
using RequestManagement.WpfClient.Messages;

namespace RequestManagement.WpfClient.ViewModels;

public partial class SparePartsOwnershipViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private readonly IEquipmentGroupService _equipmentGroupService;
    private readonly ISparePartsOwnershipService _sparePartsOwnershipService;
    private readonly Timer _filterTimer;
    private readonly Dispatcher _dispatcher;
    [ObservableProperty] private string _filterEquipmentText;
    [ObservableProperty] private string _comment;
    [ObservableProperty] private int _requiredQuantity = 1;
    [ObservableProperty] private SparePartsOwnership? _selectedSparePartsOwnership;
    [ObservableProperty] private Nomenclature? _selectedNomenclature;
    [ObservableProperty] private Warehouse? _selectedWarehouse;
    [ObservableProperty] private ObservableCollection<EquipmentGroup> _equipmentGroups = [];
    [ObservableProperty] private ObservableCollection<SparePartsOwnership> _nomenclatures = [];
    [ObservableProperty] private ObservableCollection<SparePartsOwnership> _nomenclatureAnalogs = [];
    [ObservableProperty] private CollectionViewSource _equipmentGroupsViewSource;
    [ObservableProperty] private CollectionViewSource _nomenclaturesViewSource;
    [ObservableProperty] private CollectionViewSource _nomenclatureAnalogsViewSource;
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
        NomenclatureAnalogsViewSource = new CollectionViewSource { Source = NomenclatureAnalogs };
        _dispatcher = Dispatcher.CurrentDispatcher;
        _filterTimer = new Timer(Vars.SearchDelay) { AutoReset = false };
        _filterTimer.Elapsed += async (_, _) =>
        {
            await _dispatcher.InvokeAsync(async () => { await LoadEquipmentGroupsAsync(); });
        };
        _messageBus.Subscribe<SelectResultMessage>(OnShowDialog);
    }

    private Task OnShowDialog(SelectResultMessage arg)
    {
        if (arg.Caller == typeof(SparePartsOwnershipViewModel) && arg.Item != null)
        {
            switch (arg.Message)
            {
                case MessagesEnum.SelectWarehouse:
                    SelectedWarehouse = (Warehouse)arg.Item;
                    break;
                case MessagesEnum.SelectNomenclature:
                    SelectedNomenclature = (Nomenclature)arg.Item;
                    break;
            }
        }
        return Task.CompletedTask;
    }
    partial void OnFilterEquipmentTextChanged(string value)
    {
        _filterTimer.Stop();
        _filterTimer.Start();
    }

    partial void OnSelectedSparePartsOwnershipChanged(SparePartsOwnership? value)
    {
        FieldUpdate();
    }

    [RelayCommand]
    private void FieldUpdate()
    {
        if (SelectedSparePartsOwnership == null) return;
        Comment = SelectedSparePartsOwnership.Comment ?? "";
        RequiredQuantity = SelectedSparePartsOwnership.RequiredQuantity;
        SelectedNomenclature = SelectedSparePartsOwnership.Nomenclature;
    }

    partial void OnSelectedEquipmentGroupChanged(EquipmentGroup? value) => _ = EquipmentListUpdate();
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
        await EquipmentListUpdate();
    }

    private async Task EquipmentListUpdate()
    {
        Equipments.Clear();
        if (SelectedEquipmentGroup is { Equipments.Count: > 0 })
        {
            Equipments = new ObservableCollection<Equipment>(SelectedEquipmentGroup.Equipments);
        }

        await NomenclatureListUpdate();
    }

    private async Task NomenclatureListUpdate()
    {
        Nomenclatures.Clear();
        if (SelectedEquipmentGroup == null) return;
        var currentSortDescriptions = (NomenclaturesViewSource.View?.SortDescriptions!).ToList() ?? [];
        var nomenclatureList = await _sparePartsOwnershipService.GetAllSparePartsOwnershipsAsync(SelectedEquipmentGroup.Id, SelectedWarehouse?.Id ?? 0);
        Nomenclatures = new ObservableCollection<SparePartsOwnership>(nomenclatureList.Where(x => x.AnalogId == 0));
        NomenclaturesViewSource.Source = Nomenclatures;
        NomenclatureAnalogs = new ObservableCollection<SparePartsOwnership>(nomenclatureList.Where(x => x.AnalogId != 0));
        NomenclatureAnalogsViewSource.Source = NomenclatureAnalogs;
        Comment = string.Empty;
        RequiredQuantity = 1;
        SelectedNomenclature = null;
        if (currentSortDescriptions.Any())
        {
            foreach (var sortDescription in currentSortDescriptions)
            {
                NomenclaturesViewSource.View?.SortDescriptions.Add(sortDescription);
            }
        }
        SelectedSparePartsOwnership = null;
    }

    [RelayCommand]
    private async Task SelectEquipmentGroup()
    {
        if (SelectedEquipmentGroup != null)
        {
            await EquipmentListUpdate();
        }
    }

    [RelayCommand]
    private async Task SelectNomenclature()
    {
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectNomenclature, typeof(SparePartsOwnershipViewModel)));
    }

    [RelayCommand]
    private void ClearSelectedNomenclature()
    {
        SelectedNomenclature = null;
    }

    [RelayCommand]
    private async Task SelectWarehouse()
    {
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectWarehouse, typeof(SparePartsOwnershipViewModel)));
    }

    [RelayCommand]
    private void ClearSelectedWarehouse()
    {
        SelectedWarehouse = null;
    }

    [RelayCommand]
    private async Task CreateSparePartsOwnershipAsync()
    {
        if (SelectedNomenclature != null && SelectedEquipmentGroup != null && RequiredQuantity > 0)
        {
            await _sparePartsOwnershipService.CreateSparePartsOwnershipAsync(new SparePartsOwnership
            {
                NomenclatureId = SelectedNomenclature.Id,
                EquipmentGroupId = SelectedEquipmentGroup.Id,
                Comment = Comment,
                RequiredQuantity = RequiredQuantity

            });
            await NomenclatureListUpdate();
        }
    }

    [RelayCommand]
    private async Task DeleteSparePartsOwnershipAsync()
    {
        if (SelectedSparePartsOwnership == null) return;
        await _sparePartsOwnershipService.DeleteSparePartsOwnershipAsync(SelectedSparePartsOwnership.Id);
        await NomenclatureListUpdate();
    }

    [RelayCommand]
    private async Task UpdateSparePartsOwnershipAsync()
    {
        if (SelectedNomenclature != null && SelectedEquipmentGroup != null && RequiredQuantity > 0)
        {
            await _sparePartsOwnershipService.UpdateSparePartsOwnershipAsync(new SparePartsOwnership
            {
                Id = SelectedNomenclature.Id,
                NomenclatureId = SelectedNomenclature.Id,
                EquipmentGroupId = SelectedEquipmentGroup.Id,
                Comment = Comment,
                RequiredQuantity = RequiredQuantity
            });
            await NomenclatureListUpdate();
        }
    }
}