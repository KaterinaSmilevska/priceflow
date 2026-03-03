namespace PriceFlowApp.DTOs
{
    public class FilteredSecurity
    {
        public int SecurityId { get; set; }
        public string SecurityCode { get; set; } = null!;
        public decimal? Value {  get; set; }
    }
}
