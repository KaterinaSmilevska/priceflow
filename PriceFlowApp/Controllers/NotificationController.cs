using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByUserId()
        {
            try
            {
                int userId = User.GetUserId();

                await _notificationService.CheckAndGenerateNotificationsAsync();

                List<NotificationResponse> notifications = await _notificationService.GetUserNotificationsAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching notifications for user.", detail = ex.Message });
            }
        }

        [HttpPost("debug-generate")]
        public async Task<IActionResult> DebugGenerate()
        {
            try
            {
                int userId = User.GetUserId();

                await _notificationService.CheckAndGenerateNotificationsAsync();

                List<NotificationResponse> notifications = await _notificationService.GetUserNotificationsAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching notifications for user.", detail = ex.Message });
            }
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                int userId = User.GetUserId();

                int unread = await _notificationService.GetUnreadNotificationCountAsync(userId);
                return Ok(unread);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching unread notifications count for user.", detail = ex.Message });
            }
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            { 
                await _notificationService.MarkNotificationAsReadAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error marking notification as read.", detail = ex.Message });
            }
        }        
    }
}
