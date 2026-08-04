using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IChartService
    {
        IEnumerable<PriceTrend> GetPriceTrend(int securityId, DateTime startDate, DateTime endDate);

        IEnumerable<SectorDistribution> GetSectorDistribution(DateTime date);

        IEnumerable<MonthlyIncome> GetMonthlyIncome(int portfolioId, bool isReal);

        IEnumerable<SecurityAllocation> GetAllocation(int portfolioId, bool isReal);

        IEnumerable<Security> GetSecurities();

        DateTime? FindLatestDate();
    }
}
