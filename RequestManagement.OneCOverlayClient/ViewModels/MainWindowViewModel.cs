using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OneCOverlayClient.Views;
using System.Windows;
using OneCOverlayClient.Services.Interfaces;
using System.Windows.Media;
using System.Windows.Input;

namespace OneCOverlayClient.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly MaterialsInUseViewModel _materialsInUseViewModel;
    private readonly IWindowService _windowService;

    public MainWindowViewModel(MaterialsInUseViewModel materialsInUseViewModel,IWindowService windowService)
    {
        _materialsInUseViewModel = materialsInUseViewModel;
        _windowService = windowService;
    }
    [RelayCommand]
    private void MaterialsInUse()
    {
        var materialsInUseWindow = new MaterialsInUseView(_materialsInUseViewModel);
        var window = new Window
        {
            Content = materialsInUseWindow,
            Title = "",
            Width = 669,
            Height = 118,
            Left = 1130,
            Top = 50,
            ResizeMode = ResizeMode.NoResize,
            WindowStyle = WindowStyle.None,
            Background = Brushes.Bisque,
            Topmost = true,
            ShowInTaskbar = false
        };
        window.MouseLeftButtonDown += (s, e) =>
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                window.DragMove();
        };
        _windowService.Hide(this);
        window.ShowDialog();
        _windowService.Show(this);
    }
}