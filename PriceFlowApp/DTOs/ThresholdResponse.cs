namespace PriceFlowApp.DTOs
{
    public class ThresholdResponse
    {
        public int Id { get; set; }
        public int SecurityId { get; set; }
        public string SecurityCode { get; set; } = string.Empty;
        public decimal LowerThreshold { get; set; }
        public decimal UpperThreshold { get; set; }
    }
}
