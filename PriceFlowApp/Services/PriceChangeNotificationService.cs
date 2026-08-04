using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;

namespace PriceFlowApp.Services
{
    public class PriceChangeNotificationService: IPriceChangeNotificationService
    {
        private readonly IPriceChangeNotificationsRepository _notificationRepository;
        private readonly IDailyTurnoverRepository _dailyTurnoverRepository;
        private readonly IAuthRepository _authRepository;

        public PriceChangeNotificationService(IPriceChangeNotificationsRepository notificationRepository, IDailyTurnoverRepository dailyTurnoverRepository, 
            IAuthRepository authRepository)
        {
            _notificationRepository = notificationRepository;
            _dailyTurnoverRepository = dailyTurnoverRepository;
            _authRepository = authRepository;
        } 

        public IEnumerable<PriceChangeNotificationResponse> GetUserNotifications(int userId)
        {
            Korisnici user = GetUser(userId);

            IEnumerable<IzvestuvanjaPromenaCena?> notifications = _notificationRepository.GetByUserId(userId);

            return notifications.Select(n => new PriceChangeNotificationResponse
            {
                Id = n.Id,
                HvId = n.Hvid,
                ChangePercent = n.ProcentPromena,
                TradingDate = n.DatumTrguvanje.Date,
                Message = n.Poraka,
                IsRead = n.Procitano
            }).ToList();
        }

        public int GetUnreadNotificationCount(int userId)
        {
            Korisnici user = GetUser(userId);

            return _notificationRepository.GetUnreadNotificationCount(userId);
        }

        public void CheckAndGenerateNotifications()
        {
            DateTime today = _dailyTurnoverRepository.GetLatestDate();

            bool hasTodayData = _dailyTurnoverRepository.ExistsForDate(today);

            if (!hasTodayData)
                return;

            _notificationRepository.GenerateNotifications(today);
        }

        public void MarkNotificationAsRead(int notificationId)
        {
            IzvestuvanjaPromenaCena? notification = _notificationRepository.GetById(notificationId);
            if (notification == null)
                throw new NotFoundException("NOTIFICATION_NOT_FOUND", "Notification not found.");

            _notificationRepository.MarkNotificationAsRead(notificationId);
        }

        private Korisnici GetUser(int userId)
        {
            var user = _authRepository.GetById(userId);
            if (user == null)
                throw new NotFoundException("USER_NOT_FOUND", "User not found.");

            return user;
        }
    }
}
