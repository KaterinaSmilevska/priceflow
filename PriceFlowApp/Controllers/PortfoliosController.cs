using DataAccess.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [Authorize(Roles = "Инвеститор")]
    [ApiController]
    [Route("api/[controller]")]
    public class PortfoliosController : ControllerBase
    {
        private readonly IPortfoliosService _portfoliosService;
        private readonly ITransactionsService _transactionsService;
        private readonly IPortfolioReportExportService _exportService;
        private readonly ISecurityPriceTrendReportService _securityPriceTrendReportService;

        public PortfoliosController(IPortfoliosService portfoliosService, ITransactionsService transactionsService,
            IPortfolioReportExportService exportService, ISecurityPriceTrendReportService securityPriceTrendReportService)
        {
            _portfoliosService = portfoliosService;
            _transactionsService = transactionsService;
            _exportService = exportService;
            _securityPriceTrendReportService = securityPriceTrendReportService;
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                int userId = User.GetUserId();
                Portfolio portfolio = _portfoliosService.FindById(id);

                return Ok(portfolio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching portfolios for user.", detail = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                int userId = User.GetUserId();
                IEnumerable<Portfolio> portfolios = _portfoliosService.FindUserPortfolios(userId);

                return Ok(portfolios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching portfolios for user.", detail = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] AddPortfolioRequest portfolio)
        {
            try
            {
                int userId = User.GetUserId();
                Portfolio createdPortfolio = _portfoliosService.Add(userId, portfolio);

                return Ok(createdPortfolio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating portfolio.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdatePortfolio portfolio)
        {
            try
            {
                int userId = User.GetUserId();
                Portfolio updatedPortfolio = _portfoliosService.Update(id, userId, portfolio);

                return Ok(updatedPortfolio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updading portoflio.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                int userId = User.GetUserId();
                _portfoliosService.Delete(id, userId);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting portoflio.", detail = ex.Message });
            }
        }

        [HttpGet("securities-price-trend")]
        public ActionResult<IEnumerable<OwnedSecuritiesPriceTrend>> GetSecuritiesPriceTrend([FromQuery] PriceTrendPeriod? period, [FromQuery] PriceTrendResolution? resolution)
        {
            try
            {
                int userId = User.GetUserId();
                IEnumerable<OwnedSecuritiesPriceTrend> result = _transactionsService.FindPriceTrend(userId, period, resolution);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching portfolio price trend.", detail = ex.Message });
            }
        }

        [HttpGet("{id}/performance-summary")]
        public IActionResult GetPerformanceSummary(int id, [FromQuery] DateOnly from, [FromQuery] DateOnly to, [FromQuery] string? format = null)
        {
            try
            {
                PortfolioPerformanceSummary result = _portfoliosService.GeneratePerformanceSummary(id, from, to);
                
                if(string.IsNullOrEmpty(format))
                    return Ok(result);

                if(format.ToLower() == "csv")
                {
                    byte[] bytes = _exportService.ExportToCsv(result);
                    return File(bytes, "text/csv", "portfolio_performance_summary.csv");
                }
                if(format.ToLower() == "excel")
                {
                    byte[] bytes = _exportService.ExportToExcel(result);
                    return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "portfolio_performance_summary.xlsx");
                }
                if (format.ToLower() == "pdf")
                {
                    byte[] bytes = _exportService.ExportToPDF(result);
                    return File(bytes, "application/pdf", "portfolio_performance_summary.pdf");
                }
                return BadRequest("Unsupported format.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching portfolio performance summary.", detail = ex.Message });
            }
        }

        [HttpGet("securities-price-trend-report")]
        public ActionResult<IEnumerable<SecurityPriceTrendReport>> GetSecuritiesPriceTrendReport([FromQuery] PriceTrendPeriod? period, [FromQuery] PriceTrendResolution? resolution, [FromQuery] string? securityCode)
        {
            try
            {
                int userId = User.GetUserId();
                IEnumerable<SecurityPriceTrendReport> result = _transactionsService.GetSecuritiesPriceTrendReport(userId, period, resolution, securityCode);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities price trend report.", detail = ex.Message });
            }
        }

        [HttpGet("securities-price-trend-report/pdf")]
        public ActionResult GenerateSecuritiesPriceTrendReport([FromQuery] PriceTrendPeriod? period, [FromQuery] PriceTrendResolution? resolution, [FromQuery] string? securityCode)
        {
            try
            {
                int userId = User.GetUserId();
                IEnumerable<SecurityPriceTrendReport> reports = _transactionsService.GetSecuritiesPriceTrendReport(userId, period, resolution, securityCode);

                byte[] pdf = _securityPriceTrendReportService.GenerateSecurityPriceTrendReport(reports);

                return File(pdf, "application/pdf", "SecuritiesPriceTrendReport.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error generating securities price trend report.", detail = ex.Message });
            }
        }
    }
}
