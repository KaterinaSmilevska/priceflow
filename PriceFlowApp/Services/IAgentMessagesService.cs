using DataAccess.Models;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IAgentMessagesService
    {
        IEnumerable<AgentPoraki> FindByConversationId(int conversationId);

        IEnumerable<AgentMessageResponse> FindByConversationIdAndUserId(int conversationId, int userId);

        AgentPoraki Add(int conversationId, string role, string content);
    }
}
