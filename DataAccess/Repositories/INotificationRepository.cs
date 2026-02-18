using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface INotificationRepository
    {
        Task<IzvestuvanjaPromenaCena?> GetById(int id);

        Task<List<IzvestuvanjaPromenaCena>> GetByUserAsync(int userId);

        Task<int> GetUnreadNotificationCountAsync(int userId);

        Task MarkNotificationAsReadAsync(int notificationId);

        Task GenerateNotificationsAsync(DateTime tradingDate);

        Task<bool> NotificationExistsForDateAsync(DateTime date);
    }
}
