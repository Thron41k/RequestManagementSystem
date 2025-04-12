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
        Task<List<Expense>> GetAllExpensesAsync(string filter = "");
        Task<int> CreateExpenseAsync(Expense expense);
        Task<bool> UpdateExpenseAsync(Expense expense);
        Task<bool> DeleteExpenseAsync(int id);
    }
}
