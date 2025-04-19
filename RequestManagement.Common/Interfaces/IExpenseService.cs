using RequestManagement.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Interfaces
{
    public interface IExpenseService
    {
        Task<Expense> CreateExpenseAsync(Expense expense);
        Task<bool> UpdateExpenseAsync(Expense expense);
        Task<bool> DeleteExpenseAsync(int id);
        Task<UserLastSelection?> GetUserLastSelectionAsync(int userId);
        Task<NomenclatureDefectMapping?> GetLastNomenclatureDefectMappingAsync(int userId, int nomenclatureId);
        Task SaveUserLastSelectionAsync(int userId, int? driverId, int? equipmentId);
        Task SaveNomenclatureDefectMappingAsync(int userId, int nomenclatureId, int defectId);
        Task<bool> DeleteExpensesAsync(List<int> requestId);
        Task<List<Expense>> GetAllExpensesAsync(string requestFilter, int requestWarehouseId, int requestEquipmentId, int requestDriverId, int requestDefectId, string requestFromDate, string requestToDate);
    }
}
