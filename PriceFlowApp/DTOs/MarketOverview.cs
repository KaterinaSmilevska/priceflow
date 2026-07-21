namespace PriceFlowApp.DTOs
{
    public class MarketOverview
    {
        public decimal TotalMarketCap { get; set; }
        public int AverageDailyVolume { get; set; }
        public int AverageMonthlyVolume { get; set; }
        public string TopGainer { get; set; } = null!;
        public decimal TopGainerChange  { get; set; }
        public string TopLoser { get; set; } = null!;
        public decimal TopLoserChange { get;set; }
        public int TotalSecurities { get; set; }
    }
}
