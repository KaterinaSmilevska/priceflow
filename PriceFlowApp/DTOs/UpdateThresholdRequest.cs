namespace PriceFlowApp.DTOs
{
    public class UpdateThresholdRequest
    {
        public decimal LowerThreshold { get; set; }
        public decimal UpperThreshold { get; set; }
    }
}
