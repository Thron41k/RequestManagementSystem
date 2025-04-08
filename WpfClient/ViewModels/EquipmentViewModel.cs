using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Server.Controllers;
using WpfClient.Services;
using Dispatcher = System.Windows.Threading.Dispatcher;
using Timer = System.Timers.Timer;


namespace WpfClient.ViewModels
{
    public class EquipmentViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly GrpcRequestService _requestService;
        private Equipment? _selectedEquipment;
        private string _newEquipmentLicensePlate;
        private string _newEquipmentName;
        private readonly Timer _filterTimer;
        private string _filterText;
        private readonly Dispatcher _dispatcher;
        public event EventHandler CloseWindowRequested;
        public ObservableCollection<Equipment> EquipmentList { get; } = [];
        public ICommand LoadEquipmentCommand { get; }
        public ICommand AddEquipmentCommand { get; }
        public ICommand UpdateEquipmentCommand { get; }
        public ICommand DeleteEquipmentCommand { get; }
        public ICommand SelectRowCommand { get; }

        public string FilterText
        {
            get => _filterText;
            set
            {
                if (_filterText == value) return;
                _filterText = value;
                OnPropertyChanged();
                _filterTimer.Stop(); // Сбрасываем таймер при каждом вводе
                _filterTimer.Start(); // Запускаем таймер заново
            }
        }

        public string NewEquipmentName {
            get => _newEquipmentName;
            set
            {
                if (_newEquipmentName == value) return;
                _newEquipmentName = value;
                OnPropertyChanged(); // Уведомляем UI об изменении
            }
        }
        public string NewEquipmentLicensePlate {
            get => _newEquipmentLicensePlate;
            set
            {
                if (_newEquipmentLicensePlate == value) return;
                _newEquipmentLicensePlate = value;
                OnPropertyChanged(); // Уведомляем UI об изменении
            }
        }

        public Equipment? SelectedEquipment
        {
            get => _selectedEquipment ?? null;
            set
            {
                _selectedEquipment = value;
                AddToEdit();
            }
        }

        public EquipmentViewModel(GrpcRequestService requestService)
        {
            _requestService = requestService;
            LoadEquipmentCommand = new RelayCommand(Execute);
            AddEquipmentCommand = new RelayCommand(Action);
            UpdateEquipmentCommand = new RelayCommand(Execute1);
            DeleteEquipmentCommand = new RelayCommand(Action1);
            SelectRowCommand = new RelayCommand(Action2);
            _dispatcher = Dispatcher.CurrentDispatcher;
            _filterTimer = new Timer(1000) { AutoReset = false }; // Задержка 1 секунда
            _filterTimer.Elapsed += async (s, e) => await LoadEquipmentAsync();
            return;


            async void Action1() => await DeleteEquipmentAsync();

            async void Execute1() => await UpdateEquipmentAsync();

            async void Action() => await AddEquipmentAsync();

            async void Execute() => await LoadEquipmentAsync();
            async void Action2() => await SelectAndClose();
        }

        private async Task SelectAndClose()
        {
            if (SelectedEquipment != null)
            {
                CloseWindowRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task Load()
        {
            await LoadEquipmentAsync();
        }
        private async Task LoadEquipmentAsync()
        {
            var filter = string.IsNullOrWhiteSpace(_filterText) ? "" : _filterText.Trim();
            var equipmentList = await _requestService.GetAllEquipmentAsync(filter);
            await _dispatcher.InvokeAsync(() =>
            {
                EquipmentList.Clear();
                foreach (var item in equipmentList)
                {
                    EquipmentList.Add(item);
                }
                return Task.CompletedTask;
            });
        }

        private Task AddToEdit()
        {
            if (_selectedEquipment != null)
            {
                NewEquipmentName = _selectedEquipment.Name;
                NewEquipmentLicensePlate = _selectedEquipment.LicensePlate;
            }
            return Task.CompletedTask;
        }
        private async Task AddEquipmentAsync()
        {
            if (!string.IsNullOrWhiteSpace(NewEquipmentName))
            {
                var request = new CreateEquipmentRequest
                {
                    Name = NewEquipmentName,
                    LicensePlate = NewEquipmentLicensePlate ?? ""
                };
                await _requestService.CreateEquipmentAsync(request);
                await LoadEquipmentAsync(); // Обновляем список после добавления
                NewEquipmentName = string.Empty;
                NewEquipmentLicensePlate = string.Empty;
            }
        }

        private async Task UpdateEquipmentAsync()
        {
            if (_selectedEquipment != null && !string.IsNullOrEmpty(NewEquipmentName.Trim()))
            {
                var request = new UpdateEquipmentRequest
                {
                    Id = _selectedEquipment.Id,
                    Name = NewEquipmentName,
                    LicensePlate = NewEquipmentLicensePlate
                };
                await _requestService.UpdateEquipmentAsync(request);
                await LoadEquipmentAsync(); // Обновляем список после изменения
            }
        }

        private async Task DeleteEquipmentAsync()
        {
            if (_selectedEquipment != null)
            {
                var request = new DeleteEquipmentRequest { Id = _selectedEquipment.Id };
                await _requestService.DeleteEquipmentAsync(request);
                await LoadEquipmentAsync(); // Обновляем список после удаления
                NewEquipmentName = "";
                NewEquipmentLicensePlate = "";
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}