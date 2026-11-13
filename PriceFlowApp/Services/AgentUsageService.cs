using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.Exceptions;

namespace PriceFlowApp.Services
{
    public class AgentUsageService : IAgentUsageService
    {
        private readonly IAgentUsageRepository _agentUsageRepository;

        public AgentUsageService(IAgentUsageRepository agentUsageRepository)
        {
            _agentUsageRepository = agentUsageRepository;
        }

        public AgentKoristenje? FindByUserIdAndDate(int userId, DateOnly date)
        {
            return _agentUsageRepository.GetByUserIdAndDate(userId, date);
        }

        public AgentKoristenje Add(int userId, DateOnly date)
        {
            var usage = new AgentKoristenje
            {
                KorisnikId = userId,
                Datum = date,
                BrojPoraki = 1
            };
            
            return _agentUsageRepository.Add(usage);
        }

        public AgentKoristenje Increment(int userId, DateOnly date)
        {
            AgentKoristenje usage = GetByUserIdAndDate(userId, date);
            usage.BrojPoraki += 1;

            return _agentUsageRepository.Update(usage);
        }

        private AgentKoristenje GetByUserIdAndDate(int userId, DateOnly date)
        {
            AgentKoristenje? usage = _agentUsageRepository.GetByUserIdAndDate(userId, date);
            if (usage == null)
                throw new NotFoundException("AGENT_USAGE_NOT_FOUND", "Agent usage not found.");

            return usage;
        }
    }
}
