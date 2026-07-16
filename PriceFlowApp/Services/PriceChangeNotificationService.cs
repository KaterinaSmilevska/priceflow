using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class PriceChangeNotificationService: IPriceChangeNotificationService
    {
        private readonly IPriceChangeNotificationsRepository _notificationRepository;
        private readonly IDailyTurnoverRepository _dailyTurnoverRepository;

        public PriceChangeNotificationService(IPriceChangeNotificationsRepository notificationRepository, IDailyTurnoverRepository dailyTurnoverRepository)
        {
            _notificationRepository = notificationRepository;
            _dailyTurnoverRepository = dailyTurnoverRepository;
        } 

        public async Task CheckAndGenerateNotificationsAsync()
        {
            DateTime today = await _dailyTurnoverRepository.GetLatestDateAsync();

            bool hasTodayData = await _dailyTurnoverRepository.ExistsForDateAsync(today);
            
            if(!hasTodayData)
                return;

            await _notificationRepository.GenerateNotificationsAsync(today);
        }

        public async Task<int> GetUnreadNotificationCountAsync(int userId)
        {
            return await _notificationRepository.GetUnreadNotificationCountAsync(userId);
        }

        public async Task<List<PriceChangeNotificationResponse>> GetUserNotificationsAsync(int userId)
        {
            List<IzvestuvanjaPromenaCena> notifications = await _notificationRepository.GetByUserAsync(userId);

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

        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            await _notificationRepository.MarkNotificationAsReadAsync(notificationId);
        }
    }
}
