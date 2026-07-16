using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface ISecurityFilterRepository
    {
        Task<IEnumerable<HartiiOdVrednost>> GetMostProfitableSecuritiesByDividendYieldAsync();

        Task<IEnumerable<HartiiOdVrednost>> GetMostProfitableSecuritiesByDividendPerShareAsync();

        Task<IEnumerable<HartiiOdVrednost>> GetSecuritiesWithBiggestPriceOscillationsAsync();

        Task<IEnumerable<HartiiOdVrednost>> GetSecuritiesWithSmallestPriceOscillationsAsync();

        Task<IEnumerable<HartiiOdVrednost>> GetLeastLiquidSecuritiesByTradedQuantityAsync();

        Task<IEnumerable<HartiiOdVrednost>> GetMostLiquidSecuritiesByTradedQuantityAsync();

        Task<IEnumerable<HartiiOdVrednost>> GetMostLiquidSecuritiesByNumTradingDaysAsync();

        Task<IEnumerable<HartiiOdVrednost>> GetLeastLiquidSecuritiesByNumTradingDaysAsync();

        Task<IEnumerable<Sektori>> GetMostProfitableSectorsByDividendYieldAsync();

        Task<IEnumerable<Sektori>> GetMostProfitableSectorsByProfitAsync();

        Task<IEnumerable<HartiiOdVrednost>> GetSecuritiesValuationAsync();

        Task<int> GetLatestYearAsync();

        Task<DateTime> GetLatestDateAsync();
    }
}
