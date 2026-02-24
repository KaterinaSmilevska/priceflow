using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/portfolio-notification")]
    [Authorize]
    public class PortfolioNotificationController: ControllerBase
    {
        private readonly IPortfoliosNotificationsService _portfoliosNotificationsService;

        public PortfolioNotificationController(IPortfoliosNotificationsService portfoliosNotificationsService)
        {
            _portfoliosNotificationsService = portfoliosNotificationsService;
        }

        [HttpGet("{portfolioId}")]
        public async Task<IActionResult> GetByPortfolioId(int portfolioId)
        {
            try
            {
                PortfolioNotification? notification = await _portfoliosNotificationsService.FindByPortfolioId(portfolioId);
                return Ok(notification);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching notifications for portfolio.", detail = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult<PortfolioNotification>> Update([FromBody] UpdatePortfolioNotification notification)
        {
            try
            {
                PortfolioNotification result = await _portfoliosNotificationsService.UpdateAsync(notification);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating portfolio notification.", detail = ex.Message });
            }
        }
    }
}
