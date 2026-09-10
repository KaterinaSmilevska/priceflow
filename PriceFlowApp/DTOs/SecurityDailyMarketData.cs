namespace PriceFlowApp.DTOs
{
    public class SecurityDailyMarketData
    {
        public string SecurityCode { get; set; } = null!;
        public DateTime Date { get; set; }
        public decimal? LatestTransactionPrice { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? AveragePrice { get; set; }
        public decimal? ChangePercent { get; set; }
        public int? TradedQuantity { get; set; }
        public int? TurnoverBESTDenars { get; set; }
        public int? TotalTurnoverDenars { get; set; }
    }
}
