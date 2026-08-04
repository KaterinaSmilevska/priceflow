using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IPriceChangeNotificationService
    {
        IEnumerable<PriceChangeNotificationResponse> GetUserNotifications(int userId);

        int GetUnreadNotificationCount(int userId);

        void CheckAndGenerateNotifications();

        void MarkNotificationAsRead(int notificationId);
    }
}
