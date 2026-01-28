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

        public PortfolioReturnsController(IPortfolioReturnsService portfolioReturnsService )
        => _portfolioReturnsService = portfolioReturnsService;
        

        [HttpPost]
        public async Task<IActionResult> Create(PortfolioReturns portfolioReturns)
        {
            try
            {
                PortfolioReturns returns = await _portfolioReturnsService.CreateAsync(portfolioReturns);
                return Ok(returns);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating portfolio returns.", detail = ex.Message });
            }
        }

        [HttpGet("summary/{portfolioId}")]
        public async Task<ActionResult<PortfolioReturnsSummary>> GetSummary(int portfolioId)
        {
            try
            {
                PortfolioReturnsSummary summary = await _portfolioReturnsService.CalculateSummaryAsync(portfolioId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting portfolio summary.", detail = ex.Message });
            }
        }
    }
}
