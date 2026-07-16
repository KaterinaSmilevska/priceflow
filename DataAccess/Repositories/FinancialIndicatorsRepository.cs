using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class FinancialIndicatorsRepository : IFinancialIndicatorsRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public FinancialIndicatorsRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<FinansiskiPokazateli>> GetAllAsync()
        {
            return await _dbContext.FinansiskiPokazateli.ToListAsync();
        }
    }
}
