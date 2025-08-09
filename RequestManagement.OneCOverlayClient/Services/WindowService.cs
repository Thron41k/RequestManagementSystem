using System.Windows;
using OneCOverlayClient.Services.Interfaces;

namespace OneCOverlayClient.Services;

public class WindowService : IWindowService
{
    public void Hide(object viewModel)
    {
        var window = FindWindowByViewModel(viewModel);
        window?.Hide();
    }

    public void Show(object viewModel)
    {
        var window = FindWindowByViewModel(viewModel);
        window?.Show();
    }

    private Window? FindWindowByViewModel(object viewModel)
    {
        return Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.DataContext == viewModel);
    }
}