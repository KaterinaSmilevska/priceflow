using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IPortfoliosService
    {
        Portfolio FindById(int id);

        IEnumerable<Portfolio> FindUserPortfolios(int userId);

        Portfolio Add(int userId, AddPortfolioRequest portfolio);

        Portfolio Update(int id, int userId, UpdatePortfolio portfolio);

        Portfolio Delete(int id, int userId);

        PortfolioPerformanceSummary GeneratePerformanceSummary(int portfolioId, DateOnly from, DateOnly to);
    }
}
