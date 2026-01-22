using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class ChartService : IChartService
    {
        private readonly PriceFlowDbContext _dbContext;
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly ISecuritiesRepository _securitiesRepository;

        public ChartService(PriceFlowDbContext dbContext, ITransactionsRepository transactionsRepository, ISecuritiesRepository securitiesRepository)
        {
            _dbContext = dbContext;
            _transactionsRepository = transactionsRepository;
            _securitiesRepository = securitiesRepository;
        }

        public async Task<IEnumerable<PriceTrend>> GetPriceTrendAsync(int securityId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.DnevenPromet
                .Where(dp => dp.Hvid == securityId && dp.Datum >= startDate && dp.Datum <= endDate)
                .OrderBy(dp => dp.Datum)
                .Select(dp => new PriceTrend
                {
                    Date = dp.Datum,
                    Price = (decimal)dp.CenaPoslednaTransakcija
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SectorDistribution>> GetSectorDistributionAsync(DateTime date)
        {
            return await _dbContext.HartiiOdVrednost
                .Join(_dbContext.DnevenPromet, hv => hv.Id, dp => dp.Hvid, (hv, dp) => new { hv, dp })
                .Where(x => x.dp.Datum == date)
                .Select( x => new
                {
                    SectorName = x.hv.Izdavach.Sektor.Ime,
                    MarketCap = x.hv.VkupenBrojAkcii * x.dp.CenaPoslednaTransakcija
                })
                .GroupBy(x => x.SectorName)
                .Select(g => new SectorDistribution
                {
                    SectorName = g.Key,
                    MarketCap = (decimal)g.Sum(x => x.MarketCap)

                })
                .ToListAsync();
        }

        public async Task<IEnumerable<Security>> GetSecurities()
        {
            IEnumerable<HartiiOdVrednost> foundSecurities = await _securitiesRepository.GetAllAsync();
            return foundSecurities.Select(security => new Security
            {
                Id = security.Id,
                Code = security.Kod,
            });
        }

        public DateTime? FindLatestDate()
        {
            return _dbContext.DnevenPromet
                .OrderByDescending(dp => dp.Datum)
                .Select(dp => dp.Datum)
                .FirstOrDefault();
        }

        public async Task<IEnumerable<MonthlyIncome>> GetMonthlyIncomeAsync(int portfolioid)
        {
            List<Transakcii> transactions = await _transactionsRepository.GetByPortfolioIdAsync(portfolioid);

            List<MonthlyIncome> monthlyIncome = transactions
                .Where(t => t.TipTransakcija == "Продавање")
                .GroupBy(t => new {t.Datum.Year, t.Datum.Month})
                .Select(g => new MonthlyIncome
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Income = g.Sum(t => t.Iznos)
                })
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .ToList();

            return monthlyIncome;
        }

        public async Task<IEnumerable<SecurityAllocation>> GetAllocationAsync(int portfolioId)
        {
            List<Transakcii> transactions = await _transactionsRepository.GetByPortfolioIdAsync(portfolioId);

            List<SecurityAllocation> securityAllocation = transactions
                .GroupBy(t => t.Hv.Kod)
                .Select(g => new SecurityAllocation
                {
                    SecurityCode = g.Key,
                    Quantity = g.Sum(t => t.TipTransakcija == "Купување" ? t.KolicinaAkcii : -t.KolicinaAkcii)
                })
                .Where(a => a.Quantity != 0)
                .ToList();

            return securityAllocation;
        }
    }
}
