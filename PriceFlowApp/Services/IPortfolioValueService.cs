using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IPortfolioValueService
    {
        IEnumerable<PortfolioValue> GetCurrentValue(int portfolioId, bool isReal);
    }
}
