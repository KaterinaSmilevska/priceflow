using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/agent/conversations")]
    public class AgentConversationsController: PriceFlowController
    {
        private readonly IAgentConversationsService _agentConversationsService;
        private readonly IAgentMessagesService _agentMessagesService;

        public AgentConversationsController(IAgentConversationsService agentConversationsService, IAgentMessagesService agentMessagesService)
        {
            _agentConversationsService = agentConversationsService;
            _agentMessagesService = agentMessagesService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AgentConversationResponse>> GetAll()
        {
            int userId = User.GetUserId();
            
            return Execute(() => _agentConversationsService.FindByUserId(userId));
        }

        [HttpGet("{conversationId}/messages")]
        public ActionResult<IEnumerable<AgentMessageResponse>> GetMessages(int conversationId)
        {
            int userId = User.GetUserId();

            return Execute(() => _agentMessagesService.FindByConversationIdAndUserId(conversationId, userId));
        }

        [HttpDelete("{id}")]
        public ActionResult<AgentConversationResponse> Delete(int id)
        {
            int userId = User.GetUserId();

            return Execute(() => _agentConversationsService.Delete(id, userId));
        }
    }
}
