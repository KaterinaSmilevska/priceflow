namespace PriceFlowApp.DTOs
{
    public class UpdateThresholdRequest
    {
        public int HvId { get; set; }
        public decimal LowerThreshold { get; set; }
        public decimal UpperThreshold { get; set; }
    }
}
