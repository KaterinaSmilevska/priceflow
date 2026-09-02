using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IMarketOverviewService
    {
        MarketOverview GetOverview();

        IEnumerable<SecurityPerformance> GetTopGainers(int count);

        IEnumerable<SecurityPerformance> GetTopLosers(int count);

        IEnumerable<SecurityPerformance> GetMostTrade(int count);

        LiquidityOverview FindLiquidity(int userId, int monthsBack, bool onlyOwned);
    }
}
