namespace PriceFlowApp.DTOs
{
    public class UpdateBrokerRequest
    {
        public int Id { get; set; }
        public string Company { get; set; } = null!;
        public decimal CommissionPercent { get; set; }
    }
}
