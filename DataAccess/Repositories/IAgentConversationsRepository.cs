using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IAgentConversationsRepository
    {
        AgentRazgovori? GetById(int id);

        AgentRazgovori? GetByIdAndUserId(int id, int userId);

        IEnumerable<AgentRazgovori> GetByUserId(int userId);

        AgentRazgovori Add(AgentRazgovori conversation);

        AgentRazgovori Update(AgentRazgovori conversation);

        AgentRazgovori Delete(AgentRazgovori conversation);
    }
}
