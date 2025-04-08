using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Models;
using WpfClient.Services;

namespace WpfClient.ViewModels
{
    public partial class RequestsViewModel : ObservableObject
    {
        private readonly GrpcRequestService _requestService;

        [ObservableProperty]
        private ObservableCollection<Request> requests = new();

        public IRelayCommand CreateRequestCommand { get; }

        public RequestsViewModel(GrpcRequestService requestService)
        {
            _requestService = requestService;
            CreateRequestCommand = new RelayCommand(() => { /* Логика создания */ });
        }

        public async Task LoadRequestsAsync()
        {
            
        }
    }
}