using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IPriceChangeNotificationService
    {
        Task<List<PriceChangeNotificationResponse>> GetUserNotificationsAsync(int userId);

        Task<int> GetUnreadNotificationCountAsync(int userId);

        Task MarkNotificationAsReadAsync(int notificationId);

        Task CheckAndGenerateNotificationsAsync();
    }
}
