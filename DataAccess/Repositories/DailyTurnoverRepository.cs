using DataAccess.Enums;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Runtime.Intrinsics.Arm;

namespace DataAccess.Repositories
{
    public class DailyTurnoverRepository : IDailyTurnoverRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public DailyTurnoverRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExistsForDateAsync(DateTime date)
        {
            return await _dbContext.DnevenPromet
                .AnyAsync(dp => dp.Datum == date);
        }

        public async Task<IEnumerable<DnevenPromet>> GetBySecuritiesIdsAsync(List<int> securitiesIds, PriceTrendPeriod? period, PriceTrendResolution? resolution)
        {
            DateTime startDate = period == PriceTrendPeriod.Monthly
                ? DateTime.Today.AddMonths(-1)
                : DateTime.Today.AddYears(-1);

            var query = await _dbContext.DnevenPromet
                .Include(dp => dp.Hv)
                .Where(dp => securitiesIds.Contains(dp.Hvid) && dp.Datum >= startDate &&
                    dp.CenaPoslednaTransakcija != null)
                .OrderBy(dp => dp.Datum)
                .ToListAsync();

            var result = resolution switch
            {
                PriceTrendResolution.Day =>
                query.GroupBy(dp => new
                {
                    dp.Hvid,
                    dp.Datum.Date
                })
                .Select(g => g.OrderByDescending(x => x.Datum).First()),

                PriceTrendResolution.Week =>
                query.GroupBy(dp => new
                {
                    dp.Hvid,
                    dp.Datum.Year,
                    Week = ISOWeek.GetWeekOfYear(dp.Datum)
                })
                .Select(g => g.OrderByDescending(x => x.Datum).First()),

                PriceTrendResolution.Month =>
                query.GroupBy(dp => new
                {
                    dp.Hvid,
                    dp.Datum.Year,
                    dp.Datum.Month
                })
                 .Select(g => g.OrderByDescending(x => x.Datum).First()),

                PriceTrendResolution.Quarter =>
                query
                .GroupBy(dp => new
                {
                    dp.Hvid,
                    dp.Datum.Year,
                    Quarter = (dp.Datum.Month - 1) / 3 + 1
                })
                .Where(g =>
                {
                    DateTime quarterEnd = new DateTime(
                        g.Key.Year,
                        g.Key.Quarter * 3,
                        DateTime.DaysInMonth(g.Key.Year, g.Key.Quarter * 3)
                    );
                    return quarterEnd < DateTime.Today;
        })
                .Select(g => g.OrderByDescending(x => x.Datum).First()),

                _ => query
            };

            if(resolution == PriceTrendResolution.Quarter)
            {
                result = result
                    .GroupBy(x => x.Hvid)
                    .SelectMany(g => g
                    .OrderByDescending(x => x.Datum)
                    .Take(4));
            }

            return result
                .OrderBy(x => x.Datum)
                .ToList();
        }

        public async Task<IEnumerable<DnevenPromet?>> GetBySecurityCode(string securityCode, DateTime date)
        {
            return await _dbContext.DnevenPromet
                .Include(dp => dp.Hv)
                .Where(dp => dp.Hv.Kod == securityCode && dp.Datum <= date)
                .OrderByDescending(dp => dp.Datum)
                .ToListAsync();
        }

        public async Task<DateTime> GetLatestDateAsync()
        {
            return await _dbContext.DnevenPromet
                .MaxAsync(dp => dp.Datum);
        }

        public async Task<decimal> GetLatestPriceAsync(int securityId, DateOnly date)
        {
            return (decimal)await _dbContext.DnevenPromet
                .Where(dp => dp.Hvid == securityId && DateOnly.FromDateTime(dp.Datum) <= date && dp.CenaPoslednaTransakcija.HasValue)
                .OrderByDescending(dp => dp.Datum)
                .Select(dp => dp.CenaPoslednaTransakcija)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DnevenPromet>> GetLiquidityAsync(IEnumerable<int>? securityIds, DateTime fromDate)
        {
            var query = _dbContext.DnevenPromet
                .Include(dp => dp.Hv)
                .Where(dp => dp.Datum >= fromDate && dp.KolicinaIstrguvaniAkcii != null);

            if (securityIds != null)
                query = query.Where(dp => securityIds.Contains(dp.Hvid) && dp.KolicinaIstrguvaniAkcii > 0);

            return await query.ToListAsync();
        }
    }
}
