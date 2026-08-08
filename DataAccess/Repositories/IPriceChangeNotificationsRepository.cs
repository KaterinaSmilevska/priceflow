using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IPriceChangeNotificationsRepository
    {
        IzvestuvanjaPromenaCena? GetById(int id);

        IEnumerable<IzvestuvanjaPromenaCena> GetByUserId(int userId);

        int GetUnreadNotificationCount(int userId);

        bool ExistsForUserAndSecurityAndDate(int userId, int securityId, DateTime date);

        IzvestuvanjaPromenaCena Add(IzvestuvanjaPromenaCena notification);

        void MarkNotificationAsRead(IzvestuvanjaPromenaCena notification);
    }
}
