namespace PriceFlowApp.DTOs
{
    public class AgentConversationResponse
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt {  get; set; }
        public string Title { get; set; } = null!;
    }
}
