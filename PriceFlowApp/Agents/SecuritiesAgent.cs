using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;

namespace PriceFlowApp.Agents
{
    public class SecuritiesAgent
    {
        /* An AI-powered agent that answers questions about securities price change
         * by combining data from the REST API with an LLM. */

        private readonly AIAgent _agent;
        private readonly IAgentConversationsService _agentConversationsService;
        private readonly IAgentMessagesService _agentMessagesService;
        private readonly IAgentUsageService _agentUsageService;

        private const int DailyMessageLimit = 5;
        private const int MaxHistoryMessages = 10;
        private const int MaxMessageLength = 1000;

        public SecuritiesAgent(IChatClient chatClient, SecuritiesTools securitiesTools, 
            IAgentConversationsService agentConversationsService, IAgentMessagesService agentMessagesService,
            IAgentUsageService agentUsageService)
        {
            _agentConversationsService = agentConversationsService;
            _agentMessagesService = agentMessagesService;
            _agentUsageService = agentUsageService;

            var tools = new[]
            {
                AIFunctionFactory.Create(securitiesTools.SearchSecurities),
                AIFunctionFactory.Create(securitiesTools.GetSecurity),
                AIFunctionFactory.Create(securitiesTools.GetDailyMarketData)
            };

            _agent = chatClient.AsAIAgent(
                instructions:
                """
                You are SecuritiesAgent for the Macedonian Stock Exchange (MSE), a helpful agent that answers questions about securities. 
                Be friendly, informative and concise.
                """,
                tools: tools);
        }

        public async Task<SecuritiesAgentResponse> AskAsync(int userId, int? conversationId, string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                throw new ArgumentException("Question is required.", nameof(question));
            }

            if(question.Length > MaxMessageLength)
            {
                throw new BusinessRuleException("MESSAGE_LENGTH_INVALID", "Message cannot exceed 1000 characters.");
            }

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            var usage = _agentUsageService.FindByUserIdAndDate(userId, today);

            if(usage != null && usage.BrojPoraki >= DailyMessageLimit)
            {
                throw new InvalidOperationException("Daily agent message limit reached.");
            }

            var conversation = conversationId.HasValue
                ? _agentConversationsService.FindByIdAndUserId(conversationId.Value, userId)
                : _agentConversationsService.Add(userId);

            _agentMessagesService.Add(conversation.Id, "user", question);

            var previousMessages = _agentMessagesService
                .FindByConversationId(conversation.Id)
                .OrderByDescending(message => message.CreatedAt)
                .Take(MaxHistoryMessages)
                .OrderBy(message => message.CreatedAt);

            var messages = previousMessages
                .Select(message => new ChatMessage(
                    GetChatRole(message.Uloga),
                    message.Sodrzina))
                .ToList();

            var response = await _agent.RunAsync(messages);

            var answer = response.Text ?? string.Empty;

            _agentMessagesService.Add(conversation.Id, "assistant", answer);
            _agentConversationsService.Update(conversation);

            if(usage == null)
            {
                _agentUsageService.Add(userId, today);
            }
            else
            {
                _agentUsageService.Increment(userId, today);
            }

            return new SecuritiesAgentResponse(conversation.Id, answer);
        }

        private static ChatRole GetChatRole(string role)
        {
            return role.ToLowerInvariant() switch
            {
                "user" => ChatRole.User,
                "assistant" => ChatRole.Assistant,
                "system" => ChatRole.System,
                _ => ChatRole.User
            };
        }
    }
}
