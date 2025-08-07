using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Models;
using RequestManagement.WpfClient.Messages;
using RequestManagement.WpfClient.Services.Interfaces;

namespace RequestManagement.WpfClient.ViewModels
{
    public partial class AddMaterialsInUseToOffViewModel : ObservableObject
    {
        private readonly IMessageBus _messageBus;
        [ObservableProperty] private string _documentNumber = string.Empty;
        [ObservableProperty] private ReasonsForWritingOffMaterialsFromOperation _reason = new(){Id = 1};
        [ObservableProperty] private DateTime _documentDate = DateTime.Now;
        public event EventHandler CloseWindowRequested;
        public bool DialogResult { get; set; }

        public AddMaterialsInUseToOffViewModel(IMessageBus messageBus)
        {
            _messageBus = messageBus;
            _messageBus.Subscribe<SelectResultMessage>(OnShowDialog);
        }

        private Task OnShowDialog(SelectResultMessage arg)
        {
            if (arg.Caller == typeof(AddMaterialsInUseToOffViewModel) && arg.Item != null)
            {
                switch (arg.Message)
                {
                    case MessagesEnum.ResultReasonsForWritingOffMaterialsFromOperation:
                        Reason = (ReasonsForWritingOffMaterialsFromOperation)arg.Item;
                        break;
                }
            }

            return Task.CompletedTask;
        }

        public void Init(ReasonsForWritingOffMaterialsFromOperation reason, string documentNumber = "", DateTime documentDate = default)
        {
            DocumentNumber = documentNumber;
            Reason = reason;
            DocumentDate = documentDate == DateTime.MinValue ? DateTime.Now : documentDate;
        }

        [RelayCommand]
        private void SaveResult()
        {
            DialogResult = true;
            CloseWindowRequested.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private async Task SelectReasonsForWritingOffMaterialsFromOperation()
        {
            await _messageBus.Publish(new SelectTaskMessage(MessagesEnum.SelectReasonsForWritingOffMaterialsFromOperation, typeof(AddMaterialsInUseToOffViewModel)));
        }
    }
}
