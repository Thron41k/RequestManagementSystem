

using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services
{
    public class RequestService(ApplicationDbContext dbContext) : IRequestService
    {
        private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

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
            try
            {
                var equipment = await _dbContext.Equipments
                .FirstOrDefaultAsync(e => e.Id == id);

                if (equipment == null)
                    return false;

                _dbContext.Equipments.Remove(equipment);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Equipment>> GetAllEquipmentAsync(string filter = "")
        {
            var query = _dbContext.Equipments.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                query = phrases.Aggregate(query, (current, phrase) => current.Where(e => e.Name.ToLower().Contains(phrase) || e.StateNumber!.ToLower().Contains(phrase)));
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
                query = phrases.Aggregate(query, (current, phrase) => current.Where(e => e.FullName.ToLower().Contains(phrase) || e.Position.ToLower().Contains(phrase)));
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
            try
            {
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

        public async Task<List<DefectGroup>> GetAllDefectGroupsAsync(string filter = "")
        {
            var query = _dbContext.DefectGroups.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                query = phrases.Aggregate(query, (current, phrase) => current.Where(e => e.Name.ToLower().Contains(phrase)));
            }
            return await query
                .Select(e => new DefectGroup
                {
                    Id = e.Id,
                    Name = e.Name,
                })
                .ToListAsync();
        }
        public async Task<int> CreateDefectGroupAsync(DefectGroup defectGroup)
        {
            _dbContext.DefectGroups.Add(defectGroup);
            await _dbContext.SaveChangesAsync();
            return defectGroup.Id;
        }
        public async Task<bool> UpdateDefectGroupAsync(DefectGroup defectGroup)
        {
            var existDefectGroup = await _dbContext.DefectGroups
                .FirstOrDefaultAsync(e => e.Id == defectGroup.Id);

            if (existDefectGroup == null)
                return false;

            existDefectGroup.Name = defectGroup.Name;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteDefectGroupAsync(int id)
        {
            try
            {
                var defectGroup = await _dbContext.DefectGroups
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (defectGroup == null)
                    return false;

                _dbContext.DefectGroups.Remove(defectGroup);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Defect>> GetAllDefectsAsync(string filter = "")
        {
            var query = _dbContext.Defects.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                query = phrases.Aggregate(query, (current, phrase) => current.Where(e => e.Name.ToLower().Contains(phrase) || e.DefectGroup.Name.ToLower().Contains(phrase)));
            }
            return await query
                .Select(e => new Defect
                {
                    Id = e.Id,
                    Name = e.Name,
                    DefectGroupId = e.DefectGroupId
                })
                .ToListAsync();
        }

        public async Task<int> CreateDefectAsync(Defect defect)
        {
            _dbContext.Defects.Add(defect);
            await _dbContext.SaveChangesAsync();
            return defect.Id;
        }
        public async Task<bool> UpdateDefectAsync(Defect defect)
        {
            var existDefect = await _dbContext.Defects
                .FirstOrDefaultAsync(e => e.Id == defect.Id);

            if (existDefect == null)
                return false;

            existDefect.Name = defect.Name;
            existDefect.DefectGroupId = defect.DefectGroupId;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDefectAsync(int id)
        {
            try
            {
                var defect = await _dbContext.Defects
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (defect == null)
                    return false;

                _dbContext.Defects.Remove(defect);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Nomenclature>> GetAllNomenclaturesAsync(string filter = "")
        {
            var query = _dbContext.Nomenclature.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                query = phrases.Aggregate(query, (current, phrase) => current.Where(e => e.Name.ToLower().Contains(phrase) || e.Article.ToLower().Contains(phrase) || e.Code.ToLower().Contains(phrase) || e.UnitOfMeasure.ToLower().Contains(phrase)));
            }
            return await query
                .Select(e => new Nomenclature
                {
                    Id = e.Id,
                    Name = e.Name,
                    Article = e.Article,
                    Code = e.Code,
                    UnitOfMeasure = e.UnitOfMeasure
                })
                .ToListAsync();
        }

        public async Task<int> CreateNomenclatureAsync(Nomenclature nomenclature)
        {
            _dbContext.Nomenclature.Add(nomenclature);
            await _dbContext.SaveChangesAsync();
            return nomenclature.Id;
        }

        public async Task<bool> UpdateNomenclatureAsync(Nomenclature nomenclature)
        {
            var existNomenclature = await _dbContext.Nomenclature
                .FirstOrDefaultAsync(e => e.Id == nomenclature.Id);

            if (existNomenclature == null)
                return false;

            existNomenclature.Name = nomenclature.Name;
            existNomenclature.Article = nomenclature.Article;
            existNomenclature.Code = nomenclature.Code;
            existNomenclature.UnitOfMeasure = nomenclature.UnitOfMeasure;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNomenclatureAsync(int id)
        {
            try
            {
                var nomenclature = await _dbContext.Nomenclature
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (nomenclature == null)
                    return false;

                _dbContext.Nomenclature.Remove(nomenclature);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}