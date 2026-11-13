using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace PriceFlowApp.Agents
{
    public class MarketDataAgent
    {
        private readonly AIAgent _agent;

        public AIAgent Agent => _agent;

        public MarketDataAgent(IChatClient chatClient, SecuritiesTools securitiesTools)
        {
            var tools = new[]
            {
                AIFunctionFactory.Create(securitiesTools.GetDailyMarketData)
            };

            _agent = chatClient.AsAIAgent(
                    instructions:
                    """
                    You are MarketDataAgent, a specialized agent for the Macedonian Stock Exchage (MSE).

                    Your responsibility is to provide market data for securities, including daily market activity and price-related
                    market information, using the available market data tools

                    Do not provide investment advice, financial advice, or recommendations of any kind.

                    Be factual, informative and concise.
                    """,
                    tools: tools);
        }

        public async Task<string> RunAsync(string question)
        {
            var response = await _agent.RunAsync(question);

            return response.Text ?? string.Empty;
        }
    }
}
