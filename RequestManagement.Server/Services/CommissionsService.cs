using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;

namespace RequestManagement.Server.Services
{
    public class CommissionsService(ApplicationDbContext dbContext) : ICommissionsService
    {
        private readonly ApplicationDbContext _dbContext =
            dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        public Task<List<Commissions>> GetAllCommissionsAsync(string filter = "")
        {
            var query = _dbContext.C.AsQueryable();
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
                    Code = e.Code
                })
                .ToListAsync();
        }

        public Task<int> CreateCommissionsAsync(Commissions warehouse)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCommissionsAsync(Commissions warehouse)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCommissionsAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
