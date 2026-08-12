using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/portfolio-portfolioReturns")]
    public class PortfolioReturnsController: PriceFlowController
    {
        private readonly IPortfolioReturnsService _portfolioReturnsService;

        public PortfolioReturnsController(IPortfolioReturnsService portfolioReturnsService)
        {
            _portfolioReturnsService = portfolioReturnsService;
        }

        [HttpGet("{portfolioId}")]
        public ActionResult<IEnumerable<PortfolioReturns>> GetByPortfolioId(int portfolioId)
        {
            return Execute(() => _portfolioReturnsService.FindByPortfolioId(portfolioId));
        }

        [HttpPost]
        public ActionResult<PortfolioReturns> Add(PortfolioReturns portfolioReturn)
        {
            return Execute(() => _portfolioReturnsService.Add(portfolioReturn));
        }

        [HttpGet("summary/{portfolioId}")]
        public ActionResult<PortfolioReturnsSummary> GetSummary(int portfolioId)
        {
            return Execute(() => _portfolioReturnsService.CalculateSummary(portfolioId));
        }
    }
}
