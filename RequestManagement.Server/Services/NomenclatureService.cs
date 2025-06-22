


using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services;

public class NomenclatureService(ApplicationDbContext dbContext) : INomenclatureService
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<List<Nomenclature>> GetAllNomenclaturesAsync(string filter = "")
    {
        var query = _dbContext.Nomenclatures.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            query = phrases.Aggregate(query, (current, phrase) => current.Where(e => e.Name.ToLower().Contains(phrase) || e.Article.ToLower().Contains(phrase) || e.Code.ToLower().Contains(phrase) || e.UnitOfMeasure.ToLower().Contains(phrase)));
        }
        return await query
            .Where(x=>x.Id!=1)
            .Select(e => new Nomenclature
            {
                Id = e.Id,
                Name = e.Name,
                Article = e.Article,
                Code = e.Code,
                UnitOfMeasure = e.UnitOfMeasure
            })
            .ToListAsync();
    }

    public async Task<int> CreateNomenclatureAsync(Nomenclature nomenclature)
    {
        _dbContext.Nomenclatures.Add(nomenclature);
        await _dbContext.SaveChangesAsync();
        return nomenclature.Id;
    }

    public async Task<bool> UpdateNomenclatureAsync(Nomenclature nomenclature)
    {
        if(nomenclature.Id == 1) return true;
        var existNomenclature = await _dbContext.Nomenclatures
            .FirstOrDefaultAsync(e => e.Id == nomenclature.Id);

        if (existNomenclature == null)
            return false;

        existNomenclature.Name = nomenclature.Name;
        existNomenclature.Article = nomenclature.Article;
        existNomenclature.Code = nomenclature.Code;
        existNomenclature.UnitOfMeasure = nomenclature.UnitOfMeasure;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteNomenclatureAsync(int id)
    {
        try
        {
            if (id == 1) return true;
            var nomenclature = await _dbContext.Nomenclatures
                .FirstOrDefaultAsync(e => e.Id == id);

            if (nomenclature == null)
                return false;

            _dbContext.Nomenclatures.Remove(nomenclature);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}