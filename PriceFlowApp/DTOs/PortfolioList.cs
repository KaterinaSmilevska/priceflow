namespace PriceFlowApp.DTOs
{
    public class PortfolioList
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalValue { get; set; }
    }
}
