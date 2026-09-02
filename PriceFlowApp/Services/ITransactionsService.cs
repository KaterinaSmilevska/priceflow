using DataAccess.Enums;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface ITransactionsService
    {
        IEnumerable<Transaction> FindByPortfolioId(int portfolioid);

        int FindOwnedShares(int portfolioId, string securityCode, bool isReal);

        int FindOwnedSharesAtDate(int portfolioId, string securityCode, bool isReal, DateOnly date, int? transactionIdToExclude);

        Transaction Add(int portfolioId, Transaction transaction);

        Transaction Update(int portfolioId, int id, Transaction transaction);

        Transaction Delete(int id);

        PortfolioAnalytics GetAnalytics(int portfolioId, bool isReal);

        IEnumerable<OwnedSecuritiesPriceTrend> FindPriceTrend(int userId, PriceTrendPeriod? period, PriceTrendResolution? resolution);

        IEnumerable<SecurityPriceTrendReport> GetSecuritiesPriceTrendReport(int userId, PriceTrendPeriod? period, PriceTrendResolution? resolution, string? securityCode);

    }
}
