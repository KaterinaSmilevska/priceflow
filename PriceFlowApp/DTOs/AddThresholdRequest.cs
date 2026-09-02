namespace PriceFlowApp.DTOs
{
    public class AddThresholdRequest
    {
        public int SecurityId { get; set; }
        public decimal LowerThreshold { get; set; }
        public decimal UpperThreshold { get; set; }
    }
}
