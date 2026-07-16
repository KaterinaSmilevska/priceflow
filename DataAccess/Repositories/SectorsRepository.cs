using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class SectorsRepository : ISectorsRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public SectorsRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Sektori>> GetAllAsync()
        {
            return await _dbContext.Sektori.ToListAsync();
        }

        public async Task<Sektori?> GetByIdAsync(int id)
        {
            return await _dbContext.Sektori
                .FindAsync(id);
        }
    }
}
