using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IPriceChangeNotificationService
    {
        IEnumerable<PriceChangeNotificationResponse> GetUserNotifications(int userId);

        int GetUnreadNotificationCount(int userId);

        void GenerateNotifications();

        void MarkNotificationAsRead(int userId, int notificationId);
    }
}
