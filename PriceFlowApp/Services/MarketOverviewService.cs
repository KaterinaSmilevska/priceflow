using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class MarketOverviewService : IMarketOverviewService
    {
        private readonly PriceFlowDbContext _dbContext;

        public MarketOverviewService(PriceFlowDbContext dbContext) => _dbContext = dbContext;

        public async Task<MarketOverview> GetOverviewAsync()
        {
            DateTime latestDate = await _dbContext.DnevenPromet.MaxAsync(dp => dp.Datum);

            var totalMarketCap = await _dbContext.HartiiOdVrednost
                .Join(_dbContext.DnevenPromet, hv => hv.Id, dp => dp.Hvid, (hv, dp) => new { hv, dp })
                .Where(x => x.dp.Datum == latestDate)
                .SumAsync(x => x.hv.VkupenBrojAkcii * x.dp.CenaPoslednaTransakcija);

            var averageDailyVolume = await _dbContext.DnevenPromet
                .Where(dp => dp.Datum == latestDate)
                .AverageAsync(dp => dp.KolicinaIstrguvaniAkcii);

            var topGainer = await _dbContext.DnevenPromet
                .Where(dp => dp.Datum == latestDate && dp.ProcentPromena > 0)
                .OrderByDescending(dp => dp.ProcentPromena)
                .Select(dp => new { dp.Hv.Kod, dp.ProcentPromena })
                .FirstOrDefaultAsync();

            var topLoser = await _dbContext.DnevenPromet
                .Where(dp => dp.Datum == latestDate && dp.ProcentPromena < 0)
                .OrderBy(dp => dp.ProcentPromena)
                .Select(dp => new { dp.Hv.Kod, dp.ProcentPromena })
                .FirstOrDefaultAsync();

            var totalSecurities = await _dbContext.HartiiOdVrednost
                .CountAsync();

            return new MarketOverview
            {
                TotalMarketCap = (decimal)totalMarketCap,
                AverageDailyVolume = (int)averageDailyVolume,
                TopGainer = topGainer?.Kod ?? "",
                TopGainerChange = topGainer?.ProcentPromena ?? 0,
                TopLoser = topLoser?.Kod ?? "",
                TopLoserChange = topLoser?.ProcentPromena ?? 0,
                TotalSecurities = totalSecurities
            };
        }

        public async Task<IEnumerable<SecurityPerformance>> GetTopGainersAsync(int count)
        {
            DateTime latestDate = await _dbContext.DnevenPromet.MaxAsync(dp => dp.Datum);

            return await _dbContext.DnevenPromet
                .Where(dp => dp.Datum == latestDate && dp.ProcentPromena > 0)
                .OrderByDescending(dp => dp.ProcentPromena)
                .Take(count)
                .Select(dp => new SecurityPerformance
                {
                    Code = dp.Hv.Kod,
                    ChangePercent = (decimal)dp.ProcentPromena,
                    Volume = (int)dp.KolicinaIstrguvaniAkcii
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SecurityPerformance>> GetTopLosersAsync(int count)
        {
            DateTime latestDate = await _dbContext.DnevenPromet.MaxAsync(dp => dp.Datum);

            return await _dbContext.DnevenPromet
                .Where(dp => dp.Datum == latestDate && dp.ProcentPromena < 0)
                .OrderBy(dp => dp.ProcentPromena)
                .Take(count)
                .Select(dp => new SecurityPerformance
                {
                    Code = dp.Hv.Kod,
                    ChangePercent = (decimal)dp.ProcentPromena,
                    Volume = (int)dp.KolicinaIstrguvaniAkcii
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SecurityPerformance>> GetMostTradedAsync(int count)
        {
            DateTime latestDate = await _dbContext.DnevenPromet.MaxAsync(dp => dp.Datum);

            return await _dbContext.DnevenPromet
                .Where(dp => dp.Datum == latestDate)
                .OrderByDescending(dp => dp.KolicinaIstrguvaniAkcii)
                .Take(count)
                .Select(dp => new SecurityPerformance
                {
                    Code = dp.Hv.Kod,
                    ChangePercent = (decimal)dp.ProcentPromena,
                    Volume = (int)dp.KolicinaIstrguvaniAkcii
                })
                .ToListAsync();
        }
    }
}
