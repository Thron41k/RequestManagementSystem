using RequestManagement.Server.Controllers;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using Timer = System.Timers.Timer;
using RequestManagement.Common.Interfaces;
using WpfClient.Services.Interfaces;
using WpfClient.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Input;

namespace WpfClient.ViewModels;

public partial class NomenclatureViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    private readonly INomenclatureService _nomenclatureService;
    [ObservableProperty] private RequestManagement.Common.Models.Nomenclature? _selectedNomenclature;
    [ObservableProperty] private string _newNomenclatureCode;
    [ObservableProperty] private string _newNomenclatureName;
    [ObservableProperty] private string? _newNomenclatureArticle;
    [ObservableProperty] private string _newNomenclatureUnitOfMeasure;
    [ObservableProperty] private string _filterText;
    [ObservableProperty] private ObservableCollection<RequestManagement.Common.Models.Nomenclature> _nomenclatureList = [];
    [ObservableProperty] private CollectionViewSource _nomenclatureViewSource;
    public bool DialogResult { get; private set; }
    public bool EditMode { get; set; }
    private readonly Timer _filterTimer;
    private readonly Dispatcher _dispatcher;
    public event EventHandler CloseWindowRequested;

    public NomenclatureViewModel(INomenclatureService nomenclatureService, IMessageBus messageBus)
    {
        _nomenclatureService = nomenclatureService;
        _messageBus = messageBus;
        NomenclatureViewSource = new CollectionViewSource { Source = NomenclatureList };
        _dispatcher = Dispatcher.CurrentDispatcher;
        _filterTimer = new Timer(Vars.SearchDelay) { AutoReset = false };
        _filterTimer.Elapsed += async (_, _) => await LoadNomenclatureAsync();
    }

    partial void OnSelectedNomenclatureChanged(RequestManagement.Common.Models.Nomenclature? value) => AddToEdit();

    partial void OnFilterTextChanged(string value)
    {
        _filterTimer.Stop();
        _filterTimer.Start();
    }

    private void AddToEdit()
    {
        if (SelectedNomenclature == null) return;
        NewNomenclatureName = SelectedNomenclature.Name;
        NewNomenclatureCode = SelectedNomenclature.Code;
        NewNomenclatureArticle = SelectedNomenclature.Article ?? "";
        NewNomenclatureUnitOfMeasure = SelectedNomenclature.UnitOfMeasure;
    }

    public async Task Load()
    {
        await LoadNomenclatureAsync();
    }

    private async Task LoadNomenclatureAsync()
    {
        var filter = string.IsNullOrWhiteSpace(FilterText) ? "" : FilterText.Trim();
        var nomenclatureList = await _nomenclatureService.GetAllNomenclaturesAsync(filter.ToLower());
        await _dispatcher.InvokeAsync(() =>
        {
            var currentSortDescriptions = NomenclatureViewSource.View?.SortDescriptions.ToList() ?? [];
            NomenclatureList = new ObservableCollection<RequestManagement.Common.Models.Nomenclature>(nomenclatureList);
            NomenclatureViewSource.Source = NomenclatureList;
            NewNomenclatureName = string.Empty;
            NewNomenclatureCode = string.Empty;
            NewNomenclatureUnitOfMeasure = string.Empty;
            NewNomenclatureArticle = string.Empty;
            SelectedNomenclature = null;
            if (!currentSortDescriptions.Any()) return Task.CompletedTask;
            foreach (var sortDescription in currentSortDescriptions)
            {
                NomenclatureViewSource.View?.SortDescriptions.Add(sortDescription);
            }
            return Task.CompletedTask;
        });
    }

    [RelayCommand]
    private async Task DeleteNomenclature()
    {
        if (SelectedNomenclature != null)
        {
            await _nomenclatureService.DeleteNomenclatureAsync(SelectedNomenclature.Id);
            await LoadNomenclatureAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.NomenclatureUpdated));
        }
    }

    [RelayCommand]
    private async Task UpdateNomenclature()
    {
        if (SelectedNomenclature != null && !string.IsNullOrEmpty(NewNomenclatureName.Trim()) && !string.IsNullOrEmpty(NewNomenclatureCode.Trim()) && !string.IsNullOrEmpty(NewNomenclatureUnitOfMeasure.Trim()))
        {
            await _nomenclatureService.UpdateNomenclatureAsync(new RequestManagement.Common.Models.Nomenclature { Id = SelectedNomenclature.Id, Name = NewNomenclatureName, Code = NewNomenclatureCode, Article = NewNomenclatureArticle, UnitOfMeasure = NewNomenclatureUnitOfMeasure });
            await LoadNomenclatureAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.NomenclatureUpdated));
        }
    }

    [RelayCommand]
    private async Task AddNomenclature()
    {
        if (!string.IsNullOrWhiteSpace(NewNomenclatureName.Trim()) && !string.IsNullOrWhiteSpace(NewNomenclatureCode.Trim()) && !string.IsNullOrWhiteSpace(NewNomenclatureUnitOfMeasure.Trim()))
        {
            await _nomenclatureService.CreateNomenclatureAsync(new RequestManagement.Common.Models.Nomenclature { Name = NewNomenclatureName, Code = NewNomenclatureCode, Article = NewNomenclatureArticle, UnitOfMeasure = NewNomenclatureUnitOfMeasure });
            await LoadNomenclatureAsync();
            await _messageBus.Publish(new UpdatedMessage(MessagesEnum.NomenclatureUpdated));
        }
    }

    [RelayCommand]
    private void SelectAndClose()
    {
        if (EditMode || SelectedNomenclature == null) return;
        DialogResult = true;
        CloseWindowRequested.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void ClearNewNomenclatureUnitOfMeasure()
    {
        NewNomenclatureUnitOfMeasure = string.Empty;
    }

    [RelayCommand]
    private void ClearNewNomenclatureName()
    {
        NewNomenclatureName = string.Empty;
    }

    [RelayCommand]
    private void ClearNewNomenclatureCode()
    {
        NewNomenclatureCode = string.Empty;
    }

    [RelayCommand]
    private void ClearNewNomenclatureArticle()
    {
        NewNomenclatureArticle = string.Empty;
    }
}