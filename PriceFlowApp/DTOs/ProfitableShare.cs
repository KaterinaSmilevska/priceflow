namespace PriceFlowApp.DTOs
{
    public class ProfitableShare
    {
        public string Code { get; set; } = null!;
        public string IssuerName { get; set; } = null!;
        public decimal? DividendYield { get; set; }
        public decimal? DividendPerShare { get; set; }
    }
}
