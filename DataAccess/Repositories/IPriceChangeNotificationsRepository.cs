using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IPriceChangeNotificationsRepository
    {
        Task<IzvestuvanjaPromenaCena?> GetById(int id);

        Task<List<IzvestuvanjaPromenaCena>> GetByUserAsync(int userId);

        Task<int> GetUnreadNotificationCountAsync(int userId);

        Task MarkNotificationAsReadAsync(int notificationId);

        Task GenerateNotificationsAsync(DateTime tradingDate);

        Task<bool> NotificationExistsForDateAsync(DateTime date);
    }
}
