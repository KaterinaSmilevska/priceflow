using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IPortfolioAnalyticsService
    {
        public Task<PortfolioAnalytics> GetTotalIncomeAsync(int portfolioId);
    }
}
