using CommunityToolkit.Mvvm.ComponentModel;

namespace RequestManagement.WpfClient.ViewModels;

public class BaseViewModel : ObservableObject
{
    public bool DialogResult { get; set; }
    protected Guid Id { get; set; } = Guid.NewGuid();
    public bool EditMode { get; set; }
}