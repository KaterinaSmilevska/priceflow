using DataAccess.Enums;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DataAccess.Repositories
{
    public class DailyTurnoverRepository : IDailyTurnoverRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public DailyTurnoverRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<DnevenPromet?> GetBySecurityCode(string securityCode, DateTime date)
        {
            return _dbContext.DnevenPromet
                .Include(dp => dp.Hv)
                .Where(dp => dp.Hv.Kod == securityCode && dp.Datum <= date)
                .OrderByDescending(dp => dp.Datum)
                .ToList();
        }

        public IEnumerable<DnevenPromet?> GetBySecuritiesIds(List<int> securitiesIds, PriceTrendPeriod? period, PriceTrendResolution? resolution)
        {
            DateTime today = DateTime.Today;

            DateTime startDate = period == PriceTrendPeriod.Monthly
                ? DateTime.Today.AddMonths(-1)
                : DateTime.Today.AddYears(-1);

            if (resolution == PriceTrendResolution.Week)
            {
                while (startDate.DayOfWeek != DayOfWeek.Monday)
                {
                    startDate = startDate.AddDays(-1);
                }
            }
            else if (resolution == PriceTrendResolution.Month || resolution == PriceTrendResolution.Quarter)
            {
                startDate = new DateTime(startDate.Year, startDate.Month, 1);
            }

            var query = _dbContext.DnevenPromet
                .Include(dp => dp.Hv)
                .Where(dp => securitiesIds.Contains(dp.Hvid) && dp.Datum >= startDate &&
                  dp.CenaPoslednaTransakcija != null)
                .OrderBy(dp => dp.Datum)
                .ToList();

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
                    Year = ISOWeek.GetYear(dp.Datum),
                    Week = ISOWeek.GetWeekOfYear(dp.Datum)
                })
                .Where(g =>
                {
                    DateTime weekEnd = ISOWeek.ToDateTime(
                        g.Key.Year,
                        g.Key.Week,
                        DayOfWeek.Sunday);

                    return weekEnd < today;
                })
                .Select(g => g.OrderByDescending(x => x.Datum).First()),

                PriceTrendResolution.Month =>
                query.GroupBy(dp => new
                {
                    dp.Hvid,
                    dp.Datum.Year,
                    dp.Datum.Month
                })
                .Where(g =>
                {
                    DateTime monthEnd = new DateTime(
                        g.Key.Year,
                        g.Key.Month,
                        DateTime.DaysInMonth(g.Key.Year, g.Key.Month));

                    return monthEnd < today;
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
                    return quarterEnd < today;
                })
                .Select(g => g.OrderByDescending(x => x.Datum).First()),

                _ => query
            };

            if (resolution == PriceTrendResolution.Quarter)
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

        public IEnumerable<DnevenPromet?> GetLiquidity(IEnumerable<int>? securityIds, DateTime fromDate)
        {
            var query = _dbContext.DnevenPromet
                .Include(dp => dp.Hv)
                .Where(dp => dp.Datum >= fromDate && dp.KolicinaIstrguvaniAkcii != null);

            if (securityIds != null)
                query = query.Where(dp => securityIds.Contains(dp.Hvid) && dp.KolicinaIstrguvaniAkcii > 0);

            return query.ToList();
        }

        public decimal? GetLatestPrice(int securityId, DateOnly date)
        {
            return _dbContext.DnevenPromet
                .Where(dp => dp.Hvid == securityId && DateOnly.FromDateTime(dp.Datum) <= date && dp.CenaPoslednaTransakcija.HasValue)
                .OrderByDescending(dp => dp.Datum)
                .Select(dp => dp.CenaPoslednaTransakcija)
                .FirstOrDefault();
        }

        public DateTime GetLatestDate()
        {
            return _dbContext.DnevenPromet
                .Max(dp => dp.Datum);
        }

        public bool ExistsForDate(DateTime date)
        {
            return _dbContext.DnevenPromet
                .Any(dp => dp.Datum == date);
        }
    }
}
