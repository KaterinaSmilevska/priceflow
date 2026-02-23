using DataAccess.Enums;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IDailyTurnoverRepository
    {
        Task<IEnumerable<DnevenPromet?>> GetBySecurityCode(string securityCode, DateTime date);

        Task<IEnumerable<DnevenPromet>> GetBySecuritiesIdsAsync(List<int> securitiesIds, PriceTrendPeriod period, int periodsBack);

        Task<IEnumerable<DnevenPromet>> GetLiquidityAsync(IEnumerable<int>? securityIds, DateTime fromDate);

        Task<bool> ExistsForDateAsync(DateTime date);

        Task<decimal> GetLatestPriceAsync(int securityId, DateOnly date);
    }
}
