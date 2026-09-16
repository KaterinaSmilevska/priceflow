using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class AgentConversationsRepository : IAgentConversationsRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public AgentConversationsRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public AgentRazgovori? GetById(int id)
        {
            return _dbContext.AgentRazgovori
                .Include(ar => ar.AgentPoraki)
                .FirstOrDefault(ar => ar.Id == id);
        }

        public AgentRazgovori? GetByIdAndUserId(int id, int userId)
        {
            return _dbContext.AgentRazgovori
                .Include(ar => ar.AgentPoraki)
                .FirstOrDefault(ar => ar.Id == id && ar.KorisnikId == userId);
        }


        public IEnumerable<AgentRazgovori> GetByUserId(int userId)
        {
            return _dbContext.AgentRazgovori
                .Where(ar => ar.KorisnikId == userId)
                .OrderByDescending(ar => ar.UpdatedAt)
                .ToList();
        }

        public AgentRazgovori Add(AgentRazgovori conversation)
        {
            _dbContext.AgentRazgovori.Add(conversation);
            _dbContext.SaveChanges();

            return conversation;
        }

        public AgentRazgovori Update(AgentRazgovori conversation)
        {
            _dbContext.AgentRazgovori.Update(conversation);
            _dbContext.SaveChanges();

            return conversation;
        }

        public AgentRazgovori Delete(AgentRazgovori conversation)
        {
            _dbContext.RemoveRange(conversation.AgentPoraki);
            _dbContext.Remove(conversation);
            _dbContext.SaveChanges();

            return conversation;
        }
    }
}
