using DataAccess.Models;

namespace PriceFlowApp.Services
{
    public interface IAgentUsageService
    {
        AgentKoristenje? FindByUserIdAndDate(int userId, DateOnly date);

        AgentKoristenje Add(int userId, DateOnly date);

        AgentKoristenje Increment(int userId, DateOnly date);
    }
}
