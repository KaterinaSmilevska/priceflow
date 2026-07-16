using DataAccess.Models;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class PortfolioReturnsService : IPortfolioReturnsService
    {
        private readonly DataAccess.Repositories.IPortfolioReturnsRepository _portfolioReturnsRepository;

        public PortfolioReturnsService(DataAccess.Repositories.IPortfolioReturnsRepository portfolioReturnsRepository)
        {
            _portfolioReturnsRepository = portfolioReturnsRepository;
        }

        public async Task<PortfolioReturnsSummary> CalculateSummaryAsync(int portfolioId)
        {
            IEnumerable<PortfolioPrinosi> returns = await _portfolioReturnsRepository.GetByPortfolioIdAsync(portfolioId);

            return new PortfolioReturnsSummary
            {
                    TotalDividends = returns.Sum(x => x.NetoIznos),
                    TotalTaxes = returns.Sum(x => x.Danok)
            };
        }

        public async Task<PortfolioReturnsSummary> CalculateSummaryForPeriodAsync(int portfolioId, DateOnly from, DateOnly to)
        {
            IEnumerable<PortfolioPrinosi> returns = await _portfolioReturnsRepository.GetByPortfolioIdForPeriod(portfolioId, from, to);

            return new PortfolioReturnsSummary
            {
                TotalDividends = returns.Sum(x => x.NetoIznos),
                TotalTaxes = returns.Sum(x => x.Danok)
            };
        }

        public async Task<PortfolioReturns> CreateAsync(PortfolioReturns portfolioReturns)
        {
            var entity = new PortfolioPrinosi
            {
                Datum = portfolioReturns.Date,
                NetoIznos = portfolioReturns.NetAmount,
                Danok = portfolioReturns.Tax,
                PortfolioId = portfolioReturns.PortfolioId,
                Hvid = portfolioReturns.HVId,
            };

            await _portfolioReturnsRepository.AddAsync(entity);

            return portfolioReturns;
        }

        public async Task<IEnumerable<PortfolioReturns>> FindByPortfolioId(int portfolioId)
        {
            IEnumerable<PortfolioPrinosi> entities = await _portfolioReturnsRepository.GetByPortfolioIdAsync(portfolioId);

            return entities.Select(e => new PortfolioReturns
            {
                Date = e.Datum,
                NetAmount = e.NetoIznos,
                Tax = e.Danok,
                PortfolioId = e.PortfolioId,
                HVId = e.Hvid
            });
        }
    }
}
