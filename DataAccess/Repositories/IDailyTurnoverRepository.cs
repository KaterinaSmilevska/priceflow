using DataAccess.Enums;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IDailyTurnoverRepository
    {
        IEnumerable<DnevenPromet> GetBySecurityCode(string securityCode, DateTime date);

        IEnumerable<DnevenPromet> GetBySecuritiesIds(List<int> securitiesIds, PriceTrendPeriod? period, PriceTrendResolution? resolution);

        IEnumerable<DnevenPromet> GetLiquidity(IEnumerable<int>? securityIds, DateTime fromDate);

        IEnumerable<DnevenPromet> GetBySecurityAndDateRange(int securityId, DateTime startDate, DateTime endDate);

        IEnumerable<DnevenPromet> GetByDateWithSecurity(DateTime date);

        IEnumerable<DnevenPromet> GetDailyTurnoverForTotalMarketCap(DateTime date);

        IEnumerable<DnevenPromet> GetByDateRange(DateTime startDate, DateTime endDate);

        IEnumerable<DnevenPromet> GetLatestPrices(IEnumerable<int> securityIds);

        decimal? GetLatestPrice(int securityId, DateOnly date);

        DateTime GetLatestDate();

        bool ExistsForDate(DateTime date);

        IEnumerable<DnevenPromet> GetByDate(DateTime date);
    }
}
