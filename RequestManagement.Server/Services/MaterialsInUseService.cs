using Azure.Core;
using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Common.Utilities;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services;

public class MaterialsInUseService(ApplicationDbContext dbContext) : IMaterialsInUseService
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<List<MaterialsInUse>> GetAllMaterialsInUseAsync(
        int financiallyResponsiblePersonId,
        string filter = "")
    {
        var query = dbContext.MaterialsInUse
            .Include(e => e.Nomenclature)
            .Include(e => e.Equipment)
            .Include(e => e.FinanciallyResponsiblePerson)
            .Include(e => e.ReasonForWriteOff)
            .Include(e => e.MolForMove)
            .AsQueryable();

        // Отфильтруем по ответственному лицу
        query = query.Where(e => e.FinanciallyResponsiblePersonId == financiallyResponsiblePersonId);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            // Разбиваем на слова и убираем пустые
            var words = filter
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var word in words)
            {
                var w = word; // локальная переменная для замыкания

                query = query.Where(e =>
                    EF.Functions.ILike(e.Nomenclature.Name, $"%{w}%") ||
                    EF.Functions.ILike(e.Nomenclature.Article ?? "", $"%{w}%") ||
                    EF.Functions.ILike(e.Nomenclature.Code, $"%{w}%") ||
                    // Логика FullName: Name(StateNumber) если есть, иначе Name
                    EF.Functions.ILike(
                        (e.Equipment.StateNumber != null && e.Equipment.StateNumber != "")
                            ? (e.Equipment.Name + "(" + e.Equipment.StateNumber + ")")
                            : e.Equipment.Name,
                        $"%{w}%"
                    )
                );
            }
        }

        return await query.ToListAsync();
    }


    public async Task<List<MaterialsInUse>> GetAllMaterialsInUseForOffAsync(
        int financiallyResponsiblePersonId,
        DateTime date)
    {
        return await dbContext.MaterialsInUse
            .Include(e => e.Nomenclature)
            .Include(e => e.Equipment)
            .Include(e => e.FinanciallyResponsiblePerson)
            .Include(e => e.ReasonForWriteOff)
            .Where(e => e.FinanciallyResponsiblePersonId == financiallyResponsiblePersonId)
            .Where(e => e.DateForWriteOff == date) 
            .ToListAsync();
    }

    public async Task<int> CreateMaterialsInUseAsync(MaterialsInUse materialsInUse)
    {
        _dbContext.MaterialsInUse.Add(materialsInUse);
        await _dbContext.SaveChangesAsync();
        return materialsInUse.Id;
    }

    public async Task<bool> CreateMaterialsInUseAnyAsync(IEnumerable<MaterialsInUse> materialsInUseList)
    {
        if (materialsInUseList is null)
            throw new ArgumentNullException(nameof(materialsInUseList));
        var materialsInUses = materialsInUseList as MaterialsInUse[] ?? materialsInUseList.ToArray();
        var entities = materialsInUses.ToList();
        await _dbContext.MaterialsInUse.AddRangeAsync(entities);
        await _dbContext.SaveChangesAsync();
        return entities.Count == materialsInUses.Length;
    }


    public async Task<bool> UploadMaterialsInUseAsync(List<MaterialsInUseForUpload> materialsInUse)
    {
        if (materialsInUse == null || materialsInUse.Count == 0)
            return false;

        var validItems = materialsInUse
            .Where(i => DateTimeHelper.TryParseDto(i.Date, out _))
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

        // Используем Dictionary с проверкой на дубликаты при загрузке из БД
        var nomenclatures = new Dictionary<string, Nomenclature>();
        var existingNomenclatures = await _dbContext.Nomenclatures
            .Where(n => nomenclatureCodes.Contains(n.Code))
            .ToListAsync();

        foreach (var n in existingNomenclatures)
        {
            if (!nomenclatures.ContainsKey(n.Code))
                nomenclatures[n.Code] = n;
        }

        var equipments = new Dictionary<string, Equipment>();
        var existingEquipments = await _dbContext.Equipments
            .Where(e => equipmentCodes.Contains(e.Code))
            .ToListAsync();

        foreach (var e in existingEquipments)
        {
            if (!equipments.ContainsKey(e.Code))
                equipments[e.Code] = e;
        }

        var molList = new Dictionary<string, Driver>();
        if (molNames.Any())
        {
            var existingDrivers = await _dbContext.Set<Driver>()
                .Where(d => molNames.Contains(d.FullName))
                .ToListAsync();

            foreach (var d in existingDrivers)
            {
                if (!molList.ContainsKey(d.FullName))
                    molList[d.FullName] = d;
            }
        }

        var newNomenclatures = new List<Nomenclature>();
        var newEquipments = new List<Equipment>();
        var newDrivers = new List<Driver>();

        // Сначала собираем все новые сущности без добавления в словари
        var itemsToProcess = new List<(MaterialsInUseForUpload item, DateTime date)>();

        foreach (var item in validItems)
        {
            var date = DateTimeHelper.TryParseDto(item.Date, out var parsedDate)
                ? parsedDate
                : throw new FormatException();

            itemsToProcess.Add((item, date));

            // Проверяем и собираем новые номенклатуры
            if (!nomenclatures.ContainsKey(item.NomenclatureCode))
            {
                var n = new Nomenclature
                {
                    Code = item.NomenclatureCode,
                    Name = item.NomenclatureName,
                    Article = item.NomenclatureArticle,
                    UnitOfMeasure = item.NomenclatureUnitOfMeasure
                };
                newNomenclatures.Add(n);
                // Не добавляем в словарь сразу, чтобы избежать дубликатов при повторных итерациях
            }

            // Проверяем и собираем новое оборудование
            if (!equipments.ContainsKey(item.EquipmentCode))
            {
                var e = new Equipment
                {
                    Code = item.EquipmentCode,
                    Name = item.EquipmentName
                };
                newEquipments.Add(e);
            }

            // Проверяем и собираем новых водителей
            if (!string.IsNullOrWhiteSpace(item.FinanciallyResponsiblePersonFullName) &&
                !molList.ContainsKey(item.FinanciallyResponsiblePersonFullName))
            {
                var d = new Driver
                {
                    FullName = item.FinanciallyResponsiblePersonFullName,
                    ShortName = item.FinanciallyResponsiblePersonFullName,
                    Position = "",
                    Code = ""
                };
                newDrivers.Add(d);
            }
        }

        // Удаляем дубликаты из новых списков
        newNomenclatures = newNomenclatures
            .GroupBy(n => n.Code)
            .Select(g => g.First())
            .ToList();

        newEquipments = newEquipments
            .GroupBy(e => e.Code)
            .Select(g => g.First())
            .ToList();

        newDrivers = newDrivers
            .GroupBy(d => d.FullName)
            .Select(g => g.First())
            .ToList();

        // Сохраняем новые сущности в БД
        if (newNomenclatures.Count > 0)
        {
            await _dbContext.Nomenclatures.AddRangeAsync(newNomenclatures);
            await _dbContext.SaveChangesAsync();

            // Загружаем сохраненные сущности обратно в словари
            foreach (var n in newNomenclatures)
            {
                if (!nomenclatures.ContainsKey(n.Code))
                    nomenclatures[n.Code] = n;
            }
        }

        if (newEquipments.Count > 0)
        {
            await _dbContext.Equipments.AddRangeAsync(newEquipments);
            await _dbContext.SaveChangesAsync();

            foreach (var e in newEquipments)
            {
                if (!equipments.ContainsKey(e.Code))
                    equipments[e.Code] = e;
            }
        }

        if (newDrivers.Count > 0)
        {
            await _dbContext.Set<Driver>().AddRangeAsync(newDrivers);
            await _dbContext.SaveChangesAsync();

            foreach (var d in newDrivers)
            {
                if (!molList.ContainsKey(d.FullName))
                    molList[d.FullName] = d;
            }
        }

        var newMaterials = new List<MaterialsInUse>();

        foreach (var (item, date) in itemsToProcess)
        {
            // Проверяем существование записи
            var exists = await _dbContext.Set<MaterialsInUse>().AnyAsync(mu =>
                mu.DocumentNumber == item.DocumentNumber &&
                mu.Date == date &&
                mu.Nomenclature.Code == item.NomenclatureCode &&
                mu.Equipment.Code == item.EquipmentCode &&
                mu.FinanciallyResponsiblePerson.FullName == item.FinanciallyResponsiblePersonFullName);

            if (exists)
                continue;

            newMaterials.Add(new MaterialsInUse
            {
                DocumentNumber = item.DocumentNumber,
                Date = date,
                Quantity = item.Quantity,

                Nomenclature = nomenclatures[item.NomenclatureCode],
                Equipment = equipments[item.EquipmentCode],
                FinanciallyResponsiblePerson = molList[item.FinanciallyResponsiblePersonFullName],

                MolForMoveId = 1,
                ReasonForWriteOffId = 1,
                DateForWriteOff = DateTime.MinValue
            });
        }

        if (newMaterials.Count > 0)
        {
            await _dbContext.Set<MaterialsInUse>().AddRangeAsync(newMaterials);
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
        existMaterialsInUse.DateForWriteOff = materialsInUse.ReasonForWriteOffId == 1 ? DateTime.MinValue : materialsInUse.DateForWriteOff;
        existMaterialsInUse.MolForMoveId = materialsInUse.MolForMoveId;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateMaterialsInUseAnyAsync(List<MaterialsInUse> materialsInUseAny)
    {
        var materialsInUses = materialsInUseAny.ToList();
        if (!materialsInUses.Any())
            return false;
        var filteredMaterials = materialsInUses.Where(m => m.Id != 1).ToList();
        if (filteredMaterials.Count == 0)
            return true;
        var idsToUpdate = filteredMaterials.Select(m => m.Id).ToList();
        var existingMaterials = await _dbContext.MaterialsInUse
            .Where(m => idsToUpdate.Contains(m.Id))
            .ToListAsync();

        if (existingMaterials.Count == 0)
            return false;
        foreach (var existMaterial in existingMaterials)
        {
            var updatedMaterial = filteredMaterials.First(m => m.Id == existMaterial.Id);

            existMaterial.DocumentNumber = updatedMaterial.DocumentNumber;
            existMaterial.Date = updatedMaterial.Date;
            existMaterial.Quantity = updatedMaterial.Quantity;
            existMaterial.NomenclatureId = updatedMaterial.NomenclatureId;
            existMaterial.EquipmentId = updatedMaterial.EquipmentId;
            existMaterial.IsOut = updatedMaterial.IsOut;
            existMaterial.FinanciallyResponsiblePersonId = updatedMaterial.FinanciallyResponsiblePersonId;
            existMaterial.ReasonForWriteOffId = updatedMaterial.ReasonForWriteOffId;
            existMaterial.DocumentNumberForWriteOff = updatedMaterial.DocumentNumberForWriteOff;
            existMaterial.DateForWriteOff = updatedMaterial.ReasonForWriteOffId == 1 ? DateTime.MinValue : updatedMaterial.DateForWriteOff;
        }
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