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

        public IEnumerable<PortfolioPrinosi?> GetByPortfolioId(int portfolioId)
        {
            return _dbContext.PortfolioPrinosi
                .Where(pp => pp.PortfolioId == portfolioId)
                .ToList();
        }

        public IEnumerable<PortfolioPrinosi?> GetByPortfolioIdForPeriod(int portfolioId, DateOnly from, DateOnly to)
        {
            return _dbContext.PortfolioPrinosi
                .Where(pp => pp.PortfolioId == portfolioId
                && pp.Datum >= from
                && pp.Datum <= to)
                .ToList();
        }

        public PortfolioPrinosi Add(PortfolioPrinosi portfolioPrinosi)
        {
            _dbContext.PortfolioPrinosi.Add(portfolioPrinosi);
            _dbContext.SaveChanges();

            return portfolioPrinosi;
        }
    }
}
