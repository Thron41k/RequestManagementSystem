using RequestManagement.Common.Models;

namespace RequestManagement.Common.Interfaces;

public interface IDriverService
{
    Task<List<Driver>> GetAllDriversAsync(string filter = "");
    Task<int> CreateDriverAsync(Driver driver);
    Task<bool> UpdateDriverAsync(Driver driver);
    Task<bool> DeleteDriverAsync(int id);
}