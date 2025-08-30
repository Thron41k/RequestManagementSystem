using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.WpfClient.Messages;
using RequestManagement.WpfClient.Models;
using RequestManagement.WpfClient.Services.Interfaces;
using Timer = System.Timers.Timer;

namespace RequestManagement.WpfClient.ViewModels;

public partial class MaterialInUseListViewModel : BaseViewModel
{
    private readonly IMessageBus _messageBus;
    private readonly IMaterialsInUseService _materialsInUseService;
    private readonly Timer _filterTimer;
    private readonly Dispatcher _dispatcher;
    private List<MaterialsInUse> _materialsInUseList = new();
    [ObservableProperty] private string _filterText;
    [ObservableProperty] private Driver? _selectedFinanciallyResponsiblePerson = new();
    [ObservableProperty] private Equipment? _selectedEquipment = new();
    [ObservableProperty] private MaterialsInUse? _selectedMaterialsInUse = new();
    [ObservableProperty] private ObservableCollection<Equipment> _equipmentList = [];
    [ObservableProperty] private CollectionViewSource _equipmentViewSource;
    [ObservableProperty] private ObservableCollection<MaterialsInUse> _materialsInUsetList = [];
    [ObservableProperty] private CollectionViewSource _materialsInUseViewSource;
    public MaterialInUseListViewModel(IMessageBus messageBus,
        IDriverService driverService,
        IMaterialsInUseService materialsInUseService)
    {
        _messageBus = messageBus;
        _materialsInUseService = materialsInUseService;
        _messageBus.Subscribe<SelectResultMessage>(OnSelect);
        _messageBus.Subscribe<UpdatedMessage>(OnUpdate);
        _messageBus.Subscribe<ShowResultMessageForMaterialsInUse>(OnShow);
        _dispatcher = Dispatcher.CurrentDispatcher;
        _equipmentViewSource = new CollectionViewSource { Source = EquipmentList };
        _materialsInUseViewSource = new CollectionViewSource { Source = MaterialsInUsetList };
        _filterTimer = new Timer(Vars.SearchDelay) { AutoReset = false };
        _filterTimer.Elapsed += async (_, _) =>
        {
            await _dispatcher.InvokeAsync(async () => { await LoadEquipmentAsync(); });
        };
    }

    private async Task OnUpdate(UpdatedMessage arg)
    {
        if (arg.Message == MessagesEnum.UpdateMaterialsInUseList)
        {
            await LoadEquipmentAsync();
        }
    }

    private async Task OnShow(ShowResultMessageForMaterialsInUse arg)
    {
        if (arg.Caller != typeof(MaterialInUseListViewModel)) return;
        switch (arg.Message)
        {
            case MessagesEnum.ShowAddMaterialsInUseToOffViewResult:
                if (SelectedMaterialsInUse == null) break;
                SelectedMaterialsInUse.ReasonForWriteOff = arg.Reason;
                SelectedMaterialsInUse.DocumentNumberForWriteOff = arg.DocumentNumber;
                SelectedMaterialsInUse.DateForWriteOff = arg.DocumentDate;
                SelectedMaterialsInUse.MolForMove = arg.MolForMove;
                await _materialsInUseService.UpdateMaterialsInUseAsync(SelectedMaterialsInUse);
                MaterialsInUseViewSource.View.Refresh();
                break;
        }
    }

    partial void OnSelectedFinanciallyResponsiblePersonChanged(Driver? value)
    {
        _ = LoadEquipmentAsync();
    }

    partial void OnFilterTextChanged(string value)
    {
        _filterTimer.Stop();
        _filterTimer.Start();
    }

    private async Task LoadEquipmentAsync()
    {
        if (SelectedFinanciallyResponsiblePerson == null) return;
        var filter = string.IsNullOrWhiteSpace(FilterText) ? "" : FilterText.Trim();
        _materialsInUseList = await _materialsInUseService.GetAllMaterialsInUseAsync(SelectedFinanciallyResponsiblePerson.Id, filter.ToLower());
        await _dispatcher.InvokeAsync(() =>
        {
            EquipmentList = new ObservableCollection<Equipment>(
                _materialsInUseList.Select(x => x.Equipment).GroupBy(x => x.Id).Select(g => g.First()).ToList());
            EquipmentViewSource.Source = EquipmentList;
            return Task.CompletedTask;
        });
    }

    private async Task OnSelect(SelectResultMessage arg)
    {
        if (arg.Caller != typeof(MaterialInUseListViewModel) || arg.Item == null) return;
        switch (arg.Message)
        {
            case MessagesEnum.SelectDriver:
                SelectedFinanciallyResponsiblePerson = (Driver)arg.Item;
                await LoadEquipmentAsync();
                break;
        }
    }

    private void UpdateMaterialsInUseList()
    {
        if (SelectedEquipment == null) return;
        MaterialsInUsetList = new ObservableCollection<MaterialsInUse>(_materialsInUseList.Where(x => x.Equipment?.Id == SelectedEquipment.Id));
        MaterialsInUseViewSource.Source = MaterialsInUsetList;
    }

    partial void OnSelectedEquipmentChanged(Equipment? value)
    {
        UpdateMaterialsInUseList();
    }

    [RelayCommand]
    private async Task SelectFinanciallyResponsiblePerson()
    {
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectDriver, typeof(MaterialInUseListViewModel)));
    }


    [RelayCommand]
    private void EquipmentSelectByClick()
    {
        UpdateMaterialsInUseList();
    }

    [RelayCommand]
    private async Task MaterialsInUseDoubleClick()
    {
        if (SelectedMaterialsInUse == null) return;
        await _messageBus.Publish(new ShowResultMessageForMaterialsInUse(
            MessagesEnum.ShowAddMaterialsInUseToOffView,
            typeof(MaterialInUseListViewModel),
            SelectedMaterialsInUse.DocumentNumberForWriteOff,
            SelectedMaterialsInUse.ReasonForWriteOff,
            SelectedMaterialsInUse.DateForWriteOff,
            SelectedMaterialsInUse.MolForMove ?? new Driver { Id = 1 }));
    }

    [RelayCommand]
    private async Task Print()
    {
        await _messageBus.Publish(new PrintMaterialInUseOffModel { MaterialsInUse = _materialsInUseList });
    }

    [RelayCommand]
    private async Task MaterialInUseLoad()
    {
        if (SelectedFinanciallyResponsiblePerson == null) return;
        await _messageBus.Publish(new DialogAddMaterialInUseFromExpenseModel { Caller = Id, Mol = SelectedFinanciallyResponsiblePerson });
    }
}