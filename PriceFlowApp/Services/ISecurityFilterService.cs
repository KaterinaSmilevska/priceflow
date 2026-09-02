using DataAccess.Models;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface ISecurityFilterService
    {
        IEnumerable<FilteredSecurity> FindMostProfitableSecuritiesByDividendYield();

        IEnumerable<FilteredSecurity> FindMostProfitableSecuritiesByDividendPerShare();

        IEnumerable<FilteredSecurity> FindSecuritiesWithBiggestPriceOscillations();

        IEnumerable<FilteredSecurity> FindSecuritiesWithSmallestPriceOscillations();

        IEnumerable<FilteredSecurity> FindLeastLiquidSecuritiesByTradedQuantity();

        IEnumerable<FilteredSecurity> FindMostLiquidSecuritiesByTradedQuantity();

        IEnumerable<FilteredSecurity> FindLeastLiquidSecuritiesByNumTradingDays();

        IEnumerable<FilteredSecurity> FindMostLiquidSecuritiesByNumTradingDays();

        IEnumerable<Sector> FindMostProfitableSectorsByDividendYield();

        IEnumerable<Sector> FindMostProfitableSectorsByProfit();

        IEnumerable<FilteredSecurity> FindSecuritiesValuation();
    }
}
