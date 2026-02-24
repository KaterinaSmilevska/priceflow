namespace PriceFlowApp.DTOs
{
    public class LiquidityOverview
    {

        public IEnumerable<SecurityLiquidity> MostByTradedQuantity { get; set; } = new List<SecurityLiquidity>();

        public IEnumerable<SecurityLiquidity> LeastByTradedQuantity { get; set; } = new List<SecurityLiquidity>();

        public IEnumerable<SecurityLiquidity> MostByTradingDays { get; set; } = new List<SecurityLiquidity>();

        public IEnumerable<SecurityLiquidity> LeastByTradingDays { get; set; } = new List<SecurityLiquidity>();
    }
}
