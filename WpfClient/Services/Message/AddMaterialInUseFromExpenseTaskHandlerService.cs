using RequestManagement.WpfClient.Models;
using RequestManagement.WpfClient.Services.Interfaces;
using RequestManagement.WpfClient.ViewModels;
using RequestManagement.WpfClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RequestManagement.WpfClient.Services.Message
{
    public class AddMaterialInUseFromExpenseTaskHandlerService(AddMaterialInUseFromExpenseViewModel addMaterialInUseFromExpenseViewModel) : IMessageHandlerService<DialogAddMaterialInUseFromExpenseModel>
    {
        public async Task HandleAsync(DialogAddMaterialInUseFromExpenseModel message)
        {
            if (message.Result) return;
            var addMaterialInUseFromExpenseView = new AddMaterialInUseFromExpenseView(addMaterialInUseFromExpenseViewModel);
            var window = new Window
            {
                Content = addMaterialInUseFromExpenseView,
                Title = "Загрузка эксплуатации из расходов",
                Width = 1300,
                Height = 600
            };
            await addMaterialInUseFromExpenseViewModel.Init(message.Mol);
            window.ShowDialog();
        }
    }
}
