using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChartController : ControllerBase
    {
        private readonly IChartService _chartService;

        public ChartController(IChartService chartService) => _chartService = chartService;

        [HttpGet("price-trend")]
        public async Task<ActionResult<IEnumerable<PriceTrend>>> GetPriceTrend(int securityId, DateTime startDate, DateTime endDate)
        {
            try
            {
                IEnumerable<PriceTrend> data = await _chartService.GetPriceTrendAsync(securityId, startDate, endDate);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching price trends.", detail = ex.Message });
            }
        }

        [HttpGet("sector-distribution")]
        public async Task<ActionResult<IEnumerable<SectorDistribution>>> GetSectorDistribution(DateTime date)
        {
            try
            {
                IEnumerable<SectorDistribution> data = await _chartService.GetSectorDistributionAsync(date);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching sector distribution.", detail = ex.Message });
            }
        }

        [HttpGet("securities")]
        public async Task<ActionResult<IEnumerable<Security>>> GetSecurities()
        {
            try
            {
                IEnumerable<Security> securities = await _chartService.GetSecurities();
                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("/latest-date")]
        public IActionResult GetLatestDate()
        {
            var latestDate = _chartService.FindLatestDate();

            if (latestDate == default)
                return NotFound("Latest date not found.");

            return Ok(latestDate.ToString());
        }
    }
}
