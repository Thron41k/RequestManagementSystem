using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services
{
    public class ExpenseService(ApplicationDbContext dbContext) : IExpenseService
    {
        private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        public async Task<List<Expense>> GetAllExpensesAsync(string filter = "")
        {
            var query = dbContext.Expenses
                .Include(e => e.Stock)
                .ThenInclude(s => s.Nomenclature)
                .Include(e => e.Stock)
                .ThenInclude(s => s.Warehouse)
                .Include(e => e.Equipment)
                .Include(e => e.Driver)
                .Include(e => e.Defect)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(e =>
                    e.Stock.Nomenclature.Name.Contains(filter) ||
                    e.Stock.Warehouse.Name.Contains(filter) ||
                    e.Equipment.Name.Contains(filter) ||
                    e.Driver.FullName.Contains(filter) ||
                    e.Defect.Name.Contains(filter));
            }

            return await query.ToListAsync();
        }
        public async Task<int> CreateExpenseAsync(Expense expense)
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

            return expense.Id;
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
    }
}
