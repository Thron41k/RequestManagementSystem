using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services
{
    public class StockService(ApplicationDbContext dbContext) : IStockService
    {
        private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        private static IQueryable<Stock> ApplyQuantityFilter(
            IQueryable<Stock> query,
            Expression<Func<Stock, decimal>> quantitySelector,
            Helpers.QuantityFilter? filter)
        {
            if (filter is null) return query;

            return filter.Operator switch
            {
                Helpers.ComparisonOperator.GreaterThan => query.Where(BuildComparisonExpression(quantitySelector, filter.Value, ">")),
                Helpers.ComparisonOperator.EqualTo => query.Where(BuildComparisonExpression(quantitySelector, filter.Value, "==")),
                Helpers.ComparisonOperator.LessThan => query.Where(BuildComparisonExpression(quantitySelector, filter.Value, "<")),
                _ => query // Невозможный случай, если enum ограничен
            };
        }
        private static Expression<Func<Stock, bool>> BuildComparisonExpression(
            Expression<Func<Stock, decimal>> quantitySelector,
            decimal value,
            string comparisonOperator)
        {
            var parameter = quantitySelector.Parameters[0];
            var constant = Expression.Constant(value, typeof(decimal));

            var comparison = comparisonOperator switch
            {
                ">" => Expression.GreaterThan(quantitySelector.Body, constant),
                "==" => Expression.Equal(quantitySelector.Body, constant),
                "<" => Expression.LessThan(quantitySelector.Body, constant),
                _ => throw new ArgumentException("Неподдерживаемый оператор сравнения")
            };

            return Expression.Lambda<Func<Stock, bool>>(comparison, parameter);
        }

        public async Task<List<Stock>> GetAllStocksAsync(
            int warehouseId,
            string filter = "",
            int initialQuantityFilterType = 0,
            double initialQuantity = 0,
            int receivedQuantityFilterType = 0,
            double receivedQuantity = 0,
            int consumedQuantityFilterType = 0,
            double consumedQuantity = 0,
            int finalQuantityFilterType = 0,
            double finalQuantity = 0
            )
        {
            var query = _dbContext.Stocks
                .AsQueryable()
                .Where(s => s.WarehouseId == warehouseId);
            if (!string.IsNullOrWhiteSpace(filter))
            {
                var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                query = phrases.Aggregate(query, (current, phrase) =>
                    current.Where(s => EF.Functions.ILike(s.Nomenclature.Name, $"%{phrase}%") ||
                                       EF.Functions.ILike(s.Nomenclature.Article, $"%{phrase}%") ||
                                       EF.Functions.ILike(s.Nomenclature.Code, $"%{phrase}%")));
            }
            query = ApplyQuantityFilter(query, s => s.InitialQuantity, Helpers.QuantityFilter.GetQuantityFilter((decimal)initialQuantity,initialQuantityFilterType));
            query = ApplyQuantityFilter(query, s => s.ReceivedQuantity, Helpers.QuantityFilter.GetQuantityFilter((decimal)receivedQuantity, receivedQuantityFilterType));
            query = ApplyQuantityFilter(query, s => s.ConsumedQuantity, Helpers.QuantityFilter.GetQuantityFilter((decimal)consumedQuantity, consumedQuantityFilterType));
            query = ApplyQuantityFilter(query, s => s.InitialQuantity + s.ReceivedQuantity - s.ConsumedQuantity, Helpers.QuantityFilter.GetQuantityFilter((decimal)finalQuantity, finalQuantityFilterType));
            return await query
                .Select(s => new Stock
                {
                    Id = s.Id,
                    NomenclatureId = s.NomenclatureId,
                    WarehouseId = s.WarehouseId,
                    InitialQuantity = s.InitialQuantity,
                    ReceivedQuantity = s.ReceivedQuantity,
                    ConsumedQuantity = s.ConsumedQuantity,
                    Nomenclature = s.Nomenclature
                })
                .ToListAsync();
        }
        public async Task<int> CreateStockAsync(Stock stock)
        {
            _dbContext.Stocks.Add(stock);
            await _dbContext.SaveChangesAsync();
            return stock.Id;
        }

        public async Task<bool> UpdateStockAsync(Stock stock)
        {
            var existingStock = await _dbContext.Stocks.FirstOrDefaultAsync(e => e.Id == stock.Id);
            if (existingStock == null)
                return false;
            existingStock.NomenclatureId = stock.NomenclatureId;
            existingStock.InitialQuantity = stock.InitialQuantity;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteStockAsync(int id)
        {
            try
            {
                var stock = await _dbContext.Stocks.FirstOrDefaultAsync(e => e.Id == id);
                if (stock == null)
                    return false;
                _dbContext.Stocks.Remove(stock);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
