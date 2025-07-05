using System.Collections.ObjectModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using WpfClient.Messages;
using WpfClient.Services.Interfaces;
using Dispatcher = System.Windows.Threading.Dispatcher;
using Equipment = RequestManagement.Common.Models.Equipment;
using Timer = System.Timers.Timer;

namespace WpfClient.ViewModels;

public partial class EquipmentViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private readonly IEquipmentService _requestService;
    [ObservableProperty] private Equipment? _selectedEquipment;
    [ObservableProperty] private string _newEquipmentLicensePlate;
    [ObservableProperty] private string _newEquipmentName;
    [ObservableProperty] private string _newEquipmentCode;
    [ObservableProperty] private string _filterText;
    [ObservableProperty] private ObservableCollection<Equipment> _equipmentList = [];
    [ObservableProperty] private CollectionViewSource _equipmentViewSource;
    private readonly Timer _filterTimer;
    private readonly Dispatcher _dispatcher;
    public bool DialogResult { get; private set; }
    public event EventHandler CloseWindowRequested;

    public bool EditMode { get; set; }

    partial void OnFilterTextChanged(string value)
    {
        _filterTimer.Stop();
        _filterTimer.Start();
    }

    partial void OnSelectedEquipmentChanged(Equipment? value)
    {
        AddToEdit();
    }

    public EquipmentViewModel(IEquipmentService requestService, IMessageBus messageBus)
    {
        _requestService = requestService;
        _messageBus = messageBus;
        EquipmentViewSource = new CollectionViewSource { Source = EquipmentList };
        _dispatcher = Dispatcher.CurrentDispatcher;
        _filterTimer = new Timer(Vars.SearchDelay) { AutoReset = false };
        _filterTimer.Elapsed += async (s, e) => await LoadEquipmentAsync();
    }

    public async Task Load()
    {
        await LoadEquipmentAsync();
    }
    private async Task LoadEquipmentAsync()
    {
        var filter = string.IsNullOrWhiteSpace(FilterText) ? "" : FilterText.Trim();
        var equipmentList = await _requestService.GetAllEquipmentAsync(filter.ToLower());
        await _dispatcher.InvokeAsync(() =>
        {
            EquipmentList = new ObservableCollection<Equipment>(equipmentList);
            EquipmentViewSource.Source = EquipmentList;
            NewEquipmentName = string.Empty;
            NewEquipmentLicensePlate = string.Empty;
            NewEquipmentCode = string.Empty;
            return Task.CompletedTask;
        });
    }

    private void AddToEdit()
    {
        if (SelectedEquipment == null) return;
        NewEquipmentName = SelectedEquipment.Name;
        if (SelectedEquipment.StateNumber != null) NewEquipmentLicensePlate = SelectedEquipment.StateNumber;
        NewEquipmentCode = SelectedEquipment.Code;
    }

    [RelayCommand]
    private void UpdateSelectedItem()
    {
        AddToEdit();
    }

    [RelayCommand]
    private void SelectAndClose()
    {
        if (!EditMode && SelectedEquipment != null)
        {
            DialogResult = true;
            CloseWindowRequested.Invoke(this, EventArgs.Empty);
        }
    }

    [RelayCommand]
    private async Task AddEquipment()
    {
        if (!string.IsNullOrWhiteSpace(NewEquipmentName) && !string.IsNullOrWhiteSpace(NewEquipmentCode))
        {
            await _requestService.CreateEquipmentAsync(new Equipment { Name = NewEquipmentName, StateNumber = NewEquipmentLicensePlate, Code = NewEquipmentCode });
            await LoadEquipmentAsync();
            NewEquipmentName = string.Empty;
            NewEquipmentLicensePlate = string.Empty;
            NewEquipmentCode = string.Empty;
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.EquipmentUpdated));
        }
    }

    [RelayCommand]
    private async Task UpdateEquipment()
    {
        if (SelectedEquipment != null && !string.IsNullOrEmpty(NewEquipmentName.Trim()) && !string.IsNullOrEmpty(NewEquipmentCode.Trim()))
        {
            await _requestService.UpdateEquipmentAsync(new Equipment { Id = SelectedEquipment.Id, Name = NewEquipmentName, StateNumber = NewEquipmentLicensePlate, Code = NewEquipmentCode });
            await LoadEquipmentAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.EquipmentUpdated));
        }
    }

    [RelayCommand]
    private async Task DeleteEquipment()
    {
        if (SelectedEquipment != null)
        {
            await _requestService.DeleteEquipmentAsync(SelectedEquipment.Id);
            await LoadEquipmentAsync();
            NewEquipmentName = string.Empty;
            NewEquipmentLicensePlate = string.Empty;
            NewEquipmentCode = string.Empty;
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.EquipmentUpdated));
        }
    }

    [RelayCommand]
    private void ClearEquipmentName()
    {
        NewEquipmentName = string.Empty;
    }
    [RelayCommand]
    private void ClearEquipmentLicensePlate()
    {
        NewEquipmentLicensePlate = string.Empty;
    }
    [RelayCommand]
    private void ClearEquipmentCode()
    {
        NewEquipmentCode = string.Empty;
    }
    [RelayCommand]
    private void ClearFilterText()
    {
        FilterText = string.Empty;
    }
}