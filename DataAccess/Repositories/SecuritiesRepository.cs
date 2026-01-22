using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class SecuritiesRepository : ISecuritiesRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public SecuritiesRepository(PriceFlowDbContext dbContext) => _dbContext = dbContext;

        public async Task<HartiiOdVrednost> AddAsync(HartiiOdVrednost security)
        {
            _dbContext.HartiiOdVrednost.Add(security);
            await _dbContext.SaveChangesAsync();
            return security;
        }

        public async Task DeleteAsync(int id)
        {
            var security = await _dbContext.HartiiOdVrednost.FindAsync(id);
            if(security != null)
            {
                _dbContext.HartiiOdVrednost.Remove(security);
                await _dbContext.SaveChangesAsync();
            }

        }

        public async Task<IEnumerable<HartiiOdVrednost>> GetAllAsync()
        {
            return await _dbContext.HartiiOdVrednost
                .Include(hv => hv.Izdavach)
                .Include(hv => hv.TipHv)
                .ToListAsync();
        }

        public async Task<HartiiOdVrednost?> GetByCodeAsync(string code)
        {
            return await _dbContext.HartiiOdVrednost
                .FirstOrDefaultAsync(hv => hv.Kod == code);
        }

        public async Task<HartiiOdVrednost?> GetByIdAsync(int id)
        {
            return await _dbContext.HartiiOdVrednost
                .Include(hv => hv.Izdavach)
                .Include(hv => hv.TipHv)
                .FirstOrDefaultAsync(hv => hv.Id == id);
        }

        public async Task UpdateAsync(HartiiOdVrednost security)
        {
            _dbContext.HartiiOdVrednost.Update(security);
            await _dbContext.SaveChangesAsync();
        }
    }
}
