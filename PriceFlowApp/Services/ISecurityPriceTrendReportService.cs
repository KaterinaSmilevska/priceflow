using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface ISecurityPriceTrendReportService
    {
        byte[] GenerateSecurityPriceTrendReport(IEnumerable<SecurityPriceTrendReport> reports);
    }
}
