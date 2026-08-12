using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChartController : PriceFlowController
    {
        private readonly IChartService _chartService;

        public ChartController(IChartService chartService)
        {
            _chartService = chartService;
        } 

        [HttpGet("price-trend")]
        public ActionResult<IEnumerable<PriceTrend>> GetPriceTrend(int securityId, DateTime startDate, DateTime endDate)
        {
            return Execute(() => _chartService.GetPriceTrend(securityId, startDate, endDate));
        }

        [HttpGet("sector-distribution")]
        public ActionResult<IEnumerable<SectorDistribution>> GetSectorDistribution(DateTime date)
        {
            return Execute(() => _chartService.GetSectorDistribution(date));
        }

        [HttpGet("portfolio-income")]
        public ActionResult<IEnumerable<MonthlyIncome>> GetMonthlyIncome(int portfolioId, [FromQuery] bool isReal)
        {
            return Execute(() => _chartService.GetMonthlyIncome(portfolioId, isReal));
        }

        [HttpGet("portfolio-security-allocation")]
        public ActionResult<IEnumerable<SecurityAllocation>> GetSecurityAllocation(int portfolioId, bool isReal)
        {
            return Execute(() => _chartService.GetAllocation(portfolioId, isReal));
        }

        [HttpGet("securities")]
        public ActionResult<IEnumerable<Security>> GetSecurities()
        {
            return Execute(() => _chartService.GetSecurities());
        }

        [HttpGet("/latest-date")]
        public ActionResult<DateTime> GetLatestDate()
        {
            return Execute(() => _chartService.FindLatestDate());
        }
    }
}
