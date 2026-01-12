namespace PriceFlowApp.DTOs
{
    public class PortfolioReturn
    {
        public DateOnly Date {  get; set; }
        public decimal NetAmount { get; set; }
        public decimal Tax {  get; set; }
        public string HVCode { get; set; } = null!;
    }
}
