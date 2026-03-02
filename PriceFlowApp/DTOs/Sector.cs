namespace PriceFlowApp.DTOs
{
    public class Sector
    {
        public int SectorId { get; set; }
        public string SectorName { get; set; } = null!;
        public decimal? TotalValue { get; set; }
    }
}
