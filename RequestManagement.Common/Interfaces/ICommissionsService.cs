using RequestManagement.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Interfaces
{
    public interface ICommissionsService
    {
        Task<List<Commissions>> GetAllCommissionsAsync(string filter = "");
        Task<int> CreateCommissionsAsync(Commissions warehouse);
        Task<bool> UpdateCommissionsAsync(Commissions warehouse);
        Task<bool> DeleteCommissionsAsync(int id);
    }
}
