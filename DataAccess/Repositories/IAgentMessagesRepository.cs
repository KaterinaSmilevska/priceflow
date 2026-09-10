using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IAgentMessagesRepository
    {
        IEnumerable<AgentPoraki> GetByConversationId(int conversationId);

        IEnumerable<AgentPoraki> GetByConversationIdAndUserId(int conversationId, int userId);

        AgentPoraki Add(AgentPoraki message);
    }
}
