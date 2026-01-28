using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class PortfolioReturnsService : IPortfolioReturnsService
    {
        private readonly IPortfolioReturnsRepository _portfolioReturnsRepository;

        public PortfolioReturnsService(IPortfolioReturnsRepository portfolioReturnsRepository)
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
    }
}
