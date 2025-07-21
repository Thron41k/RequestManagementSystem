using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services;

public class EquipmentGroupService(ApplicationDbContext dbContext) : IEquipmentGroupService
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<List<EquipmentGroup>> GetAllEquipmentGroupsAsync(string filter = "")
    {
        var query = _dbContext.EquipmentGroups.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            query = phrases.Aggregate(query, (current, phrase) => current.Where(e => e.Name.ToLower().Contains(phrase)));
        }
        return await query
            .Where(x => x.Id != 1)
            .Select(e => new EquipmentGroup
            {
                Id = e.Id,
                Name = e.Name,
            })
            .ToListAsync();
    }
    public async Task<int> CreateEquipmentGroupAsync(EquipmentGroup equipmentGroup)
    {
        _dbContext.EquipmentGroups.Add(equipmentGroup);
        await _dbContext.SaveChangesAsync();
        return equipmentGroup.Id;
    }
    public async Task<bool> UpdateEquipmentGroupAsync(EquipmentGroup equipmentGroup)
    {
        if (equipmentGroup.Id == 1) return true;
        var existEquipmentGroup = await _dbContext.EquipmentGroups
            .FirstOrDefaultAsync(e => e.Id == equipmentGroup.Id);

        if (existEquipmentGroup == null)
            return false;

        existEquipmentGroup.Name = equipmentGroup.Name;
        await _dbContext.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteEquipmentGroupAsync(int id)
    {
        try
        {
            if (id == 1) return true;
            var equipmentGroup = await _dbContext.EquipmentGroups
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipmentGroup == null)
                return false;

            _dbContext.EquipmentGroups.Remove(equipmentGroup);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}