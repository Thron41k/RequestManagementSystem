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
        public async Task<List<Commissions>> GetAllCommissionsAsync(string filter = "")
        {
            var query = _dbContext.Commissions.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                var phrases = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                query = phrases.Aggregate(query, (current, phrase) => current.Where(e => e.Name.ToLower().Contains(phrase)));
            }
            return await query
                .Select(e => new Commissions
                {
                    Id = e.Id,
                    Name = e.Name,
                    ApproveForAct = e.ApproveForAct,
                    ApproveForDefectAndLimit = e.ApproveForDefectAndLimit,
                    Chairman = e.Chairman,
                    Member1 = e.Member1,
                    Member2 = e.Member2,
                    Member3 = e.Member3,
                    Member4 = e.Member4
                })
                .ToListAsync();
        }

        public async Task<int> CreateCommissionsAsync(Commissions commissions)
        {
            _dbContext.Commissions.Add(commissions);
            await _dbContext.SaveChangesAsync();
            return commissions.Id;
        }

        public async Task<bool> UpdateCommissionsAsync(Commissions commissions)
        {
            try
            {
                var existCommissions = await _dbContext.Commissions
                    .FirstOrDefaultAsync(e => e.Id == commissions.Id);

                if (existCommissions == null)
                    return false;

                existCommissions.Name = commissions.Name;
                existCommissions.ApproveForActId = commissions.ApproveForActId;
                existCommissions.ApproveForDefectAndLimitId = commissions.ApproveForDefectAndLimitId;
                existCommissions.ChairmanId = commissions.ChairmanId;
                existCommissions.Member1Id = commissions.Member1Id;
                existCommissions.Member2Id = commissions.Member2Id;
                existCommissions.Member3Id = commissions.Member3Id;
                existCommissions.Member4Id = commissions.Member4Id;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<bool> DeleteCommissionsAsync(int id)
        {
            try
            {
                var commissions = await _dbContext.Commissions
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (commissions == null)
                    return false;

                _dbContext.Commissions.Remove(commissions);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}
