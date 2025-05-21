using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Threading;
using WpfClient.Services.Interfaces;
using RequestManagement.Common.Interfaces;
using WpfClient.Messages;
using Incoming = RequestManagement.Common.Models.Incoming;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;
using Warehouse = RequestManagement.Common.Models.Warehouse;
using System.Windows;
using Google.Protobuf.WellKnownTypes;
using RequestManagement.Common.Models.Extensions;
using static OfficeOpenXml.ExcelErrorValue;

namespace WpfClient.ViewModels;

public partial class IncomingListViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private readonly IIncomingService _incomingService;
    private readonly System.Timers.Timer _filterTimer;
    private List<Incoming> _incomings = [];
    [ObservableProperty] private ObservableCollection<Incoming> _labelList = [];
    [ObservableProperty] private CollectionViewSource _incomingsDocsViewSource;
    [ObservableProperty] private ObservableCollection<Incoming> _incomingsDocs = [];
    [ObservableProperty] private CollectionViewSource _incomingsItemsViewSource;
    [ObservableProperty] private ObservableCollection<Incoming> _incomingsItems = [];
    [ObservableProperty] private ObservableCollection<Incoming> _incomingsLabels = [];
    [ObservableProperty] private Incoming? _selectedIncomingDoc = new();
    [ObservableProperty] private Incoming? _selectedIncomingItem = new();
    [ObservableProperty] private Warehouse _selectedWarehouse = new();
    [ObservableProperty] private string _menuDeleteItemText = "Удалить отмеченные";
    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private DateTime _fromDate = DateTime.Parse("01.04.2025");
    [ObservableProperty] private DateTime _toDate = DateTime.Parse("30.04.2025");
    [ObservableProperty] private int _notificationCount = 0;

    public IncomingListViewModel() { }

    public IncomingListViewModel(IMessageBus messageBus, IIncomingService incomingService)
    {
        _messageBus = messageBus;
        _incomingService = incomingService;
        _messageBus.Subscribe<SelectResultMessage>(OnSelect);
        _messageBus.Subscribe<ShowResultMessage>(OnShow);
        _incomingsDocsViewSource = new CollectionViewSource { Source = _incomingsDocs };
        IncomingsItemsViewSource = new CollectionViewSource { Source = _incomingsItems };
        var dispatcher = Dispatcher.CurrentDispatcher;
        _filterTimer = new System.Timers.Timer(1000) { AutoReset = false };
        _filterTimer.Elapsed += async (_, _) =>
        {
            await dispatcher.InvokeAsync(async () => { await LoadIncomingsAsync(); });
        };
    }

    private async Task OnShow(ShowResultMessage arg)
    {
        if (arg.Caller == typeof(IncomingListViewModel) && arg.Items.Count != 0)
            switch (arg.Message)
            {
                case MessagesEnum.ResultLabelCountSelector:
                    foreach (var incoming in arg.Items)
                    {
                        LabelList.Add(incoming);
                    }

                    NotificationCount = LabelList.Select(x=>(int)x.Quantity).Sum();
                    break;
            }
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
    private async Task ShowLabelPrintDialog()
    {
        await _messageBus.Publish(new ShowResultMessage(MessagesEnum.ShowLabelPrintListViewDialog, typeof(IncomingListViewModel), LabelList.ToList()));
    }
    [RelayCommand]
    private async Task IncomingDeleteAsync()
    {
        //if (SelectedIncoming != null)
        //{
        //    var result = await _incomingService.DeleteIncomingAsync(SelectedIncoming.Id);
        //    if (result)
        //    {
        //        await LoadIncomingsAsync();
        //    }
        //}
    }
    [RelayCommand]
    private async Task IncomingDeleteRangeAsync()
    {
        //var list = Incomings.Where(x => x.IsSelected).Select(x => x.Id).ToList();
        //if (list.Count > 0)
        //{
        //    var result = await _incomingService.DeleteIncomingsAsync(list);
        //    if (result)
        //    {
        //        await LoadIncomingsAsync();
        //    }
        //}
    }
    [RelayCommand]
    private void SelectAll()
    {
        //foreach (var incoming in Incomings)
        //{
        //    incoming.IsSelected = true;
        //}

        //IncomingsViewSource.View.Refresh(); // Принудительно обновляем DataGrid
        //IncomingListCheckedUpdate();
    }
    [RelayCommand]
    private void DeselectAll()
    {
        //foreach (var incoming in Incomings)
        //{
        //    incoming.IsSelected = false;
        //}
        //IncomingsViewSource.View.Refresh(); // Принудительно обновляем DataGrid
        //IncomingListCheckedUpdate();
    }
    [RelayCommand]
    private void InvertSelected()
    {
        //foreach (var incoming in Incomings)
        //{
        //    incoming.IsSelected = !incoming.IsSelected;
        //}
        //IncomingsViewSource.View.Refresh(); // Принудительно обновляем DataGrid
        //IncomingListCheckedUpdate();
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
        if(SelectedIncomingItem != null)
            await _messageBus.Publish(new ShowResultMessage(MessagesEnum.ShowLabelCountSelector, typeof(IncomingListViewModel), [SelectedIncomingItem.Clone()]));
    }
    [RelayCommand]
    private async Task LoadIncomingsAsync()
    {
        if (!ValidateDates()) return;
        if (SelectedWarehouse.Id == 0) return;
        var currentSortDescriptions = IncomingsDocsViewSource.View?.SortDescriptions.ToList() ?? [];
        _incomings = await _incomingService.GetAllIncomingsAsync(SearchText, SelectedWarehouse.Id, FromDate.ToString(CultureInfo.CurrentCulture), ToDate.ToString(CultureInfo.CurrentCulture));
        IncomingsDocs = new ObservableCollection<Incoming>(_incomings.DistinctBy(x=>x.Code));
        IncomingsDocsViewSource.Source = IncomingsDocs;
        if (currentSortDescriptions.Any())
        {
            foreach (var sortDescription in currentSortDescriptions)
            {
                IncomingsDocsViewSource.View?.SortDescriptions.Add(sortDescription);
            }
        }
        IncomingListCheckedUpdate();
    }
    [RelayCommand]
    private void IncomingListCheckedUpdate()
    {
       // MenuDeleteItemText = $"Удалить отмеченные({Incomings.Count(x => x.IsSelected)})";
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
    private async Task ClearSearchText()
    {
        SearchText = "";
        await LoadIncomingsAsync();
    }
    partial void OnSearchTextChanged(string value)
    {
        _filterTimer.Stop();
        _filterTimer.Start();
    }

    partial void OnSelectedIncomingDocChanged(Incoming? value)
    {
        _incomingsItems = new ObservableCollection<Incoming>(_incomings.Where(x=>x.Code == value?.Code));
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