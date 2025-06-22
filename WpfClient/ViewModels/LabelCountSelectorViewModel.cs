using CommunityToolkit.Mvvm.ComponentModel;
using RequestManagement.Common.Models;
using System.Collections.ObjectModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Input;
using WpfClient.Services.Interfaces;

namespace WpfClient.ViewModels;

public partial class LabelCountSelectorViewModel : ObservableObject
{
    private readonly IMessageBus _messageBus;
    [ObservableProperty] private CollectionViewSource _labelListViewSource;
    [ObservableProperty] private ObservableCollection<Incoming> _labelList = [];
    public event EventHandler CloseWindowRequested;
    public bool DialogResult { get; set; }

    public LabelCountSelectorViewModel()
    {
            
    }
    public LabelCountSelectorViewModel(IMessageBus messageBus)
    {
        _messageBus = messageBus;
        _labelListViewSource = new CollectionViewSource
        {
            Source = LabelList
        };
    }
    public void Init(List<Incoming> labelList)
    {
        DialogResult = false;
        foreach (var incoming in labelList.Where(incoming => incoming.Quantity < 1))
        {
            incoming.Quantity = Math.Round(incoming.Quantity,MidpointRounding.AwayFromZero);
        }
        LabelList = new ObservableCollection<Incoming>(labelList);
        LabelListViewSource = new CollectionViewSource
        {
            Source = LabelList
        };
    }
    [RelayCommand]
    private void Accept()
    {
        DialogResult = true;
        CloseWindowRequested?.Invoke(this, EventArgs.Empty);
    }
}