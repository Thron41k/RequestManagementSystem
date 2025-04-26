using RequestManagement.Common.Models;

namespace RequestManagement.Common.Interfaces;

public interface IRequestService
{
    Task<int> CreateEquipmentAsync(Equipment equipment);
    Task<bool> UpdateEquipmentAsync(Equipment equipment);
    Task<bool> DeleteEquipmentAsync(int id);
    Task<List<Equipment>> GetAllEquipmentAsync(string filter = "");

    Task<List<Driver>> GetAllDriversAsync(string filter = "");
    Task<int> CreateDriverAsync(Driver driver);
    Task<bool> UpdateDriverAsync(Driver driver);
    Task<bool> DeleteDriverAsync(int id);

    Task<List<DefectGroup>> GetAllDefectGroupsAsync(string filter = "");
    Task<int> CreateDefectGroupAsync(DefectGroup driver);
    Task<bool> UpdateDefectGroupAsync(DefectGroup driver);
    Task<bool> DeleteDefectGroupAsync(int id);

    Task<List<Defect>> GetAllDefectsAsync(string filter = "");
    Task<int> CreateDefectAsync(Defect driver);
    Task<bool> UpdateDefectAsync(Defect driver);
    Task<bool> DeleteDefectAsync(int id);

    Task<List<Nomenclature>> GetAllNomenclaturesAsync(string filter = "");
    Task<int> CreateNomenclatureAsync(Nomenclature nomenclature);
    Task<bool> UpdateNomenclatureAsync(Nomenclature nomenclature);
    Task<bool> DeleteNomenclatureAsync(int id);
}