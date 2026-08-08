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

        public ChartController(IChartService chartService)
        {
            _chartService = chartService;
        } 

        [HttpGet("price-trend")]
        public ActionResult<IEnumerable<PriceTrend>> GetPriceTrend(int securityId, DateTime startDate, DateTime endDate)
        {
            try
            {
                IEnumerable<PriceTrend> data = _chartService.GetPriceTrend(securityId, startDate, endDate);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching price trends.", detail = ex.Message });
            }
        }

        [HttpGet("sector-distribution")]
        public ActionResult<IEnumerable<SectorDistribution>> GetSectorDistribution(DateTime date)
        {
            try
            {
                IEnumerable<SectorDistribution> data = _chartService.GetSectorDistribution(date);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching sector distribution.", detail = ex.Message });
            }
        }

        [HttpGet("portfolio-income")]
        public ActionResult<IEnumerable<MonthlyIncome>> GetMonthlyIncome(int portfolioId, [FromQuery] bool isReal)
        {
            try
            {
                IEnumerable<MonthlyIncome> monthlyIncome = _chartService.GetMonthlyIncome(portfolioId, isReal);

                return Ok(monthlyIncome);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching monthly income for portfolio.", detail = ex.Message });
            }
        }

        [HttpGet("portfolio-security-allocation")]
        public ActionResult<IEnumerable<MonthlyIncome>> GetSecurityAllocation(int portfolioId, bool isReal)
        {
            try
            {
                IEnumerable<SecurityAllocation> securityAllocation = _chartService.GetAllocation(portfolioId, isReal);

                return Ok(securityAllocation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching security allocation for portfolio.", detail = ex.Message });
            }
        }

        [HttpGet("securities")]
        public ActionResult<IEnumerable<Security>> GetSecurities()
        {
            try
            {
                IEnumerable<Security> securities = _chartService.GetSecurities();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("/latest-date")]
        public ActionResult<DateTime> GetLatestDate()
        {
            var latestDate = _chartService.FindLatestDate();

            if (latestDate == default)
                return NotFound("Latest date not found.");

            return Ok(latestDate.ToString());
        }
    }
}
