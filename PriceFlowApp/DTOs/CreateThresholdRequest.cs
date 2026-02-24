namespace PriceFlowApp.DTOs
{
    public class CreateThresholdRequest
    {
        public int HvId { get; set; }
        public decimal LowerThreshold { get; set; }
        public decimal UpperThreshold { get; set; }
    }
}
