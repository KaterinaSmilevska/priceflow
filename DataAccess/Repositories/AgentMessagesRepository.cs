using DataAccess.Models;

namespace DataAccess.Repositories
{
    public class AgentMessagesRepository : IAgentMessagesRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public AgentMessagesRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<AgentPoraki> GetByConversationId(int conversationId)
        {
            return _dbContext.AgentPoraki
                .Where(ap => ap.RazgovorId == conversationId)
                .OrderBy(ap => ap.CreatedAt)
                .ToList();
        }

        public IEnumerable<AgentPoraki> GetByConversationIdAndUserId(int conversationId, int userId)
        {
            return _dbContext.AgentPoraki
                .Where(ap => ap.RazgovorId == conversationId && ap.Razgovor.KorisnikId == userId)
                .OrderBy(ap => ap.CreatedAt)
                .ToList();
        }

        public AgentPoraki Add(AgentPoraki message)
        {
            _dbContext.AgentPoraki.Add(message);
            _dbContext.SaveChanges();

            return message;
        }
    }
}
