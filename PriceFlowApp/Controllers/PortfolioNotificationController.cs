using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/portfolio-notification")]
    [Authorize]
    public class PortfolioNotificationController: PriceFlowController
    {
        private readonly IPortfoliosNotificationsService _portfoliosNotificationsService;

        public PortfolioNotificationController(IPortfoliosNotificationsService portfoliosNotificationsService)
        {
            _portfoliosNotificationsService = portfoliosNotificationsService;
        }

        [HttpGet("{portfolioId}")]
        public ActionResult<PortfolioNotification> GetByPortfolioId(int portfolioId)
        {
            return Execute(() => _portfoliosNotificationsService.FindByPortfolioId(portfolioId));
        }

        [HttpPut]
        public ActionResult<PortfolioNotification> Update([FromBody] UpdatePortfolioNotification notification)
        {
            return Execute(() => _portfoliosNotificationsService.Update(notification));
        }
    }
}
