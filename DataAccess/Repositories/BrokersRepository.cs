using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class BrokersRepository : IBrokersRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public BrokersRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Brokeri?> GetByIdAsync(int id)
        {
            return await _dbContext.Brokeri
                .FindAsync(id);
        }

        public async Task<Brokeri?> GetByCompanyAsync(string company)
        {
            return await _dbContext.Brokeri
                .FirstOrDefaultAsync(b => b.Kompanija == company);
        }

        public async Task<IEnumerable<Brokeri>> GetAllAsync()
        {
            return await _dbContext.Brokeri
                .ToListAsync();
        }

        public async Task UpdateAsync(Brokeri broker)
        {
            _dbContext.Brokeri.Update(broker);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Brokeri? broker = await _dbContext.Brokeri.FindAsync(id);
            if (broker != null)
            {
                _dbContext.Brokeri.Remove(broker);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Brokeri> AddAsync(Brokeri broker)
        {
            await _dbContext.Brokeri.AddAsync(broker);
            await _dbContext.SaveChangesAsync();

            return broker;
        }
    }
}
