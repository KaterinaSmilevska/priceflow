namespace PriceFlowApp.DTOs
{
    public class PortfolioPerformanceSummary
    {
        public string PortfolioName { get; set; } = "";
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public decimal StartingValue { get; set; }
        public decimal EndingValue { get; set; }
        public decimal Dividends {  get; set; }
        public decimal Commissions { get; set; }
        public decimal AbsoluteReturn {  get; set; }
    }
}
