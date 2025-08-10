using OneCOverlayClient.Messages.Models;
using OneCOverlayClient.Services.Interfaces;
using System.Windows;
using OneCOverlayClient.ViewModels;
using OneCOverlayClient.Views;

namespace OneCOverlayClient.Messages.Handlers
{
    public class SelectDriverTaskHandler(IMessageBus messageBus, DriverViewModel driverViewModel) : IMessageHandler<SelectDriverTaskModel>
    {
        public async Task HandleAsync(SelectDriverTaskModel message)
        {
            if (message.Result) return;
            var driverView = new DriverView(driverViewModel);
            var window = new Window
            {
                Content = driverView,
                Title = "Сотрудники",
                Width = 1070,
                Height = 700
            };
            _ = driverViewModel.Load();
            driverViewModel.EditMode = message.EditMode;
            window.ShowDialog();
            if (driverViewModel.SelectedDriver != null && message.Caller != null && driverViewModel.DialogResult)
            {
                message.Driver = driverViewModel.SelectedDriver;
                message.Result = true;
                await messageBus.Publish(message);
            }


        }
    }
}
