namespace PriceFlowApp.DTOs
{
    public class PortfolioDetails
    {
        public int Id {  get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<Transaction> Transactions { get; set; } = [];
        public List<PortfolioReturn> Returns { get; set; } = [];
    }
}
