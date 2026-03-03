namespace PriceFlowApp.DTOs
{
    public class SecurityValuation
    {
        public string Code { get; set; } = null!;
        public decimal? LastPrice { get; set; }
        public decimal? BookValuePerShare { get; set; }
        public decimal? DeviationPercent { get; set; }
    }
}
