namespace PriceFlowApp.DTOs
{
    public class Transaction
    {
        public int Id { get; set; }
        public string HVCode { get; set; } = null!;
        public int SharesQuantity { get; set; }
        public int SharesUnitPrice { get; set; }
        public decimal Amount {  get; set; }
        public string TypeTransaction { get; set; } = null!;
        public DateOnly Date {  get; set; }
    }
}
