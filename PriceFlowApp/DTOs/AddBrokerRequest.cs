namespace PriceFlowApp.DTOs
{
    public class AddBrokerRequest
    {
        public string Company { get; set; } = null!;
        public decimal CommissionPercent { get; set; }
    }
}
