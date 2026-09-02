namespace PriceFlowApp.DTOs
{
    public class Sector
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal? TotalValue { get; set; }
    }
}
