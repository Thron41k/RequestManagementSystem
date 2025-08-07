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
            .Include(e => e.ReasonForWriteOff)
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
        var validItems = materialsInUse
            .Where(i => DateTime.TryParse(i.Date, out _))
            .Select(i =>
            {
                i.Date = DateTime.Parse(i.Date).ToString("yyyy-MM-dd");
                return i;
            })
            .ToList();
        if (validItems.Count == 0)
            return false;
        var nomenclatureCodes = validItems.Select(i => i.NomenclatureCode).Distinct().ToList();
        var equipmentCodes = validItems.Select(i => i.EquipmentCode).Distinct().ToList();
        var molNames = validItems
            .Where(i => !string.IsNullOrWhiteSpace(i.FinanciallyResponsiblePersonFullName))
            .Select(i => i.FinanciallyResponsiblePersonFullName)
            .Distinct()
            .ToList();
        var nomenclatures = await _dbContext.Nomenclatures
            .Where(n => nomenclatureCodes.Contains(n.Code))
            .ToDictionaryAsync(n => n.Code);
        var equipments = await _dbContext.Equipments
            .Where(e => equipmentCodes.Contains(e.Code))
            .ToDictionaryAsync(e => e.Code);
        var molList = await _dbContext.Set<Driver>()
            .Where(d => molNames.Contains(d.FullName))
            .ToDictionaryAsync(d => d.FullName);
        var newNomenclatures = new List<Nomenclature>();
        var newEquipments = new List<Equipment>();
        var newMolList = new List<Driver>();
        var newMaterialsInUse = new List<MaterialsInUse>();
        foreach (var item in validItems)
        {
            var date = DateTime.Parse(item.Date);
            if (!nomenclatures.TryGetValue(item.NomenclatureCode, out var nomenclature))
            {
                nomenclature = new Nomenclature
                {
                    Code = item.NomenclatureCode,
                    Name = item.NomenclatureName,
                    Article = item.NomenclatureArticle,
                    UnitOfMeasure = item.NomenclatureUnitOfMeasure
                };
                newNomenclatures.Add(nomenclature);
                nomenclatures[item.NomenclatureCode] = nomenclature;
            }
            if (!equipments.TryGetValue(item.EquipmentCode, out var equipment))
            {
                equipment = new Equipment
                {
                    Code = item.EquipmentCode,
                    Name = item.EquipmentName
                };
                newEquipments.Add(equipment);
                equipments[item.EquipmentCode] = equipment;
            }
            Driver? mol = null;
            if (!string.IsNullOrWhiteSpace(item.FinanciallyResponsiblePersonFullName))
            {
                if (!molList.TryGetValue(item.FinanciallyResponsiblePersonFullName, out mol))
                {
                    mol = new Driver
                    {
                        FullName = item.FinanciallyResponsiblePersonFullName,
                        ShortName = item.FinanciallyResponsiblePersonFullName,
                        Position = "",
                        Code = ""
                    };
                    newMolList.Add(mol);
                    molList[item.FinanciallyResponsiblePersonFullName] = mol;
                }
            }
            var exists = await _dbContext.Set<MaterialsInUse>().AnyAsync(mu =>
                mu.DocumentNumber == item.DocumentNumber &&
                mu.Date == date &&
                mu.Nomenclature.Code == item.NomenclatureCode &&
                mu.Equipment!.Code == item.EquipmentCode &&
                mu.FinanciallyResponsiblePerson.FullName == mol!.FullName);
            if (!exists)
            {
                newMaterialsInUse.Add(new MaterialsInUse
                {
                    DocumentNumber = item.DocumentNumber,
                    Date = date,
                    Quantity = item.Quantity,
                    Nomenclature = nomenclature,
                    Equipment = equipment,
                    FinanciallyResponsiblePerson = mol!
                });
            }
        }
        if (newNomenclatures.Count > 0)
            await _dbContext.Nomenclatures.AddRangeAsync(newNomenclatures);
        if (newEquipments.Count > 0)
            await _dbContext.Equipments.AddRangeAsync(newEquipments);
        if (newMolList.Count > 0)
            await _dbContext.Set<Driver>().AddRangeAsync(newMolList);
        await _dbContext.SaveChangesAsync();
        if (newMaterialsInUse.Count > 0)
        {
            foreach (var entry in newMaterialsInUse)
            {
                entry.NomenclatureId = entry.Nomenclature.Id;
                entry.EquipmentId = entry.Equipment!.Id;
                entry.FinanciallyResponsiblePersonId = entry.FinanciallyResponsiblePerson.Id;
                entry.ReasonForWriteOffId = 1;
                entry.DateForWriteOff = DateTime.MinValue;
            }
            await _dbContext.Set<MaterialsInUse>().AddRangeAsync(newMaterialsInUse);
            await _dbContext.SaveChangesAsync();
        }
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
        existMaterialsInUse.IsOut = materialsInUse.IsOut;
        existMaterialsInUse.FinanciallyResponsiblePersonId = materialsInUse.FinanciallyResponsiblePersonId;
        existMaterialsInUse.ReasonForWriteOffId = materialsInUse.ReasonForWriteOffId;
        existMaterialsInUse.DocumentNumberForWriteOff = materialsInUse.DocumentNumberForWriteOff;
        existMaterialsInUse.DateForWriteOff = materialsInUse.DateForWriteOff;
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