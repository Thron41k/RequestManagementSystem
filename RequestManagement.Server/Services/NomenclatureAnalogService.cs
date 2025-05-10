using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services
{
    public class NomenclatureAnalogService(ApplicationDbContext dbContext) : INomenclatureAnalogService
    {
        private readonly ApplicationDbContext _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        public async Task<List<Nomenclature>> GetAllNomenclatureAnalogsAsync(int filter)
        {
            try
            {
                var allAnalogIds = await GetTransitiveAnalogIdsAsync(filter);
                return await _dbContext.Nomenclature
                    .Where(n => allAnalogIds.Contains(n.Id) && n.Id != filter)
                    .Distinct()
                    .ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return [];
            }
        }
        private async Task<HashSet<int>> GetTransitiveAnalogIdsAsync(int startId)
        {
            var result = new HashSet<int>();
            var queue = new Queue<int>();
            queue.Enqueue(startId);

            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();

                // Находим все связанные ID (как оригиналы, так и аналоги)
                var relatedIds = await _dbContext.NomenclatureAnalog
                    .Where(x => x.AnalogId == currentId || x.OriginalId == currentId)
                    .Select(x => x.OriginalId == currentId ? x.AnalogId : x.OriginalId)
                    .Distinct()
                    .ToListAsync();

                foreach (var id in relatedIds.Where(id => id != currentId && !result.Contains(id)))
                {
                    result.Add(id);
                    queue.Enqueue(id);
                }
            }

            return result;
        }
        public async Task<int> AddNomenclatureAnalogAsync(NomenclatureAnalog nomenclatureAnalog)
        {
            try
            {
                var result = await _dbContext.NomenclatureAnalog.Where(x => (x.AnalogId == nomenclatureAnalog.AnalogId && x.OriginalId == nomenclatureAnalog.OriginalId) || (x.AnalogId == nomenclatureAnalog.OriginalId && x.OriginalId == nomenclatureAnalog.AnalogId)).FirstOrDefaultAsync();
                if (result != null) return result.Id;
                _dbContext.NomenclatureAnalog.Add(nomenclatureAnalog);
                await _dbContext.SaveChangesAsync();
                return nomenclatureAnalog.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
        }

        public async Task<bool> DeleteNomenclatureAnalogAsync(int originalId, int analogId)
        {
            try
            {
                var result = await _dbContext.NomenclatureAnalog.Where(x => (x.AnalogId == analogId && x.OriginalId == originalId) || (x.AnalogId == originalId && x.OriginalId == analogId)).FirstOrDefaultAsync();
                if (result == null) return true;
                _dbContext.NomenclatureAnalog.Remove(result);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
