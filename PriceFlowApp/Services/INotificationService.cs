using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface INotificationService
    {
        Task<List<NotificationResponse>> GetUserNotificationsAsync(int userId);

        Task<int> GetUnreadNotificationCountAsync(int userId);

        Task MarkNotificationAsReadAsync(int notificationId);

        Task CheckAndGenerateNotificationsAsync();


    }
}
