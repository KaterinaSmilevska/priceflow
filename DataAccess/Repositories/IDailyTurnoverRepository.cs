using DataAccess.Enums;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IDailyTurnoverRepository
    {
        IEnumerable<DnevenPromet?> GetBySecurityCode(string securityCode, DateTime date);

        IEnumerable<DnevenPromet?> GetBySecuritiesIds(List<int> securitiesIds, PriceTrendPeriod? period, PriceTrendResolution? resolution);

        IEnumerable<DnevenPromet?> GetLiquidity(IEnumerable<int>? securityIds, DateTime fromDate);

        decimal? GetLatestPrice(int securityId, DateOnly date);

        DateTime GetLatestDate();

        bool ExistsForDate(DateTime date);
    }
}
