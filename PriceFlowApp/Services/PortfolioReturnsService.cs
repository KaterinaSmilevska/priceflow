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
            Portfolija? portfolio = GetPortfolio(portfolioId);

            IEnumerable<PortfolioPrinosi?> entities = _portfolioReturnsRepository.GetByPortfolioId(portfolioId);

            return entities.Select(e => new PortfolioReturns
            {
                Date = e.Datum,
                NetAmount = e.NetoIznos,
                Tax = e.Danok,
                PortfolioId = e.PortfolioId,
                HVId = e.Hvid
            });
        }

        public PortfolioReturns Add(PortfolioReturns portfolioReturns)
        {
            Portfolija portfolio = GetPortfolio(portfolioReturns.PortfolioId);

            HartiiOdVrednost? security = _securitiesRepository.GetById(portfolioReturns.HVId);
            if (security == null)
                throw new NotFoundException("SECURITY_NOT_FOUND", "Security not found.");

            var entity = new PortfolioPrinosi
            {
                Datum = portfolioReturns.Date,
                NetoIznos = portfolioReturns.NetAmount,
                Danok = portfolioReturns.Tax,
                PortfolioId = portfolioReturns.PortfolioId,
                Hvid = portfolioReturns.HVId,
            };

            PortfolioPrinosi createdPortfolioReturns = _portfolioReturnsRepository.Add(entity);

            return new PortfolioReturns
            {
                Date = createdPortfolioReturns.Datum,
                NetAmount = createdPortfolioReturns.NetoIznos,
                Tax = createdPortfolioReturns.Danok,
                PortfolioId = createdPortfolioReturns.PortfolioId,
                HVId = createdPortfolioReturns.Hvid
            };
        }

        public PortfolioReturnsSummary CalculateSummary(int portfolioId)
        {
            Portfolija portfolio = GetPortfolio(portfolioId);

            IEnumerable<PortfolioPrinosi?> returns = _portfolioReturnsRepository.GetByPortfolioId(portfolioId);

            return new PortfolioReturnsSummary
            {
                    TotalDividends = returns.Sum(x => x.NetoIznos),
                    TotalTaxes = returns.Sum(x => x.Danok)
            };
        }

        public PortfolioReturnsSummary CalculateSummaryForPeriod(int portfolioId, DateOnly from, DateOnly to)
        {
            Portfolija portfolio = GetPortfolio(portfolioId);

            if (from > to)
                throw new ValidationException("INVALID_DATE_RANGE", "From date cannot be after to date.");

            IEnumerable<PortfolioPrinosi?> returns = _portfolioReturnsRepository.GetByPortfolioIdForPeriod(portfolioId, from, to);

            return new PortfolioReturnsSummary
            {
                TotalDividends = returns.Sum(x => x.NetoIznos),
                TotalTaxes = returns.Sum(x => x.Danok)
            };
        }

        private Portfolija GetPortfolio(int portfolioId)
        {
            Portfolija? portfolio = _portfoliosRepository.GetById(portfolioId);

            if(portfolio == null)
                throw new NotFoundException("PORTFOLIO_NOT_FOUND", "Portfolio not found.");

            return portfolio;
        }
    }
}
