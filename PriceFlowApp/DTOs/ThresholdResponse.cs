namespace PriceFlowApp.DTOs
{
    public class ThresholdResponse
    {
        public int Id { get; set; }
        public int HvId { get; set; }
        public string HvCode { get; set; } = string.Empty;
        public decimal LowerThreshold { get; set; }
        public decimal UpperThreshold { get; set; }
    }
}
