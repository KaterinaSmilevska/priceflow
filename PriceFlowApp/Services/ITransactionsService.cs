using DataAccess.Enums;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface ITransactionsService
    {
        Task<List<Transaction>> FindByPortfolioIdAsync(int portfolioid);

        Task<int> FindOwnedSharesAsync(int portfolioId, string securityCode, bool isReal);

        Task<int> FindOwnedSharesAtDateAsync(int portfolioId, string securityCode, bool isReal, DateOnly date);

        Task<Transaction> AddAsync(int portfolioId, Transaction transaction);

        Task<Transaction> UpdateAsync(int id, Transaction transaction);

        Task DeleteAsync(int id);

        Task<PortfolioAnalytics> GetAnalyticsAsync(int portfolioId, bool isReal);

        Task<List<OwnedSecuritiesPriceTrend>> FindPriceTrendAsync(int userId, PriceTrendPeriod? period, PriceTrendResolution? resolution);

        Task<List<SecurityPriceTrendReport>> GetSecuritiesPriceTrendReportAsync(int userId, PriceTrendPeriod? period, PriceTrendResolution? resolution, string? securityCode);

    }
}
