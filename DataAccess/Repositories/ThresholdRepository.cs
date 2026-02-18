using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class ThresholdRepository: IThresholdRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public ThresholdRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HvPromenaCena> AddAsync(HvPromenaCena entity)
        {
           await _dbContext.HvPromenaCena.AddAsync(entity);
           await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(HvPromenaCena entity)
        {

           _dbContext.HvPromenaCena.Remove(entity);
           await _dbContext.SaveChangesAsync();

        }

        public async Task<HvPromenaCena?> GetByIdAsync(int id)
        {
            return await _dbContext.HvPromenaCena
                .Include(e => e.Hv)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<HvPromenaCena?> GetByUserandSecurityCodeAsync(int userId, int securityId)
        {
            return await _dbContext.HvPromenaCena
                .FirstOrDefaultAsync(e => e.KorisnikId == userId && e.Hvid == securityId);
        }

        public async Task<IEnumerable<HvPromenaCena>> GetByUserAsync(int userId)
        {
            return await _dbContext.HvPromenaCena
                .Where(e => e.KorisnikId == userId)
                .Include(e => e.Hv)
                .ToListAsync();
        }

        public async Task<HvPromenaCena> UpdateAsync(HvPromenaCena entity)
        {
            _dbContext.HvPromenaCena.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;

        }
    }
}
