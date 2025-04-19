using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services
{
    public class ExpenseService(ApplicationDbContext dbContext) : IExpenseService
    {
        public async Task<List<Expense>> GetAllExpensesAsync(string filter, int requestWarehouseId, int requestEquipmentId, int requestDriverId,
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
        public async Task<Expense> CreateExpenseAsync(Expense expense)
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
                return new Expense();
            }

        }
        public async Task<bool> UpdateExpenseAsync(Expense expense)
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

            // Корректировка ConsumedQuantity
            oldStock.ConsumedQuantity -= existingExpense.Quantity; // Убираем старое значение
            newStock.ConsumedQuantity += expense.Quantity; // Добавляем новое значение

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

        public async Task SaveNomenclatureDefectMappingAsync(int userId, int nomenclatureId, int defectId)
        {
            var existing = await dbContext.NomenclatureDefectMappings
                .FirstOrDefaultAsync(m => m.UserId == userId && m.NomenclatureId == nomenclatureId);

            if (existing == null)
            {
                existing = new NomenclatureDefectMapping
                {
                    UserId = userId,
                    NomenclatureId = nomenclatureId,
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
