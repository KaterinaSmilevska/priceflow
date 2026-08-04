using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IPortfolioReturnsService
    {
        IEnumerable<PortfolioReturns> FindByPortfolioId(int  portfolioId);

        PortfolioReturns Add(PortfolioReturns portfolioReturns);

        PortfolioReturnsSummary CalculateSummary(int portfolioId);

        PortfolioReturnsSummary CalculateSummaryForPeriod(int portfolioId, DateOnly from, DateOnly to);
    }
}
