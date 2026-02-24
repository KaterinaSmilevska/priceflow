using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IPortfolioReturnsService
    {
        Task<IEnumerable<PortfolioReturns>> FindByPortfolioId(int  portfolioId);

        Task<PortfolioReturns> CreateAsync(PortfolioReturns portfolioReturns);

        Task<PortfolioReturnsSummary> CalculateSummaryAsync(int portfolioId);

        Task<PortfolioReturnsSummary> CalculateSummaryForPeriodAsync(int portfolioId, DateOnly from, DateOnly to);
    }
}
