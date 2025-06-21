using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services;

public class DriverService(ApplicationDbContext dbContext) : IDriverService
{
    private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<List<Driver>> GetAllDriversAsync(string filter = "")
    {
        var query = _dbContext.Drivers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            query = phrases.Aggregate(query, (current, phrase) => current.Where(e => e.FullName.ToLower().Contains(phrase) || e.Position.ToLower().Contains(phrase) || e.Code.ToLower().Contains(phrase)));
        }
        query = query.Where(e => e.Id != 1);
        return await query
            .Select(e => new Driver
            {
                Id = e.Id,
                FullName = e.FullName,
                ShortName = e.ShortName,
                Position = e.Position,
                Code = e.Code
            })
            .ToListAsync();
    }

    public async Task<int> CreateDriverAsync(Driver driver)
    {
        _dbContext.Drivers.Add(driver);
        await _dbContext.SaveChangesAsync();
        return driver.Id;
    }

    public async Task<bool> UpdateDriverAsync(Driver driver)
    {
        if(driver.Id == 1)return true;
        var existDriver = await _dbContext.Drivers
            .FirstOrDefaultAsync(e => e.Id == driver.Id);

        if (existDriver == null)
            return false;

        existDriver.FullName = driver.FullName;
        existDriver.ShortName = driver.ShortName;
        existDriver.Position = driver.Position;
        existDriver.Code = driver.Code;

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDriverAsync(int id)
    {
        try
        {
            if (id == 1) return true;
            var driver = await _dbContext.Drivers
                .FirstOrDefaultAsync(e => e.Id == id);

            if (driver == null)
                return false;

            _dbContext.Drivers.Remove(driver);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}