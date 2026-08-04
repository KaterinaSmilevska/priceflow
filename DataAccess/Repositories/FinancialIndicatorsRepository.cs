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

        public IEnumerable<FinansiskiPokazateli> GetAll()
        {
            return _dbContext.FinansiskiPokazateli
                .ToList();
        }
    }
}
