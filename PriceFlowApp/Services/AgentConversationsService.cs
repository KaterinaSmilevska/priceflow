using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;

namespace PriceFlowApp.Services
{
    public class AgentConversationsService : IAgentConversationsService
    {
        private readonly IAgentConversationsRepository _agentConversationsRepository;

        public AgentConversationsService(IAgentConversationsRepository agentConversationsRepository)
        {
            _agentConversationsRepository = agentConversationsRepository;
        }

        public AgentRazgovori FindById(int id)
        {
            return GetById(id);
        }

        public AgentRazgovori FindByIdAndUserId(int id, int userId)
        {
            return GetByIdAndUserId(id, userId);
        }

        public IEnumerable<AgentConversationResponse> FindByUserId(int userId)
        {
            IEnumerable<AgentRazgovori> conversations = _agentConversationsRepository.GetByUserId(userId);

            return conversations.Select(
                MapToAgentConversation)
                .ToList();
        }

        public AgentRazgovori Add(int userId)
        {
            var conversation = new AgentRazgovori
            {
                KorisnikId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return _agentConversationsRepository.Add(conversation);
        }

        public AgentRazgovori Update(AgentRazgovori conversation)
        {
            conversation.UpdatedAt = DateTime.UtcNow;

            return _agentConversationsRepository.Update(conversation);
        }

        private AgentRazgovori GetById(int id)
        {
            AgentRazgovori? conversation = _agentConversationsRepository.GetById(id);
            if (conversation == null)
                throw new NotFoundException("CONVERSATION_NOT_FOUND", "Conversation not found.");

            return conversation;
        }

        private AgentRazgovori GetByIdAndUserId(int id, int userId)
        {
            AgentRazgovori? conversation = _agentConversationsRepository.GetByIdAndUserId(id, userId);
            if (conversation == null)
                throw new NotFoundException("CONVERSATION_NOT_FOUND", "Conversation not found.");

            return conversation;
        }

        private AgentConversationResponse MapToAgentConversation(AgentRazgovori conversation)
        {
            AgentRazgovori? foundConversation = _agentConversationsRepository.GetById(conversation.Id);
            var firstMessage = foundConversation?.AgentPoraki
                .Where(message => message.Uloga == "user")
                .OrderBy(message => message.CreatedAt)
                .FirstOrDefault();

            return new AgentConversationResponse
            {
                Id = conversation.Id,
                CreatedAt = conversation.CreatedAt,
                UpdatedAt = conversation.UpdatedAt,
                Title = firstMessage?.Sodrzina ?? "New conversation"
            };
        }
    }
}
