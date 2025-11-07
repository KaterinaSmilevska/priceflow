using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IMarketOverviewService
    {
        Task<MarketOverview> GetOverviewAsync();

        Task<IEnumerable<SecurityPerformance>> GetTopGainersAsync(int count);

        Task<IEnumerable<SecurityPerformance>> GetTopLosersAsync(int count);

        Task<IEnumerable<SecurityPerformance>> GetMostTradedAsync(int count);
    }
}
