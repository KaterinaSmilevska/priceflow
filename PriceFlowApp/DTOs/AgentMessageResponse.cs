namespace PriceFlowApp.DTOs
{
    public class AgentMessageResponse
    {
        public int Id {  get; set; }
        public string Role { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
