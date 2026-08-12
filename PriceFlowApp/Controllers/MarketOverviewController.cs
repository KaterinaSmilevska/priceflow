using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketOverviewController : PriceFlowController
    {
        private readonly IMarketOverviewService _marketOverviewService;

        public MarketOverviewController(IMarketOverviewService marketOverviewService)
        {
            _marketOverviewService = marketOverviewService;
        }

        [HttpGet]
        public ActionResult<MarketOverview> GetMarketOverview()
        {
            return Execute(() => _marketOverviewService.GetOverview());
        }

        [HttpGet("top-gainers")]
        public ActionResult<IEnumerable<SecurityPerformance>> GetTopGainers([FromQuery] int count)
        {
            return Execute(() => _marketOverviewService.GetTopGainers(count));
        }

        [HttpGet("top-losers")]
        public ActionResult<IEnumerable<SecurityPerformance>> GetTopLosers([FromQuery] int count)
        {
            return Execute(() => _marketOverviewService.GetTopLosers(count));
        }

        [HttpGet("most-traded")]
        public ActionResult<IEnumerable<SecurityPerformance>> GetMostTraded([FromQuery] int count)
        {
            return Execute(() => _marketOverviewService.GetMostTrade(count));
        }

        [HttpGet("liquidity")]
        public ActionResult<LiquidityOverview> GetLiquidity([FromQuery] int months = 6, [FromQuery] bool onlyOwned = true)
        {
            return Execute(() =>
            {
                int userId = User.GetUserId();
                return _marketOverviewService.FindLiquidity(userId, months, onlyOwned);
            });
        }
    }
}
