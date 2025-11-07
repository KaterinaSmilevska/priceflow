using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChartsController : ControllerBase
    {
        private readonly IChartService _chartService;

        public ChartsController(IChartService chartService) => _chartService = chartService;

        [HttpGet("price-trend")]
        public async Task<ActionResult<IEnumerable<PriceTrend>>> GetPriceTrend(int securityId, DateTime start, DateTime end)
        {
            try
            {
                IEnumerable<PriceTrend> data = await _chartService.GetPriceTrendAsync(securityId, start, end);
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
    }
}
