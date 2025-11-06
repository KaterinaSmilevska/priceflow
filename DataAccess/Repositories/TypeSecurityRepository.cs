using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class TypeSecurityRepository : ITypeSecurityRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public TypeSecurityRepository(PriceFlowDbContext dbContext) => _dbContext = dbContext;

        public async Task<IEnumerable<TipHv>> GetAllAsync()
        {
            return await _dbContext.TipHv.ToListAsync();
        }

        public async Task<TipHv?> GetByIdAsync(int id)
        {
            return await _dbContext.TipHv
                .FindAsync(id);
        }
    }
}
