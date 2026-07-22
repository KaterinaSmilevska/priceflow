using PriceFlowApp.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;

namespace PriceFlowApp.Services
{
    public class SecurityPriceTrendReportService : ISecurityPriceTrendReportService
    {

        public byte[] GenerateSecurityPriceTrendReport(List<SecurityPriceTrendReport> reports)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                foreach (var report in reports)
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(50);

                        page.Header()
                            .Text("Securities Price Trend Report")
                            .FontSize(18)
                            .Bold();

                        page.Content()
                        .PaddingTop(20)
                        .Column(column =>
                        {
                            column.Spacing(15);

                            column.Item()
                            .Text(text =>
                            {
                                text.Span("Security code: ")
                                .Bold();

                                text.Span(report.SecurityCode ?? "");
                            });

                            column.Item()
                           .Text(text =>
                           {
                               text.Span("Period: ")
                               .Bold();

                               text.Span(report.Period ?? "");
                           });

                            column.Item()
                           .Text(text =>
                           {
                               text.Span("Date range: ")
                               .Bold();

                               text.Span($"{report.StartDate:dd.MM.yyyy} - {report.EndDate:dd.MM.yyyy}");
                           });

                            column.Item()
                            .PaddingTop(20)
                            .Text("Summary")
                            .FontSize(16)
                            .Bold();

                            column.Item()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                AddRow(
                                    table,
                                    "Measurements",
                                    report.NumberOfMeasurements.ToString());

                                AddRow(table, "Start price", $"{report.StartPrice:N2} MKD");
                                AddRow(table, "End price", $"{report.EndPrice:N2} MKD");
                                AddRow(table, "Lowest price", $"{report.LowestPrice:N2} MKD");
                                AddRow(table, "Highest price", $"{report.HighestPrice:N2} MKD");
                                AddRow(table, "Average price", $"{report.AveragePrice:N2} MKD");

                                AddRow(table, "Absolute price change", $"{report.PriceChange:+0.00;-0.00} MKD");
                                AddRow(table, "Price change (%)", $"{report.PriceChangePercent:+0.00;-0.00}%");

                                AddRow(table, "Trend", report.Trend ?? "");
                            });
                        });

                        page.Footer()
                        .PaddingTop(10)
                        .BorderTop(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Row(row =>
                        {
                            row.RelativeItem()
                            .Text($"Generated on {DateTime.Now:dd.MM.yyyy HH:mm}")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1);

                            row.ConstantItem(100)
                            .AlignRight()
                            .Text(text =>
                            {
                                text.DefaultTextStyle(x => x
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken1));

                                text.CurrentPageNumber();
                                text.Span(" / ");
                                text.TotalPages();
                            });
                        });
                    });
                }
            });
            return document.GeneratePdf();
        }

        private void AddRow(TableDescriptor table, string label, string value)
        {
            table.Cell()
                .Text(label)
                .Bold();

            table.Cell()
                .Text(value);
        }
    }
}
