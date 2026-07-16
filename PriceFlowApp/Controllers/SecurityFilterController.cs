using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/security-filter")]
    [Authorize]
    public class SecurityFilterController : ControllerBase
    {
        private readonly ISecurityFilterService _securityFilterService;

        public SecurityFilterController(ISecurityFilterService securityFilterService)
        {
            _securityFilterService = securityFilterService;
        }

        [HttpGet("securities/most-profitable/by-dividend-yield")]
        public async Task<ActionResult<IEnumerable<FilteredSecurity>>> GetMostPriftableSecuritiesByDividendYield()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = await _securityFilterService.FindMostProfitableSecuritiesByDividendYieldAsync();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("securities/most-profitable/by-dividend-per-share")]
        public async Task<ActionResult<IEnumerable<FilteredSecurity>>> GetMostPriftableSecuritiesByDividendPerShare()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = await _securityFilterService.FindMostProfitableSecuritiesByDividendPerShareAsync();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("securities/price-oscillation/biggest")]
        public async Task<ActionResult<IEnumerable<FilteredSecurity>>> GetSecuritiesWithBiggestPriceOscillations()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = await _securityFilterService.FindSecuritiesWithBiggestPriceOscillationsAsync();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("securities/price-oscillation/smallest")]
        public async Task<ActionResult<IEnumerable<FilteredSecurity>>> GetSecuritiesWithSmallestPriceOscillations()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = await _securityFilterService.FindSecuritiesWithSmallestPriceOscillationsAsync();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("securities/liquidity/least-by-traded-quantity")]
        public async Task<ActionResult<IEnumerable<FilteredSecurity>>> GetLeastLiquidSecuritiesByTradedQuantity()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = await _securityFilterService.FindLeastLiquidSecuritiesByTradedQuantityAsync();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("securities/liquidity/most-by-traded-quantity")]
        public async Task<ActionResult<IEnumerable<FilteredSecurity>>> GetMostLiquidSecuritiesByTradedQuantity()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = await _securityFilterService.FindMostLiquidSecuritiesByTradedQuantityAsync();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("securities/liquidity/least-by-trading-days")]
        public async Task<ActionResult<IEnumerable<FilteredSecurity>>> GetLeastLiquidSecuritiesByNumTradingDays()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = await _securityFilterService.FindLeastLiquidSecuritiesByNumTradingDaysAsync();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("securities/liquidity/most-by-trading-days")]
        public async Task<ActionResult<IEnumerable<FilteredSecurity>>> GetMostLiquidSecuritiesByNumTradingDays()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = await _securityFilterService.FindMostLiquidSecuritiesByNumTradingDaysAsync();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("sectors/most-profitable/by-dividend-yield")]
        public async Task<ActionResult<IEnumerable<Sector>>> GetMostProfitableSectorsByDividendYield()
        {
            try
            {
                IEnumerable<Sector> sectors = await _securityFilterService.FindMostProfitableSectorsByDividendYieldAsync();

                return Ok(sectors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching sectors.", detail = ex.Message });
            }
        }

        [HttpGet("sectors/most-profitable/by-profit")]
        public async Task<ActionResult<IEnumerable<Sector>>> GetMostProfitableSectorsByProfit()
        {
            try
            {
                IEnumerable<Sector> sectors = await _securityFilterService.FindMostProfitableSectorsByProfitAsync();

                return Ok(sectors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching sectors.", detail = ex.Message });
            }
        }

        [HttpGet("securities/valuation")]
        public async Task<ActionResult<IEnumerable<FilteredSecurity>>> GetSecuritiesValuation()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = await _securityFilterService.FindSecuritiesValuationAsync();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }
    }
}
