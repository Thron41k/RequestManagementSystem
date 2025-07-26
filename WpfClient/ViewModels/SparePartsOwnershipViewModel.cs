using CommunityToolkit.Mvvm.ComponentModel;
using RequestManagement.WpfClient.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.WpfClient.ViewModels
{
    public partial class SparePartsOwnershipViewModel : ObservableObject
    {
        private readonly IMessageBus _messageBus;

        public SparePartsOwnershipViewModel()
        {
            
        }

        public SparePartsOwnershipViewModel(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }
    }
}
