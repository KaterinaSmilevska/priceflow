using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class PortfoliosService : IPortfoliosService
    {
        private readonly IPortfoliosRepository _portfolijaRepository;
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IDailyTurnoverRepository _dailyTurnoverRepository;
        private readonly IPortfolioReturnsService _portfolioReturnsService;

        public PortfoliosService(IPortfoliosRepository portfolijaRepository, ITransactionsRepository transactionsRepository,
            IDailyTurnoverRepository dailyTurnoverRepository, IPortfolioReturnsService portfolioReturnsService)
        {
            _portfolijaRepository = portfolijaRepository;
            _transactionsRepository = transactionsRepository;
            _dailyTurnoverRepository = dailyTurnoverRepository;
            _portfolioReturnsService = portfolioReturnsService;
        } 

        public async Task DeletePortfolio(int id, int userId)
        {
            var portfolio = await _portfolijaRepository.GetByIdAsync(id);
            if (portfolio == null || portfolio.KorisnikId != userId)
                throw new Exception("Portfolio not found");

            await _portfolijaRepository.DeleteAsync(portfolio);
        }

        public async Task<IEnumerable<Portfolio>> FindUserPortfoliosAsync(int userId)
        {
            IEnumerable<Portfolija?> items = await _portfolijaRepository.GetByUserAsync(userId);

            return items.Select(p => new Portfolio
            {
                Id = p.Id,
                Name = p.Ime,
                Description = p.Opis
            }).ToList();
        }

        public async Task<Portfolio> CreatePortfolio(int userId, CreatePortfolio portfolio)
        {
            Portfolija entity = new Portfolija
            {
                Ime = portfolio.Name,
                Opis = portfolio.Description,
                KorisnikId = userId
            };

            entity = await _portfolijaRepository.CreateAsync(entity);

            return new Portfolio
            {
                Id = entity.Id,
                Name = entity.Ime,
                Description = entity.Opis
            };
        }

        public async Task<Portfolio> UpdatePortfolio(int id, int userId, UpdatePortfolio portfolio)
        {
            var foundPortfolio = await _portfolijaRepository.GetByIdAsync(id);

            if (foundPortfolio == null || foundPortfolio.KorisnikId != userId)
                throw new Exception("Portfolio not found");

            foundPortfolio.Ime = portfolio.Name;
            foundPortfolio.Opis = portfolio.Description;

            var updated = await _portfolijaRepository.UpdateAsync(foundPortfolio);

            return new Portfolio
            {
                Id = updated.Id,
                Name = updated.Ime,
                Description = updated.Opis
            };
        }

        public async Task<Portfolio> FindById(int id)
        {
            Portfolija? portfolio = await _portfolijaRepository.GetByIdAsync(id);

            if (portfolio == null)
                throw new Exception("Portfolio not found");

            return new Portfolio
            {
                Id = portfolio.Id,
                Name = portfolio.Ime,
                Description = portfolio.Opis
            };
        }

        public async Task<PortfolioPerformanceSummary> GeneratePerformanceSummaryAsync(int portfolioId, DateOnly from, DateOnly to)
        {
            Portfolija? portfolio = await _portfolijaRepository.GetByIdAsync(portfolioId);

            if (portfolio == null)
                throw new Exception("Portfolio not found.");

            IEnumerable<Transakcii> transactions = await _transactionsRepository.GetByPortfolioUntilDateAsync(portfolioId, to);

            Dictionary<int, decimal> holdingsAtStart = CalculateHoldingsUntilDate(transactions, from);

            Dictionary<int, decimal> holdingsAtEnd = CalculateHoldingsUntilDate(transactions, to);

            decimal startingValue = await CalculatePortfolioValueAsync(holdingsAtStart, from);

            decimal endingValue = await CalculatePortfolioValueAsync(holdingsAtEnd, to);

            PortfolioReturnsSummary returns = await _portfolioReturnsService.CalculateSummaryForPeriodAsync(portfolioId, from, to);
            decimal dividends = returns.TotalDividends;

            decimal commissions = CalculateTotalCommissions(transactions, from, to);

            decimal absoluteReturn = (endingValue - startingValue) + dividends - commissions;

            return new PortfolioPerformanceSummary
            {
                PortfolioName = portfolio.Ime,
                FromDate = from,
                ToDate = to,
                StartingValue = startingValue,
                EndingValue = endingValue,
                Dividends = dividends,
                Commissions = commissions,
                AbsoluteReturn = absoluteReturn
            };
        }

        private Dictionary<int, decimal> CalculateHoldingsUntilDate(IEnumerable<Transakcii> transactions, DateOnly date)
        {
            Dictionary<int, decimal> holdings = new Dictionary<int, decimal>();

            foreach(Transakcii t in transactions.Where(t => t.Datum <= date))
            {
                if(!holdings.ContainsKey(t.Hvid))
                    holdings[t.Hvid] = 0;
                holdings[t.Hvid] +=
                    t.TipTransakcija == "Купување"
                    ? t.KolicinaAkcii
                    : -t.KolicinaAkcii;
            }

            return holdings;
        }

        private async Task<decimal> CalculatePortfolioValueAsync(Dictionary<int, decimal> holdings, DateOnly date)
        {
            decimal totalValue = 0;

            foreach(var holding in holdings)
            {
                decimal? price = await GetLatestPriceAsync(holding.Key, date);
                totalValue += (decimal)(holding.Value * price);
            }

            return totalValue;
        }

        private async Task<decimal> GetLatestPriceAsync(int securityId, DateOnly date)
        {
            decimal price = await _dailyTurnoverRepository.GetLatestPriceAsync(securityId, date);

            return price;
        }

        private decimal CalculateTotalCommissions(IEnumerable<Transakcii> transactions, DateOnly from, DateOnly to)
        {
            return transactions
                .Where(t => t.Datum >= from && t.Datum <= to)
                .Sum(t => CalculateCommission(t));
        }

        private decimal CalculateCommission(Transakcii transaction)
        {
            decimal tradeValue = transaction.Iznos;

            decimal commissionPercent = transaction.BrokerskaProvizija
                + transaction.BerzanskaProvizija
                + transaction.Cdhvprovizija;
            decimal commission = tradeValue * commissionPercent / 100;

            return Math.Round(commission, 2);
        }
    }
}
