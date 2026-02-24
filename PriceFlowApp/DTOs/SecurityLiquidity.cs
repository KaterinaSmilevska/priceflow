namespace PriceFlowApp.DTOs
{
    public class SecurityLiquidity
    {
        public int SecurityId { get; set; }
        public string SecurityCode { get; set; } = null!;
        public int TradingDays { get; set; }
        public int? TradedQuantity { get; set; }
        public decimal? AverageDailyVolume { get; set; }
        public DateTime LastTradeDate { get; set; }
    }
}
