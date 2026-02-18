namespace PriceFlowApp.DTOs
{
    public class SecurityDailyPrices
    {
        public string SecurityCode { get; set; } = null!;
        public DateTime PriceDate { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? AveragePrice { get; set; }
    }
}
