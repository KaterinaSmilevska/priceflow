namespace PriceFlowApp.DTOs
{
    public class PortfolioValue
    {
        public int SecurityId { get; set; }
        public string SecurityCode { get; set; } = null!;
        public int TotalQuantity { get; set; }
        public decimal LastPrice { get; set; }
        public decimal CurrentValue { get; set; }
        public bool IsReal {  get; set; }
    }
}
