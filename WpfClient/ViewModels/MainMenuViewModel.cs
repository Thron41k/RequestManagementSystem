using System.Windows;
using System.Windows.Input;
using WpfClient.Helpers;
using WpfClient.Views;

namespace WpfClient.ViewModels
{
    public class MainMenuViewModel
    {
        private readonly EquipmentViewModel _equipmentViewModel;

        public ICommand ShowEquipmentCommand { get; }

        public MainMenuViewModel(EquipmentViewModel equipmentViewModel)
        {
            _equipmentViewModel = equipmentViewModel;
            ShowEquipmentCommand = new RelayCommand(ShowEquipment);
        }

        private void ShowEquipment()
        {
            var equipmentView = new EquipmentView(_equipmentViewModel,true);
            var window = new Window
            {
                Content = equipmentView,
                Title = "Транспорт и ДСТ",
                Width = 800,
                Height = 600
            };
            window.Show();
            _ = _equipmentViewModel.Load();
        }
    }
}