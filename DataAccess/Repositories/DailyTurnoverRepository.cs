using DataAccess.Enums;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class DailyTurnoverRepository : IDailyTurnoverRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public DailyTurnoverRepository(PriceFlowDbContext dbContext) => _dbContext = dbContext;

        public async Task<bool> ExistsForDateAsync(DateTime date)
        {
            return await _dbContext.DnevenPromet
                .AnyAsync(dp => dp.Datum == date);
        }

        public async Task<IEnumerable<DnevenPromet>> GetBySecuritiesIdsAsync(List<int> securitiesIds, PriceTrendPeriod period, int periodsBack)
        {
            DateTime startDate = period == PriceTrendPeriod.Monthly
                ? DateTime.Today.AddMonths(-periodsBack)
                : DateTime.Today.AddYears(-periodsBack);

            var query = await _dbContext.DnevenPromet
                .Include(dp => dp.Hv)
                .Where(dp => securitiesIds.Contains(dp.Hvid) && dp.Datum >= startDate &&
                    dp.CenaPoslednaTransakcija != null)
                .OrderBy(dp => dp.Datum)
                .ToListAsync();

            if(period == PriceTrendPeriod.Monthly)
            {
                return query
                    .GroupBy(dp => new { dp.Hvid, dp.Datum.Year, dp.Datum.Month })
                    .Select(g =>
                        g.OrderByDescending(x => x.Datum).First())
                    .OrderBy(x => x.Datum)
                    .ToList();
            }

            return query
                 .GroupBy(dp => new { dp.Hvid, dp.Datum.Year })
                    .Select(g =>
                        g.OrderByDescending(x => x.Datum).First())
                .OrderBy(dp => dp.Datum)
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
