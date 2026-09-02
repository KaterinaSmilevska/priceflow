using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/security-filter")]
    [Authorize]
    public class SecurityFilterController : PriceFlowController
    {
        private readonly ISecurityFilterService _securityFilterService;

        public SecurityFilterController(ISecurityFilterService securityFilterService)
        {
            _securityFilterService = securityFilterService;
        }

        [HttpGet("securities/most-profitable/by-dividend-yield")]
        public ActionResult<IEnumerable<FilteredSecurity>> GetMostPriftableSecuritiesByDividendYield()
        {
            return Execute(() => _securityFilterService.FindMostProfitableSecuritiesByDividendYield());
        }

        [HttpGet("securities/most-profitable/by-dividend-per-share")]
        public ActionResult<IEnumerable<FilteredSecurity>> GetMostPriftableSecuritiesByDividendPerShare()
        {
            return Execute(() => _securityFilterService.FindMostProfitableSecuritiesByDividendPerShare());
        }

        [HttpGet("securities/price-oscillation/biggest")]
        public ActionResult<IEnumerable<FilteredSecurity>> GetSecuritiesWithBiggestPriceOscillations()
        {
            return Execute(() => _securityFilterService.FindSecuritiesWithBiggestPriceOscillations());
        }

        [HttpGet("securities/price-oscillation/smallest")]
        public ActionResult<IEnumerable<FilteredSecurity>> GetSecuritiesWithSmallestPriceOscillations()
        {
            return Execute(() => _securityFilterService.FindSecuritiesWithSmallestPriceOscillations());
        }

        [HttpGet("securities/liquidity/least-by-traded-quantity")]
        public ActionResult<IEnumerable<FilteredSecurity>> GetLeastLiquidSecuritiesByTradedQuantity()
        {
            return Execute(() => _securityFilterService.FindLeastLiquidSecuritiesByTradedQuantity());
        }

        [HttpGet("securities/liquidity/most-by-traded-quantity")]
        public ActionResult<IEnumerable<FilteredSecurity>> GetMostLiquidSecuritiesByTradedQuantity()
        {
            return Execute(() => _securityFilterService.FindMostLiquidSecuritiesByTradedQuantity());
        }

        [HttpGet("securities/liquidity/least-by-trading-days")]
        public ActionResult<IEnumerable<FilteredSecurity>> GetLeastLiquidSecuritiesByNumTradingDays()
        {
            return Execute(() => _securityFilterService.FindLeastLiquidSecuritiesByNumTradingDays());
        }

        [HttpGet("securities/liquidity/most-by-trading-days")]
        public ActionResult<IEnumerable<FilteredSecurity>> GetMostLiquidSecuritiesByNumTradingDays()
        {
            return Execute(() => _securityFilterService.FindMostLiquidSecuritiesByNumTradingDays());
        }

        [HttpGet("sectors/most-profitable/by-dividend-yield")]
        public ActionResult<IEnumerable<Sector>> GetMostProfitableSectorsByDividendYield()
        {
            return Execute(() => _securityFilterService.FindMostProfitableSectorsByDividendYield());
        }

        [HttpGet("sectors/most-profitable/by-profit")]
        public ActionResult<IEnumerable<Sector>> GetMostProfitableSectorsByProfit()
        {
            return Execute(() => _securityFilterService.FindMostProfitableSectorsByProfit());
        }

        [HttpGet("securities/valuation")]
        public ActionResult<IEnumerable<FilteredSecurity>> GetSecuritiesValuation()
        {
            return Execute(() => _securityFilterService.FindSecuritiesValuation());
        }
    }
}
