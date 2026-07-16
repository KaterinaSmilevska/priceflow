using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IPortfolioReturnsRepository
    {
        Task<IEnumerable<PortfolioPrinosi>> GetByPortfolioIdAsync(int portfolioId);

        Task<PortfolioPrinosi> AddAsync(PortfolioPrinosi portfolioPrinosi);

        Task<IEnumerable<PortfolioPrinosi>> GetByPortfolioIdForPeriod(int portfolioId, DateOnly from, DateOnly to);
    }
}
