using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OneCOverlayClient.Views;
using System.Windows;
using OneCOverlayClient.Services.Interfaces;
using System.Windows.Input;
using RequestManagement.Common.Models;
using OneCOverlayClient.Messages.Models;

namespace OneCOverlayClient.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly MaterialsInUseViewModel _materialsInUseViewModel;
    private readonly IWindowService _windowService;
    private readonly IMessageBus _messageBus;
    [ObservableProperty] private DateTime _workDateTime = DateTime.Now;
    [ObservableProperty] private Driver? _selectedFinanciallyResponsiblePerson;

    public MainWindowViewModel(MaterialsInUseViewModel materialsInUseViewModel, IWindowService windowService, IMessageBus messageBus)
    {
        _materialsInUseViewModel = materialsInUseViewModel;
        _windowService = windowService;
        _messageBus = messageBus;
        _messageBus.Subscribe<SelectDriverTaskModel>(OnSelect);
    }

    private Task OnSelect(SelectDriverTaskModel selectDriverTaskModel)
    {
        if (selectDriverTaskModel.Caller != typeof(MainWindowViewModel) || !selectDriverTaskModel.Result) return Task.CompletedTask;
        SelectedFinanciallyResponsiblePerson = selectDriverTaskModel.Driver;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private void ClearSelectedFinanciallyResponsiblePerson() => SelectedFinanciallyResponsiblePerson = null;

    [RelayCommand]
    private async Task SelectFinanciallyResponsiblePerson()
    {
        Console.WriteLine($"Публикуем тип: {typeof(SelectDriverTaskModel).Name}");

        await _messageBus.Publish(new SelectDriverTaskModel
        {
            Caller = typeof(MainWindowViewModel),
            EditMode = false
        });
    }

    [RelayCommand]
    private async Task MaterialsInUse()
    {
        var materialsInUseWindow = new MaterialsInUseView(_materialsInUseViewModel);
        var window = new Window
        {
            Content = materialsInUseWindow,
            Title = "",
            Width = 729,
            Height = 118,
            Left = 1130,
            Top = 50,
            ResizeMode = ResizeMode.NoResize,
            WindowStyle = WindowStyle.None,
            Topmost = true,
            ShowInTaskbar = false
        };
        window.MouseLeftButtonDown += (s, e) =>
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                window.DragMove();
        };
        _windowService.Hide(this);
        await _materialsInUseViewModel.Init(WorkDateTime, SelectedFinanciallyResponsiblePerson);
        window.ShowDialog();
        _windowService.Show(this);
    }
}