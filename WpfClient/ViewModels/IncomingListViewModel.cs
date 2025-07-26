using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models.Extensions;
using RequestManagement.WpfClient.Messages;
using RequestManagement.WpfClient.Services.Interfaces;
using WpfClient;
using Incoming = RequestManagement.Common.Models.Incoming;
using Warehouse = RequestManagement.Common.Models.Warehouse;

namespace RequestManagement.WpfClient.ViewModels;

public partial class IncomingListViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private readonly IIncomingService _incomingService;
    private readonly System.Timers.Timer _filterTimer;
    private List<Incoming> _incomings = [];
    private int _lastSelectionIndex = -1;
    [ObservableProperty] private ObservableCollection<Incoming> _labelList = [];
    [ObservableProperty] private CollectionViewSource _incomingsDocsViewSource = new();
    [ObservableProperty] private ObservableCollection<Incoming> _incomingsDocs = [];
    [ObservableProperty] private CollectionViewSource _incomingsItemsViewSource = new();
    [ObservableProperty] private ObservableCollection<Incoming> _incomingsItems = [];
    [ObservableProperty] private ObservableCollection<Incoming> _incomingsLabels = [];
    [ObservableProperty] private Incoming? _selectedIncomingDoc = new();
    [ObservableProperty] private Incoming? _selectedIncomingItem = new();
    [ObservableProperty] private Warehouse _selectedWarehouse = new();
    [ObservableProperty] private string _menuDeleteItemText = "Удалить отмеченные";
    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private DateTime _fromDate = DateTime.Parse("01.07.2025");
    [ObservableProperty] private DateTime _toDate = DateTime.Parse("15.07.2025");
    [ObservableProperty] private int _notificationCount = 0;
    [ObservableProperty] private int _selectionIndex = -1;

    public IncomingListViewModel() { }

    partial void OnSelectionIndexChanged(int value)
    {
        if (value > 0) _lastSelectionIndex = value;
    }

    public IncomingListViewModel(IMessageBus messageBus, IIncomingService incomingService)
    {
        _messageBus = messageBus;
        _incomingService = incomingService;
        _messageBus.Subscribe<SelectResultMessage>(OnSelect);
        _messageBus.Subscribe<ShowResultMessage>(OnShow);
        IncomingsDocsViewSource = new CollectionViewSource { Source = _incomingsDocs };
        IncomingsItemsViewSource = new CollectionViewSource { Source = _incomingsItems };
        var dispatcher = Dispatcher.CurrentDispatcher;
        _filterTimer = new System.Timers.Timer(Vars.SearchDelay) { AutoReset = false };
        _filterTimer.Elapsed += async (_, _) =>
        {
            await dispatcher.InvokeAsync(async () => { await LoadIncomingsAsync(); });
        };
    }

    private Task OnShow(ShowResultMessage arg)
    {
        if (arg.Caller == typeof(IncomingListViewModel))
            switch (arg.Message)
            {
                case MessagesEnum.ResultLabelCountSelector:
                    foreach (var incoming in arg.Items)
                    {
                        LabelList.Add(incoming);
                    }
                    NotificationCount = LabelList.Select(x => (int)x.Quantity).Sum();
                    break;
                case MessagesEnum.UpdateLabelPrintList:
                    LabelList.Clear();
                    foreach (var incoming in arg.Items)
                    {
                        LabelList.Add(incoming);
                    }
                    NotificationCount = LabelList.Select(x => (int)x.Quantity).Sum();
                    break;
            }

        return Task.CompletedTask;
    }

    public async Task Load()
    {
        await LoadIncomingsAsync();
    }
    private async Task OnSelect(SelectResultMessage arg)
    {
        if (arg.Caller != typeof(IncomingListViewModel) || arg.Item == null) return;
        switch (arg.Message)
        {
            case MessagesEnum.SelectWarehouse:
                SelectedWarehouse = (Warehouse)arg.Item;
                await LoadIncomingsAsync();
                break;
        }
    }

    [RelayCommand]
    private async Task Print()
    {
        var selectedDoc = IncomingsDocs.Where(x => x.IsSelected).ToList();
        await _messageBus.Publish(new ShowTaskPrintDialogMessageIncoming(
            MessagesEnum.ShowPrintReportDialog,
            typeof(IncomingListViewModel),
            false,
            _incomings.Where(x => selectedDoc.Any(y => y.Code == x.Code) && x.DocType == "Перемещение").ToList(), FromDate, ToDate));
    }

    [RelayCommand]
    private async Task ShowLabelPrintDialog()
    {
        await _messageBus.Publish(new ShowResultMessage(MessagesEnum.ShowLabelPrintListViewDialog, typeof(IncomingListViewModel), LabelList.ToList()));
    }

    [RelayCommand]
    private async Task IncomingDeleteAsync()
    {
        if (SelectedIncomingItem != null)
        {
            var result = await _incomingService.DeleteIncomingAsync(SelectedIncomingItem.Id);
            if (result)
            {
                await LoadIncomingsAsync();
            }
        }
    }

    [RelayCommand]
    private async Task IncomingsDeleteAsync()
    {
        var selected = IncomingsDocs.Where(x => x.IsSelected).Select(x => x.Code).ToList();
        if (selected.Any())
        {
            var result = await _incomingService.DeleteIncomingsAsync(_incomings.Where(x => selected.Contains(x.Code)).Select(x => x.Id).ToList());
            if (result)
            {
                await LoadIncomingsAsync();
            }
        }
    }

    [RelayCommand]
    private async Task DoubleClickDoc()
    {
        if (SelectedIncomingDoc != null)
        {
            var newList = _incomings.Where(x => x.Code == SelectedIncomingDoc.Code)
                .Select(x => x.Clone())
                .ToList();
            await _messageBus.Publish(new ShowResultMessage(MessagesEnum.ShowLabelCountSelector,
                typeof(IncomingListViewModel), newList));
        }
    }

    [RelayCommand]
    private async Task DoubleClickItem()
    {
        if (SelectedIncomingItem != null)
            await _messageBus.Publish(new ShowResultMessage(MessagesEnum.ShowLabelCountSelector, typeof(IncomingListViewModel), [SelectedIncomingItem.Clone()]));
    }
    [RelayCommand]
    private async Task LoadIncomingsAsync()
    {
        if (!ValidateDates()) return;
        if (SelectedWarehouse.Id == 0)
        {
            IncomingsDocs = [];
            IncomingsDocsViewSource.Source = IncomingsDocs;
            return;
        }
        var currentSortDescriptions = (IncomingsDocsViewSource.View?.SortDescriptions!).ToList() ?? [];
        _incomings = await _incomingService.GetAllIncomingsAsync(SearchText, SelectedWarehouse.Id, FromDate.ToString(CultureInfo.CurrentCulture), ToDate.ToString(CultureInfo.CurrentCulture));
        IncomingsDocs = new ObservableCollection<Incoming>(_incomings.DistinctBy(x => x.Code));
        IncomingsDocsViewSource.Source = IncomingsDocs;
        if (_lastSelectionIndex != -1) SelectionIndex = _lastSelectionIndex;
        if (currentSortDescriptions.Any())
        {
            foreach (var sortDescription in currentSortDescriptions)
            {
                IncomingsDocsViewSource.View?.SortDescriptions.Add(sortDescription);
            }
        }
    }

    [RelayCommand]
    private async Task SelectWarehouse()
    {
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectWarehouse, typeof(IncomingListViewModel)));
    }
    [RelayCommand]
    private async Task ClearSelectedWarehouse()
    {
        SelectedWarehouse = new Warehouse();
        await LoadIncomingsAsync();
    }
    [RelayCommand]
    private void ClearSearchText()
    {
        SearchText = "";
    }
    partial void OnSearchTextChanged(string value)
    {
        _filterTimer.Stop();
        _filterTimer.Start();
    }

    [RelayCommand]
    private void SelectAll()
    {
        foreach (var incoming in IncomingsDocs)
        {
            incoming.IsSelected = true;
        }

        IncomingsDocsViewSource.View.Refresh();
    }
    [RelayCommand]
    private void DeselectAll()
    {
        foreach (var incoming in IncomingsDocs)
        {
            incoming.IsSelected = false;
        }

        IncomingsDocsViewSource.View.Refresh();
    }
    [RelayCommand]
    private void InvertSelected()
    {
        foreach (var incoming in IncomingsDocs)
        {
            incoming.IsSelected = !incoming.IsSelected;
        }

        IncomingsDocsViewSource.View.Refresh();
    }

    partial void OnSelectedIncomingDocChanged(Incoming? value)
    {
        _incomingsItems = new ObservableCollection<Incoming>(_incomings.Where(x => x.Code == value?.Code));
        IncomingsItemsViewSource.Source = _incomingsItems;
    }
    private bool ValidateDates()
    {
        // Проверка на нулевые или экстремальные значения
        if (FromDate == DateTime.MinValue || ToDate == DateTime.MinValue ||
            FromDate == DateTime.MaxValue || ToDate == DateTime.MaxValue)
        {
            MessageBox.Show("Пожалуйста, выберите корректные даты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        // Проверка, что FromDate не позже ToDate
        if (FromDate > ToDate)
        {
            MessageBox.Show("Дата начала не может быть позже даты окончания.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        // Проверка на разумный диапазон (например, не раньше 2000 года и не в будущем)
        if (FromDate.Year < 2000 || ToDate.Year < 2000)
        {
            MessageBox.Show("Даты не могут быть раньше 2000 года.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        // Проверка на слишком большой диапазон (например, не больше года)
        if ((ToDate - FromDate).TotalDays > 365)
        {
            MessageBox.Show("Диапазон дат не должен превышать одного года.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }
}