using RequestManagement.Common.Models;

namespace RequestManagement.Common.Interfaces;

public interface IStockService
{
    Task<int> CreateStockAsync(Stock stock);
    Task<bool> UpdateStockAsync(Stock stock);
    Task<bool> DeleteStockAsync(int id);
    Task<List<Stock>> GetAllStocksAsync(int warehouseId,
        string filter = "",
        int initialQuantityFilterType = 0,
        double initialQuantityFilter = 0,
        int receivedQuantityFilterType = 0,
        double receivedQuantityFilter = 0,
        int consumedQuantityFilterType = 0,
        double consumedQuantityFilter = 0,
        int finalQuantityFilterType = 0,
        double finalQuantityFilter = 0
        );
}