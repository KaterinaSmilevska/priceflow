using Microsoft.Extensions.AI;

namespace PriceFlowApp.Agents
{
    public class SecuritiesAgent
    {
        /* An AI-powered agent that answers questions about securities price change
         * by combining data from the REST API with an LLM. */

        private readonly IChatClient _chatClient;
        private readonly IHttpClientFactory _httpClientFactory;

        public SecuritiesAgent(IChatClient chatClient,  IHttpClientFactory httpClientFactory)
        {
            _chatClient = chatClient;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> AskAsync(string question)
        {
            var securitiesData = await GetSecuritiesDataFromApiAsync();
            var messages = new List<ChatMessage>
            {
                new (ChatRole.System, $"""
                You are SecuritiesAgent, a helpful agent that answers questions about securities. Be friendly, informative and concise.

                Current data (JSON): {securitiesData}
                """),
                new (ChatRole.User, question)
            };

            var response = await _chatClient.GetResponseAsync(messages);

            return response.Text;
        }

        private async Task<string> GetSecuritiesDataFromApiAsync()
        {
            var client = _httpClientFactory.CreateClient("SecuritiesApi");
            var response = await client.GetAsync("/api/securities");

            return await response.Content.ReadAsStringAsync();
        }
        
    }
}
