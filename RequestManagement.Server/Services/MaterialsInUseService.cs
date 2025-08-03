using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services;

public class MaterialsInUseService(ApplicationDbContext dbContext) : IMaterialsInUseService
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<List<MaterialsInUse>> GetAllMaterialsInUseAsync(int financiallyResponsiblePersonId, string filter = "")
    {
        var query = dbContext.MaterialsInUse
            .Include(e => e.Nomenclature)
            .Include(e => e.Equipment)
            .Include(e => e.FinanciallyResponsiblePerson)
            .AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            query = query.Where(e => EF.Functions.ILike(e.Nomenclature.Name, $"%{filter}%") ||
                                     EF.Functions.ILike(e.Nomenclature.Article ?? "", $"%{filter}%") ||
                                     EF.Functions.ILike(e.Nomenclature.Code, $"%{filter}%"));
        }
        return await query.ToListAsync();
    }
    public async Task<int> CreateMaterialsInUseAsync(MaterialsInUse materialsInUse)
    {
        _dbContext.MaterialsInUse.Add(materialsInUse);
        await _dbContext.SaveChangesAsync();
        return materialsInUse.Id;
    }

    public async Task<bool> UploadMaterialsInUseAsync(List<MaterialsInUseForUpload> materialsInUse)
    {
        if (materialsInUse.Count == 0)
            return false;

        foreach (var item in materialsInUse)
        {
            // Парсинг даты
            if (!DateTime.TryParse(item.Date, out var parsedDate))
                continue; // Логирование ошибки возможно

            // Проверка номенклатуры
            var nomenclature = await _dbContext.Nomenclatures
                .FirstOrDefaultAsync(n => n.Code == item.NomenclatureCode);

            if (nomenclature is null)
            {
                nomenclature = new Nomenclature
                {
                    Code = item.NomenclatureCode,
                    Name = item.NomenclatureName,
                    Article = item.NomenclatureArticle,
                    UnitOfMeasure = item.NomenclatureUnitOfMeasure
                };

                _dbContext.Nomenclatures.Add(nomenclature);
                await _dbContext.SaveChangesAsync(); // нужно сохранить, чтобы получить Id
            }

            // Проверка техники
            var equipment = await _dbContext.Equipments
                .FirstOrDefaultAsync(e => e.Code == item.EquipmentCode);

            if (equipment is null)
            {
                equipment = new Equipment
                {
                    Code = item.EquipmentCode,
                    Name = item.EquipmentName
                };

                _dbContext.Equipments.Add(equipment);
                await _dbContext.SaveChangesAsync();
            }

            // Проверка МОЛ
            Driver? mol = null;
            if (!string.IsNullOrWhiteSpace(item.FinanciallyResponsiblePersonFullName))
            {
                mol = await _dbContext.Set<Driver>()
                    .FirstOrDefaultAsync(d => d.FullName == item.FinanciallyResponsiblePersonFullName);

                if (mol is null)
                {
                    mol = new Driver
                    {
                        FullName = item.FinanciallyResponsiblePersonFullName,
                        ShortName = item.FinanciallyResponsiblePersonFullName,
                        Position = "",
                        Code = ""
                    };

                    _dbContext.Set<Driver>().Add(mol);
                    await _dbContext.SaveChangesAsync();
                }
            }

            // Проверка на дублирование записи
            var exists = await _dbContext.Set<MaterialsInUse>().AnyAsync(mu =>
                mu.DocumentNumber == item.DocumentNumber &&
                mu.Date == parsedDate &&
                mu.NomenclatureId == nomenclature.Id &&
                mu.EquipmentId == equipment.Id &&
                mu.FinanciallyResponsiblePersonId == mol!.Id);

            if (!exists)
            {
                var newEntry = new MaterialsInUse
                {
                    DocumentNumber = item.DocumentNumber,
                    Date = parsedDate,
                    Quantity = item.Quantity,
                    NomenclatureId = nomenclature.Id,
                    EquipmentId = equipment.Id,
                    FinanciallyResponsiblePersonId = mol?.Id
                };

                _dbContext.Set<MaterialsInUse>().Add(newEntry);
            }
        }

        await _dbContext.SaveChangesAsync();
        return true;
    }


    public async Task<bool> UpdateMaterialsInUseAsync(MaterialsInUse materialsInUse)
    {
        if (materialsInUse.Id == 1) return true;
        var existMaterialsInUse = await _dbContext.MaterialsInUse
            .FirstOrDefaultAsync(e => e.Id == materialsInUse.Id);

        if (existMaterialsInUse == null)
            return false;

        existMaterialsInUse.DocumentNumber = materialsInUse.DocumentNumber;
        existMaterialsInUse.Date = materialsInUse.Date;
        existMaterialsInUse.Quantity = materialsInUse.Quantity;
        existMaterialsInUse.NomenclatureId = materialsInUse.NomenclatureId;
        existMaterialsInUse.EquipmentId = materialsInUse.EquipmentId;
        existMaterialsInUse.FinanciallyResponsiblePersonId = materialsInUse.FinanciallyResponsiblePersonId;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteMaterialsInUseAsync(int id)
    {
        try
        {
            if (id == 1) return true;
            var materialsInUse = await _dbContext.MaterialsInUse
                .FirstOrDefaultAsync(e => e.Id == id);

            if (materialsInUse == null)
                return false;

            _dbContext.MaterialsInUse.Remove(materialsInUse);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}