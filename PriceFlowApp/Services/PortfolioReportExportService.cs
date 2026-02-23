using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using PriceFlowApp.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Text;

namespace PriceFlowApp.Services
{
    public class PortfolioReportExportService : IPortfolioReportExportService
    {
        public byte[] ExportToCsv(PortfolioPerformanceSummary report)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Portfolio Performance Summary");
            sb.AppendLine($"Portfolio, {report.PortfolioName}");
            sb.AppendLine($"Period, {report.FromDate:yyyy-MM-dd} - {report.ToDate:yyyy-MM-dd}");
            sb.AppendLine();
            sb.AppendLine("Starting value, " + report.StartingValue);
            sb.AppendLine("Ending value, " + report.EndingValue);
            sb.AppendLine("Dividends, " + report.Dividends);
            sb.AppendLine("Commissions, " + report.Commissions);
            sb.AppendLine("Absolute return, " + report.AbsoluteReturn);

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public byte[] ExportToExcel(PortfolioPerformanceSummary report)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Portfolio Performance Summary");

            worksheet.Cell(1, 1).Value = "Portfolio Performance Summary";

            worksheet.Cell(3, 1).Value = "Portfolio: ";
            worksheet.Cell(3, 2).Value = report.PortfolioName;

            worksheet.Cell(4, 1).Value = "Period: ";
            worksheet.Cell(4, 2).Value = $"{report.FromDate:yyyy-MM-dd} - {report.ToDate:yyyy-MM-dd}";


            worksheet.Cell(6, 1).Value = "Starting value: ";
            worksheet.Cell(6, 2).Value = report.StartingValue;

            worksheet.Cell(7, 1).Value = "Ending value: ";
            worksheet.Cell(7, 2).Value = report.EndingValue;

            worksheet.Cell(8, 1).Value = "Dividends: ";
            worksheet.Cell(8, 2).Value = report.Dividends;

            worksheet.Cell(9, 1).Value = "Commissions: ";
            worksheet.Cell(9, 2).Value = report.Commissions;

            worksheet.Cell(10, 1).Value = "Absolute return: ";
            worksheet.Cell(10, 2).Value = report.AbsoluteReturn;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        public byte[] ExportToPDF(PortfolioPerformanceSummary report)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);

                    page.Header()
                    .Text("Portfolio Performance Summary")
                    .FontSize(20)
                    .Bold()
                    .AlignCenter();

                    page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        column.Spacing(10);

                        column.Item().Text($"Portfolio: {report.PortfolioName}");
                        column.Item().Text($"Period: {report.FromDate:yyyy-MM-dd} - {report.ToDate:yyyy-MM-dd}");

                        column.Item().LineHorizontal(1);

                        column.Item().Text($"Starting value: {report.StartingValue:C}");
                        column.Item().Text($"Ending value: {report.EndingValue:C}");
                        column.Item().Text($"Dividends: {report.Dividends:C}");
                        column.Item().Text($"Commissions: {report.Commissions:C}");
                        column.Item().Text($"Absolute return: {report.AbsoluteReturn:C}");
                    });

                    page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated on ");

                        x.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"));
                    });
                });
            });
            return document.GeneratePdf();
        }
    }
}
