using RequestManagement.Common.Models;

namespace RequestManagement.Common.Interfaces;

public interface IWarehouseService
{
    Task<List<Warehouse>> GetAllWarehousesAsync(string filter = "");
    Task<Warehouse> GetOrCreateWarehousesAsync(string filter);
    Task<int> CreateWarehouseAsync(Warehouse warehouse);
    Task<bool> UpdateWarehouseAsync(Warehouse warehouse);
    Task<bool> DeleteWarehouseAsync(int id);
}