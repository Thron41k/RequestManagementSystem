


using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services;

public class DefectGroupService(ApplicationDbContext dbContext) : IDefectGroupService
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<List<DefectGroup>> GetAllDefectGroupsAsync(string filter = "")
    {
        var query = _dbContext.DefectGroups.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            query = phrases.Aggregate(query, (current, phrase) => current.Where(e => e.Name.ToLower().Contains(phrase)));
        }
        return await query
            .Where(x => x.Id != 1)
            .Select(e => new DefectGroup
            {
                Id = e.Id,
                Name = e.Name,
            })
            .ToListAsync();
    }
    public async Task<int> CreateDefectGroupAsync(DefectGroup defectGroup)
    {
        _dbContext.DefectGroups.Add(defectGroup);
        await _dbContext.SaveChangesAsync();
        return defectGroup.Id;
    }
    public async Task<bool> UpdateDefectGroupAsync(DefectGroup defectGroup)
    {
        if (defectGroup.Id == 1) return true;
        var existDefectGroup = await _dbContext.DefectGroups
            .FirstOrDefaultAsync(e => e.Id == defectGroup.Id);

        if (existDefectGroup == null)
            return false;

        existDefectGroup.Name = defectGroup.Name;
        await _dbContext.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteDefectGroupAsync(int id)
    {
        try
        {
            if (id == 1) return true;
            var defectGroup = await _dbContext.DefectGroups
                .FirstOrDefaultAsync(e => e.Id == id);

            if (defectGroup == null)
                return false;

            _dbContext.DefectGroups.Remove(defectGroup);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}