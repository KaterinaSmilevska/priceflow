using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IPriceChangeNotificationsRepository
    {
        IzvestuvanjaPromenaCena? GetById(int id);

        IEnumerable<IzvestuvanjaPromenaCena?> GetByUserId(int userId);

        int GetUnreadNotificationCount(int userId);

        void MarkNotificationAsRead(int notificationId);

        void GenerateNotifications(DateTime tradingDate);

        bool NotificationExistsForDate(DateTime date);
    }
}
