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

public partial class ReasonsForWritingOffMaterialsFromOperationViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private readonly IReasonsForWritingOffMaterialsFromOperationService _reasonsForWritingOffMaterialsFromOperationService;
    [ObservableProperty] private RequestManagement.Common.Models.ReasonsForWritingOffMaterialsFromOperation? _selectedReasonsForWritingOffMaterialsFromOperation;
    [ObservableProperty] private string _newReasonsForWritingOffMaterialsFromOperationName;
    [ObservableProperty] private string _filterText;
    [ObservableProperty] private ObservableCollection<RequestManagement.Common.Models.ReasonsForWritingOffMaterialsFromOperation> _reasonsForWritingOffMaterialsFromOperationList = [];
    [ObservableProperty] private CollectionViewSource _reasonsForWritingOffMaterialsFromOperationViewSource;
    public bool DialogResult { get; private set; }
    public bool EditMode { get; set; }
    private readonly Dispatcher _dispatcher;
    private readonly Timer _filterTimer;
    public event EventHandler CloseWindowRequested;

    public ReasonsForWritingOffMaterialsFromOperationViewModel(IReasonsForWritingOffMaterialsFromOperationService reasonsForWritingOffMaterialsFromOperationService, IMessageBus messageBus)
    {
        _reasonsForWritingOffMaterialsFromOperationService = reasonsForWritingOffMaterialsFromOperationService;
        _messageBus = messageBus;
        _dispatcher = Dispatcher.CurrentDispatcher;
        ReasonsForWritingOffMaterialsFromOperationViewSource = new CollectionViewSource { Source = ReasonsForWritingOffMaterialsFromOperationList };
        _filterTimer = new Timer(Vars.SearchDelay) { AutoReset = false };
        _filterTimer.Elapsed += async (_, _) => await LoadReasonsForWritingOffMaterialsFromOperationAsync();
    }

    partial void OnSelectedReasonsForWritingOffMaterialsFromOperationChanged(RequestManagement.Common.Models.ReasonsForWritingOffMaterialsFromOperation? value) => AddToEdit();

    partial void OnFilterTextChanged(string value)
    {
        _filterTimer.Stop();
        _filterTimer.Start();
    }

    public async Task Load()
    {
        await LoadReasonsForWritingOffMaterialsFromOperationAsync();
    }

    [RelayCommand]
    private async Task DeleteReasonsForWritingOffMaterialsFromOperationAsync()
    {
        if (SelectedReasonsForWritingOffMaterialsFromOperation != null)
        {
            await _reasonsForWritingOffMaterialsFromOperationService.DeleteReasonsForWritingOffMaterialsFromOperationAsync(SelectedReasonsForWritingOffMaterialsFromOperation.Id);
            await LoadReasonsForWritingOffMaterialsFromOperationAsync();
            //await _messageBus.Publish(new UpdatedMessage(MessagesEnum.ReasonsForWritingOffMaterialsFromOperationUpdated));
        }
    }

    [RelayCommand]
    private async Task LoadReasonsForWritingOffMaterialsFromOperationAsync()
    {
        var filter = string.IsNullOrWhiteSpace(FilterText) ? "" : FilterText.Trim();
        var reasonsForWritingOffMaterialsFromOperationList = await _reasonsForWritingOffMaterialsFromOperationService.GetAllReasonsForWritingOffMaterialsFromOperationAsync(filter.ToLower());
        await _dispatcher.InvokeAsync(() =>
        {
            var currentSortDescriptions = (ReasonsForWritingOffMaterialsFromOperationViewSource.View?.SortDescriptions!).ToList();
            ReasonsForWritingOffMaterialsFromOperationList = new ObservableCollection<RequestManagement.Common.Models.ReasonsForWritingOffMaterialsFromOperation>(reasonsForWritingOffMaterialsFromOperationList);
            ReasonsForWritingOffMaterialsFromOperationViewSource.Source = ReasonsForWritingOffMaterialsFromOperationList;
            SelectedReasonsForWritingOffMaterialsFromOperation = null;
            NewReasonsForWritingOffMaterialsFromOperationName = string.Empty;
            if (!currentSortDescriptions.Any()) return Task.CompletedTask;
            foreach (var sortDescription in currentSortDescriptions)
            {
                ReasonsForWritingOffMaterialsFromOperationViewSource.View?.SortDescriptions.Add(sortDescription);
            }
            return Task.CompletedTask;
        });
    }

    [RelayCommand]
    private async Task UpdateReasonsForWritingOffMaterialsFromOperationAsync()
    {
        if (SelectedReasonsForWritingOffMaterialsFromOperation != null && !string.IsNullOrEmpty(NewReasonsForWritingOffMaterialsFromOperationName.Trim()))
        {
            await _reasonsForWritingOffMaterialsFromOperationService.UpdateReasonsForWritingOffMaterialsFromOperationAsync(new RequestManagement.Common.Models.ReasonsForWritingOffMaterialsFromOperation
            {
                Id = SelectedReasonsForWritingOffMaterialsFromOperation.Id,
                Reason = NewReasonsForWritingOffMaterialsFromOperationName.Trim()
            });
            await LoadReasonsForWritingOffMaterialsFromOperationAsync();
           // await _messageBus.Publish(new UpdatedMessage(MessagesEnum.ReasonsForWritingOffMaterialsFromOperationUpdated));
        }
    }

    [RelayCommand]
    private async Task AddReasonsForWritingOffMaterialsFromOperationAsync()
    {
        if (!string.IsNullOrWhiteSpace(NewReasonsForWritingOffMaterialsFromOperationName.Trim()))
        {
            await _reasonsForWritingOffMaterialsFromOperationService.CreateReasonsForWritingOffMaterialsFromOperationAsync(new RequestManagement.Common.Models.ReasonsForWritingOffMaterialsFromOperation
            {
                Reason = NewReasonsForWritingOffMaterialsFromOperationName.Trim()
            });
            await LoadReasonsForWritingOffMaterialsFromOperationAsync();
            //await _messageBus.Publish(new UpdatedMessage(MessagesEnum.ReasonsForWritingOffMaterialsFromOperationUpdated));
        }
    }

    private void AddToEdit()
    {
        if (SelectedReasonsForWritingOffMaterialsFromOperation != null)
        {
            NewReasonsForWritingOffMaterialsFromOperationName = SelectedReasonsForWritingOffMaterialsFromOperation.Reason;
        }
    }

    [RelayCommand]
    private void SelectAndClose()
    {
        if (EditMode || SelectedReasonsForWritingOffMaterialsFromOperation == null) return;
        DialogResult = true;
        CloseWindowRequested.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void ClearReasonsForWritingOffMaterialsFromOperationName()
    {
        NewReasonsForWritingOffMaterialsFromOperationName = string.Empty;
    }
}