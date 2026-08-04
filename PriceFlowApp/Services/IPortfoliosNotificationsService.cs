using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IPortfoliosNotificationsService
    {
        PortfolioNotification? FindByPortfolioId(int portfolioId);

        PortfolioNotification Update(UpdatePortfolioNotification portfolioNotification);

        void SendScheduledNotifications();
    }
}
