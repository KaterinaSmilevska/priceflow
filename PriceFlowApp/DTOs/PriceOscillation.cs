namespace PriceFlowApp.DTOs
{
    public class PriceOscillation
    {
        public string SecurityCode { get; set; } = null!;
        public decimal? MaxPrice { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? Oscillation { get; set; }
        public decimal? ChangePercent { get; set; }
    }
}
