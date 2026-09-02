namespace PriceFlowApp.DTOs
{
    public class SecurityPerformance
    {
        public string SecurityCode { get; set; } = null!;
        public decimal? ChangePercent { get; set; }
        public int? Volume { get; set; }
    }
}
