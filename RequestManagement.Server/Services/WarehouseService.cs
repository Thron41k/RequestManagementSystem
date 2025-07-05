using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services;

public class WarehouseService(ApplicationDbContext dbContext) : IWarehouseService
{
    private readonly ApplicationDbContext _dbContext =
        dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<List<Warehouse>> GetAllWarehousesAsync(string filter = "")
    {
        var query = _dbContext.Warehouses.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            query = phrases.Aggregate(query, (current, phrase) => current.Where(e => e.Name.ToLower().Contains(phrase)));
        }
        return await query
            .Select(e => new Warehouse
            {
                Id = e.Id,
                Name = e.Name,
                LastUpdated = e.LastUpdated,
                Code = e.Code,
                FinanciallyResponsiblePerson = e.FinanciallyResponsiblePerson,
                FinanciallyResponsiblePersonId = e.FinanciallyResponsiblePersonId
            })
            .ToListAsync();
    }

    public async Task<Warehouse> GetOrCreateWarehousesAsync(string filter)
    {
        var query = _dbContext.Warehouses.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            var normalizedFilter = filter.Trim().ToLower();
            query = query.Where(e => e.Name.ToLower() == normalizedFilter);
        }
        var result = await query
            .Select(e => new Warehouse
            {
                Id = e.Id,
                Name = e.Name,
                LastUpdated = e.LastUpdated,
                Code = e.Code,
                FinanciallyResponsiblePersonId = e.FinanciallyResponsiblePersonId
            })
            .ToListAsync();
        if (result.Count != 0) return result[0];
        var newWarehouse = new Warehouse { Name = filter, LastUpdated = DateTime.Now };
        await _dbContext.Warehouses.AddAsync(newWarehouse);
        await _dbContext.SaveChangesAsync();
        return newWarehouse;
    }

    public async Task<int> CreateWarehouseAsync(Warehouse warehouse)
    {
        _dbContext.Warehouses.Add(warehouse);
        await _dbContext.SaveChangesAsync();
        return warehouse.Id;
    }

    public async Task<bool> UpdateWarehouseAsync(Warehouse warehouse)
    {
        var existWarehouse = await _dbContext.Warehouses
            .FirstOrDefaultAsync(e => e.Id == warehouse.Id);

        if (existWarehouse == null)
            return false;

        existWarehouse.Name = warehouse.Name;
        existWarehouse.Code = warehouse.Code;
        existWarehouse.LastUpdated = warehouse.LastUpdated;
        existWarehouse.FinanciallyResponsiblePersonId = warehouse.FinanciallyResponsiblePersonId == 0 ? null : warehouse.FinanciallyResponsiblePersonId;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteWarehouseAsync(int id)
    {
        try
        {
            var warehouse = await _dbContext.Warehouses
                .FirstOrDefaultAsync(e => e.Id == id);

            if (warehouse == null)
                return false;

            _dbContext.Warehouses.Remove(warehouse);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}