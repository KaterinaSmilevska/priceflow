namespace PriceFlowApp.DTOs
{
    public class PortfolioReturn
    {
        public DateOnly Date {  get; set; }
        public decimal NetoAmount { get; set; }
        public decimal Tax {  get; set; }
        public int HVId { get; set; }
        public string HVCode { get; set; } = null!;
        public string HVIsin { get; set; } = null!;
        public decimal TotalReturn => NetoAmount;
    }
}
