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
        public ActionResult<IEnumerable<FilteredSecurity>> GetMostPriftableSecuritiesByDividendYield()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = _securityFilterService.FindMostProfitableSecuritiesByDividendYield();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("securities/most-profitable/by-dividend-per-share")]
        public ActionResult<IEnumerable<FilteredSecurity>> GetMostPriftableSecuritiesByDividendPerShare()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = _securityFilterService.FindMostProfitableSecuritiesByDividendPerShare();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("securities/price-oscillation/biggest")]
        public ActionResult<IEnumerable<FilteredSecurity>> GetSecuritiesWithBiggestPriceOscillations()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = _securityFilterService.FindSecuritiesWithBiggestPriceOscillations();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("securities/price-oscillation/smallest")]
        public ActionResult<IEnumerable<FilteredSecurity>> GetSecuritiesWithSmallestPriceOscillations()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = _securityFilterService.FindSecuritiesWithSmallestPriceOscillations();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("securities/liquidity/least-by-traded-quantity")]
        public ActionResult<IEnumerable<FilteredSecurity>> GetLeastLiquidSecuritiesByTradedQuantity()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = _securityFilterService.FindLeastLiquidSecuritiesByTradedQuantity();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("securities/liquidity/most-by-traded-quantity")]
        public ActionResult<IEnumerable<FilteredSecurity>> GetMostLiquidSecuritiesByTradedQuantity()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = _securityFilterService.FindMostLiquidSecuritiesByTradedQuantity();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("securities/liquidity/least-by-trading-days")]
        public ActionResult<IEnumerable<FilteredSecurity>> GetLeastLiquidSecuritiesByNumTradingDays()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = _securityFilterService.FindLeastLiquidSecuritiesByNumTradingDays();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("securities/liquidity/most-by-trading-days")]
        public ActionResult<IEnumerable<FilteredSecurity>> GetMostLiquidSecuritiesByNumTradingDays()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = _securityFilterService.FindMostLiquidSecuritiesByNumTradingDays();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("sectors/most-profitable/by-dividend-yield")]
        public ActionResult<IEnumerable<Sector>> GetMostProfitableSectorsByDividendYield()
        {
            try
            {
                IEnumerable<Sector> sectors = _securityFilterService.FindMostProfitableSectorsByDividendYield();

                return Ok(sectors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching sectors.", detail = ex.Message });
            }
        }

        [HttpGet("sectors/most-profitable/by-profit")]
        public ActionResult<IEnumerable<Sector>> GetMostProfitableSectorsByProfit()
        {
            try
            {
                IEnumerable<Sector> sectors = _securityFilterService.FindMostProfitableSectorsByProfit();

                return Ok(sectors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching sectors.", detail = ex.Message });
            }
        }

        [HttpGet("securities/valuation")]
        public ActionResult<IEnumerable<FilteredSecurity>> GetSecuritiesValuation()
        {
            try
            {
                IEnumerable<FilteredSecurity> securities = _securityFilterService.FindSecuritiesValuation();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }
    }
}
