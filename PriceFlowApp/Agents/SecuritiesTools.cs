using PriceFlowApp.Services;
using System.ComponentModel;
using System.Text.Json;

namespace PriceFlowApp.Agents
{
    public class SecuritiesTools
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISecuritiesService _securitiesService;

        public SecuritiesTools(IHttpClientFactory httpClientFactory, ISecuritiesService securitiesService)
        {
            _httpClientFactory = httpClientFactory;
            _securitiesService = securitiesService;
        }


        public async Task<string> SearchSecurities([Description("The security code of part of the security code to search for.")] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return "Search term is required.";

            var client = _httpClientFactory.CreateClient("SecuritiesApi");
            var url = $"/api/securities/search?searchTerm={Uri.EscapeDataString(searchTerm)}";
            var response = await client.GetAsync(url);
            
            if(!response.IsSuccessStatusCode)
            {
                return $"Unable to search securities. HTTP status: {(int)response.StatusCode}";
            }
            
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetSecurity([Description("The code of the security.")] string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return "Code is required.";

            var client = _httpClientFactory.CreateClient("SecuritiesApi");
            var response = await client.GetAsync($"/api/securities/{code}");

            if(!response.IsSuccessStatusCode)
            {
                return $"Security with code {code} was not found.";
            }

            return await response.Content.ReadAsStringAsync();
        }

        [Description("Gets daily market data for a security, including latest transaction price, minimum, maximum, average price and change percent.")]
        public string GetDailyMarketData([Description("The security code.")] string securityCode, [Description("The date for which the user wants market data.")] DateTime date)
        {
            if (string.IsNullOrWhiteSpace(securityCode))
                return "Security code is required.";

            var data = _securitiesService.GetDailyMarketData(securityCode, date);

            if (data == null)
                return $"No market data was found for {securityCode}.";

            return JsonSerializer.Serialize(data);
        }
    }
}
