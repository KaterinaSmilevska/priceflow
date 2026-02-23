namespace PriceFlowApp.DTOs
{
    public class UpdatePortfolioNotification
    {
        public int PortfolioId { get; set; }
        public bool IsEnabled { get; set; }
        public string Frequency { get; set; } = null!;
    }
}
