using RequestManagement.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Interfaces
{
    public interface IWarehouseService
    {
        Task<List<Warehouse>> GetAllWarehousesAsync(string filter = "");
        Task<int> CreateWarehouseAsync(Warehouse warehouse);
        Task<bool> UpdateWarehouseAsync(Warehouse warehouse);
        Task<bool> DeleteWarehouseAsync(int id);
    }
}
