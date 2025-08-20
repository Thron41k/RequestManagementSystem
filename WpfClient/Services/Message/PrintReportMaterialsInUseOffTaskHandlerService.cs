using System.Windows;
using RequestManagement.WpfClient.Models;
using RequestManagement.WpfClient.Services.Interfaces;
using RequestManagement.WpfClient.ViewModels;
using RequestManagement.WpfClient.Views;

namespace RequestManagement.WpfClient.Services.Message;

public class PrintReportMaterialsInUseOffTaskHandlerService(PrintReportViewModel printReportViewModel) : IMessageHandlerService<PrintMaterialInUseOffModel>
{
    public Task HandleAsync(PrintMaterialInUseOffModel message)
    {
        var localPrintReportView = new PrintReportView(printReportViewModel);
        var window = new Window
        {
            Content = localPrintReportView,
            Title = "Печать отчётов",
            Width = 850,
            Height = 490,
            ResizeMode = ResizeMode.NoResize
        };
        printReportViewModel.Init(message.MaterialsInUse);
        window.ShowDialog();
        return Task.CompletedTask;
    }
}