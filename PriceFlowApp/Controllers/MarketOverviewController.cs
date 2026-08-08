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
        public ActionResult<MarketOverview> GetMarketOverview()
        {
            try
            {
                MarketOverview overview = _marketOverviewService.GetOverview();

                return Ok(overview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error has occured while fetching market overview.", detail = ex.Message });
            }
        }

        [HttpGet("top-gainers")]
        public ActionResult<IEnumerable<SecurityPerformance>> GetTopGainers([FromQuery] int count)
        {
            try
            {
                IEnumerable<SecurityPerformance> securityPerformance = _marketOverviewService.GetTopGainers(count);

                return Ok(securityPerformance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error has occured while fetching top gainers.", detail = ex.Message });
            }
        }

        [HttpGet("top-losers")]
        public ActionResult<IEnumerable<SecurityPerformance>> GetTopLosers([FromQuery] int count)
        {
            try
            {
                IEnumerable<SecurityPerformance> securityPerformance = _marketOverviewService.GetTopLosers(count);

                return Ok(securityPerformance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error has occured while fetching top losers.", detail = ex.Message });
            }
        }

        [HttpGet("most-traded")]
        public ActionResult<IEnumerable<SecurityPerformance>> GetMostTraded([FromQuery] int count)
        {
            try
            {
                IEnumerable<SecurityPerformance> securityPerformance = _marketOverviewService.GetMostTrade(count);

                return Ok(securityPerformance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error has occured while fetching most traded securities.", detail = ex.Message });
            }
        }

        [HttpGet("liquidity")]
        public ActionResult<LiquidityOverview> GetLiquidity([FromQuery] int months = 6, [FromQuery] bool onlyOwned = true)
        {
            try
            {
                int userId = User.GetUserId();
                LiquidityOverview result = _marketOverviewService.FindLiquidity(userId, months, onlyOwned);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities liquidity.", detail = ex.Message });
            }
        }
    }
}
