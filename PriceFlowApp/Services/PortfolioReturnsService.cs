using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;

namespace PriceFlowApp.Services
{
    public class PortfolioReturnsService : IPortfolioReturnsService
    {
        private readonly IPortfolioReturnsRepository _portfolioReturnsRepository;
        private readonly IPortfoliosRepository _portfoliosRepository;
        private readonly ISecuritiesRepository _securitiesRepository;

        public PortfolioReturnsService(IPortfolioReturnsRepository portfolioReturnsRepository, IPortfoliosRepository portfoliosRepository, ISecuritiesRepository securitiesRepository)
        {
            _portfolioReturnsRepository = portfolioReturnsRepository;
            _portfoliosRepository = portfoliosRepository;
            _securitiesRepository = securitiesRepository;
        }

        public IEnumerable<PortfolioReturns> FindByPortfolioId(int portfolioId)
        {
            Portfolija portfolio = GetPortfolioById(portfolioId);

            IEnumerable<PortfolioPrinosi> portfolioReturns = _portfolioReturnsRepository.GetByPortfolioId(portfolio.Id);

            return portfolioReturns
                .Select(MapToPortfolioReturns)
                .ToList();
        }

        public PortfolioReturns Add(PortfolioReturns portfolioReturns)
        {
            Portfolija portfolio = GetPortfolioById(portfolioReturns.PortfolioId);

            HartiiOdVrednost security = GetBySecurityId(portfolioReturns.HVId);

            PortfolioPrinosi portfolioReturn = new PortfolioPrinosi
            {
                Datum = portfolioReturns.Date,
                NetoIznos = portfolioReturns.NetAmount,
                Danok = portfolioReturns.Tax,
                PortfolioId = portfolio.Id,
                Hvid = security.Id,
            };

            PortfolioPrinosi createdPortfolioReturns = _portfolioReturnsRepository.Add(portfolioReturn);

            return MapToPortfolioReturns(createdPortfolioReturns);
        }

        public PortfolioReturnsSummary CalculateSummary(int portfolioId)
        {
            Portfolija portfolio = GetPortfolioById(portfolioId);

            IEnumerable<PortfolioPrinosi> returns = _portfolioReturnsRepository.GetByPortfolioId(portfolio.Id);

            return new PortfolioReturnsSummary
            {
                TotalDividends = returns.Sum(x => x.NetoIznos),
                TotalTaxes = returns.Sum(x => x.Danok)
            };
        }

        public PortfolioReturnsSummary CalculateSummaryForPeriod(int portfolioId, DateOnly fromDate, DateOnly toDate)
        {
            Portfolija portfolio = GetPortfolioById(portfolioId);

            ValidateDateRange(fromDate, toDate);

            IEnumerable<PortfolioPrinosi> returns = _portfolioReturnsRepository.GetByPortfolioIdForPeriod(portfolio.Id, fromDate, toDate);

            return new PortfolioReturnsSummary
            {
                TotalDividends = returns.Sum(x => x.NetoIznos),
                TotalTaxes = returns.Sum(x => x.Danok)
            };
        }

        private Portfolija GetPortfolioById(int portfolioId)
        {
            Portfolija? portfolio = _portfoliosRepository.GetById(portfolioId);
            if(portfolio == null)
                throw new NotFoundException("PORTFOLIO_NOT_FOUND", "Portfolio not found.");

            return portfolio;
        }

        private HartiiOdVrednost GetBySecurityId(int securityId)
        {
            HartiiOdVrednost? security = _securitiesRepository.GetById(securityId);
            if (security == null)
                throw new NotFoundException("SECURITY_NOT_FOUND", "Security not found.");

            return security;
        }

        private void ValidateDateRange(DateOnly fromDate, DateOnly toDate)
        {
            if (fromDate > toDate)
                throw new ValidationException("INVALID_DATE_RANGE", "FromDate cannot be after ToDate.");
        }

        private PortfolioReturns MapToPortfolioReturns(PortfolioPrinosi portfolioReturns)
        {
            return new PortfolioReturns
            {
                Date = portfolioReturns.Datum,
                NetAmount = portfolioReturns.NetoIznos,
                Tax = portfolioReturns.Danok,
                PortfolioId = portfolioReturns.PortfolioId,
                HVId = portfolioReturns.Hvid
            };
        }
    }
}
