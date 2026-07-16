using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class PortfolioReturnsRepository : IPortfolioReturnsRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public PortfolioReturnsRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PortfolioPrinosi> AddAsync(PortfolioPrinosi portfolioPrinosi)
        {
            _dbContext.PortfolioPrinosi.Add(portfolioPrinosi);
            await _dbContext.SaveChangesAsync();

            return portfolioPrinosi;
        }

        public async Task<IEnumerable<PortfolioPrinosi>> GetByPortfolioIdAsync(int portfolioId)
        {
            return await _dbContext.PortfolioPrinosi
                .Where(pp => pp.PortfolioId == portfolioId)
                .ToListAsync();
        }

        public async Task<IEnumerable<PortfolioPrinosi>> GetByPortfolioIdForPeriod(int portfolioId, DateOnly from, DateOnly to)
        {
            return await _dbContext.PortfolioPrinosi
                .Where(pp => pp.PortfolioId == portfolioId 
                && pp.Datum >= from
                && pp.Datum <= to)
                .ToListAsync();
        }
    }
}
