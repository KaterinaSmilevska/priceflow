using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IPortfolioReportExportService
    {
        byte[] ExportToCsv(PortfolioPerformanceSummary report);

        byte[] ExportToExcel(PortfolioPerformanceSummary report);

        byte[] ExportToPDF(PortfolioPerformanceSummary report);
    }
}
