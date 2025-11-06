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

        public async Task<IEnumerable<HartiiOdVrednost>> GetAllAsync()
        {
            return await _dbContext.HartiiOdVrednost
                .Include(hv => hv.Izdavach)
                .Include(hv => hv.TipHv)
                .ToListAsync();
        }

        public async Task<HartiiOdVrednost?> GetByIdAsync(int id)
        {
            return await _dbContext.HartiiOdVrednost
                .Include(hv => hv.Izdavach)
                .Include(hv => hv.TipHv)
                .FirstOrDefaultAsync(hv => hv.Id == id);
        }
    }
}
