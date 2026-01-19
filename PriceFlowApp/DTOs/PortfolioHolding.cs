namespace PriceFlowApp.DTOs
{
    public class PortfolioHolding
    {
        public int HVId { get; set; }
        public string HVCode { get; set; } = null!;
        public string HVIsin { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal AvgPrice { get; set; }
        public decimal TotalInvested => Quantity * AvgPrice;

    }
}
