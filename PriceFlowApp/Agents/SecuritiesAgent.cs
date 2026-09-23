using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Helpers;
using PriceFlowApp.Services;

namespace PriceFlowApp.Agents
{
    public class SecuritiesAgent
    {
        /* An AI-powered agent that answers questions about securities price change
         * by combining data from the REST API with an LLM. */

        private readonly AIAgent _agent;

        public AIAgent Agent => _agent;

        public SecuritiesAgent(IChatClient chatClient, SecuritiesTools securitiesTools)
        {
            var tools = new[]
            {
                AIFunctionFactory.Create(securitiesTools.SearchSecurities),
                AIFunctionFactory.Create(securitiesTools.GetSecurity)
            };

            _agent = chatClient.AsAIAgent(
                instructions:
                """
                You are SecuritiesAgent, a specialized agent for the Macedonian Stock Exchange (MSE).
                
                Your responsibility is to provide information about securities, issuers, 
                and other basic security-related information using the available securities tools.

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
