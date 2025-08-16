using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
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
        var initialNomenclatureIds = ownershipByNomenclatureId.Keys.ToHashSet();

        // 3. Находим ВСЕ связанные номенклатуры (рекурсивно через аналоги)
        var allRelatedNomenclatureIds = await GetAllRelatedNomenclatureIdsAsync(initialNomenclatureIds);

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

            // Строим карту аналогов (аналог -> ближайший оригинал из ownerships)
            var analogToOriginal = BuildAnalogToOriginalMap(initialNomenclatureIds, allRelatedNomenclatureIds);

            foreach (var nomenclature in missingNomenclatures)
            {
                var newSpo = new SparePartsOwnership
                {
                    EquipmentGroupId = equipmentGroupId,
                    NomenclatureId = nomenclature.Id,
                    Nomenclature = nomenclature,
                    RequiredQuantity = 0,
                    CurrentQuantity = 0,
                    AnalogId = analogToOriginal.TryGetValue(nomenclature.Id, out var originalId) &&
                               allOwnershipByNomenclatureId.TryGetValue(originalId, out var originalSpo)
                               ? originalSpo.Id
                               : null
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

    private async Task<HashSet<int>> GetAllRelatedNomenclatureIdsAsync(HashSet<int> initialIds)
    {
        var allRelatedIds = new HashSet<int>(initialIds);
        var queue = new Queue<int>(initialIds);

        while (queue.Count > 0)
        {
            var currentId = queue.Dequeue();

            // Находим все прямые аналоги (в обе стороны)
            var analogs = await dbContext.NomenclatureAnalogs
                .Where(na => na.OriginalId == currentId || na.AnalogId == currentId)
                .Select(na => na.OriginalId == currentId ? na.AnalogId : na.OriginalId)
                .ToListAsync();

            foreach (var analogId in analogs)
            {
                if (!allRelatedIds.Contains(analogId))
                {
                    allRelatedIds.Add(analogId);
                    queue.Enqueue(analogId);
                }
            }
        }

        return allRelatedIds;
    }

    private Dictionary<int, int> BuildAnalogToOriginalMap(
        HashSet<int> originalNomenclatureIds,
        HashSet<int> allRelatedNomenclatureIds)
    {
        var analogToOriginal = new Dictionary<int, int>();
        var visited = new HashSet<int>();
        var queue = new Queue<(int current, int? original)>();

        // Инициализируем очередь оригиналами
        foreach (var id in originalNomenclatureIds)
        {
            queue.Enqueue((id, null));
            visited.Add(id);
        }

        while (queue.Count > 0)
        {
            var (currentId, originalId) = queue.Dequeue();

            // Находим все прямые аналоги (в обе стороны)
            var analogs = dbContext.NomenclatureAnalogs
                .Where(na => na.OriginalId == currentId || na.AnalogId == currentId)
                .AsEnumerable()
                .Select(na => na.OriginalId == currentId ? na.AnalogId : na.OriginalId)
                .Where(id => allRelatedNomenclatureIds.Contains(id));

            foreach (var analogId in analogs)
            {
                if (!visited.Contains(analogId))
                {
                    visited.Add(analogId);
                    // Если это первый уровень, то оригинал - currentId, иначе передаем originalId
                    var newOriginalId = originalId ?? currentId;
                    analogToOriginal[analogId] = newOriginalId;
                    queue.Enqueue((analogId, newOriginalId));
                }
            }
        }

        return analogToOriginal;
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