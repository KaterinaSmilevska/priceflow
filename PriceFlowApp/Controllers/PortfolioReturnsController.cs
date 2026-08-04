using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/portfolio-returns")]
    public class PortfolioReturnsController: ControllerBase
    {
        private readonly IPortfolioReturnsService _portfolioReturnsService;

        public PortfolioReturnsController(IPortfolioReturnsService portfolioReturnsService)
        {
            _portfolioReturnsService = portfolioReturnsService;
        }

        [HttpGet("{portfolioId}")]
        public ActionResult<IEnumerable<PortfolioReturns>> GetByPortfolioId(int portfolioId)
        {
            try
            {
                IEnumerable<PortfolioReturns> returns = _portfolioReturnsService.FindByPortfolioId(portfolioId);

                return Ok(returns);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting portfolio returns.", detail = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create(PortfolioReturns portfolioReturns)
        {
            try
            {
                PortfolioReturns returns = _portfolioReturnsService.Add(portfolioReturns);

                return Ok(returns);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating portfolio returns.", detail = ex.Message });
            }
        }

        [HttpGet("summary/{portfolioId}")]
        public ActionResult<PortfolioReturnsSummary> GetSummary(int portfolioId)
        {
            try
            {
                PortfolioReturnsSummary summary = _portfolioReturnsService.CalculateSummary(portfolioId);

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting portfolio summary.", detail = ex.Message });
            }
        }
    }
}
