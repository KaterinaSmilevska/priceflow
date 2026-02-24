namespace PriceFlowApp.DTOs
{
    public class PortfolioReturns
    {
        public DateOnly Date {  get; set; }
        public decimal NetAmount { get; set; }
        public decimal Tax {  get; set; }
        public int PortfolioId { get; set; }
        public int HVId { get; set; }
    }
}
