using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface ISecurityFilterRepository
    {
        IEnumerable<HartiiOdVrednost?> GetMostProfitableSecuritiesByDividendYield();

        IEnumerable<HartiiOdVrednost?> GetMostProfitableSecuritiesByDividendPerShare();

        IEnumerable<HartiiOdVrednost?> GetSecuritiesWithBiggestPriceOscillations();

        IEnumerable<HartiiOdVrednost?> GetSecuritiesWithSmallestPriceOscillations();

        IEnumerable<HartiiOdVrednost?> GetLeastLiquidSecuritiesByTradedQuantity();

        IEnumerable<HartiiOdVrednost?> GetMostLiquidSecuritiesByTradedQuantity();

        IEnumerable<HartiiOdVrednost?> GetLeastLiquidSecuritiesByNumTradingDays();

        IEnumerable<HartiiOdVrednost?> GetMostLiquidSecuritiesByNumTradingDays();

        IEnumerable<Sektori?> GetMostProfitableSectorsByDividendYield();

        IEnumerable<Sektori?> GetMostProfitableSectorsByProfit();

        IEnumerable<HartiiOdVrednost?> GetSecuritiesValuation();

        DateTime GetLatestDate();

        int GetLatestYear();
    }
}
