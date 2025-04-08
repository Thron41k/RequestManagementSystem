using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace RequestManagement.Server.Services
{
    public class RequestService : IRequestService
    {
        private readonly ApplicationDbContext _dbContext;

        public RequestService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<int> CreateEquipmentAsync(Equipment equipment)
        {
            _dbContext.Equipments.Add(equipment);
            await _dbContext.SaveChangesAsync();
            return equipment.Id;
        }

        public async Task<bool> UpdateEquipmentAsync(Equipment equipment)
        {
            var existingEquipment = await _dbContext.Equipments
                .FirstOrDefaultAsync(e => e.Id == equipment.Id);

            if (existingEquipment == null)
                return false;

            existingEquipment.Name = equipment.Name;
            existingEquipment.StateNumber = equipment.StateNumber;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteEquipmentAsync(int id)
        {
            var equipment = await _dbContext.Equipments
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipment == null)
                return false;

            _dbContext.Equipments.Remove(equipment);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Equipment>> GetAllEquipmentAsync(string filter = "")
        {
            var query = _dbContext.Equipments.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                query = phrases.Aggregate(query, (current, phrase) => current.Where(e => e.Name.Contains(phrase) || e.StateNumber!.Contains(phrase)));
            }
            return await query
                .Select(e => new Equipment
                {
                    Id = e.Id,
                    Name = e.Name,
                    StateNumber = e.StateNumber ?? ""
                })
                .ToListAsync();
        }

        public async Task<List<Driver>> GetAllDriversAsync(string filter = "")
        {
            var query = _dbContext.Drivers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                query = phrases.Aggregate(query, (current, phrase) => current.Where(e => e.FullName.Contains(phrase) || e.Position!.Contains(phrase)));
            }
            return await query
                .Select(e => new Driver
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    ShortName = e.ShortName,
                    Position = e.Position
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
            var existDriver = await _dbContext.Drivers
                .FirstOrDefaultAsync(e => e.Id == driver.Id);

            if (existDriver == null)
                return false;

            existDriver.FullName = driver.FullName;
            existDriver.ShortName = driver.ShortName;
            existDriver.Position = driver.Position;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDriverAsync(int id)
        {
            var driver = await _dbContext.Drivers
                .FirstOrDefaultAsync(e => e.Id == id);

            if (driver == null)
                return false;

            _dbContext.Drivers.Remove(driver);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}