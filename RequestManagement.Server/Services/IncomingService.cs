using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services
{
    public class IncomingService(ApplicationDbContext dbContext) : IIncomingService
    {
        public async Task<List<Incoming>> GetAllIncomingsAsync(string filter, int requestWarehouseId,  string requestFromDate, string requestToDate)
        {
            var query = dbContext.Incoming
                .Include(e => e.Stock)
                .ThenInclude(s => s.Nomenclature)
                .Include(e => e.Stock)
                .ThenInclude(s => s.Warehouse)
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

                dbContext.Incoming.Add(incoming);
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

            var existingIncoming = await dbContext.Incoming
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

            // Корректировка ReceivedQuantity
            oldStock.ReceivedQuantity -= existingIncoming.Quantity; // Убираем старое значение
            newStock.ReceivedQuantity += incoming.Quantity; // Добавляем новое значение

            existingIncoming.StockId = incoming.StockId;
            existingIncoming.Quantity = incoming.Quantity;
            existingIncoming.Date = incoming.Date;

            await dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteIncomingAsync(int id)
        {
            var incoming = await dbContext.Incoming
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

            dbContext.Incoming.Remove(incoming);
            await dbContext.SaveChangesAsync();
            return true;
        }

     

        public async Task<bool> DeleteIncomingsAsync(List<int> requestIds)
        {
            var incomings = await dbContext.Incoming
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
            dbContext.Incoming.RemoveRange(incomings);
            var deletedCount = await dbContext.SaveChangesAsync();
            return deletedCount > 0;
        }
    }
}
