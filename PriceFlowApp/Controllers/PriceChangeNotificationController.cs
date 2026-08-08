using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/price-change-notification")]
    [Authorize]
    public class PriceChangeNotificationController : ControllerBase
    {
        private readonly IPriceChangeNotificationService _notificationService;

        public PriceChangeNotificationController(IPriceChangeNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult GetByUserId()
        {
            try
            {
                int userId = User.GetUserId();

                _notificationService.GenerateNotifications();

                IEnumerable<PriceChangeNotificationResponse> notifications = _notificationService.GetUserNotifications(userId);

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching notifications for user.", detail = ex.Message });
            }
        }

        [HttpGet("unread-count")]
        public ActionResult<int> GetUnreadCount()
        {
            try
            {
                int userId = User.GetUserId();

                int unread = _notificationService.GetUnreadNotificationCount(userId);

                return Ok(unread);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching unread notifications count for user.", detail = ex.Message });
            }
        }

        [HttpPost("{id}/read")]
        public IActionResult MarkAsRead(int id)
        {
            try
            {
                int userId = User.GetUserId();
                _notificationService.MarkNotificationAsRead(userId, id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error marking notification as read.", detail = ex.Message });
            }
        }

        [HttpPost("debug-generate")]
        public IActionResult DebugGenerate()
        {
            try
            {
                int userId = User.GetUserId();

                _notificationService.GenerateNotifications();

                IEnumerable<PriceChangeNotificationResponse> notifications = _notificationService.GetUserNotifications(userId);

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching notifications for user.", detail = ex.Message });
            }
        }
    }
}
