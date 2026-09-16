using DataAccess.Models;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IAgentConversationsService
    {
        AgentRazgovori FindById(int id);

        AgentRazgovori FindByIdAndUserId(int id, int userId);

        IEnumerable<AgentConversationResponse> FindByUserId(int userId);

        AgentRazgovori Add(int userId);

        AgentRazgovori Update(AgentRazgovori conversation);

        AgentConversationResponse Delete(int id, int userId);
    }
}
