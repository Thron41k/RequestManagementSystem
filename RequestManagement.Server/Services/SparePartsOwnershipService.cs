using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Data;
using SparePartsOwnership = RequestManagement.Common.Models.SparePartsOwnership;

namespace RequestManagement.Server.Services;

public class SparePartsOwnershipService(ApplicationDbContext dbContext) : ISparePartsOwnershipService
{
    public async Task<List<SparePartsOwnership>> GetAllSparePartsOwnershipsAsync(int equipmentGroupId, int warehouseId)
    {
        var result = await dbContext.SparePartsOwnerships
            .Where(spo => spo.EquipmentGroupId == equipmentGroupId)
            .Join(
                dbContext.Stocks,
                spo => spo.NomenclatureId,
                stock => stock.NomenclatureId,
                (spo, stock) => new { SparePartsOwnership = spo, Stock = stock })
            .Where(x => x.Stock.WarehouseId == warehouseId)
            .SelectMany(
                x => dbContext.NomenclatureAnalogs
                    .Where(na => na.OriginalId == x.SparePartsOwnership.NomenclatureId
                                 || na.AnalogId == x.SparePartsOwnership.NomenclatureId)
                    .Select(na => new { x.SparePartsOwnership })
                    .Distinct())
            .Select(x => x.SparePartsOwnership)
            .ToListAsync();

        return result;
    }

    public async Task<int> CreateSparePartsOwnershipAsync(SparePartsOwnership sparePartsOwnership)
    {
        dbContext.SparePartsOwnerships.Add(sparePartsOwnership);
        await dbContext.SaveChangesAsync();
        return sparePartsOwnership.Id;
    }

    public async Task<bool> UpdateSparePartsOwnershipAsync(SparePartsOwnership sparePartsOwnership)
    {
        if (sparePartsOwnership.Id == 1) return true;
        var existSparePartsOwnership = await dbContext.SparePartsOwnerships
            .FirstOrDefaultAsync(e => e.Id == sparePartsOwnership.Id);
        if (existSparePartsOwnership == null)
            return false;
        existSparePartsOwnership.RequiredQuantity = sparePartsOwnership.RequiredQuantity;
        existSparePartsOwnership.CurrentQuantity = sparePartsOwnership.CurrentQuantity;
        existSparePartsOwnership.Comment = sparePartsOwnership.Comment;
        existSparePartsOwnership.EquipmentGroupId = sparePartsOwnership.EquipmentGroupId;
        existSparePartsOwnership.NomenclatureId = sparePartsOwnership.NomenclatureId;
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteSparePartsOwnershipAsync(int id)
    {
        if (id == 1) return true;
        var sparePartsOwnerships = await dbContext.SparePartsOwnerships
            .FirstOrDefaultAsync(e => e.Id == id);
        if (sparePartsOwnerships == null)
            return false;
        dbContext.SparePartsOwnerships.Remove(sparePartsOwnerships);
        await dbContext.SaveChangesAsync();
        return true;
    }
}