using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services;

public class ReasonsForWritingOffMaterialsFromOperationService(ApplicationDbContext dbContext) : IReasonsForWritingOffMaterialsFromOperationService
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<List<ReasonsForWritingOffMaterialsFromOperation>> GetAllReasonsForWritingOffMaterialsFromOperationAsync(string filter = "")
    {
        var query = _dbContext.ReasonsForWritingOffMaterialsFromOperation.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            query = phrases.Aggregate(query, (current, phrase) => current.Where(e => e.Reason.ToLower().Contains(phrase)));
        }
        return await query
            .Where(x => x.Id != 1)
            .Select(e => new ReasonsForWritingOffMaterialsFromOperation
            {
                Id = e.Id,
                Reason = e.Reason
            })
            .ToListAsync();
    }

    public async Task<int> CreateReasonsForWritingOffMaterialsFromOperationAsync(ReasonsForWritingOffMaterialsFromOperation reasonsForWritingOffMaterialsFromOperation)
    {
        _dbContext.ReasonsForWritingOffMaterialsFromOperation.Add(reasonsForWritingOffMaterialsFromOperation);
        await _dbContext.SaveChangesAsync();
        return reasonsForWritingOffMaterialsFromOperation.Id;
    }
    public async Task<bool> UpdateReasonsForWritingOffMaterialsFromOperationAsync(ReasonsForWritingOffMaterialsFromOperation reasonsForWritingOffMaterialsFromOperation)
    {
        if (reasonsForWritingOffMaterialsFromOperation.Id == 1) return true;
        var existReasonsForWritingOffMaterialsFromOperation = await _dbContext.ReasonsForWritingOffMaterialsFromOperation
            .FirstOrDefaultAsync(e => e.Id == reasonsForWritingOffMaterialsFromOperation.Id);

        if (existReasonsForWritingOffMaterialsFromOperation == null)
            return false;

        existReasonsForWritingOffMaterialsFromOperation.Reason = reasonsForWritingOffMaterialsFromOperation.Reason;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteReasonsForWritingOffMaterialsFromOperationAsync(int id)
    {
        try
        {
            if (id == 1) return true;
            var reasonsForWritingOffMaterialsFromOperation = await _dbContext.ReasonsForWritingOffMaterialsFromOperation
                .FirstOrDefaultAsync(e => e.Id == id);

            if (reasonsForWritingOffMaterialsFromOperation == null)
                return false;

            _dbContext.ReasonsForWritingOffMaterialsFromOperation.Remove(reasonsForWritingOffMaterialsFromOperation);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}