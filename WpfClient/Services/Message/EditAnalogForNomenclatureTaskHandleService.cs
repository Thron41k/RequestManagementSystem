using System.Windows;
using RequestManagement.WpfClient.Models;
using RequestManagement.WpfClient.Services.Interfaces;
using RequestManagement.WpfClient.ViewModels;
using RequestManagement.WpfClient.Views;

namespace RequestManagement.WpfClient.Services.Message;

public class EditAnalogForNomenclatureTaskHandleService(SparePartsAnalogsViewModel sparePartsAnalogsViewModel, IMessageBus messageBus) : IMessageHandlerService<DialogSparePartsAnalogsModel>
{
    public async Task HandleAsync(DialogSparePartsAnalogsModel message)
    {
        if(message.Result) return;
        var sparePartsAnalogsView = new SparePartsAnalogsView(sparePartsAnalogsViewModel);
        var window = new Window
        {
            Content = sparePartsAnalogsView,
            Title = "Аналоги",
            Width = 850,
            Height = 490,
            ResizeMode = ResizeMode.NoResize
        };
        sparePartsAnalogsViewModel.Init(message.InitNomenclature);
        window.ShowDialog();
        if (sparePartsAnalogsViewModel.DialogResult)
        {
            message.Result = true;
            await messageBus.Publish(message);
        }
    }
}