using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IPortfolioReturnsService
    {
        Task<PortfolioReturns> CreateAsync(PortfolioReturns portfolioReturns);

        Task<PortfolioReturnsSummary> CalculateSummaryAsync(int portfolioId);
    }
}
