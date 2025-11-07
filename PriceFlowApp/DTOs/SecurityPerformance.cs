namespace PriceFlowApp.DTOs
{
    public class SecurityPerformance
    {
        public string Code { get; set; } = null!;
        public decimal ChangePercent { get; set; }
        public int Volume { get; set; }
    }
}
