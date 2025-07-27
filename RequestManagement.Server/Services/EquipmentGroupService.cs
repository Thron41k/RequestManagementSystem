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
        try
        {
            var query = _dbContext.EquipmentGroups
                .Where(g => g.Id != 1);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var phrase in phrases)
                {
                    var localPhrase = $"%{phrase}%";

                    query = query.Where(g =>
                        EF.Functions.ILike(g.Name, localPhrase) ||
                        g.Equipments.Any(e =>
                            EF.Functions.ILike(e.Name, localPhrase) ||
                            (e.StateNumber != null && EF.Functions.ILike(e.StateNumber, localPhrase))
                        ));
                }
            }

            return await query
                .Select(g => new EquipmentGroup
                {
                    Id = g.Id,
                    Name = g.Name,
                    Equipments = g.Equipments
                        .Select(e => new Equipment
                        {
                            Id = e.Id,
                            Name = e.Name,
                            ShortName = e.ShortName,
                            StateNumber = e.StateNumber,
                            Code = e.Code
                        })
                        .ToList()
                })
                .ToListAsync();

        }
        catch(Exception ex){
            Console.WriteLine(ex.Message);
        }
        return new List<EquipmentGroup>();
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