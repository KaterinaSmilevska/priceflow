using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/price-change-notification")]
    [Authorize]
    public class PriceChangeNotificationController : PriceFlowController
    {
        private readonly IPriceChangeNotificationService _notificationService;

        public PriceChangeNotificationController(IPriceChangeNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PriceChangeNotificationResponse>> GetByUserId()
        {
            return Execute(() =>
            {
                int userId = User.GetUserId();

                _notificationService.GenerateNotifications();

                return _notificationService.GetUserNotifications(userId);
            });
        }

        [HttpGet("unread-count")]
        public ActionResult<int> GetUnreadCount()
        {
            return Execute(() =>
            {
                int userId = User.GetUserId();
                return _notificationService.GetUnreadNotificationCount(userId);
            });
        }

        [HttpPost("{id}/read")]
        public IActionResult MarkAsRead(int id)
        {
            return Execute(() =>
            {
                int userId = User.GetUserId();
                _notificationService.MarkNotificationAsRead(userId, id);
            });
        }

        [HttpPost("debug-generate")]
        public ActionResult<IEnumerable<PriceChangeNotificationResponse>> DebugGenerate()
        {
            return Execute(() =>
            {
                int userId = User.GetUserId();

                _notificationService.GenerateNotifications();

                return _notificationService.GetUserNotifications(userId);
            });
        }
    }
}
