namespace PriceFlowApp.DTOs
{
    public class Transaction
    {
        public int Id { get; set; }
        public int HVId { get; set; }
        public string HVCode { get; set; } = null!;
        public int SharesQuantity { get; set; }
        public decimal SharesUnitPrice { get; set; }
        public decimal Amount {  get; set; }
        public string TypeTransaction { get; set; } = null!;
        public bool IsReal { get; set; }
        public decimal StockExchangeCommission { get; set; }
        public decimal BrokerageCommission { get; set; }
        public decimal CDHVCommission { get; set; }
        public DateOnly Date {  get; set; }
    }
}
