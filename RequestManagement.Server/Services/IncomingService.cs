using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Common.Utilities;
using RequestManagement.Server.Data;
using System.Diagnostics;
using System.Globalization;

namespace RequestManagement.Server.Services
{
    public class IncomingService(ApplicationDbContext dbContext) : IIncomingService
    {
        public async Task<List<Incoming>> GetAllIncomingsAsync(string filter, int requestWarehouseId, string requestFromDate, string requestToDate)
        {
            var query = dbContext.Incomings
                .Include(e => e.Stock)
                .ThenInclude(s => s.Nomenclature)
                .Include(e => e.Stock)
                .ThenInclude(s => s.Warehouse)
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
                    .Select(m => (m.Code, m.Article, m.ItemName, m.Unit))
                    .Distinct()
                    .ToList();

                var codes = nomenclatureKeys.Select(k => k.Code).ToHashSet();
                var articles = nomenclatureKeys.Select(k => k.Article).ToHashSet();
                var names = nomenclatureKeys.Select(k => k.ItemName).ToHashSet();
                var units = nomenclatureKeys.Select(k => k.Unit).ToHashSet();

                //logger.LogInformation("Подготовлены списки: Drivers={DriverCount}, Equipments={EquipmentCount}, Nomenclatures={NomenclatureCount}",
                //   driverNames.Count, equipmentCodes.Count, nomenclatureKeys.Count);

                var existingIncomings = await dbContext.Incomings
                    .Where(i => incomingCodes.Contains(i.Code))
                    .ToDictionaryAsync(i => (i.StockId, i.ApplicationId, i.Code));

                var existingDrivers = driverNames.Any()
                    ? await dbContext.Drivers
                        .Where(d => driverNames.Contains(d.FullName))
                        .ToDictionaryAsync(d => d.FullName)
                    : new Dictionary<string, Driver>();

                var existingEquipments = equipmentCodes.Any()
                    ? await dbContext.Equipments
                        .Where(e => equipmentCodes.Contains(e.Code))
                        .ToDictionaryAsync(e => e.Code)
                    : new Dictionary<string, Equipment>();

                Dictionary<(string, string, string, string), Nomenclature> existingNomenclatures;
                if (!codes.Any() || !names.Any() || !units.Any())
                {
                    //logger.LogWarning("Один из списков (codes, names, units) пуст. Пропускаем загрузку Nomenclatures.");
                    existingNomenclatures = new Dictionary<(string, string, string, string), Nomenclature>();
                }
                else
                {
                    const int batchSize = 1000;
                    var existingNomenclaturesList = new List<Nomenclature>();
                    for (int i = 0; i < codes.Count; i += batchSize)
                    {
                        var batchCodes = codes.Skip(i).Take(batchSize).ToList();
                        var batchArticles = articles.Skip(i).Take(batchSize).ToList();
                        var batchNames = names.Skip(i).Take(batchSize).ToList();
                        var batchUnits = units.Skip(i).Take(batchSize).ToList();

                        var batchNomenclatures = await dbContext.Nomenclatures
                            .Where(n => batchCodes.Contains(n.Code) &&
                                        (n.Article == null ? batchArticles.Contains(null) : batchArticles.Contains(n.Article)) &&
                                        batchNames.Contains(n.Name) &&
                                        batchUnits.Contains(n.UnitOfMeasure))
                            .ToListAsync();

                        existingNomenclaturesList.AddRange(batchNomenclatures);
                    }
                    existingNomenclatures = existingNomenclaturesList
                        .ToDictionary(n => (n.Code, n.Article ?? string.Empty, n.Name, n.UnitOfMeasure), n => n);
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
                            Name = key.ItemName,
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
                    .Select(i => {
                        if (DateTime.TryParse(i.ApplicationDate, out var date))
                        {
                            // Берем только дату (без времени)
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
                        int responsibleId = 1;
                        if (!string.IsNullOrEmpty(item.ApplicationResponsibleName) &&
                            existingDrivers.TryGetValue(item.ApplicationResponsibleName, out var responsible))
                        {
                            responsibleId = responsible.Id;
                        }

                        int equipmentId = 1;
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

                        string incomingCode;
                        DateTime incomingDate;
                        string incomingType;
                        if (!string.IsNullOrEmpty(item.ReceiptOrderNumber) &&
                            !string.IsNullOrEmpty(item.ReceiptOrderDate) &&
                            DateTime.TryParse(item.ReceiptOrderDate, out var receiptOrderDate))
                        {
                            incomingCode = item.ReceiptOrderNumber;
                            incomingDate = receiptOrderDate;
                            incomingType = "Приходный ордер";
                        }
                        else
                        {
                            incomingCode = item.RegistratorNumber ?? $"AUTO_{Guid.NewGuid().ToString("N")[..8]}";
                            incomingDate = DateTime.TryParse(item.RegistratorDate, out var registratorDate)
                                ? registratorDate
                                : DateTime.UtcNow;
                            incomingType = item.RegistratorType;
                        }

                        if (incomingCode == "БПТР0006356") 
                            Console.WriteLine("БПТР0006356");
                        var incomingKey = (stock.Id, application?.Id ?? 1, incomingCode);
                        if (existingIncomings.TryGetValue(incomingKey, out var existingIncoming))
                        {
                            // Обновляем существующую запись, если количество изменилось
                            if (existingIncoming.Quantity != (decimal)material.FinalBalance)
                            {
                                var quantityDifference = (decimal)material.FinalBalance - existingIncoming.Quantity;
                                existingIncoming.Quantity = (decimal)material.FinalBalance;
                                existingIncoming.Date = incomingDate;
                                existingIncoming.DocType = incomingType;
                                stock.ReceivedQuantity += quantityDifference;
                                incomingsToUpdate.Add(existingIncoming);
                            }
                            continue;
                        }
                        var incomingEntry = new Incoming
                        {
                            StockId = stock.Id,
                            Quantity = (decimal)material.FinalBalance,
                            Date = incomingDate,
                            Code = incomingCode,
                            DocType = incomingType,
                            ApplicationId = application?.Id ?? 1
                        };
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
    }
}
