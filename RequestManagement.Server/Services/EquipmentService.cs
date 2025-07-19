using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services;

public class EquipmentService(ApplicationDbContext dbContext) : IEquipmentService
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<int> CreateEquipmentAsync(Equipment equipment)
    {
        _dbContext.Equipments.Add(equipment);
        await _dbContext.SaveChangesAsync();
        return equipment.Id;
    }

    public async Task<bool> UpdateEquipmentAsync(Equipment equipment)
    {
        if (equipment.Id == 1) return true;
        var existingEquipment = await _dbContext.Equipments
            .FirstOrDefaultAsync(e => e.Id == equipment.Id);

        if (existingEquipment == null)
            return false;

        existingEquipment.Name = equipment.Name;
        existingEquipment.StateNumber = equipment.StateNumber;
        existingEquipment.Code = equipment.Code;
        existingEquipment.ShortName = equipment.ShortName;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteEquipmentAsync(int id)
    {
        try
        {
            if (id == 1) return true;
            var equipment = await _dbContext.Equipments
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipment == null)
                return false;

            _dbContext.Equipments.Remove(equipment);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<Equipment>> GetAllEquipmentAsync(string filter = "")
    {
        var query = _dbContext.Equipments.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            query = phrases.Aggregate(query, (current, phrase) => current.Where(e => e.Name.ToLower().Contains(phrase) || e.StateNumber!.ToLower().Contains(phrase) || e.Code.ToLower().Contains(phrase)));
        }
        return await query
            .Where(x => x.Id != 1)
            .Select(e => new Equipment
            {
                Id = e.Id,
                Name = e.Name,
                StateNumber = e.StateNumber ?? "",
                Code = e.Code,
                ShortName = e.ShortName ?? ""
            })
            .ToListAsync();
    }
}