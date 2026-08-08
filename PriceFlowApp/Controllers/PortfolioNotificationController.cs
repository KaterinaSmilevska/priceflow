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
        public ActionResult<PortfolioNotification> GetByPortfolioId(int portfolioId)
        {
            try
            {
                PortfolioNotification notification = _portfoliosNotificationsService.FindByPortfolioId(portfolioId);

                return Ok(notification);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching notifications for portfolio.", detail = ex.Message });
            }
        }

        [HttpPut]
        public ActionResult<PortfolioNotification> Update([FromBody] UpdatePortfolioNotification notification)
        {
            try
            {
                PortfolioNotification result = _portfoliosNotificationsService.Update(notification);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating portfolio notification.", detail = ex.Message });
            }
        }
    }
}
