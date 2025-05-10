using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Common.Utilities;
using RequestManagement.Server.Controllers;
using RequestManagement.Server.Data;
using System.Linq;

namespace RequestManagement.Server.Services
{
    public class ExpenseService(ApplicationDbContext dbContext) : IExpenseService
    {
        public readonly record struct NomenclatureKey(string Name, string Code, string Article, string UnitOfMeasure);

        public async Task<List<RequestManagement.Common.Models.Expense>> GetAllExpensesAsync(string filter, int requestWarehouseId, int requestEquipmentId, int requestDriverId,
            int requestDefectId, string requestFromDate, string requestToDate)
        {
            var query = dbContext.Expenses
                .Include(e => e.Stock)
                .ThenInclude(s => s.Nomenclature)
                .Include(e => e.Stock)
                .ThenInclude(s => s.Warehouse)
                .Include(e => e.Equipment)
                .Include(e => e.Driver)
                .Include(e => e.Defect)
                .ThenInclude(s => s.DefectGroup)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(e => EF.Functions.Like(e.Stock.Nomenclature.Name, $"%{filter}%") ||
                                         EF.Functions.Like(e.Stock.Nomenclature.Article, $"%{filter}%") ||
                                         EF.Functions.Like(e.Stock.Nomenclature.Code, $"%{filter}%"));
            }
            if (requestWarehouseId != 0)
            {
                query = query.Where(e => e.Stock.WarehouseId == requestWarehouseId);
            }
            if (requestEquipmentId != 0)
            {
                query = query.Where(e => e.EquipmentId == requestEquipmentId);
            }
            if (requestDriverId != 0)
            {
                query = query.Where(e => e.DriverId == requestDriverId);
            }
            if (requestDefectId != 0)
            {
                query = query.Where(e => e.DefectId == requestDefectId);
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

        public async Task<bool> UploadMaterialsExpenseAsync(List<RequestManagement.Common.Models.MaterialExpense>? materials, int warehouseId)
        {

            if (materials == null || materials.Count == 0)
                return true;

            try
            {
                var driverCodes = materials.Select(m => m.DriverCode).Distinct().ToList();
                var equipmentCodes = materials.Select(m => m.EquipmentCode).Distinct().ToList();
                var nomenclatureKeys = materials
                    .Select(m => new NomenclatureKey(m.NomenclatureName, m.NomenclatureCode, m.NomenclatureArticle, m.NomenlatureUnitOfMeasure))
                    .Distinct()
                    .ToList();

                // Загружаем данные из базы одним запросом
                var existingDrivers = await dbContext.Drivers
                    .Where(d => driverCodes.Contains(d.Code))
                    .ToDictionaryAsync(d => d.Code);

                var existingEquipments = await dbContext.Equipments
                    .Where(e => equipmentCodes.Contains(e.Code))
                    .ToDictionaryAsync(e => e.Code);

                var existingStocks = await dbContext.Stocks
                    .Include(s => s.Nomenclature)
                    .Where(s => s.WarehouseId == warehouseId)
                    .ToListAsync();

                var stockMap = existingStocks
                    .Select(s => new
                    {
                        Stock = s,
                        Key = new NomenclatureKey(s.Nomenclature.Name, s.Nomenclature.Code, s.Nomenclature.Article!, s.Nomenclature.UnitOfMeasure)
                    })
                    .Where(x => nomenclatureKeys.Contains(x.Key))
                    .ToDictionary(x => x.Key, x => x.Stock);


                var nomenclatureIds = stockMap.Values.Select(s => s.NomenclatureId).Distinct().ToList();
                var defectMappings = await dbContext.NomenclatureDefectMappings
                    .Where(m => nomenclatureIds.Contains(m.NomenclatureId))
                    .ToDictionaryAsync(m => m.NomenclatureId, m => m.DefectId);

                // Подготовка к вставке
                var newDrivers = new List<RequestManagement.Common.Models.Driver>();
                var newEquipments = new List<RequestManagement.Common.Models.Equipment>();
                var newExpenses = new List<RequestManagement.Common.Models.Expense>();

                foreach (var material in materials)
                {
                    // Драйвер
                    if (!existingDrivers.TryGetValue(material.DriverCode, out var driver))
                    {
                        driver = new RequestManagement.Common.Models.Driver
                        {
                            Code = material.DriverCode,
                            FullName = material.DriverFullName,
                            Position = "",
                            ShortName = NameFormatter.FormatToShortName(material.DriverFullName)
                        };
                        newDrivers.Add(driver);
                        existingDrivers[driver.Code] = driver; // Добавляем в словарь, чтобы переиспользовать
                    }

                    // Оборудование
                    if (!existingEquipments.TryGetValue(material.EquipmentCode, out var equipment))
                    {
                        equipment = new RequestManagement.Common.Models.Equipment
                        {
                            Code = material.EquipmentCode,
                            Name = material.EquipmentName
                        };
                        newEquipments.Add(equipment);
                        existingEquipments[equipment.Code] = equipment;
                    }

                    // Склад
                    var key = new NomenclatureKey(
                        material.NomenclatureName,
                        material.NomenclatureCode,
                        material.NomenclatureArticle,
                        material.NomenlatureUnitOfMeasure);

                    if (!stockMap.TryGetValue(key, out var stock))
                        continue;

                    var defectId = defectMappings.TryGetValue(stock.NomenclatureId, out var id) ? id : 1;

                    var expense = new RequestManagement.Common.Models.Expense
                    {
                        Code = material.Number,
                        Date = material.Date,
                        Quantity = material.Quantity,
                        StockId = stock.Id,
                        Driver = driver,
                        Equipment = equipment,
                        DefectId = defectId
                    };
                    stock.ConsumedQuantity += expense.Quantity;
                    newExpenses.Add(expense);
                }

                // Добавляем новые сущности
                await dbContext.Drivers.AddRangeAsync(newDrivers);
                await dbContext.Equipments.AddRangeAsync(newEquipments);
                await dbContext.Expenses.AddRangeAsync(newExpenses);

                await dbContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        public async Task<Common.Models.Expense> CreateExpenseAsync(Common.Models.Expense expense)
        {
            try
            {
                if (expense == null) throw new ArgumentNullException(nameof(expense));

                // Проверка наличия записи в Stock
                var stock = await dbContext.Stocks
                    .FirstOrDefaultAsync(s => s.Id == expense.StockId);

                if (stock == null)
                {
                    throw new InvalidOperationException("Stock with the given ID does not exist.");
                }

                // Обновление ConsumedQuantity
                stock.ConsumedQuantity += expense.Quantity;

                dbContext.Expenses.Add(expense);
                await dbContext.SaveChangesAsync();

                return expense;
            }
            catch
            {
                return new RequestManagement.Common.Models.Expense();
            }

        }
        public async Task<bool> UpdateExpenseAsync(Common.Models.Expense expense)
        {
            if (expense == null) throw new ArgumentNullException(nameof(expense));

            var existingExpense = await dbContext.Expenses
                .FirstOrDefaultAsync(e => e.Id == expense.Id);

            if (existingExpense == null)
            {
                return false;
            }

            // Проверка наличия записи в Stock
            var oldStock = await dbContext.Stocks
                .FirstOrDefaultAsync(s => s.Id == existingExpense.StockId);
            var newStock = await dbContext.Stocks
                .FirstOrDefaultAsync(s => s.Id == expense.StockId);

            if (oldStock == null || newStock == null)
            {
                throw new InvalidOperationException("Stock with the given ID does not exist.");
            }
            oldStock.ConsumedQuantity -= existingExpense.Quantity;
            newStock.ConsumedQuantity += expense.Quantity;

            existingExpense.StockId = expense.StockId;
            existingExpense.Quantity = expense.Quantity;
            existingExpense.EquipmentId = expense.EquipmentId;
            existingExpense.DriverId = expense.DriverId;
            existingExpense.DefectId = expense.DefectId;
            existingExpense.Date = expense.Date;

            await dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteExpenseAsync(int id)
        {
            var expense = await dbContext.Expenses
                .FirstOrDefaultAsync(e => e.Id == id);

            if (expense == null)
            {
                return false;
            }

            // Проверка наличия записи в Stock
            var stock = await dbContext.Stocks
                .FirstOrDefaultAsync(s => s.Id == expense.StockId);

            if (stock == null)
            {
                throw new InvalidOperationException("Stock with the given ID does not exist.");
            }

            // Уменьшение ConsumedQuantity
            stock.ConsumedQuantity -= expense.Quantity;

            dbContext.Expenses.Remove(expense);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<UserLastSelection?> GetUserLastSelectionAsync(int userId)
        {
            return await dbContext.UserLastSelections
                .Include(s => s.Driver)
                .Include(s => s.Equipment)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<NomenclatureDefectMapping?> GetLastNomenclatureDefectMappingAsync(int userId, int nomenclatureId)
        {
            return await dbContext.NomenclatureDefectMappings
                .Include(m => m.Nomenclature)
                .Include(m => m.Defect)
                .FirstOrDefaultAsync(m => m.UserId == userId && m.NomenclatureId == nomenclatureId);
        }

        public async Task SaveUserLastSelectionAsync(int userId, int? driverId, int? equipmentId)
        {
            var existing = await dbContext.UserLastSelections
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (existing == null)
            {
                existing = new UserLastSelection
                {
                    UserId = userId,
                    DriverId = driverId,
                    EquipmentId = equipmentId,
                    LastUpdated = DateTime.UtcNow
                };
                dbContext.UserLastSelections.Add(existing);
            }
            else
            {
                existing.DriverId = driverId;
                existing.EquipmentId = equipmentId;
                existing.LastUpdated = DateTime.UtcNow;
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task SaveNomenclatureDefectMappingAsync(int userId, int stockId, int defectId)
        {
            var stock = await dbContext.Stocks.FirstOrDefaultAsync(x => x.Id == stockId);
            if(stock == null)return;
            var existing = await dbContext.NomenclatureDefectMappings
                .FirstOrDefaultAsync(m => m.UserId == userId && m.NomenclatureId == stock.NomenclatureId);

            if (existing == null)
            {
                existing = new NomenclatureDefectMapping
                {
                    UserId = userId,
                    NomenclatureId = stock.NomenclatureId,
                    DefectId = defectId,
                    LastUsed = DateTime.UtcNow
                };
                dbContext.NomenclatureDefectMappings.Add(existing);
            }
            else
            {
                existing.DefectId = defectId;
                existing.LastUsed = DateTime.UtcNow;
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteExpensesAsync(List<int> requestIds)
        {
            var expenses = await dbContext.Expenses
                .Where(e => requestIds.Contains(e.Id))
                .Include(e => e.Stock)
                .ToListAsync();
            if (!expenses.Any())
            {
                return false;
            }

            foreach (var expense in expenses)
            {
                expense.Stock.ConsumedQuantity -= expense.Quantity;
            }
            dbContext.Expenses.RemoveRange(expenses);
            var deletedCount = await dbContext.SaveChangesAsync();
            return deletedCount > 0;
        }
    }
}
