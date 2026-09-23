using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Helpers;
using PriceFlowApp.Services;

namespace PriceFlowApp.Agents
{
    public class OrchestratorAgent
    {
        private readonly AIAgent _agent;

        private readonly IAgentConversationsService _agentConversationsService;
        private readonly IAgentMessagesService _agentMessagesService;
        private readonly IAgentUsageService _agentUsageService;

        private const int DailyMessageLimit = 5;
        private const int MaxHistoryMessages = 10;
        private const int MaxMessageLength = 1000;

        public AIAgent Agent => _agent;

        public OrchestratorAgent(IChatClient chatClient, SecuritiesAgent securitiesAgent, MarketDataAgent marketDataAgent, 
            AnalysisAgent analysisAgent, IAgentConversationsService agentConversationsService, 
            IAgentMessagesService agentMessagesService, IAgentUsageService agentUsageService)
        {
            _agentConversationsService = agentConversationsService;
            _agentMessagesService = agentMessagesService;
            _agentUsageService = agentUsageService;

            var tools = new[]
            {
                securitiesAgent.Agent.AsAIFunction(
                    new AIFunctionFactoryOptions
                    {
                        Name = "SecuritiesAgent",
                        Description = "Use this agent for security information such as security codes, ISINs, issuers," +
                        " and other basic information about a specific security. The input must be a security-related query."
                    }),
                marketDataAgent.Agent.AsAIFunction(
                    new AIFunctionFactoryOptions
                    {
                        Name = "MarketDataAgent",
                        Description = "Use this agent for market data such as latest price, daily market data, historical prices, " +
                        "trading activity, volume, turnover, price change, minimum price, maximum price, and average price." +
                        " The input must be a market-data specific query and should include the identified security code when available."
                    }),
                analysisAgent.Agent.AsAIFunction(
                    new AIFunctionFactoryOptions
                    {
                        Name = "AnalysisAgent",
                        Description = "Use this agent to analyze, compare, or summarize factual information retrieved from the other" +
                        " MSE agents. Use it when the user asks to compare securities, prices, price changes, volume, turnover, or" +
                        " other factual market metrics."
                    })
            };

            _agent = chatClient.AsAIAgent(
                instructions:
                """
                You are OrchestratorAgent for the Macedonian Stock Exchange (MSE).

                Your responsibility is to understand the user's request, delegate specialized tasks to the appropriate agents, 
                and provide one complete final answer to the user.

                You have access to two specialized agents:

                1. SecuritiesAgent
                 Handles:
                 - security codes
                 - ISINs
                 - issuers
                 - basic security information.
                2. MarketDataAgent
                 Handles: 
                 - latest price
                 - current price
                 - daily market data
                 - historical prices
                 - trading activity
                 - traded quantity
                 - turnover
                 - price change
                 - minimum price
                 - maximum price
                 - average price
                 3. AnalysisAgent
                  Handles:
                  - comparison of securities
                  - comparison of market data
                  - factual calculations
                  - summaries of retrieved market information
                  - differences between prices, volume, turnover and price changes

                Delegation rules:

                - Use SecuritiesAgent for security-related information.
                - Use MarketDataAgent for market-related information.
                - If the user requests both, use both agents.

                IMPORTANT for a combined request:
                1. First identify the security using SecuritiesAgent when necessary.
                2. Then use MarketDataAgent for the market-data portion.
                3. When calling MarketDataAgent, send a market-data-specific query, not the original mixed request.
                4. Include the identified security code in the MarketDataAgent query.
                5. After receiving the specialist results, combine them into one final answer.

                IMPORTANT for analytical or comparison requests:
                1. Identify the relevant securities using SecuritiesAgent when necessary.
                2. Retrieve the required market information using MarketDataAgent.
                3. Send the retrieved factual information to AnalysisAgent when analysis or comparison is required.
                4. Use the AnalysisAgent result when producing the final answer.
                5. Do not ask AnalysisAgent to invent or retrieve data that should come from the specialist agents.

                Do not send the original mixed request to MarketDataAgent.

                Do not claim that market data is unavailable unless MarketDataAgent actually returns that no data was found.

                Do not invent information.

                Do not provide investment advice, financial advice, or recommendations of any kind.
                
                Be factual, informative and concise.
                """,
                tools: tools,
                name: "OrchestratorAgent",
                description: "Coordinates specialized MSE agents and combines their results.");
        }

        public async Task<SecuritiesAgentResponse> AskAsync(int userId, int? conversationId, string question)
        {
            ValidationHelper.ValidateRequiredField(question, "Question", "QUESTION_VALIDATION_REQUIRED");

            if (question.Length > MaxMessageLength)
            {
                throw new BusinessRuleException("MESSAGE_LENGTH_INVALID", "Message cannot exceed 1000 characters.");
            }

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            var usage = _agentUsageService.FindByUserIdAndDate(userId, today);

            if (usage != null && usage.BrojPoraki >= DailyMessageLimit)
            {
                throw new BusinessRuleException("DAILY_LIMIT_REACHED",
                    $"You have reached the daily limit of {DailyMessageLimit} agent messages.");
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

            if (usage == null)
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
