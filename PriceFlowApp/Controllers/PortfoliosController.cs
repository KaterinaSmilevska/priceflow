using DataAccess.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [Authorize(Roles = "Инвеститор")]
    [ApiController]
    [Route("api/[controller]")]
    public class PortfoliosController : PriceFlowController
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
        public ActionResult<Portfolio> GetById(int id)
        {
            return Execute(() => _portfoliosService.FindById(id));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Portfolio>> GetAll()
        {
            return Execute(() =>
            {
                int userId = User.GetUserId();
                return _portfoliosService.FindUserPortfolios(userId);
            });
        }

        [HttpPost]
        public ActionResult<Portfolio> Add([FromBody] AddPortfolioRequest portfolio)
        {
            return Execute(() =>
            {
                int userId = User.GetUserId();
                return _portfoliosService.Add(userId, portfolio);
            });
        }

        [HttpPut("{id}")]
        public ActionResult<Portfolio> Update(int id, [FromBody] UpdatePortfolio portfolio)
        {
            if(id != portfolio.Id)
                return BadRequest(new {message = "Portfolio Id mismatch."});

            return Execute(() =>
            {
                int userId = User.GetUserId();
                return _portfoliosService.Update(id, userId, portfolio);
            });
        }

        [HttpDelete("{id}")]
        public ActionResult<Portfolio> Delete(int id)
        {
            return Execute(() =>
            {
                int userId = User.GetUserId();
                return _portfoliosService.Delete(id, userId);
            });
        }

        [HttpGet("securities-price-trend")]
        public ActionResult<IEnumerable<OwnedSecuritiesPriceTrend>> GetSecuritiesPriceTrend([FromQuery] PriceTrendPeriod? period, [FromQuery] PriceTrendResolution? resolution)
        {
            return Execute(() =>
            {
                int userId = User.GetUserId();
                return _transactionsService.FindPriceTrend(userId, period, resolution);
            });
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
            catch (PriceFlowException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("securities-price-trend-report")]
        public ActionResult<IEnumerable<SecurityPriceTrendReport>> GetSecuritiesPriceTrendReport([FromQuery] PriceTrendPeriod? period, [FromQuery] PriceTrendResolution? resolution, [FromQuery] string? securityCode)
        {
            return Execute(() =>
            {
                int userId = User.GetUserId();
                return _transactionsService.GetSecuritiesPriceTrendReport(userId, period, resolution, securityCode);
            });
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
            catch (PriceFlowException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
