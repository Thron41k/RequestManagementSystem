using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Common.Utilities;
using RequestManagement.Server.Controllers;
using RequestManagement.Server.Data;
using System.Diagnostics;
using System.Globalization;
using Driver = RequestManagement.Common.Models.Driver;
using Equipment = RequestManagement.Common.Models.Equipment;
using Incoming = RequestManagement.Common.Models.Incoming;
using Nomenclature = RequestManagement.Common.Models.Nomenclature;
using Stock = RequestManagement.Common.Models.Stock;
using Warehouse = RequestManagement.Common.Models.Warehouse;

namespace RequestManagement.Server.Services;

public class IncomingService(ApplicationDbContext dbContext) : IIncomingService
{
    public async Task<List<Incoming>> GetAllIncomingsAsync(string filter, int requestWarehouseId, string requestFromDate, string requestToDate)
    {
        var query = dbContext.Incomings
            .Include(e => e.InWarehouse)
            .ThenInclude(s => s!.FinanciallyResponsiblePerson)
            .Include(e => e.Stock)
            .ThenInclude(s => s.Nomenclature)
            .Include(e => e.Stock)
            .ThenInclude(s => s.Warehouse)
            .ThenInclude(d => d.FinanciallyResponsiblePerson)
            .Include(e => e.Application)
            .ThenInclude(s => s!.Responsible)
            .Include(e => e.Application)
            .ThenInclude(s => s!.Equipment)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter))
        {

            query = query.Where(e => EF.Functions.ILike(e.Code, $"%{filter}%") ||
                                     EF.Functions.ILike(e.Application!.Number, $"%{filter}%") ||
                                     EF.Functions.ILike(e.Stock.Nomenclature.Name, $"%{filter}%") ||
                                     EF.Functions.ILike(e.Stock.Nomenclature.Article!, $"%{filter}%") ||
                                     EF.Functions.ILike(e.Stock.Nomenclature.Code, $"%{filter}%"));
        }
        if (requestWarehouseId != 0)
        {
            query = query.Where(e => e.Stock.WarehouseId == requestWarehouseId);
        }
        if (DateTime.TryParse(requestFromDate, out var fromDate) &&
            DateTime.TryParse(requestToDate, out var toDate))
        {
            query = query.Where(e => e.Date >= fromDate && e.Date < toDate.AddDays(1));
        }
        else
        {
            if (DateTime.TryParse(requestFromDate, out fromDate))
            {
                query = query.Where(e => e.Date >= fromDate);
            }

            if (DateTime.TryParse(requestToDate, out toDate))
            {
                query = query.Where(e => e.Date < toDate.AddDays(1));
            }
        }
        return await query.ToListAsync();
    }

    public async Task<bool> UploadIncomingsAsync(MaterialIncoming incoming)
    {
        if (incoming == null || incoming.Items == null || !incoming.Items.Any())
        {
            //logger.LogWarning("Некорректный ввод: incoming равен null или не содержит элементов.");
            return false;
        }

        var stopwatch = Stopwatch.StartNew();
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var warehouse = await dbContext.Warehouses
                .FirstOrDefaultAsync(w => w.Name == incoming.WarehouseName) ?? new Warehouse
                {
                    Name = incoming.WarehouseName,
                    Code = string.Empty,
                    LastUpdated = DateTime.UtcNow
                };

            if (warehouse.Id == 0)
            {
                dbContext.Warehouses.Add(warehouse);
                await dbContext.SaveChangesAsync();
            }

            var inWarehouses = incoming.Items
                .Select(i => (i.InWarehouseName, i.InWarehouseCode))
                .Where(item => !string.IsNullOrEmpty(item.InWarehouseName) && !string.IsNullOrEmpty(item.InWarehouseCode))
                .Distinct()
                .ToHashSet();

            var incomingCodes = incoming.Items
                .Select(i => i.ReceiptOrderNumber == "" ? i.RegistratorNumber : i.ReceiptOrderNumber)
                .Where(code => !string.IsNullOrEmpty(code))
                .Distinct()
                .ToList();

            var driverNames = incoming.Items
                .Select(i => i.ApplicationResponsibleName)
                .Where(name => !string.IsNullOrEmpty(name))
                .Distinct()
                .ToHashSet();

            var equipmentCodes = incoming.Items
                .Select(i => i.ApplicationEquipmentCode)
                .Where(code => !string.IsNullOrEmpty(code))
                .Distinct()
                .ToHashSet();

            var nomenclatureKeys = incoming.Items
                .SelectMany(i => i.Items)
                .Select(m => (Code: m.Code ?? string.Empty,
                    Article: m.Article ?? string.Empty,
                    Name: m.ItemName ?? string.Empty,
                    Unit: m.Unit ?? string.Empty))
                .Distinct()
                .ToList();

            //logger.LogInformation("Подготовлены списки: Drivers={DriverCount}, Equipments={EquipmentCount}, Nomenclatures={NomenclatureCount}",
            //   driverNames.Count, equipmentCodes.Count, nomenclatureKeys.Count);

            var existingWarehouses = inWarehouses.Any()
                ? (await dbContext.Warehouses.ToListAsync())
                .Where(d => inWarehouses.Any(w => w.InWarehouseName == d.Name && w.InWarehouseCode == d.Code))
                .ToDictionary(d => d.Name)
                : new Dictionary<string, Warehouse>();

            var existingIncomings = await dbContext.Incomings
                .Where(i => incomingCodes.Contains(i.Code))
                .ToDictionaryAsync(i => (i.StockId, i.ApplicationId, i.Code));

            var existingDrivers = driverNames.Any()
                ? (await dbContext.Drivers
                    .Where(d => driverNames.Contains(d.FullName))
                    .ToListAsync())
                .GroupBy(d => d.FullName)
                .ToDictionary(g => g.Key, g => g.First())
                : new Dictionary<string, Driver>();

            var existingEquipments = equipmentCodes.Any()
                ? await dbContext.Equipments
                    .Where(e => equipmentCodes.Contains(e.Code))
                    .ToDictionaryAsync(e => e.Code)
                : new Dictionary<string, Equipment>();

            var allNomenclatures = await dbContext.Nomenclatures.ToListAsync();

            var existingNomenclatures = allNomenclatures
                .Where(n => nomenclatureKeys.Any(k =>
                    k.Code == n.Code &&
                    k.Article == (n.Article ?? string.Empty) &&
                    k.Name == n.Name &&
                    k.Unit == n.UnitOfMeasure))
                .ToDictionary(n => (
                    Code: n.Code,
                    Article: n.Article ?? string.Empty,
                    Name: n.Name,
                    Unit: n.UnitOfMeasure
                ));

            var newWarehouses = new List<Warehouse>();
            foreach (var item in inWarehouses)
            {
                if (!existingWarehouses.ContainsKey(item.InWarehouseName))
                {
                    var newWarehouse = new Warehouse
                    {
                        Name = item.InWarehouseName,
                        Code = item.InWarehouseCode,
                        LastUpdated = DateTime.UtcNow
                    };
                    newWarehouses.Add(newWarehouse);
                    existingWarehouses[item.InWarehouseName] = newWarehouse;
                }
            }
            if (newWarehouses.Any())
            {
                await dbContext.Warehouses.AddRangeAsync(newWarehouses);
                await dbContext.SaveChangesAsync();
            }

            // 3. Создаем новые Drivers и сохраняем их
            var newDrivers = new List<Driver>();
            foreach (var name in driverNames)
            {
                if (!existingDrivers.ContainsKey(name))
                {
                    var driver = new Driver
                    {
                        Code = string.Empty,
                        FullName = name,
                        ShortName = NameFormatter.FormatToShortName(name),
                        Position = string.Empty
                    };
                    newDrivers.Add(driver);
                    existingDrivers[name] = driver;
                }
            }
            if (newDrivers.Any())
            {
                await dbContext.Drivers.AddRangeAsync(newDrivers);
                await dbContext.SaveChangesAsync();
            }

            // 4. Создаем новые Equipments и сохраняем их
            var newEquipments = new List<Equipment>();
            foreach (var code in equipmentCodes)
            {
                if (!existingEquipments.ContainsKey(code))
                {
                    var equipment = new Equipment
                    {
                        Code = code,
                        Name = incoming.Items.FirstOrDefault(i => i.ApplicationEquipmentCode == code)?.ApplicationEquipmentName ?? string.Empty,
                        StateNumber = string.Empty
                    };
                    newEquipments.Add(equipment);
                    existingEquipments[code] = equipment;
                }
            }
            if (newEquipments.Any())
            {
                await dbContext.Equipments.AddRangeAsync(newEquipments);
                await dbContext.SaveChangesAsync();
            }

            // 5. Создаем новые Nomenclatures и сохраняем их
            var newNomenclatures = new List<Nomenclature>();
            foreach (var key in nomenclatureKeys)
            {
                if (!existingNomenclatures.ContainsKey(key))
                {
                    var nomenclature = new Nomenclature
                    {
                        Code = key.Code,
                        Name = key.Name,
                        Article = key.Article,
                        UnitOfMeasure = key.Unit
                    };
                    newNomenclatures.Add(nomenclature);
                    existingNomenclatures[key] = nomenclature;
                }
            }
            if (newNomenclatures.Any())
            {
                await dbContext.Nomenclatures.AddRangeAsync(newNomenclatures);
                await dbContext.SaveChangesAsync();
            }

            // 6. Подготавливаем Applications
            var applicationKeys = incoming.Items
                .Where(i => !string.IsNullOrEmpty(i.ApplicationNumber) && !string.IsNullOrEmpty(i.ApplicationDate))
                .Select(i =>
                {
                    if (DateTime.TryParse(i.ApplicationDate, out var date))
                    {
                        return (Number: i.ApplicationNumber, Date: date.Date);
                    }
                    return (Number: i.ApplicationNumber, Date: (DateTime?)null);
                })
                .Where(a => a.Date.HasValue)
                .Distinct()
                .ToList();
            var existing = applicationKeys.Any() ? dbContext.Applications
                .Where(a => applicationKeys.Select(k => k.Number).Contains(a.Number)).ToList() : [];
            existing = applicationKeys.Any() && existing.Any() ? existing
                .Where(a => applicationKeys.Select(k => k.Date.Value).Contains(a.Date.Date)).ToList() : [];
            var existingApplications = existing.DistinctBy(x => x.Number).DistinctBy(x => x.Date.Date).ToDictionary(a => (a.Number, a.Date.Date));

            var newApplications = new List<Application>();
            foreach (var item in incoming.Items)
            {
                if (string.IsNullOrEmpty(item.ApplicationNumber) || string.IsNullOrEmpty(item.ApplicationDate) ||
                    !DateTime.TryParse(item.ApplicationDate, out var applicationDate))
                {
                    continue;
                }

                var appKey = (item.ApplicationNumber, applicationDate);
                if (!existingApplications.ContainsKey(appKey))
                {
                    var responsibleId = 1;
                    if (!string.IsNullOrEmpty(item.ApplicationResponsibleName) &&
                        existingDrivers.TryGetValue(item.ApplicationResponsibleName, out var responsible))
                    {
                        responsibleId = responsible.Id;
                    }

                    var equipmentId = 1;
                    if (!string.IsNullOrEmpty(item.ApplicationEquipmentCode) &&
                        existingEquipments.TryGetValue(item.ApplicationEquipmentCode, out var equipment))
                    {
                        equipmentId = equipment.Id;
                    }

                    var application = new Application
                    {
                        Number = item.ApplicationNumber,
                        Date = applicationDate,
                        ResponsibleId = responsibleId,
                        EquipmentId = equipmentId
                    };
                    newApplications.Add(application);
                    existingApplications[appKey] = application;
                }
            }
            if (newApplications.Any())
            {
                await dbContext.Applications.AddRangeAsync(newApplications);
                await dbContext.SaveChangesAsync();
            }

            // 7. Создаем Stocks и сохраняем их
            var existingStocks = await dbContext.Stocks
                .Where(s => s.WarehouseId == warehouse.Id)
                .ToDictionaryAsync(s => (s.NomenclatureId, s.WarehouseId), s => s);

            var newStocks = new List<Stock>();
            var stockMappings = new Dictionary<(int NomenclatureId, int WarehouseId), Stock>();

            foreach (var item in incoming.Items)
            {
                foreach (var material in item.Items)
                {
                    var nomenclatureKey = (material.Code, material.Article, material.ItemName, material.Unit);
                    if (!existingNomenclatures.TryGetValue(nomenclatureKey, out var nomenclature))
                    {
                        //logger.LogWarning("Номенклатура с ключом {NomenclatureKey} не найдена после сохранения.", nomenclatureKey);
                        continue;
                    }

                    var stockKey = (nomenclature.Id, warehouse.Id);
                    if (!existingStocks.TryGetValue(stockKey, out var stock) && !stockMappings.TryGetValue(stockKey, out stock))
                    {
                        stock = new Stock
                        {
                            WarehouseId = warehouse.Id,
                            NomenclatureId = nomenclature.Id,
                            InitialQuantity = 0,
                            ReceivedQuantity = 0,
                            ConsumedQuantity = 0
                        };
                        newStocks.Add(stock);
                        stockMappings[stockKey] = stock;
                    }
                }
            }

            if (newStocks.Any())
            {
                await dbContext.Stocks.AddRangeAsync(newStocks);
                await dbContext.SaveChangesAsync();
                // Обновляем existingStocks после сохранения
                foreach (var stock in newStocks)
                {
                    var stockKey = (stock.NomenclatureId, stock.WarehouseId);
                    existingStocks[stockKey] = stock;
                }
            }

            // 8. Создаем Incomings
            var newIncomings = new List<Incoming>();
            var incomingsToUpdate = new List<Incoming>();
            foreach (var item in incoming.Items)
            {
                Application? application = null;
                if (!string.IsNullOrEmpty(item.ApplicationNumber) && !string.IsNullOrEmpty(item.ApplicationDate) &&
                    DateTime.TryParse(item.ApplicationDate, out var applicationDate))
                {
                    var appKey = (item.ApplicationNumber, applicationDate);
                    existingApplications.TryGetValue(appKey, out application);
                }

                existingWarehouses.TryGetValue(item.InWarehouseName, out var currentWarehouse);
                foreach (var material in item.Items)
                {
                    var nomenclatureKey = (material.Code, material.Article, material.ItemName, material.Unit);
                    if (!existingNomenclatures.TryGetValue(nomenclatureKey, out var nomenclature))
                    {
                        //logger.LogWarning("Номенклатура с ключом {NomenclatureKey} не найдена.", nomenclatureKey);
                        continue;
                    }

                    var stockKey = (nomenclature.Id, warehouse.Id);
                    if (!existingStocks.TryGetValue(stockKey, out var stock))
                    {
                        //logger.LogWarning("Запас с ключом {StockKey} не найден для номенклатуры {NomenclatureId}.", stockKey, nomenclature.Id);
                        continue;
                    }
                    var incomingCode = item.RegistratorNumber ?? $"AUTO_{Guid.NewGuid().ToString("N")[..8]}";
                    var incomingDate = DateTime.TryParse(item.RegistratorDate, out var registratorDate)
                        ? registratorDate
                        : DateTime.UtcNow;
                    var incomingType = item.RegistratorType;
                    var incomingKey = (stock.Id, application?.Id ?? 1, incomingCode);
                    if (existingIncomings.TryGetValue(incomingKey, out var existingIncoming))
                    {
                        var quantityDifference = (decimal)material.FinalBalance - existingIncoming.Quantity;
                        existingIncoming.Quantity = (decimal)material.FinalBalance;
                        existingIncoming.Date = incomingDate;
                        existingIncoming.DocType = incomingType;
                        existingIncoming.InWarehouseId = currentWarehouse?.Id ?? null;
                        stock.ReceivedQuantity += quantityDifference;
                        incomingsToUpdate.Add(existingIncoming);
                        continue;
                    }
                    var incomingEntry = new Incoming
                    {
                        StockId = stock.Id,
                        Quantity = (decimal)material.FinalBalance,
                        Date = incomingDate,
                        Code = incomingCode,
                        DocType = incomingType,
                        ApplicationId = application?.Id ?? 1,
                        InWarehouseId = currentWarehouse?.Id ?? null
                    }
                ;
                    stock.ReceivedQuantity += incomingEntry.Quantity;
                    newIncomings.Add(incomingEntry);
                    existingIncomings[incomingKey] = incomingEntry;
                }
            }

            if (newIncomings.Any())
            {
                await dbContext.Incomings.AddRangeAsync(newIncomings);
            }

            if (incomingsToUpdate.Any())
            {
                dbContext.Incomings.UpdateRange(incomingsToUpdate);
            }

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            stopwatch.Stop();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine(ex.Message);
            //logger.LogError(ex, "Ошибка в UploadIncomingsAsync для склада {WarehouseName}", incoming?.WarehouseName);
            return false;
        }
    }

    private async Task<Nomenclature> GetOrAddNomenclatureAsync(
        string code,
        string? article,
        string name,
        string unit,
        Dictionary<(string Code, string Article, string Name, string Unit), Nomenclature> cache,
        List<Nomenclature> newNomenclatures)
    {
        var key = (code.Trim(), article?.Trim() ?? "", name.Trim(), unit.Trim());

        // Проверка в локальном кэше
        if (cache.TryGetValue(key, out var existing))
            return existing;

        // Проверка в БД
        var dbNomenclature = await dbContext.Nomenclatures
            .FirstOrDefaultAsync(n =>
                n.Code == key.Item1 &&
                (n.Article ?? "") == key.Item2 &&
                n.Name == key.Item3 &&
                n.UnitOfMeasure == key.Item4);

        if (dbNomenclature != null)
        {
            cache[key] = dbNomenclature;
            return dbNomenclature;
        }

        // Проверка среди новых, еще не добавленных
        var inMemory = newNomenclatures.FirstOrDefault(n =>
            n.Code == key.Item1 &&
            (n.Article ?? "") == key.Item2 &&
            n.Name == key.Item3 &&
            n.UnitOfMeasure == key.Item4);

        if (inMemory != null)
        {
            cache[key] = inMemory;
            return inMemory;
        }

        // Создание новой номенклатуры
        var newNomenclature = new Nomenclature
        {
            Code = key.Item1,
            Article = string.IsNullOrWhiteSpace(key.Item2) ? null : key.Item2,
            Name = key.Item3,
            UnitOfMeasure = key.Item4
        };

        newNomenclatures.Add(newNomenclature);
        cache[key] = newNomenclature;

        return newNomenclature;
    }


    public async Task<Incoming> CreateIncomingAsync(Incoming incoming)
    {
        try
        {
            if (incoming == null) throw new ArgumentNullException(nameof(incoming));

            // Проверка наличия записи в Stock
            var stock = await dbContext.Stocks
                .FirstOrDefaultAsync(s => s.Id == incoming.StockId);

            if (stock == null)
            {
                throw new InvalidOperationException("Stock with the given ID does not exist.");
            }

            // Обновление ReceivedQuantity
            stock.ReceivedQuantity += incoming.Quantity;

            dbContext.Incomings.Add(incoming);
            await dbContext.SaveChangesAsync();

            return incoming;
        }
        catch
        {
            return new Incoming();
        }

    }
    public async Task<bool> UpdateIncomingAsync(Incoming incoming)
    {
        if (incoming == null) throw new ArgumentNullException(nameof(incoming));

        var existingIncoming = await dbContext.Incomings
            .FirstOrDefaultAsync(e => e.Id == incoming.Id);

        if (existingIncoming == null)
        {
            return false;
        }

        // Проверка наличия записи в Stock
        var oldStock = await dbContext.Stocks
            .FirstOrDefaultAsync(s => s.Id == existingIncoming.StockId);
        var newStock = await dbContext.Stocks
            .FirstOrDefaultAsync(s => s.Id == incoming.StockId);

        if (oldStock == null || newStock == null)
        {
            throw new InvalidOperationException("Stock with the given ID does not exist.");
        }
        oldStock.ReceivedQuantity -= existingIncoming.Quantity;
        newStock.ReceivedQuantity += incoming.Quantity;
        existingIncoming.InWarehouseId = incoming.InWarehouseId;
        existingIncoming.StockId = incoming.StockId;
        existingIncoming.Quantity = incoming.Quantity;
        existingIncoming.Date = incoming.Date;

        await dbContext.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteIncomingAsync(int id)
    {
        var incoming = await dbContext.Incomings
            .FirstOrDefaultAsync(e => e.Id == id);

        if (incoming == null)
        {
            return false;
        }

        // Проверка наличия записи в Stock
        var stock = await dbContext.Stocks
            .FirstOrDefaultAsync(s => s.Id == incoming.StockId);

        if (stock == null)
        {
            throw new InvalidOperationException("Stock with the given ID does not exist.");
        }

        // Уменьшение ReceivedQuantity
        stock.ReceivedQuantity -= incoming.Quantity;

        dbContext.Incomings.Remove(incoming);
        await dbContext.SaveChangesAsync();
        return true;
    }



    public async Task<bool> DeleteIncomingsAsync(List<int> requestIds)
    {
        var incomings = await dbContext.Incomings
            .Where(e => requestIds.Contains(e.Id))
            .Include(e => e.Stock)
            .ToListAsync();
        if (!incomings.Any())
        {
            return false;
        }

        foreach (var incoming in incomings)
        {
            incoming.Stock.ReceivedQuantity -= incoming.Quantity;
        }
        dbContext.Incomings.RemoveRange(incomings);
        var deletedCount = await dbContext.SaveChangesAsync();
        return deletedCount > 0;
    }

    public async Task<Incoming> FindIncomingByIdAsync(int id)
    {
        var query = dbContext.Incomings
            .Include(e => e.InWarehouse)
            .ThenInclude(s => s!.FinanciallyResponsiblePerson)
            .Include(e => e.Stock)
            .ThenInclude(s => s.Nomenclature)
            .Include(e => e.Stock)
            .ThenInclude(s => s.Warehouse)
            .ThenInclude(d => d.FinanciallyResponsiblePerson)
            .Include(e => e.Application)
            .ThenInclude(s => s!.Responsible)
            .Include(e => e.Application)
            .ThenInclude(s => s!.Equipment)
            .AsQueryable();
        return await query.FirstOrDefaultAsync(e => e.Id == id) ?? new Incoming();
    }
}