using RequestManagement.Common.Models;

namespace RequestManagement.Common.Interfaces;

public interface ISparePartsOwnershipService
{
    Task<List<SparePartsOwnership>> GetAllSparePartsOwnershipsAsync(int equipmentId, int warehouseId);
    Task<int> CreateSparePartsOwnershipAsync(SparePartsOwnership sparePartsOwnership);
    Task<bool> UpdateSparePartsOwnershipAsync(SparePartsOwnership sparePartsOwnership);
    Task<bool> DeleteSparePartsOwnershipAsync(int id);
}