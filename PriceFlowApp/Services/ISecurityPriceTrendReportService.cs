using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface ISecurityPriceTrendReportService
    {
        byte[] GenerateSecurityPriceTrendReport(List<SecurityPriceTrendReport> reports);
    }
}
