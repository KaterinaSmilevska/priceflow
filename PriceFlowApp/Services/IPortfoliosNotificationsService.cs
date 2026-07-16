using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IPortfoliosNotificationsService
    {
        Task SendScheduledNotificationsAsync();

        Task<PortfolioNotification?> FindByPortfolioId(int portfolioId);

        Task<PortfolioNotification> UpdateAsync(UpdatePortfolioNotification portfolioNotification);
    }
}
