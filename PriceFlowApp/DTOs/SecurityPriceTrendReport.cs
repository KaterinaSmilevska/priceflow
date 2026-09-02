
namespace PriceFlowApp.DTOs
{
    public class SecurityPriceTrendReport
    {
        public string? SecurityCode { get; set; }
        public string? Period { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfMeasurements { get; set; }
        public decimal StartPrice { get; set; }
        public decimal EndPrice { get; set; }
        public decimal LowestPrice { get; set; }
        public decimal HighestPrice { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal PriceChange { get; set; }
        public decimal PriceChangePercent { get; set; }
        public string? Trend { get; set; }
    }
}
