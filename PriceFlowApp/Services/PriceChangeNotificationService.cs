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
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IThresholdRepository _thresholdRepository;

        public PriceChangeNotificationService(IPriceChangeNotificationsRepository notificationRepository, IDailyTurnoverRepository dailyTurnoverRepository, 
            IAuthRepository authRepository, ITransactionsRepository transactionsRepository, IThresholdRepository thresholdRepository)
        {
            _notificationRepository = notificationRepository;
            _dailyTurnoverRepository = dailyTurnoverRepository;
            _authRepository = authRepository;
            _transactionsRepository = transactionsRepository;
            _thresholdRepository = thresholdRepository;
        } 

        public IEnumerable<PriceChangeNotificationResponse> GetUserNotifications(int userId)
        {
            Korisnici user = GetUserById(userId);

            IEnumerable<IzvestuvanjaPromenaCena> notifications = _notificationRepository.GetByUserId(user.Id);

            return notifications
                .Select(MapToPriceChangeNotification)
                .ToList();
        }

        public int GetUnreadNotificationCount(int userId)
        {
            Korisnici user = GetUserById(userId);

            return _notificationRepository.GetUnreadNotificationCount(user.Id);
        }

        public void GenerateNotifications()
        {
            DateTime latestDate = _dailyTurnoverRepository.GetLatestDate();

            if (!_dailyTurnoverRepository.ExistsForDate(latestDate))
                return;

            IEnumerable<DnevenPromet> dailyTurnover = _dailyTurnoverRepository.GetByDate(latestDate);

            foreach(DnevenPromet dailyData in dailyTurnover)
            {
                if (dailyData.ProcentPromena == null || dailyData.ProcentPromena == 0)
                    continue;

                IEnumerable<HvPromenaCena> alerts = _thresholdRepository.GetBySecurityId(dailyData.Hvid);

                foreach(HvPromenaCena alert in alerts)
                {
                    int ownedShares = _transactionsRepository.GetOwnedSharesByUser(alert.KorisnikId, alert.Hvid);

                    if (ownedShares <= 0)
                        continue;

                    bool thresholdReached = dailyData.ProcentPromena <= alert.DolnaGranica ||
                        dailyData.ProcentPromena >= alert.GornaGranica;

                    if (!thresholdReached)
                        continue;

                    bool notificationExists = _notificationRepository.ExistsForUserAndSecurityAndDate(alert.KorisnikId, alert.Hvid, latestDate);

                    if (notificationExists)
                        continue;

                    AddNotification(alert, dailyData, latestDate);
                }
            }
        }

        public void MarkNotificationAsRead(int userId, int notificationId)
        {
            GetUserById(userId);
            IzvestuvanjaPromenaCena notification = GetPriceChangeNotificationById(notificationId);

            if (notification.KorisnikId != userId)
                throw new UnauthorizedException("NOTIFICATION_ACCESS_DENIED", "You cannot access this notification.");

            _notificationRepository.MarkNotificationAsRead(notification);
        }

        private Korisnici GetUserById(int userId)
        {
            Korisnici? user = _authRepository.GetById(userId);
            if (user == null)
                throw new NotFoundException("USER_NOT_FOUND", "User not found.");

            return user;
        }

        private IzvestuvanjaPromenaCena GetPriceChangeNotificationById(int notificationId)
        {
            IzvestuvanjaPromenaCena? notification = _notificationRepository.GetById(notificationId);
            if (notification == null)
                throw new NotFoundException("NOTIFICATION_NOT_FOUND", "Notification not found.");

            return notification;
        }

        private PriceChangeNotificationResponse AddNotification(HvPromenaCena alert, DnevenPromet dailyData, DateTime tradingDate)
        {
            IzvestuvanjaPromenaCena notification = new IzvestuvanjaPromenaCena
            {
                KorisnikId = alert.KorisnikId,
                Hvid = alert.Hvid,
                ProcentPromena = dailyData.ProcentPromena!.Value,
                DatumTrguvanje = tradingDate,
                Poraka =
                               $"Security {alert.Hv.Kod} changed {dailyData.ProcentPromena:F2}% " +
                               $"(Threshold: {alert.DolnaGranica}% / {alert.GornaGranica}%)",
                Procitano = false
            };
            IzvestuvanjaPromenaCena addedNotification = _notificationRepository.Add(notification);

            return MapToPriceChangeNotification(addedNotification);
        }

        private PriceChangeNotificationResponse MapToPriceChangeNotification(IzvestuvanjaPromenaCena priceChangeNotification)
        {
            return new PriceChangeNotificationResponse
            {
                Id = priceChangeNotification.Id,
                SecurityId = priceChangeNotification.Hvid,
                ChangePercent = priceChangeNotification.ProcentPromena,
                TradingDate = priceChangeNotification.DatumTrguvanje.Date,
                Message = priceChangeNotification.Poraka,
                IsRead = priceChangeNotification.Procitano
            };
        }
    }
}
