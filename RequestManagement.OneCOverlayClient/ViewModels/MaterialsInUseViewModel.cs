using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;

namespace OneCOverlayClient.ViewModels
{
    public partial class MaterialsInUseViewModel : ObservableObject
    {
        [ObservableProperty]
        private MaterialsInUse _mainSelectedMaterialsInUse;
        [ObservableProperty]
        private MaterialsInUse _selectedMaterialsInUse;
        private readonly IMaterialsInUseService _materialsInUseService;
        private DateTime _workDateTime;
        private List<MaterialsInUse> _materialsInUse = new();
        private List<MaterialsInUse> _itemsMaterialsInUse = new();
        private List<MaterialsInUse> _materialsInUseDocOnly = new();
        private Driver? _selectedFinanciallyResponsiblePerson;
        private int _equipmentIndex = 0;
        private int _itemIndex = 0;
        public event EventHandler CloseWindowRequested;

        public MaterialsInUseViewModel(IMaterialsInUseService materialsInUseService)
        {
            _materialsInUseService = materialsInUseService;
        }

        private void UpdateEquipmentField()
        {
            if(_materialsInUseDocOnly.Count == 0) return;
            if (_equipmentIndex > _materialsInUseDocOnly.Count - 1)
            {
                _equipmentIndex = 0;
            }

            if (_equipmentIndex < 0)
            {
                _equipmentIndex = _materialsInUseDocOnly.Count - 1;
            }

            MainSelectedMaterialsInUse = _materialsInUseDocOnly[_equipmentIndex];
            _itemsMaterialsInUse = _materialsInUse.Where(x => x.Equipment?.Id == MainSelectedMaterialsInUse.Equipment?.Id).ToList(); //ItemsMaterialsInUse
            _itemIndex = 0;
            UpdateItemField();
        }


        private void UpdateItemField()
        {
            if (_itemsMaterialsInUse.Count == 0) return;
            if (_itemIndex > _itemsMaterialsInUse.Count - 1)
            {
                _itemIndex = 0;
            }

            if (_itemIndex < 0)
            {
                _itemIndex = _itemsMaterialsInUse.Count - 1;
            }

            SelectedMaterialsInUse = _itemsMaterialsInUse[_itemIndex];
        }

        public async Task Init(DateTime workDateTime, Driver? selectedFinanciallyResponsiblePerson)
        {
            _workDateTime = workDateTime;
            _selectedFinanciallyResponsiblePerson = selectedFinanciallyResponsiblePerson;
            if(_selectedFinanciallyResponsiblePerson == null)
                throw new ArgumentNullException(nameof(_selectedFinanciallyResponsiblePerson));
            _materialsInUse = await _materialsInUseService.GetAllMaterialsInUseForOffAsync(_selectedFinanciallyResponsiblePerson.Id, workDateTime);
            _materialsInUseDocOnly = _materialsInUse
                .DistinctBy(m => m.Equipment?.Id)
                .ToList();
            UpdateEquipmentField();
        }

        [RelayCommand]
        private void CloseWindow()
        {
            CloseWindowRequested.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void PrevEquipment()
        {
            _equipmentIndex--;
            UpdateEquipmentField();
        }

        [RelayCommand]
        private void NextEquipment()
        {
            _equipmentIndex++;
            UpdateEquipmentField();
        }

        [RelayCommand]
        private void PrevItem()
        {
            _itemIndex--;
            UpdateItemField();
        }

        [RelayCommand]
        private void NextItem()
        {
            _itemIndex++;
            UpdateItemField();
        }
    }
}
