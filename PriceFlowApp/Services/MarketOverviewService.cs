using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class MarketOverviewService : IMarketOverviewService
    {
        private readonly PriceFlowDbContext _dbContext;
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IDailyTurnoverRepository _dailyTurnoverRepository;

        public MarketOverviewService(PriceFlowDbContext dbContext, ITransactionsRepository transactionsRepository,
                IDailyTurnoverRepository dailyTurnoverRepository) 
        {
            _dbContext = dbContext;
            _transactionsRepository = transactionsRepository;
            _dailyTurnoverRepository = dailyTurnoverRepository;
        } 

        public async Task<MarketOverview> GetOverviewAsync()
        {
            DateTime latestDate = await _dbContext.DnevenPromet.MaxAsync(dp => dp.Datum);

            decimal? totalMarketCap = await _dbContext.HartiiOdVrednost
                .Join(_dbContext.DnevenPromet, hv => hv.Id, dp => dp.Hvid, (hv, dp) => new { hv, dp })
                .Where(x => x.dp.Datum == latestDate && x.dp.CenaPoslednaTransakcija != null)
                .SumAsync(x => x.hv.VkupenBrojAkcii * x.dp.CenaPoslednaTransakcija);

            double? averageDailyVolume = await _dbContext.DnevenPromet
                .Where(dp => dp.Datum == latestDate && dp.KolicinaIstrguvaniAkcii != null)
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
                TotalMarketCap = (decimal)(totalMarketCap ?? 0),
                AverageDailyVolume = (int)(averageDailyVolume ?? 0),
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
                .Where(dp => dp.Datum == latestDate && dp.ProcentPromena > 0 && dp.KolicinaIstrguvaniAkcii != null)
                .OrderByDescending(dp => dp.ProcentPromena)
                .Take(count)
                .Select(dp => new SecurityPerformance
                {
                    Code = dp.Hv.Kod,
                    ChangePercent = (decimal?)dp.ProcentPromena,
                    Volume = (int)(dp.KolicinaIstrguvaniAkcii ?? 0)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SecurityPerformance>> GetTopLosersAsync(int count)
        {
            DateTime latestDate = await _dbContext.DnevenPromet.MaxAsync(dp => dp.Datum);

            return await _dbContext.DnevenPromet
                .Where(dp => dp.Datum == latestDate && dp.ProcentPromena < 0 && dp.KolicinaIstrguvaniAkcii != null)
                .OrderBy(dp => dp.ProcentPromena)
                .Take(count)
                .Select(dp => new SecurityPerformance
                {
                    Code = dp.Hv.Kod,
                    ChangePercent = (decimal?)dp.ProcentPromena,
                    Volume = (int)(dp.KolicinaIstrguvaniAkcii ?? 0)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SecurityPerformance>> GetMostTradedAsync(int count)
        {
            DateTime latestDate = await _dbContext.DnevenPromet.MaxAsync(dp => dp.Datum);

            return await _dbContext.DnevenPromet
                .Where(dp => dp.Datum == latestDate && dp.KolicinaIstrguvaniAkcii != null)
                .OrderByDescending(dp => dp.KolicinaIstrguvaniAkcii)
                .Take(count)
                .Select(dp => new SecurityPerformance
                {
                    Code = dp.Hv.Kod,
                    ChangePercent = (decimal?)dp.ProcentPromena,
                    Volume = (int)(dp.KolicinaIstrguvaniAkcii ?? 0)
                })
                .ToListAsync();
        }

        public async Task<LiquidityOverview> FindLiquidityAsync(int userId, int monthsBack, bool onlyOwned)
        {
            DateTime fromDate = DateTime.Today.AddMonths(-monthsBack);

            IEnumerable<int>? securitiesIds = null;

            if (onlyOwned)
            {
                securitiesIds = await _transactionsRepository.GetOwnedSecuritiesIdsAsync(userId);
            }

            IEnumerable<DnevenPromet> dailyTurnover = await _dailyTurnoverRepository.GetLiquidityAsync(securitiesIds, fromDate);

            var grouped = dailyTurnover.GroupBy(dp => new { dp.Hvid, dp.Hv.Kod })
                .Select(g => new
                    SecurityLiquidity
                {
                    SecurityId = g.Key.Hvid,
                    SecurityCode = g.Key.Kod,
                    TradingDays = g.Select(x => x.Datum).Distinct().Count(),
                    TradedQuantity = g.Sum(x => x.KolicinaIstrguvaniAkcii),
                    AverageDailyVolume = g.Sum(x => x.KolicinaIstrguvaniAkcii) /
                                (decimal)g.Select(x => x.Datum.Date).Distinct().Count(),
                    LastTradeDate = g.Max(x => x.Datum)
                })
                .ToList();

            return new LiquidityOverview
            {
                MostByTradedQuantity = grouped
                .OrderByDescending(x => x.TradedQuantity)
                .Take(5),

                LeastByTradedQuantity = grouped
                .OrderBy(x => x.TradedQuantity)
                .Take(5),

                MostByTradingDays = grouped
                .OrderByDescending(x => x.TradingDays)
                .Take(5),

                LeastByTradingDays = grouped
                .OrderBy(x => x.TradingDays)
                .Take(5)
            };
        }
    }
}
