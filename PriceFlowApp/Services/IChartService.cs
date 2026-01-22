using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IChartService
    {
        Task<IEnumerable<PriceTrend>> GetPriceTrendAsync(int securityId, DateTime startDate, DateTime endDate);

        Task<IEnumerable<SectorDistribution>> GetSectorDistributionAsync(DateTime date);

        Task<IEnumerable<Security>> GetSecurities();

        DateTime? FindLatestDate();

        Task<IEnumerable<MonthlyIncome>> GetMonthlyIncomeAsync(int portfolioid);

        Task<IEnumerable<SecurityAllocation>> GetAllocationAsync(int portfolioId);
    }
}
