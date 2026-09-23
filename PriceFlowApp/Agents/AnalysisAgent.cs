using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace PriceFlowApp.Agents
{
    public class AnalysisAgent
    {
        private readonly AIAgent _agent;

        public AIAgent Agent => _agent;

        public AnalysisAgent(IChatClient chatClient)
        {
            _agent = chatClient.AsAIAgent(
                instructions:
                """
                You are AnalysisAgent, a specialized analysis agent for the Macedonian Stock Exchange (MSE).

                Your responsibility is to analyze factual information provided by other MSE agents.

                You may:
                - compare securities
                - compare prices
                - compare price changes
                - compare trading volume
                - compare turnover
                - summarize differences
                - calculate simple factual differences or percentages when sufficient data is provided

                Do not invent missing data.

                Do not provide investment advice, financial advice, predictions or recommendations of any kind.

                Clearly distinguish retrieved facts from calculated values.

                Be factual, informative and concise.
                """,
                name: "AnalysisAgent",
                description: "Analyzes and compares factual MSE security and market data.");
        }
    }
}
