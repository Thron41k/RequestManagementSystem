using RequestManagement.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Interfaces
{
    public interface IDriverService
    {
        Task<List<Driver>> GetAllDriversAsync(string filter = "");
        Task<int> CreateDriverAsync(Driver driver);
        Task<bool> UpdateDriverAsync(Driver driver);
        Task<bool> DeleteDriverAsync(int id);
    }
}
