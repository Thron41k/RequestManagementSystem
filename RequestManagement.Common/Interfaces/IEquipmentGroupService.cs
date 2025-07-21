using RequestManagement.Common.Models;

namespace RequestManagement.Common.Interfaces;

public interface IEquipmentGroupService
{
    Task<List<EquipmentGroup>> GetAllEquipmentGroupsAsync(string filter = "");
    Task<int> CreateEquipmentGroupAsync(EquipmentGroup equipmentGroup);
    Task<bool> UpdateEquipmentGroupAsync(EquipmentGroup equipmentGroup);
    Task<bool> DeleteEquipmentGroupAsync(int id);
}