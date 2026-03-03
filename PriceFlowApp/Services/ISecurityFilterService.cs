using DataAccess.Models;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface ISecurityFilterService
    {
        Task<IEnumerable<FilteredSecurity>> FindMostProfitableSecuritiesByDividendYieldAsync();

        Task<IEnumerable<FilteredSecurity>> FindMostProfitableSecuritiesByDividendPerShareAsync();

        Task<IEnumerable<FilteredSecurity>> FindSecuritiesWithBiggestPriceOscillationsAsync();

        Task<IEnumerable<FilteredSecurity>> FindSecuritiesWithSmallestPriceOscillationsAsync();

        Task<IEnumerable<FilteredSecurity>> FindLeastLiquidSecuritiesByTradedQuantityAsync();

        Task<IEnumerable<FilteredSecurity>> FindMostLiquidSecuritiesByTradedQuantityAsync();

        Task<IEnumerable<FilteredSecurity>> FindMostLiquidSecuritiesByNumTradingDaysAsync();

        Task<IEnumerable<FilteredSecurity>> FindLeastLiquidSecuritiesByNumTradingDaysAsync();

        Task<IEnumerable<Sector>> FindMostProfitableSectorsByDividendYieldAsync();

        Task<IEnumerable<Sector>> FindMostProfitableSectorsByProfitAsync();

        Task<IEnumerable<FilteredSecurity>> FindSecuritiesValuationAsync();
    }
}
