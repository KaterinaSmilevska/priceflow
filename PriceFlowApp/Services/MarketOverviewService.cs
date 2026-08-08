using DataAccess.Models;
using DataAccess.Repositories;
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

        public MarketOverview GetOverview()
        {
            DateTime latestDate = FindLatestDate();
            DateTime startMonth = new DateTime(latestDate.Year, latestDate.Month, 1);

            decimal? totalMarketCap = _dbContext.DnevenPromet
                .Where(dp => dp.Datum == latestDate && dp.CenaPoslednaTransakcija != null)
                .Join(_dbContext.HartiiOdVrednost, dp => dp.Hvid, hv => hv.Id, (dp, hv) => new { dp, hv })
                .Sum(x => x.hv.VkupenBrojAkcii * x.dp.CenaPoslednaTransakcija);

            double? averageDailyVolume = _dbContext.DnevenPromet
                .Where(dp => dp.Datum == latestDate && dp.KolicinaIstrguvaniAkcii != null)
                .Average(dp => dp.KolicinaIstrguvaniAkcii);

            double? averageMonthlyVolume = _dbContext.DnevenPromet
                .Where(dp => dp.Datum >= startMonth && dp.Datum <= latestDate && dp.KolicinaIstrguvaniAkcii != null)
                .Average(dp => dp.KolicinaIstrguvaniAkcii);

            var topGainer = _dbContext.DnevenPromet
                .Where(dp => dp.Datum == latestDate && dp.ProcentPromena > 0)
                .OrderByDescending(dp => dp.ProcentPromena)
                .Select(dp => new { dp.Hv.Kod, dp.ProcentPromena })
                .FirstOrDefault();

            var topLoser = _dbContext.DnevenPromet
                .Where(dp => dp.Datum == latestDate && dp.ProcentPromena < 0)
                .OrderBy(dp => dp.ProcentPromena)
                .Select(dp => new { dp.Hv.Kod, dp.ProcentPromena })
                .FirstOrDefault();

            var totalSecurities = _dbContext.HartiiOdVrednost
                .Count();

            return new MarketOverview
            {
                TotalMarketCap = (decimal)(totalMarketCap ?? 0),
                AverageDailyVolume = (int)(averageDailyVolume ?? 0),
                AverageMonthlyVolume = (int)(averageMonthlyVolume ?? 0),
                TopGainer = topGainer?.Kod ?? "",
                TopGainerChange = topGainer?.ProcentPromena ?? 0,
                TopLoser = topLoser?.Kod ?? "",
                TopLoserChange = topLoser?.ProcentPromena ?? 0,
                TotalSecurities = totalSecurities
            };
        }

        public IEnumerable<SecurityPerformance> GetTopGainers(int count)
        {
            DateTime latestDate = FindLatestDate();

            return _dbContext.DnevenPromet
                .Where(dp => dp.Datum == latestDate && dp.ProcentPromena > 0 && dp.KolicinaIstrguvaniAkcii != null)
                .OrderByDescending(dp => dp.ProcentPromena)
                .Take(count)
                .Select(dp => new SecurityPerformance
                {
                    Code = dp.Hv.Kod,
                    ChangePercent = (decimal?)dp.ProcentPromena,
                    Volume = (int)(dp.KolicinaIstrguvaniAkcii ?? 0)
                })
                .ToList();
        }

        public IEnumerable<SecurityPerformance> GetTopLosers(int count)
        {
            DateTime latestDate = FindLatestDate();

            return _dbContext.DnevenPromet
                .Where(dp => dp.Datum == latestDate && dp.ProcentPromena < 0 && dp.KolicinaIstrguvaniAkcii != null)
                .OrderBy(dp => dp.ProcentPromena)
                .Take(count)
                .Select(dp => new SecurityPerformance
                {
                    Code = dp.Hv.Kod,
                    ChangePercent = (decimal?)dp.ProcentPromena,
                    Volume = (int)(dp.KolicinaIstrguvaniAkcii ?? 0)
                })
                .ToList();
        }

        public IEnumerable<SecurityPerformance> GetMostTrade(int count)
        {
            DateTime latestDate = FindLatestDate();

            return _dbContext.DnevenPromet
                .Where(dp => dp.Datum == latestDate && dp.KolicinaIstrguvaniAkcii != null)
                .OrderByDescending(dp => dp.KolicinaIstrguvaniAkcii)
                .Take(count)
                .Select(dp => new SecurityPerformance
                {
                    Code = dp.Hv.Kod,
                    ChangePercent = (decimal?)dp.ProcentPromena,
                    Volume = (int)(dp.KolicinaIstrguvaniAkcii ?? 0)
                })
                .ToList();
        }

        public LiquidityOverview FindLiquidity(int userId, int monthsBack, bool onlyOwned)
        {
            DateTime fromDate = DateTime.Today.AddMonths(-monthsBack);

            IEnumerable<int>? securitiesIds = null;

            if (onlyOwned)
            {
                securitiesIds = _transactionsRepository.GetOwnedSecuritiesIds(userId);
            }

            IEnumerable<DnevenPromet?> dailyTurnover = _dailyTurnoverRepository.GetLiquidity(securitiesIds, fromDate);

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

        private DateTime FindLatestDate()
        {
            return _dbContext.DnevenPromet
                .Max(dp => dp.Datum);
        }
    }
}
