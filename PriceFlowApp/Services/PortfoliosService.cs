using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;

namespace PriceFlowApp.Services
{
    public class PortfoliosService : IPortfoliosService
    {
        private readonly IPortfoliosRepository _portfoliosRepository;
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IDailyTurnoverRepository _dailyTurnoverRepository;
        private readonly IPortfolioReturnsService _portfolioReturnsService;
        private readonly IAuthRepository _authRepository;

        public PortfoliosService(IPortfoliosRepository portfoliosRepository, ITransactionsRepository transactionsRepository,
            IDailyTurnoverRepository dailyTurnoverRepository, IPortfolioReturnsService portfolioReturnsService, IAuthRepository authRepository)
        {
            _portfoliosRepository = portfoliosRepository;
            _transactionsRepository = transactionsRepository;
            _dailyTurnoverRepository = dailyTurnoverRepository;
            _portfolioReturnsService = portfolioReturnsService;
            _authRepository = authRepository;
        }

        public Portfolio FindById(int id)
        {
            Portfolija portfolio = GetPortfolio(id);

            return new Portfolio
            {
                Id = portfolio.Id,
                Name = portfolio.Ime,
                Description = portfolio.Opis
            };
        }

        public IEnumerable<Portfolio> FindUserPortfolios(int userId)
        {
            Korisnici user = GetUser(userId);

            IEnumerable<Portfolija?> items = _portfoliosRepository.GetByUserId(userId);

            return items.Select(p => new Portfolio
            {
                Id = p.Id,
                Name = p.Ime,
                Description = p.Opis
            }).ToList();
        }

        public Portfolio Add(int userId, CreatePortfolio portfolio)
        {
            Korisnici user = GetUser(userId);

            if (string.IsNullOrWhiteSpace(portfolio.Name))
                throw new ValidationException("PORTFOLIO_NAME_REQUIRED", "Portfolio name cannot be null or empty.");

            var entity = new Portfolija
            {
                Ime = portfolio.Name,
                Opis = portfolio.Description,
                KorisnikId = userId
            };

            var createdPortfolio = _portfoliosRepository.Add(entity);

            return new Portfolio
            {
                Id = createdPortfolio.Id,
                Name = createdPortfolio.Ime,
                Description = createdPortfolio.Opis
            };
        }

        public Portfolio Update(int userId, UpdatePortfolio portfolio)
        {
            var existingPortfolio = GetPortfolio(portfolio.Id);

            if (existingPortfolio.KorisnikId != userId)
                throw new UnauthorizedException("PORTFOLIO_ACCESS_DENIED", "You do not have access to this portfolio.");

            if (string.IsNullOrWhiteSpace(portfolio.Name))
                throw new ValidationException("PORTFOLIO_NAME_REQUIRED", "Portfolio name cannot be null or empty.");

            existingPortfolio.Ime = portfolio.Name;
            existingPortfolio.Opis = portfolio.Description;

            var updatedPortfolio = _portfoliosRepository.Update(existingPortfolio);

            return new Portfolio
            {
                Id = updatedPortfolio.Id,
                Name = updatedPortfolio.Ime,
                Description = updatedPortfolio.Opis
            };
        }

        public Portfolio Delete(int id, int userId)
        {
            Portfolija existingPortfolio = GetPortfolio(id);

            if (existingPortfolio.KorisnikId != userId)
                throw new UnauthorizedException("ACCESS_DENIED", "You cannot access this portfolio.");

            _portfoliosRepository.Delete(existingPortfolio);

            return new Portfolio
            {
                Id = existingPortfolio.Id,
                Name = existingPortfolio.Ime,
                Description = existingPortfolio.Opis
            };
        }

        public PortfolioPerformanceSummary GeneratePerformanceSummary(int portfolioId, DateOnly from, DateOnly to)
        {
            Portfolija portfolio = GetPortfolio(portfolioId);

            IEnumerable<Transakcii?> transactions = _transactionsRepository.GetByPortfolioIdUntilDate(portfolioId, to);

            Dictionary<int, decimal> holdingsAtStart = CalculateHoldingsUntilDate(transactions, from);

            Dictionary<int, decimal> holdingsAtEnd = CalculateHoldingsUntilDate(transactions, to);

            decimal startingValue = CalculatePortfolioValue(holdingsAtStart, from);

            decimal endingValue = CalculatePortfolioValue(holdingsAtEnd, to);

            PortfolioReturnsSummary returns = _portfolioReturnsService.CalculateSummaryForPeriod(portfolioId, from, to);
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

        private decimal CalculatePortfolioValue(Dictionary<int, decimal> holdings, DateOnly date)
        {
            decimal totalValue = 0;

            foreach(var holding in holdings)
            {
                decimal? price = GetLatestPrice(holding.Key, date);
                totalValue += (decimal)(holding.Value * price);
            }

            return totalValue;
        }

        private decimal? GetLatestPrice(int securityId, DateOnly date)
        {
            decimal? price = _dailyTurnoverRepository.GetLatestPrice(securityId, date);

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

        private Portfolija GetPortfolio(int portfolioId)
        {
            var portfolio = _portfoliosRepository.GetById(portfolioId);
            if (portfolio == null)
                throw new NotFoundException("PORTFOLIO_NOT_FOUND", "Portfolio not found.");

            return portfolio;
        }

        private Korisnici GetUser(int userId)
        {
            var user = _authRepository.GetById(userId);
            if (user == null)
                throw new NotFoundException("USER_NOT_FOUND", "User not found.");

            return user;
        }
    }
}
