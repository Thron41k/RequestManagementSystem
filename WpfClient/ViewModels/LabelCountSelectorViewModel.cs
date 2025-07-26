using System.Collections.ObjectModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Models;
using RequestManagement.WpfClient.Messages;
using RequestManagement.WpfClient.Services.Interfaces;

namespace RequestManagement.WpfClient.ViewModels;

public partial class LabelCountSelectorViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    [ObservableProperty] private CollectionViewSource _labelListViewSource;
    [ObservableProperty] private ObservableCollection<Incoming> _labelList = [];
    [ObservableProperty] private Incoming? _selectedIncoming;
    public event EventHandler CloseWindowRequested;
    public bool DialogResult { get; set; }

    public LabelCountSelectorViewModel()
    {

    }
    public LabelCountSelectorViewModel(IMessageBus messageBus)
    {
        _messageBus = messageBus;
        _messageBus.Subscribe<SelectResultMessage>(OnSelect);
        _labelListViewSource = new CollectionViewSource
        {
            Source = LabelList
        };
    }

    private async Task OnSelect(SelectResultMessage arg)
    {
        if (arg.Caller != typeof(LabelCountSelectorViewModel) || arg.Item == null) return;
        switch (arg.Message)
        {
            case MessagesEnum.SelectEquipment:
                var equipment = (Equipment)arg.Item;
                if (equipment != null && SelectedIncoming != null)
                {
                    var incoming = LabelList.FirstOrDefault(x => x.Id == SelectedIncoming.Id);
                    if(incoming is { Application:  null })
                    {
                        incoming.Application ??= new Application();
                    }
                    incoming!.Application.Equipment = equipment;
                    LabelListViewSource.View.Refresh();
                }
                break;
        }
    }
    public void Init(List<Incoming> labelList)
    {
        DialogResult = false;
        foreach (var incoming in labelList.Where(incoming => incoming.Quantity < 1))
        {
            incoming.Quantity = Math.Round(incoming.Quantity, MidpointRounding.AwayFromZero);
        }
        LabelList = new ObservableCollection<Incoming>(labelList);
        LabelListViewSource = new CollectionViewSource
        {
            Source = LabelList
        };
    }

    [RelayCommand]
    private async Task SelectEquipment()
    {
        if (SelectedIncoming == null) return;
        await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectEquipment, typeof(LabelCountSelectorViewModel)));
    }

    [RelayCommand]
    private void Accept()
    {
        DialogResult = true;
        CloseWindowRequested?.Invoke(this, EventArgs.Empty);
    }
}