using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;
using WpfClient.Models;
using EFCore.BulkExtensions;
using Windows.UI;

namespace RequestManagement.Server.Services;

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
        query = ApplyQuantityFilter(query, s => s.InitialQuantity, Helpers.QuantityFilter.GetQuantityFilter((decimal)initialQuantity, initialQuantityFilterType));
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

    public async Task<bool> UploadMaterialsStockAsync(List<MaterialStock> materials, int warehouseId, DateTime date)
    {
        if (materials == null || !materials.Any())
        {
            return false;
        }

        try
        {
            // Получаем существующие номенклатуры и склады
            var existingNomenclatures = await _dbContext.Nomenclatures
                .Where(n => materials.Select(m => m.Code).Contains(n.Code) &&
                            materials.Select(m => m.Article).Contains(n.Article) &&
                            materials.Select(m => m.ItemName).Contains(n.Name) &&
                            materials.Select(m => m.Unit).Contains(n.UnitOfMeasure))
                .ToDictionaryAsync(n => (n.Code, n.Article, n.Name, n.UnitOfMeasure), n => n.Id);

            var warehouseExists = await _dbContext.Warehouses.AnyAsync(w => w.Id == warehouseId);
            if (!warehouseExists)
            {
                return false;
            }

            // Получаем существующие записи Stock для указанного склада
            var existingStocks = await _dbContext.Stocks
                .Include(s => s.Nomenclature)
                .Where(s => s.WarehouseId == warehouseId)
                .ToDictionaryAsync(s => (s.NomenclatureId, s.WarehouseId), s => s);

            // Подготавливаем данные для обновления или создания
            var stocksToAdd = new List<Stock>();
            var stocksToUpdate = new List<Stock>();

            foreach (var material in materials)
            {
                // Проверяем, существует ли номенклатура
                var nomenclatureKey = (material.Code, material.Article, material.ItemName, material.Unit);
                if (!existingNomenclatures.TryGetValue(nomenclatureKey, out var nomenclatureId))
                {
                    // Создаем новую номенклатуру, если не существует
                    var newNomenclature = new Nomenclature
                    {
                        Code = material.Code,
                        Name = material.ItemName,
                        Article = material.Article,
                        UnitOfMeasure = material.Unit
                    };
                    _dbContext.Nomenclatures.Add(newNomenclature);
                    await _dbContext.SaveChangesAsync();
                    nomenclatureId = newNomenclature.Id;
                    existingNomenclatures[nomenclatureKey] = nomenclatureId;
                }

                var stockKey = (nomenclatureId, warehouseId);
                if (existingStocks.TryGetValue(stockKey, out var existingStock))
                {
                    // Обновляем существующую запись
                    existingStock.InitialQuantity = (decimal)material.FinalBalance;

                    // Пересчитываем ReceivedQuantity и ConsumedQuantity
                    var receivedQuantity = await _dbContext.Incomings
                        .Where(i => i.StockId == existingStock.Id && i.Date >= date)
                        .SumAsync(i => i.Quantity);

                    var consumedQuantity = await _dbContext.Expenses
                        .Where(e => e.StockId == existingStock.Id && e.Date >= date)
                        .SumAsync(e => e.Quantity);

                    existingStock.ReceivedQuantity = receivedQuantity;
                    existingStock.ConsumedQuantity = consumedQuantity;

                    stocksToUpdate.Add(existingStock);
                }
                else
                {
                    // Создаем новую запись
                    var newStock = new Stock
                    {
                        WarehouseId = warehouseId,
                        NomenclatureId = nomenclatureId,
                        InitialQuantity = (decimal)material.FinalBalance,
                        ReceivedQuantity = 0,
                        ConsumedQuantity = 0
                    };
                    stocksToAdd.Add(newStock);
                }
            }

            // Пакетное добавление новых записей
            if (stocksToAdd.Any())
            {
                await _dbContext.Stocks.AddRangeAsync(stocksToAdd);
            }

            // Пакетное обновление существующих записей
            if (stocksToUpdate.Any())
            {
                _dbContext.Stocks.UpdateRange(stocksToUpdate);
            }

            // Сохраняем изменения
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            // Логирование ошибки (предполагается, что используется ILogger)
            return false;
        }
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