using PriceFlowApp.DTOs;

namespace PriceFlowApp.Agents
{
    public interface IOrchestratorAgent
    {
        Task<SecuritiesAgentResponse> AskAsync(int userId, int? conversationId, string question);
    }
}
