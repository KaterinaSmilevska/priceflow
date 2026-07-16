using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IPortfolioValueService
    {
        Task<List<PortfolioValue>> GetCurrentValueAsync(int portfolioId, bool isReal);
    }
}
