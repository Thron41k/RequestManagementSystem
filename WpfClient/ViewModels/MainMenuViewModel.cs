using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfClient.Services;
using WpfClient.Views;

namespace WpfClient.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
    {
        private readonly GrpcRequestService _requestService;
        private object _currentView;
        public ICommand ViewRequestsCommand { get; }
        public ICommand CreateRequestCommand { get; }
        public ICommand EditEquipmentCommand { get; }
        public ICommand EditDriversCommand { get; }
        public ICommand EditDefectGroupsCommand { get; }
        public ICommand EditDefectsCommand { get; }
        public ICommand EditNomenclatureCommand { get; }
        public MainMenuViewModel(GrpcRequestService requestService)
        {
            _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));

            ViewRequestsCommand = new Helpers.RelayCommand(ViewRequests);
            CreateRequestCommand = new Helpers.RelayCommand(CreateRequest);
            EditEquipmentCommand = new Helpers.RelayCommand(EditEquipment);
            EditDriversCommand = new Helpers.RelayCommand(EditDrivers);
            EditDefectGroupsCommand = new Helpers.RelayCommand(EditDefectGroups);
            EditDefectsCommand = new Helpers.RelayCommand(EditDefects);
            EditNomenclatureCommand = new Helpers.RelayCommand(EditNomenclature);

            IsAdminVisible = AuthTokenStore.UserRole == (int)UserRole.Administrator;
        }
        public bool IsAdminVisible { get; }

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        private void ViewRequests()
        {
            CurrentView = new RequestsView(); // Создадим позже
        }

        private void CreateRequest()
        {
            CurrentView = new CreateRequestView(); // Создадим позже
        }

        private void EditEquipment()
        {
            CurrentView = new EquipmentView(); // Создадим позже
        }

        private void EditDrivers()
        {
            CurrentView = new DriversView(); // Создадим позже
        }

        private void EditDefectGroups()
        {
            CurrentView = new DefectGroupsView(); // Создадим позже
        }

        private void EditDefects()
        {
            CurrentView = new DefectsView(); // Создадим позже
        }

        private void EditNomenclature()
        {
            CurrentView = new NomenclatureView(); // Создадим позже
        }
    }
}
