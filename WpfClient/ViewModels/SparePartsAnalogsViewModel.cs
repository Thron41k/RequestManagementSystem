using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.WpfClient.Messages;
using RequestManagement.WpfClient.Services.Interfaces;

namespace RequestManagement.WpfClient.ViewModels;

public partial class SparePartsAnalogsViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private readonly INomenclatureService _nomenclatureService;
    private readonly INomenclatureAnalogService _nomenclatureAnalogService;
    [ObservableProperty] private ObservableCollection<Nomenclature> _nomenclatureList = [];
    [ObservableProperty] private ObservableCollection<Nomenclature> _nomenclatureAnalogList = [];
    [ObservableProperty] private CollectionViewSource _nomenclatureViewSource;
    [ObservableProperty] private Nomenclature? _selectedNomenclature;
    [ObservableProperty] private Nomenclature? _selectedNomenclatureAnalog;
    [ObservableProperty] private CollectionViewSource _nomenclatureAnalogViewSource;
    [ObservableProperty] private string _filterText = "";
    [ObservableProperty] private bool _addButtonIsEnabled;
    private readonly System.Timers.Timer _filterTimer;
    public SparePartsAnalogsViewModel()
    {
    }
    public SparePartsAnalogsViewModel(IMessageBus messageBus, INomenclatureService nomenclatureService, INomenclatureAnalogService nomenclatureAnalogService)
    {
        _messageBus = messageBus;
        _nomenclatureService = nomenclatureService;
        _nomenclatureAnalogService = nomenclatureAnalogService;
        _nomenclatureViewSource = new CollectionViewSource { Source = _nomenclatureList };
        _nomenclatureAnalogViewSource = new CollectionViewSource { Source = _nomenclatureAnalogList };
        _messageBus.Subscribe<SelectResultMessage>(OnSelect);
        var dispatcher = Dispatcher.CurrentDispatcher;
        _filterTimer = new System.Timers.Timer(1000) { AutoReset = false };
        _filterTimer.Elapsed += async (_, _) =>
        {
            await dispatcher.InvokeAsync(async () => { await LoadNomenclatureAsync(); });
        };
    }

    partial void OnSelectedNomenclatureChanged(Nomenclature? value)
    {
        if (value == null)
        {
            AddButtonIsEnabled = false;
        }
        else
        {
            _ = LoadNomenclatureAnalogAsync();
            AddButtonIsEnabled = true;
        }
    }
    partial void OnFilterTextChanged(string value)
    {
        _filterTimer.Stop();
        _filterTimer.Start();
    }

    private async Task OnSelect(SelectResultMessage arg)
    {
        if (arg.Caller == typeof(SparePartsAnalogsViewModel) && arg is { Item: not null, Message: MessagesEnum.SelectNomenclature })
        {
            await AddNomenclatureAnalogAsync((Nomenclature)arg.Item);
        }
    }

    private async Task LoadNomenclatureAsync()
    {
        if (string.IsNullOrWhiteSpace(FilterText))
        {
            NomenclatureViewSource.Source = new ObservableCollection<Nomenclature>();
            return;
        }
        var filter = FilterText.Trim();
        var nomenclatureList = await _nomenclatureService.GetAllNomenclaturesAsync(filter.ToLower());
        NomenclatureList = new ObservableCollection<Nomenclature>(nomenclatureList);
        NomenclatureViewSource.Source = NomenclatureList;
    }

    private async Task LoadNomenclatureAnalogAsync()
    {
        if (SelectedNomenclature == null) return;
        var nomenclatureAnalogList = await _nomenclatureAnalogService.GetAllNomenclatureAnalogsAsync(SelectedNomenclature.Id);
        NomenclatureAnalogList = new ObservableCollection<Nomenclature>(nomenclatureAnalogList);
        NomenclatureAnalogViewSource.Source = NomenclatureAnalogList;
    }

    private async Task AddNomenclatureAnalogAsync(Nomenclature nomenclature)
    {
        if (SelectedNomenclature == null) return;
        await _nomenclatureAnalogService.AddNomenclatureAnalogAsync(new NomenclatureAnalog { OriginalId = SelectedNomenclature.Id, AnalogId = nomenclature.Id });
        await LoadNomenclatureAnalogAsync();
    }

    [RelayCommand]
    private void ClearFilterText()
    {
        FilterText = "";
    }

    [RelayCommand]
    private async Task AddNomenclatureAnalog()
    {
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectNomenclature, typeof(SparePartsAnalogsViewModel)));
    }

    [RelayCommand]
    private async Task DeleteNomenclatureAnalog()
    {
        if(SelectedNomenclature == null || SelectedNomenclatureAnalog == null) return;
        await _nomenclatureAnalogService.DeleteNomenclatureAnalogAsync(SelectedNomenclature.Id,SelectedNomenclatureAnalog.Id);
        await LoadNomenclatureAnalogAsync();
    }
}