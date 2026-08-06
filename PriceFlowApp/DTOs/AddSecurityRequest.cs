namespace PriceFlowApp.DTOs
{
    public class AddSecurityRequest
    {
        public string Isin { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int TotalNumShares { get; set; }
        public int TypeSecurityId { get; set; }
        public int IssuerId { get; set; }
    }
}
