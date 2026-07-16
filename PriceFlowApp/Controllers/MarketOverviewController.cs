using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketOverviewController : ControllerBase
    {

        private readonly IMarketOverviewService _marketOverviewService;

        public MarketOverviewController(IMarketOverviewService marketOverviewService)
        {
            _marketOverviewService = marketOverviewService;
        }

        [HttpGet]
        public async Task<ActionResult<MarketOverview>> GetMarketOverview()
        {
            try
            {
                MarketOverview overview = await _marketOverviewService.GetOverviewAsync();

                return Ok(overview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error has occured while fetching market overview.", detail = ex.Message });
            }

        }

        [HttpGet("top-gainers")]
        public async Task<IActionResult> GetTopGainers([FromQuery] int count)
        {
            try
            {
                IEnumerable<SecurityPerformance> securityPerformance = await _marketOverviewService.GetTopGainersAsync(count);

                return Ok(securityPerformance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error has occured while fetching top gainers.", detail = ex.Message });
            }
        }

        [HttpGet("top-losers")]
        public async Task<IActionResult> GetTopLosers([FromQuery] int count)
        {
            try
            {
                IEnumerable<SecurityPerformance> securityPerformance = await _marketOverviewService.GetTopLosersAsync(count);

                return Ok(securityPerformance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error has occured while fetching top losers.", detail = ex.Message });
            }
        }

        [HttpGet("most-traded")]
        public async Task<IActionResult> GetMostTraded([FromQuery] int count)
        {
            try
            {
                IEnumerable<SecurityPerformance> securityPerformance = await _marketOverviewService.GetMostTradedAsync(count);

                return Ok(securityPerformance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error has occured while fetching most traded securities.", detail = ex.Message });
            }
        }

        [HttpGet("liquidity")]
        public async Task<ActionResult<LiquidityOverview>> GetLiquidity([FromQuery] int months = 6, [FromQuery] bool onlyOwned = true)
        {
            try
            {
                int userId = User.GetUserId();
                LiquidityOverview result = await _marketOverviewService.FindLiquidityAsync(userId, months, onlyOwned);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities liquidity.", detail = ex.Message });
            }
        }
    }
}
