


using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services;

public class RequestService(ApplicationDbContext dbContext) : IRequestService
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

    public async Task<List<Defect>> GetAllDefectsAsync(string filter = "")
    {
        var query = _dbContext.Defects.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            query = phrases.Aggregate(query, (current, phrase) => current.Where(e => e.Name.ToLower().Contains(phrase) || e.DefectGroup.Name.ToLower().Contains(phrase)));
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