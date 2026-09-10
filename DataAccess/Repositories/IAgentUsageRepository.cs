using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IAgentUsageRepository
    {
        AgentKoristenje? GetByUserIdAndDate(int userId, DateOnly date);

        AgentKoristenje Add(AgentKoristenje usage);

        AgentKoristenje Update(AgentKoristenje usage);
    }
}
