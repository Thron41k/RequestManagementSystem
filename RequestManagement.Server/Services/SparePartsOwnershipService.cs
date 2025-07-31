using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using RequestManagement.Server.Data;
using SparePartsOwnership = RequestManagement.Common.Models.SparePartsOwnership;

namespace RequestManagement.Server.Services;

public class SparePartsOwnershipService(ApplicationDbContext dbContext) : ISparePartsOwnershipService
{
    public async Task<List<SparePartsOwnership>> GetAllSparePartsOwnershipsAsync(int equipmentGroupId, int warehouseId)
    {
        // 1. Получаем все SPO для группы
        var ownerships = await dbContext.SparePartsOwnerships
            .Where(spo => spo.EquipmentGroupId == equipmentGroupId)
            .ToListAsync();

        if (!ownerships.Any())
            return [];

        var ownershipByNomenclatureId = ownerships.ToDictionary(spo => spo.NomenclatureId);

        // 2. Получаем все ID номенклатуры из ownerships
        var nomenclatureIds = ownershipByNomenclatureId.Keys.ToHashSet();

        // 3. Находим все аналоги (в обе стороны)
        var analogs = await dbContext.NomenclatureAnalogs
            .Where(na => nomenclatureIds.Contains(na.OriginalId) || nomenclatureIds.Contains(na.AnalogId))
            .ToListAsync();

        var allRelatedNomenclatureIds = new HashSet<int>(nomenclatureIds);
        foreach (var analog in analogs)
        {
            allRelatedNomenclatureIds.Add(analog.OriginalId);
            allRelatedNomenclatureIds.Add(analog.AnalogId);
        }

        // 4. Получаем SPO по группе и связанным номенклатурам, включая Nomenclature
        var allOwnerships = await dbContext.SparePartsOwnerships
            .Where(spo => spo.EquipmentGroupId == equipmentGroupId &&
                          allRelatedNomenclatureIds.Contains(spo.NomenclatureId))
            .Include(spo => spo.Nomenclature)
            .ToListAsync();

        var allOwnershipByNomenclatureId = allOwnerships.ToDictionary(spo => spo.NomenclatureId);

        // 5. Если warehouseId задан — получаем остатки
        Dictionary<int, decimal> stocks = [];
        if (warehouseId != 0)
        {
            stocks = await dbContext.Stocks
                .Where(s => s.WarehouseId == warehouseId && allRelatedNomenclatureIds.Contains(s.NomenclatureId))
                .ToDictionaryAsync(s => s.NomenclatureId, s => s.FinalQuantity);
        }

        // 6. Добавляем аналоги, которых нет в SPO, но которые есть в аналогах или остатках
        var allNomenclatureIdsInOwnership = allOwnershipByNomenclatureId.Keys.ToHashSet();
        var missingNomenclatureIds = allRelatedNomenclatureIds
            .Except(allNomenclatureIdsInOwnership)
            .ToList();

        if (missingNomenclatureIds.Any())
        {
            var missingNomenclatures = await dbContext.Nomenclatures
                .Where(n => missingNomenclatureIds.Contains(n.Id))
                .ToListAsync();

            foreach (var nomenclature in missingNomenclatures)
            {
                var newSpo = new SparePartsOwnership
                {
                    EquipmentGroupId = equipmentGroupId,
                    NomenclatureId = nomenclature.Id,
                    Nomenclature = nomenclature,
                    RequiredQuantity = 0,
                    CurrentQuantity = 0
                };

                if (warehouseId != 0 && stocks.TryGetValue(nomenclature.Id, out var quantity))
                {
                    newSpo.CurrentQuantity = (int)quantity;
                }

                allOwnerships.Add(newSpo);
            }
        }

        // 7. Обновляем остатки для существующих SPO
        if (warehouseId != 0)
        {
            foreach (var spo in allOwnerships)
            {
                stocks.TryGetValue(spo.NomenclatureId, out var quantity);
                spo.CurrentQuantity = (int)quantity;
            }
        }

        return allOwnerships;
    }




    public async Task<int> CreateSparePartsOwnershipAsync(SparePartsOwnership sparePartsOwnership)
    {
        dbContext.SparePartsOwnerships.Add(sparePartsOwnership);
        await dbContext.SaveChangesAsync();
        return sparePartsOwnership.Id;
    }

    public async Task<bool> UpdateSparePartsOwnershipAsync(SparePartsOwnership sparePartsOwnership)
    {
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
        var sparePartsOwnerships = await dbContext.SparePartsOwnerships
            .FirstOrDefaultAsync(e => e.Id == id);
        if (sparePartsOwnerships == null)
            return false;
        dbContext.SparePartsOwnerships.Remove(sparePartsOwnerships);
        await dbContext.SaveChangesAsync();
        return true;
    }
}