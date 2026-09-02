using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using System.Runtime.Intrinsics.Arm;

namespace PriceFlowApp.Services
{
    public class ChartService : IChartService
    {
        private readonly IDailyTurnoverRepository _dailyTurnoverRepository;
        private readonly IPortfoliosRepository _portfoliosRepository;
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly ISecuritiesRepository _securitiesRepository;

        public ChartService(IDailyTurnoverRepository dailyTurnoverRepository, ITransactionsRepository transactionsRepository, ISecuritiesRepository securitiesRepository, IPortfoliosRepository portfoliosRepository)
        {
            _dailyTurnoverRepository = dailyTurnoverRepository;
            _transactionsRepository = transactionsRepository;
            _securitiesRepository = securitiesRepository;
            _portfoliosRepository = portfoliosRepository;
        }

        public IEnumerable<PriceTrend> GetPriceTrend(int securityId, DateTime startDate, DateTime endDate)
        {
            ValidateDateRange(startDate, endDate);

            HartiiOdVrednost security = GetSecurityById(securityId);

            IEnumerable<DnevenPromet> dailyPrices = _dailyTurnoverRepository.GetBySecurityAndDateRange(securityId, startDate, endDate);

            return dailyPrices
                .Select(dp => new PriceTrend
                {
                    Date = dp.Datum,
                    Price = (decimal)dp.CenaPoslednaTransakcija
                })
                .ToList();
        }

        public IEnumerable<SectorDistribution> GetSectorDistribution(DateTime date)
        {
            IEnumerable<DnevenPromet> dailyTurnover = _dailyTurnoverRepository.GetByDateWithSecurity(date);

            return dailyTurnover
                .GroupBy(dp => dp.Hv.Izdavach.Sektor.Ime)
                .Select(g => new SectorDistribution
                {
                    SectorName = g.Key,
                    MarketCap = g.Sum(dp => dp.Hv.VkupenBrojAkcii * (dp.CenaPoslednaTransakcija ?? 0))
                })
                .ToList();
        }

        public IEnumerable<MonthlyIncome> GetMonthlyIncome(int portfolioId, bool isReal)
        {
            Portfolija portfolio = GetPortfolioById(portfolioId);

            IEnumerable<Transakcii> transactions = _transactionsRepository.GetByPortfolioId(portfolioId);

            List<MonthlyIncome> monthlyIncome = transactions
                .Where(t => t.TipTransakcija == "Продавање" && t.Realna == isReal)
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

        public IEnumerable<SecurityAllocation> GetAllocation(int portfolioId, bool isReal)
        {
            Portfolija portfolio = GetPortfolioById(portfolioId);

            IEnumerable<Transakcii> transactions = _transactionsRepository.GetByPortfolioId(portfolioId);

            List<SecurityAllocation> securityAllocation = transactions
                .Where(t => t.Realna == isReal)
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

        public IEnumerable<Security> GetSecurities()
        {
            IEnumerable<HartiiOdVrednost> foundSecurities = _securitiesRepository.GetAll();
            return foundSecurities.Select(security => new Security
            {
                Id = security.Id,
                Code = security.Kod
            });
        }

        public DateTime FindLatestDate()
        {
            return _dailyTurnoverRepository.GetLatestDate();
        }

        private HartiiOdVrednost GetSecurityById(int securityId)
        {
            HartiiOdVrednost? security = _securitiesRepository.GetById(securityId);
            if (security == null)
                throw new NotFoundException("SECURITY_NOT_FOUND", "Security not found.");

            return security;
        }

        private Portfolija GetPortfolioById(int portfolioId)
        {
            Portfolija? portfolio = _portfoliosRepository.GetById(portfolioId);
            if (portfolio == null)
                throw new NotFoundException("PORTFOLIO_NOT_FOUND", "Portfolio not found.");

            return portfolio;
        }

        private void ValidateDateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ValidationException("INVALID_DATE_RANGE", "Start date cannot be after end date.");
        }
    }
}
