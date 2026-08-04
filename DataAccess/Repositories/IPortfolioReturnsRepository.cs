using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IPortfolioReturnsRepository
    {
        IEnumerable<PortfolioPrinosi?> GetByPortfolioId(int portfolioId);

        IEnumerable<PortfolioPrinosi?> GetByPortfolioIdForPeriod(int portfolioId, DateOnly from, DateOnly to);

        PortfolioPrinosi Add(PortfolioPrinosi portfolioPrinosi);
    }
}
