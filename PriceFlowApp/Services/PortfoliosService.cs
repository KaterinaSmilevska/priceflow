using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Helpers;

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
            Portfolija portfolio = GetPortfolioById(id);

            return MapToPortfolio(portfolio);
        }

        public IEnumerable<Portfolio> FindUserPortfolios(int userId)
        {
            Korisnici user = GetUserById(userId);

            IEnumerable<Portfolija> portfolios = _portfoliosRepository.GetByUserId(user.Id);

            return portfolios
                .Select(MapToPortfolio)
                .ToList();
        }

        public Portfolio Add(int userId, AddPortfolioRequest request)
        {
            Korisnici user = GetUserById(userId);

            ValidationHelper.ValidateRequiredField(request.Name, "Name", "NAME_VALIDATION_REQUIRED");
            ValidateNameAvailability(request.Name, user.Id);

            Portfolija portfolio = new Portfolija
            {
                Ime = request.Name,
                Opis = request.Description,
                KorisnikId = user.Id
            };

            Portfolija addedPortfolio = _portfoliosRepository.Add(portfolio);

            return MapToPortfolio(addedPortfolio);
        }

        public Portfolio Update(int id, int userId, UpdatePortfolio portfolio)
        {
            ValidationHelper.ValidateRequiredField(portfolio.Name, "Name", "NAME_VALIDATION_REQUIRED");
            ValidateNameAvailability(portfolio.Name, userId, id);
            
            Portfolija existingPortfolio = GetPortfolioById(id);

            existingPortfolio.Ime = portfolio.Name;
            existingPortfolio.Opis = portfolio.Description;

            Portfolija updatedPortfolio = _portfoliosRepository.Update(existingPortfolio);

            return MapToPortfolio(updatedPortfolio);
        }

        public Portfolio Delete(int id, int userId)
        {
            Portfolija existingPortfolio = GetPortfolioById(id);

            if (existingPortfolio.KorisnikId != userId)
                throw new UnauthorizedException("ACCESS_DENIED", "You cannot access this portfolio.");

            Portfolija deletedPortfolio = _portfoliosRepository.Delete(existingPortfolio);

            return MapToPortfolio(deletedPortfolio);
        }

        public PortfolioPerformanceSummary GeneratePerformanceSummary(int portfolioId, DateOnly from, DateOnly to)
        {
            Portfolija portfolio = GetPortfolioById(portfolioId);

            IEnumerable<Transakcii?> transactions = _transactionsRepository.GetByPortfolioIdUntilDate(portfolio.Id, to);

            Dictionary<int, decimal> holdingsAtStart = CalculateHoldingsUntilDate(transactions, from);

            Dictionary<int, decimal> holdingsAtEnd = CalculateHoldingsUntilDate(transactions, to);

            decimal startingValue = CalculatePortfolioValue(holdingsAtStart, from);

            decimal endingValue = CalculatePortfolioValue(holdingsAtEnd, to);

            PortfolioReturnsSummary returns = _portfolioReturnsService.CalculateSummaryForPeriod(portfolio.Id, from, to);
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

        private Portfolija GetPortfolioById(int portfolioId)
        {
            Portfolija? portfolio = _portfoliosRepository.GetById(portfolioId);
            if (portfolio == null)
                throw new NotFoundException("PORTFOLIO_NOT_FOUND", "Portfolio not found.");

            return portfolio;
        }

        private Korisnici GetUserById(int userId)
        {
            Korisnici? user = _authRepository.GetById(userId);
            if (user == null)
                throw new NotFoundException("USER_NOT_FOUND", "User not found.");

            return user;
        }

        private void ValidateNameAvailability(string name, int userId, int? portfolioId = null)
        {
            Portfolija? existingPortfolio = _portfoliosRepository.GetByName(name, userId);
            if (existingPortfolio != null && existingPortfolio.Id != portfolioId && existingPortfolio.KorisnikId == userId)
                throw new AlreadyExistsException("NAME_ALREADY_EXISTS", "Portfolio already exists for this user.");
        }

        private Portfolio MapToPortfolio(Portfolija portfolio)
        {
            return new Portfolio
            {
                Id = portfolio.Id,
                Name = portfolio.Ime,
                Description = portfolio.Opis
            };
        }
    }
}
