using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class TypeSecurityRepository : ITypeSecurityRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public TypeSecurityRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        } 

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
