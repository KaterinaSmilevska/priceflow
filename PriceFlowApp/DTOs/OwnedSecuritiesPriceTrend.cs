namespace PriceFlowApp.DTOs
{
    public class OwnedSecuritiesPriceTrend
    {
        public DateTime Date { get; set; }
        public int SecurityId { get; set; }
        public string SecurityCode { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
