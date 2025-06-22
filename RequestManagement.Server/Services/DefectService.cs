using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services;

public class DefectService(ApplicationDbContext dbContext) : IDefectService
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<List<Defect>> GetAllDefectsAsync(string filter = "")
    {
        var query = _dbContext.Defects.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            query = phrases.Aggregate(query, (current, phrase) => current.Where(e => e.Name.ToLower().Contains(phrase)));
        }
        return await query
            .Where(x => x.Id != 1)
            .Select(e => new Defect
            {
                Id = e.Id,
                Name = e.Name,
                DefectGroupId = e.DefectGroupId
            })
            .ToListAsync();
    }

    public async Task<int> CreateDefectAsync(Defect defect)
    {
        _dbContext.Defects.Add(defect);
        await _dbContext.SaveChangesAsync();
        return defect.Id;
    }
    public async Task<bool> UpdateDefectAsync(Defect defect)
    {
        if (defect.Id == 1) return true;
        var existDefect = await _dbContext.Defects
            .FirstOrDefaultAsync(e => e.Id == defect.Id);

        if (existDefect == null)
            return false;

        existDefect.Name = defect.Name;
        existDefect.DefectGroupId = defect.DefectGroupId;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDefectAsync(int id)
    {
        try
        {
            if (id == 1) return true;
            var defect = await _dbContext.Defects
                .FirstOrDefaultAsync(e => e.Id == id);

            if (defect == null)
                return false;

            _dbContext.Defects.Remove(defect);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}