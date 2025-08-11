using System.Globalization;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;

namespace OneCOverlayClient.ViewModels
{
    public partial class MaterialsInUseViewModel : ObservableObject
    {
        [ObservableProperty]
        private MaterialsInUse? _mainSelectedMaterialsInUse;
        [ObservableProperty]
        private MaterialsInUse? _selectedMaterialsInUse;
        [ObservableProperty]
        private string _progress = string.Empty;
        [ObservableProperty]
        private string _mainProgress = string.Empty;
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
            if (_materialsInUseDocOnly.Count == 0) return;
            if (_equipmentIndex > _materialsInUseDocOnly.Count - 1)
            {
                _equipmentIndex = 0;
            }

            if (_equipmentIndex < 0)
            {
                _equipmentIndex = _materialsInUseDocOnly.Count - 1;
            }

            MainSelectedMaterialsInUse = _materialsInUseDocOnly[_equipmentIndex];
            _itemsMaterialsInUse = _materialsInUse
                .Where(x => x.Equipment.Id == MainSelectedMaterialsInUse.Equipment.Id)
                .Where(x =>
                    MainSelectedMaterialsInUse.ReasonForWriteOff.Id is 2 or 12
                        ? x.ReasonForWriteOff.Id is 2 or 12
                        : x.ReasonForWriteOff.Id != 2 && x.ReasonForWriteOff.Id != 12
                )
                .ToList();
            _itemIndex = 0;
            MainProgress = $"{_equipmentIndex + 1}/{_materialsInUseDocOnly.Count}";
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

            Progress = $"{_itemIndex+1}/{_itemsMaterialsInUse.Count}";
            SelectedMaterialsInUse = _itemsMaterialsInUse[_itemIndex];
        }

        public async Task Init(DateTime workDateTime, Driver? selectedFinanciallyResponsiblePerson)
        {
            _workDateTime = workDateTime;
            _selectedFinanciallyResponsiblePerson = selectedFinanciallyResponsiblePerson;
            if (_selectedFinanciallyResponsiblePerson == null)
                throw new ArgumentNullException(nameof(_selectedFinanciallyResponsiblePerson));
            _materialsInUse = await _materialsInUseService.GetAllMaterialsInUseForOffAsync(_selectedFinanciallyResponsiblePerson.Id, workDateTime);
            _materialsInUseDocOnly = _materialsInUse
                .GroupBy(m => m.Equipment.Id)
                .SelectMany(g =>
                {
                    var groupSpecial = g.Where(x => x.ReasonForWriteOff.Id is 2 or 12).Take(1);
                    var groupOthers = g.Where(x => x.ReasonForWriteOff.Id != 2 && x.ReasonForWriteOff.Id != 12).Take(1);
                    return groupSpecial.Concat(groupOthers);
                })
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

        [RelayCommand]
        private async Task SaveDocNumberAsync()
        {
            if (MainSelectedMaterialsInUse == null) return;
            _itemsMaterialsInUse.ForEach(x => x.DocumentNumberForWriteOff = MainSelectedMaterialsInUse.DocumentNumberForWriteOff);
            await _materialsInUseService.UpdateMaterialsInUseAnyAsync(_itemsMaterialsInUse);
        }

        [RelayCommand]
        private void CopyEquipmentNameToClipboard()
        {
            if (MainSelectedMaterialsInUse == null) return;
            Clipboard.SetText(MainSelectedMaterialsInUse.Equipment.Name);
        }

        [RelayCommand]
        private void CopyEquipmentCodeToClipboard()
        {
            if (MainSelectedMaterialsInUse == null) return;
            Clipboard.SetText(MainSelectedMaterialsInUse.Equipment.Code);
        }

        [RelayCommand]
        private void CopyNomenclatureNameToClipboard()
        {
            if (SelectedMaterialsInUse == null) return;
            Clipboard.SetText(SelectedMaterialsInUse.Nomenclature.Name);
        }

        [RelayCommand]
        private void CopyNomenclatureCodeToClipboard()
        {
            if (SelectedMaterialsInUse == null) return;
            Clipboard.SetText(SelectedMaterialsInUse.Nomenclature.Code);
        }

        [RelayCommand]
        private void CopyNomenclatureQuantityToClipboard()
        {
            if (SelectedMaterialsInUse == null) return;
            Clipboard.SetText(SelectedMaterialsInUse.Quantity.ToString(CultureInfo.InvariantCulture));
        }

        [RelayCommand]
        private void CopyDocumentNumberToClipboard()
        {
            if (SelectedMaterialsInUse == null) return;
            Clipboard.SetText(SelectedMaterialsInUse.DocumentNumber);
        }

        [RelayCommand]
        private void CopyDocumentDateToClipboard()
        {
            if (SelectedMaterialsInUse == null) return;
            Clipboard.SetText(SelectedMaterialsInUse.Date.ToString("dd.MM.yyyy"));
        }
    }
}
