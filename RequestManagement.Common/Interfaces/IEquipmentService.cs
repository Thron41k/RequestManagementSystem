using RequestManagement.Common.Models;

namespace RequestManagement.Common.Interfaces;

public interface IEquipmentService
{
    Task<int> CreateEquipmentAsync(Equipment equipment);
    Task<bool> UpdateEquipmentAsync(Equipment equipment);
    Task<bool> DeleteEquipmentAsync(int id);
    Task<List<Equipment>> GetAllEquipmentAsync(string filter = "");
}