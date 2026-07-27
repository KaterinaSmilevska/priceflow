using DataAccess.Enums;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IDailyTurnoverRepository
    {
        Task<IEnumerable<DnevenPromet?>> GetBySecurityCode(string securityCode, DateTime date);

        Task<IEnumerable<DnevenPromet>> GetBySecuritiesIdsAsync(List<int> securitiesIds, PriceTrendPeriod? period, PriceTrendResolution? resolution);

        Task<IEnumerable<DnevenPromet>> GetLiquidityAsync(IEnumerable<int>? securityIds, DateTime fromDate);

        Task<bool> ExistsForDateAsync(DateTime date);

        Task<decimal> GetLatestPriceAsync(int securityId, DateOnly date);

        Task<DateTime> GetLatestDateAsync();
    }
}
