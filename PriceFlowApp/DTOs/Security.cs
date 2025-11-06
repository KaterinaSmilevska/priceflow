namespace PriceFlowApp.DTOs
{
    public class Security
    {
        public int Id { get; set; }
        public string Isin { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string TypeSecurityName { get; set; } = null!;
        public string IssuerName { get; set; } = null!;
        public int TotalNumShares { get; set; }
    }
}
