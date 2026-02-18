namespace PriceFlowApp.DTOs
{
    public class NotificationResponse
    {
        public int Id { get; set; }
        public int HvId { get; set; }
        public decimal ChangePercent { get; set; }
        public DateTime TradingDate { get; set; }
        public string Message { get; set; } = null!;
        public bool IsRead { get; set; }
    }
}
