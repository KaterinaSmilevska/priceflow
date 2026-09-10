using DataAccess.Models;

namespace DataAccess.Repositories
{
    public class AgentUsageRepository : IAgentUsageRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public AgentUsageRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public AgentKoristenje? GetByUserIdAndDate(int userId, DateOnly date)
        {
            return _dbContext.AgentKoristenje
                .FirstOrDefault(ak => ak.KorisnikId == userId && ak.Datum == date);
        }

        public AgentKoristenje Add(AgentKoristenje usage)
        {
            _dbContext.AgentKoristenje.Add(usage);
            _dbContext.SaveChanges();

            return usage;
        }

        public AgentKoristenje Update(AgentKoristenje usage)
        {
            _dbContext.AgentKoristenje.Update(usage);
            _dbContext.SaveChanges();

            return usage;
        }
    }
}
