using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/portfolio-portfolioReturns")]
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
                return StatusCode(500, new { message = "Error getting portfolio portfolioReturns.", detail = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult<PortfolioReturns> Add(PortfolioReturns portfolioReturn)
        {
            try
            {
                PortfolioReturns portfolioReturns = _portfolioReturnsService.Add(portfolioReturn);

                return Ok(portfolioReturns);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating portfolio portfolioReturns.", detail = ex.Message });
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
