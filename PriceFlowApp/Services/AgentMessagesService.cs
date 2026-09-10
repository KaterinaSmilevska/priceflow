using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class AgentMessagesService : IAgentMessagesService
    {
        private readonly IAgentMessagesRepository _agentMessagesRepository;

        public AgentMessagesService(IAgentMessagesRepository agentMessagesRepository)
        {
            _agentMessagesRepository = agentMessagesRepository;
        }

        public IEnumerable<AgentPoraki> FindByConversationId(int conversationId)
        {
            return _agentMessagesRepository.GetByConversationId(conversationId); 
        }


        public IEnumerable<AgentMessageResponse> FindByConversationIdAndUserId(int conversationId, int userId)
        {
            IEnumerable<AgentPoraki> messages = _agentMessagesRepository.GetByConversationIdAndUserId(conversationId, userId);

            return messages.Select(
                MapToAgentMessageResponse)
                .ToList();
        }

        public AgentPoraki Add(int conversationId, string role, string content)
        {
            var message = new AgentPoraki
            {
                RazgovorId = conversationId,
                Uloga = role,
                Sodrzina = content,
                CreatedAt = DateTime.UtcNow
            };

            return _agentMessagesRepository.Add(message);
        }

        private AgentMessageResponse MapToAgentMessageResponse(AgentPoraki message)
        {
            return new AgentMessageResponse
            {
                Id = message.Id,
                Role = message.Uloga,
                Content = message.Sodrzina,
                CreatedAt = message.CreatedAt
            };
        }
    }
}
