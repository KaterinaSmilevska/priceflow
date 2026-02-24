namespace PriceFlowApp.DTOs
{
    public class CreateBrokerRequest
    {
        public string Company { get; set; } = null!;
        public decimal CommissionPercent { get; set; }
    }
}
