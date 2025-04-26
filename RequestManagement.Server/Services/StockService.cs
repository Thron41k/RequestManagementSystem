using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;
using WpfClient.Models;
using EFCore.BulkExtensions;

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

            // Валидация входных данных MaterialStock
            foreach (var material in materials)
            {
                if (string.IsNullOrWhiteSpace(material.Code))
                {
                    throw new ArgumentException("Material Code cannot be null or empty.", nameof(material.Code));
                }
                if (string.IsNullOrWhiteSpace(material.ItemName))
                {
                    throw new ArgumentException("Material Name cannot be null or empty.", nameof(material.ItemName));
                }
                if (string.IsNullOrWhiteSpace(material.Unit))
                {
                    throw new ArgumentException("Material Unit cannot be null or empty.", nameof(material.Unit));
                }
            }

            // Валидация существования склада
            var warehouseExists = await _dbContext.Warehouses.AnyAsync(w => w.Id == warehouseId);
            if (!warehouseExists)
            {
                throw new InvalidOperationException($"Warehouse with ID {warehouseId} does not exist.");
            }

            try
            {
                // Собираем ключи материалов для фильтрации
                var materialKeys = materials.Select(m => (m.Code, m.Article, m.ItemName, m.Unit)).ToList();

                // Проверяем входные данные на дубликаты
                var duplicateMaterials = materialKeys
                    .GroupBy(k => (k.Code, k.Article ?? "", k.ItemName, k.Unit))
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();
                if (duplicateMaterials.Any())
                {
                    Console.WriteLine($"Found duplicate materials: {string.Join(", ", duplicateMaterials.Select(k => $"Code={k.Code}, Article={k.Item2}, Name={k.ItemName}, Unit={k.Unit}"))}");
                    materials = materials
                        .GroupBy(m => (m.Code, m.Article, m.ItemName, m.Unit))
                        .Select(g => new MaterialStock
                        {
                            Code = g.Key.Code,
                            Article = g.Key.Article,
                            ItemName = g.Key.ItemName,
                            Unit = g.Key.Unit,
                            FinalBalance = g.Sum(m => m.FinalBalance)
                        })
                        .ToList();
                    materialKeys = materials.Select(m => (m.Code, m.Article, m.ItemName, m.Unit)).ToList();
                }

                // Загружаем существующие номенклатуры
                var existingNomenclatures = await _dbContext.Nomenclature
                    .Where(n => materialKeys.Select(k => k.Code).Contains(n.Code))
                    .ToListAsync();

                var nomenclatureMap = existingNomenclatures
                    .Where(n => materialKeys.Any(k =>
                        k.Code == n.Code &&
                        (k.Article == n.Article || (k.Article == null && n.Article == null)) &&
                        k.ItemName == n.Name &&
                        k.Unit == n.UnitOfMeasure))
                    .ToDictionary(n => (n.Code, n.Article ?? "", n.Name, n.UnitOfMeasure), n => n);

                var newNomenclatures = new List<Nomenclature>();
                foreach (var material in materials)
                {
                    var key = (material.Code, material.Article ?? "", material.ItemName, material.Unit);
                    if (!nomenclatureMap.ContainsKey(key))
                    {
                        var nomenclature = new Nomenclature
                        {
                            Code = material.Code,
                            Article = material.Article,
                            Name = material.ItemName,
                            UnitOfMeasure = material.Unit
                        };
                        newNomenclatures.Add(nomenclature);
                    }
                }

                // Массовая вставка новых номенклатур и обновление nomenclatureMap
                if (newNomenclatures.Any())
                {
                    Console.WriteLine($"Inserting {newNomenclatures.Count} new nomenclatures");
                    await _dbContext.BulkInsertAsync(newNomenclatures);
                    // Обновляем nomenclatureMap с новыми Id
                    foreach (var nomenclature in newNomenclatures)
                    {
                        var key = (nomenclature.Code, nomenclature.Article ?? "", nomenclature.Name, nomenclature.UnitOfMeasure);
                        nomenclatureMap[key] = nomenclature;
                    }
                }

                // Предварительная загрузка существующих записей Stocks
                var existingStocks = await _dbContext.Stocks
                    .Where(s => s.WarehouseId == warehouseId)
                    .ToDictionaryAsync(s => (s.WarehouseId, s.NomenclatureId), s => s);

                // Подготовка записей Stocks
                var stocksToProcess = new List<Stock>();
                foreach (var material in materials)
                {
                    var key = (material.Code, material.Article ?? "", material.ItemName, material.Unit);
                    var nomenclature = nomenclatureMap[key];
                    var stockKey = (warehouseId, nomenclature.Id);

                    if (existingStocks.TryGetValue(stockKey, out var stock))
                    {
                        stock.InitialQuantity = (decimal)material.FinalBalance;
                        stocksToProcess.Add(stock);
                    }
                    else
                    {
                        stock = new Stock
                        {
                            WarehouseId = warehouseId,
                            NomenclatureId = nomenclature.Id,
                            InitialQuantity = (decimal)material.FinalBalance,
                            ReceivedQuantity = 0,
                            ConsumedQuantity = 0,
                            Nomenclature = nomenclature
                        };
                        stocksToProcess.Add(stock);
                    }
                }

                // Проверяем stocksToProcess на дубликаты
                var duplicateStocks = stocksToProcess
                    .GroupBy(s => (s.WarehouseId, s.NomenclatureId))
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();
                if (duplicateStocks.Any())
                {
                    Console.WriteLine($"Found duplicate stocks: {string.Join(", ", duplicateStocks.Select(k => $"WarehouseId={k.WarehouseId}, NomenclatureId={k.NomenclatureId}"))}");
                    stocksToProcess = stocksToProcess
                        .GroupBy(s => (s.WarehouseId, s.NomenclatureId))
                        .Select(g => g.Last())
                        .ToList();
                }

                // Массовая вставка или обновление записей Stocks
                Console.WriteLine($"Processing {stocksToProcess.Count} stocks for insert/update");
                await _dbContext.BulkInsertOrUpdateAsync(stocksToProcess, new BulkConfig
                {
                    UpdateByProperties = new List<string> { nameof(Stock.WarehouseId), nameof(Stock.NomenclatureId) },
                    PropertiesToInclude = new List<string> { nameof(Stock.InitialQuantity), nameof(Stock.ReceivedQuantity), nameof(Stock.ConsumedQuantity) }
                });

                // Пересчет ReceivedQuantity и ConsumedQuantity
                var stockIds = stocksToProcess.Select(s => s.Id).ToList();
                var incomingSums = await _dbContext.Incoming
                    .Where(i => stockIds.Contains(i.StockId) && i.Date >= date)
                    .GroupBy(i => i.StockId)
                    .Select(g => new { StockId = g.Key, Total = g.Sum(i => i.Quantity) })
                    .ToDictionaryAsync(x => x.StockId, x => x.Total);

                var expenseSums = await _dbContext.Expenses
                    .Where(e => stockIds.Contains(e.StockId) && e.Date >= date)
                    .GroupBy(e => e.StockId)
                    .Select(g => new { StockId = g.Key, Total = g.Sum(e => e.Quantity) })
                    .ToDictionaryAsync(x => x.StockId, x => x.Total);

                foreach (var stock in stocksToProcess)
                {
                    stock.ReceivedQuantity = incomingSums.TryGetValue(stock.Id, out var received) ? received : 0;
                    stock.ConsumedQuantity = expenseSums.TryGetValue(stock.Id, out var consumed) ? consumed : 0;
                }

                // Массовая обновление ReceivedQuantity и ConsumedQuantity
                Console.WriteLine($"Updating {stocksToProcess.Count} stocks for quantities");
                await _dbContext.BulkUpdateAsync(stocksToProcess, new BulkConfig
                {
                    UpdateByProperties = new List<string> { nameof(Stock.Id) },
                    PropertiesToInclude = new List<string> { nameof(Stock.ReceivedQuantity), nameof(Stock.ConsumedQuantity) }
                });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UploadMaterialsStockAsync: {ex}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException}");
                }
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
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
}
